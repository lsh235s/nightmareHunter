using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace nightmareHunter {
    public class UiController : MonoBehaviour
    {
        // 왼쪽 상단 재화
        public TextMeshProUGUI _timerText;
        public TextMeshProUGUI _gold;
        public Image _imagePlayHp;
        public TextMeshProUGUI _playerHp;
        public Image _imageClientHp;
        public TextMeshProUGUI _clientHp;

        
        // 저장 데이터
        public UnitObject _unitObject;
        public StateMonsterBatch _stateMonsterBatch ;

        // 스테이지 모드
        public int sceneMode;  //0: sun, 1: moon

        // 스테이지 타이머
        float _countSec = 0.0f;
        int _Min = 0;
        int _Hour = 0;


        // 저장 기능 관련
        public GameDataManager gameDataManager ;

        public GameObject[] _summoner;

        // Start is called before the first frame update
        public GameObject _playGameObject; 
        
        // Start is called before the first frame update
        
        void Awake()
        {
            _timerText = GameObject.Find("Canvas/UIGroup/Pause/PauseValue/Timer").GetComponent<TextMeshProUGUI>(); 
            _gold = GameObject.Find("Canvas/UIGroup/Gold/GoldValue/Text").GetComponent<TextMeshProUGUI>(); 
            _playerHp = GameObject.Find("Canvas/UIGroup/Hp/HpValue/Text").GetComponent<TextMeshProUGUI>(); 
            _imagePlayHp = GameObject.Find("Canvas/UIGroup/Hp/HpImage").GetComponent<Image>(); 
            _clientHp = GameObject.Find("Canvas/UIGroup/Client/ClientValue/Text").GetComponent<TextMeshProUGUI>(); 
            _imageClientHp = GameObject.Find("Canvas/UIGroup/Client/ClientImage").GetComponent<Image>(); 
            _gold.text = 10000.ToString();

            // 현재 게임 도드 정리
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if("GameSun".Equals(sceneName)) {
                sceneMode = 0;
            }  else {
                sceneMode = 1;
            }

            gameDataManager = new GameDataManager();

            //소환수 배치
            summonerInit();
            //플레이어 능력치 load
            playerInit();
        }

        // Update is called once per frame
        void Update()
        {
            timerSet();
        }



        // 플레이어 배치 관련
        void playerInit() {
            _playGameObject.GetComponent<Player>().playerDataLoad(gameDataManager.LoadPlayerInfo(_unitObject)); 
        }

        // 소환수 배치 관련
        void summonerInit() {
           List<PlayerInfo> existTargetInfo = gameDataManager.SummerListLoad(_unitObject);
           for(int i = 0; i < existTargetInfo.Count; i++)
           {
                SummonerGet(existTargetInfo[i]);
           }
        }

        
        public void SummonerGet(PlayerInfo PlayerInfo) {
            GameObject select = null;
            
            select = _summoner[PlayerInfo.id];
            select.GetComponent<Summons>().playerDataLoad(PlayerInfo);
            string[] vectorValues = PlayerInfo.positionInfo.Trim('(', ')').Split(',');
            
            Vector3 vector = new Vector3(float.Parse(vectorValues[0]), float.Parse(vectorValues[1]), float.Parse(vectorValues[2]));
          
            _summoner[PlayerInfo.id] = Instantiate(select);   
            _summoner[PlayerInfo.id].transform.position = vector;
         
        }


        void timerSet() {
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
                    SceneMoveManager.SceneMove("GameMoon");
                } else {
                    SceneMoveManager.SceneMove("GameSun");
                }
            }
        }

        public void skipTime() {
            _Hour = 11;
            _Min = 50;
        }

        public void goldUseSet(int useGold) {
            if(int.Parse(_gold.text) >= useGold) {
                int nowGold = int.Parse(_gold.text) - useGold;
                _gold.text = nowGold.ToString();
            }
        }
    }
}
