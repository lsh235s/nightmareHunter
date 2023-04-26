using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

namespace nightmareHunter {
    public class UnitFrameController : MonoBehaviour
    {
        
        [SerializeField]
        private Image frameImage;
        [SerializeField]
        private GameObject summon;
        [SerializeField]
        private TextMeshProUGUI priceText;

        [SerializeField]
        private Button priceButton;

        [SerializeField]
        private int price;

        private bool _isChange = false;
        private float _changeTime = 0.3f;

        private int frameName = 1;


        UiController _uiController;

        PlayerInfo NowSummonInfo;

        int objectIndex;

        void Start() {
            priceButton.onClick.AddListener(summonEnforce);

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();

            priceText.text = price.ToString();
                
            objectIndex = transform.GetSiblingIndex();
            //현재 세팅된 유닛의 저장된 능력치를 가져온다
            NowSummonInfo = _uiController.gameDataManager.LoadSummerInfo(objectIndex ,_uiController._unitObject);
        }

        void Update()
        {
            if(_isChange) {
                _changeTime -= Time.deltaTime;
                if(_changeTime <= 0) {
                    if(frameName < 3) {
                        frameName++;
                    } else {
                        frameName = 1;
                    }
                    string unitImage = "ui/"+frameName.ToString();
                    frameImage.sprite = Resources.Load<Sprite>(unitImage) ;
                    _changeTime = 0.5f;
                }
            }
        }

    
        public void OnMouseEnter()
        {
            if(_uiController.sceneMode == 0) {
                _isChange = true;
            }
        }

        public void OnMouseExit()
        {
            if(_uiController.sceneMode == 0) {
                _isChange = false;
                string unitImage = "ui/1";
                frameImage.sprite = Resources.Load<Sprite>(unitImage);
                _changeTime = 0.5f;
            }
        }


        public void OnMouseBeginDrag()
        {
            if(_uiController.sceneMode == 0) {
                if(summon != null && _uiController._summoner[objectIndex] == null) {
                    // 마우스 좌표를 월드 좌표로 변환
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // 오브젝트의 위치 설정
                    summon.transform.position = mousePosition;

                    // Cube 오브젝트 생성
                    _uiController._summoner[objectIndex] = Instantiate(summon);

                    _uiController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo); 
                    _uiController._summoner[objectIndex].GetComponent<Summons>().nowStatgeTime = _uiController.sceneMode;
                  
                    _uiController._summoner[objectIndex].GetComponent<Collider2D>().isTrigger = true;
                    
                    // 생성한 Cube 오브젝트 활성화
                    _uiController._summoner[objectIndex].SetActive(true);
                }
            }
        }

        public void OnMouseEndDrag()
        {
            if(_uiController.sceneMode == 0) {
                _uiController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                _uiController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.positionInfo = _uiController._summoner[objectIndex].transform.position.ToString();
                _uiController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.summonsExist = true;
                _uiController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.spritesName = "Exorcist";
                _uiController.gameDataManager.SaveSummerInfo("Exorcist",_uiController._summoner[objectIndex].GetComponent<Summons>()._playerinfo);
            }
        }

        public void OnMouseDrag()
        {
            if(_uiController.sceneMode == 0) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _uiController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                _uiController._summoner[objectIndex].transform.position = mousePosition;
            }
        }

        public void summonEnforce() {
            _uiController.goldUseSet(price);
            NowSummonInfo.playerLevel += 1;

            _uiController.gameDataManager.SaveSummerInfo("Exorcist",NowSummonInfo);
            NowSummonInfo = _uiController.gameDataManager.LoadSummerInfo(objectIndex ,_uiController._unitObject);

            _uiController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo);

        }
    }
}