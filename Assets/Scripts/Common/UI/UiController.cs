using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace nightmareHunter {
    public class UiController : MonoBehaviour
    {
        public static UiController Instance { get; private set; }

        
        // 튜토리얼 포인트 이미지
        public Sprite[] _pointSprite; 
        public GameObject[] DamageEffect;

        // 왼쪽 상단 재화
        public TextMeshProUGUI _timerText;
        public TextMeshProUGUI _gold;
        public TextMeshProUGUI _integer;
        public Image _imagePlayHp;
        public TextMeshProUGUI _playerHp;
        public Image _imageClientHp;

        public TextMeshProUGUI _clientHp;
        int maxTargetHp;

        
        public bool timePause = true;
        // 스테이지 타이머
        float _countSec = 0.0f;
        int _Min = 0;
        int _Hour = 0;

        // 스테이지 모드
        public int sceneMode;  //0: sun, 1: moon


        // 스테이지 정보 저장
        public SystemSaveInfo systemSaveInfo; 

        // 드랍아이템
        public GameObject dropItem;
        
        // Start is called before the first frame update
        
        void Awake()
        {
            // 이미 인스턴스가 있는지 확인합니다.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // 중복되는 인스턴스가 있는 경우, 이 게임 객체를 파괴합니다.
                Destroy(this.gameObject);
            }
        }

        public void LoadStart() {
            _timerText = GameObject.Find("Canvas/UIGroup/Pause/Timer").GetComponent<TextMeshProUGUI>(); 
            _gold = GameObject.Find("Canvas/UIGroup/Gold/Text").GetComponent<TextMeshProUGUI>(); 
            _integer = GameObject.Find("Canvas/UIGroup/Integer/Text").GetComponent<TextMeshProUGUI>(); 
            _playerHp = GameObject.Find("Canvas/UIGroup/Hp/Text").GetComponent<TextMeshProUGUI>(); 
            _imagePlayHp = GameObject.Find("Canvas/UIGroup/Hp/HpImage").GetComponent<Image>(); 
            _clientHp = GameObject.Find("Canvas/UIGroup/Client/Text").GetComponent<TextMeshProUGUI>(); 
            _imageClientHp = GameObject.Find("Canvas/UIGroup/Client/ClientImage").GetComponent<Image>(); 
     
            getSceneMode();

            SystemInit();
        }

        public int getSceneMode() {
                   // 현재 게임 도드 정리
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if("GameSun".Equals(sceneName)) {
                sceneMode = 0;
                GameObject.Find("Canvas/UIGroup/Integer").SetActive(false);
            }  else {
                sceneMode = 1;
                GameObject.Find("Canvas/UIGroup/Gold").SetActive(false);
            }

            return sceneMode;
        }

        // Update is called once per frame
        void Update()
        {
            timerSet();
        }


        //재화 시나리오 전개 관련
        void SystemInit() {
            // 재화 시나리오 전개
            systemSaveInfo = GameDataManager.Instance.LoadSystemSaveInfo();
            _gold.text = systemSaveInfo.money.ToString();
            _integer.text = systemSaveInfo.integer.ToString();
            maxTargetHp = systemSaveInfo.targetHP;
            _clientHp.text = systemSaveInfo.targetHP.ToString();
            TargetHP(0, null);
        }

        public void SystemDataSave() {
            GameDataManager.Instance.SaveSystemInfo(systemSaveInfo);
        }


        void timerSet() {
            if(timePause) {
                _countSec += Time.deltaTime;

                if(_countSec >= 2.5f) {
                    _countSec = 0;
                    _Min += 10;
                }

                if(_Min >= 60) {
                    _Hour++;
                    _Min = 0;
                }
                
                _timerText.text = string.Format("{0:00}:{1:00}", _Hour, _Min);
                if(_Hour >= 12) {
                    if(sceneMode == 0) {
                        _Hour=0;
                        _Min=0;
                        SceneMoveManager.SceneMove("GameMoon");
                    } else {
                        _Hour=0;
                        _Min=0;
                        stageClear();
                    }
                }
            }
        }

        public void stageClear() {
            systemSaveInfo.stageId++;
            SystemDataSave();
            SceneMoveManager.SceneMove("GameSun");
        }

        public void skipTime() {
            _Hour = 11;
            _Min = 50;
        }

        public void goldUseSet(int useGold,string type) {
            if("-".Equals(type)) {
                if(int.Parse(_gold.text) >= useGold) {
                    systemSaveInfo.money = int.Parse(_gold.text) - useGold;
                    _gold.text = systemSaveInfo.money.ToString();
                    
                    SystemDataSave();
                }
            } else {
                systemSaveInfo.money = int.Parse(_gold.text) + useGold;
                _gold.text = systemSaveInfo.money.ToString();
                SystemDataSave();
            }
            
        }


        public void integerUseSet(int inputInteger,string type) {
            if("-".Equals(type)) {
                if(int.Parse(_integer.text) >= inputInteger) {
                    systemSaveInfo.integer = int.Parse(_integer.text) - inputInteger;
                    _integer.text = systemSaveInfo.integer.ToString();
                    
                    SystemDataSave();
                }
            } else {
                 systemSaveInfo.integer = int.Parse(_integer.text) + inputInteger;
                _integer.text = systemSaveInfo.integer.ToString();
                SystemDataSave();
            }
        }



        // 타겟 체력 관련
        public void TargetHP(float nowHp, Sprite[] _clientHpImage) {
            if(_clientHpImage != null) {
                float hpDiff = (int.Parse(_clientHp.text) - nowHp) ;
                float hpRate = hpDiff / (float)maxTargetHp * 100;
                //Debug.Log("attak : " + nowHp + "/total:"+maxTargetHp +"/now:"+ hpDiff + "/rate:"+hpRate);

                if(hpRate > 80 && hpRate == 100.0f) {
                    _imageClientHp.sprite = _clientHpImage[0];
                } else if (hpRate > 50.0f && hpRate <= 80.0f) {
                    _imageClientHp.sprite = _clientHpImage[1];
                } else if (hpRate > 25.0f && hpRate <= 50.0f) {
                    _imageClientHp.sprite = _clientHpImage[2];
                } else if (hpRate > 10.0f && hpRate <= 25.0f) {
                    _imageClientHp.sprite = _clientHpImage[3];
                } else if (hpRate > 0.0f && hpRate <= 10.0f) {
                    _imageClientHp.sprite = _clientHpImage[4];
                } else if (hpRate <= 0.0f) {
                    _imageClientHp.sprite = _clientHpImage[5];
                }
                _clientHp.text = hpDiff.ToString(); 
            }
        }
    }
}
