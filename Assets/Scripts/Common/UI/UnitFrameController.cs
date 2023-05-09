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


        UnitController _uiItController;

        PlayerInfo NowSummonInfo;

        int objectIndex;

        void Start() {
            priceButton.onClick.AddListener(summonEnforce);

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();

            priceText.text = price.ToString();
                
            objectIndex = transform.GetSiblingIndex();
            //현재 세팅된 유닛의 저장된 능력치를 가져온다
            NowSummonInfo = _uiItController.gameDataManager.LoadSummerInfo(objectIndex ,_uiItController._unitObject);
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
            if(UiController.Instance.sceneMode == 0) {
                _isChange = true;
            }
        }

        public void OnMouseExit()
        {
            if(UiController.Instance.sceneMode == 0) {
                _isChange = false;
                string unitImage = "ui/1";
                frameImage.sprite = Resources.Load<Sprite>(unitImage);
                _changeTime = 0.5f;
            }
        }


        public void OnMouseBeginDrag()
        {
            if(UiController.Instance.sceneMode == 0) {
                Debug.Log(_uiItController._summoner[objectIndex].tag);
                
                if(summon != null && !"Summon".Equals(_uiItController._summoner[objectIndex].tag)) {
                    if(price <= int.Parse(UiController.Instance._gold.text)) {
                        UiController.Instance.goldUseSet(price);

                        // 마우스 좌표를 월드 좌표로 변환
                        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        // 오브젝트의 위치 설정
                        summon.transform.position = mousePosition;

                        // Cube 오브젝트 생성
                        _uiItController._summoner[objectIndex] = Instantiate(summon);

                        _uiItController._summoner[objectIndex].tag = "Summon";
                        _uiItController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo); 
                        _uiItController._summoner[objectIndex].GetComponent<Summons>().nowStatgeTime = UiController.Instance.sceneMode;
                    
                        _uiItController._summoner[objectIndex].GetComponent<Collider2D>().isTrigger = true;
                        
                        // 생성한 Cube 오브젝트 활성화
                        _uiItController._summoner[objectIndex].SetActive(true);
                    }
                }
            }
        }

        public void OnMouseEndDrag()
        {
            if(UiController.Instance.sceneMode == 0 && "Summon".Equals(_uiItController._summoner[objectIndex].tag)) {
                _uiItController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.positionInfo = _uiItController._summoner[objectIndex].transform.position.ToString();
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.summonsExist = true;
                _uiItController._summoner[objectIndex].GetComponent<Collider2D>().isTrigger = false;
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.spritesName = "Exorcist";
                _uiItController.gameDataManager.SaveSummerInfo("Exorcist",_uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo);

            }
        }

        public void OnMouseDrag()
        {
            if(UiController.Instance.sceneMode == 0 && "Summon".Equals(_uiItController._summoner[objectIndex].tag)) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _uiItController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                _uiItController._summoner[objectIndex].transform.position = mousePosition;
            }
        }

        public void summonEnforce() {
            UiController.Instance.goldUseSet(price);
            
            NowSummonInfo.playerLevel += 1;

            _uiItController.gameDataManager.SaveSummerInfo("Exorcist",NowSummonInfo);
            NowSummonInfo = _uiItController.gameDataManager.LoadSummerInfo(objectIndex ,_uiItController._unitObject);

            _uiItController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo);

        }
    }
}