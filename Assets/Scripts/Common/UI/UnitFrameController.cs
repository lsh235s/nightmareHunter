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
        private GameObject frameImage;
        [SerializeField]
        private GameObject summon;
        [SerializeField]
        private TextMeshProUGUI priceText;

        [SerializeField]
        private Button priceButton;

        [SerializeField]
        private int price;
        [SerializeField]
        private string _spritesName;

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
                    frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage) ;
                    _changeTime = 0.5f;
                }
            }
        }

    
        public void OnMouseDown()
        {
            if(UiController.Instance.sceneMode == 0) {
                Color currentColor = frameImage.GetComponent<Image>().color;
                frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 255f);
                _isChange = true;
            }
        }



        public void OnMouseBeginDrag()
        {
            if(UiController.Instance.sceneMode == 0) {
                _uiItController._summoner[objectIndex].GetComponent<Summons>().rangeObject.SetActive(true);
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
                        _uiItController._summoner[objectIndex].GetComponent<Collider2D>().isTrigger = true;
                        
                        // 생성한 Cube 오브젝트 활성화
                        _uiItController._summoner[objectIndex].SetActive(true);
                    }
                }
            } else {
                _uiItController._summoner[objectIndex].GetComponent<Summons>().rangeObject.SetActive(false);
            }
        }

        public void OnMouseEndDrag()
        {
            if(UiController.Instance.sceneMode == 0 && "Summon".Equals(_uiItController._summoner[objectIndex].tag)) {
               
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.positionInfo = _uiItController._summoner[objectIndex].transform.position.ToString();
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.summonsExist = true;
                _uiItController._summoner[objectIndex].GetComponent<Collider2D>().isTrigger = true;
                _uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo.spritesName = _spritesName;
                _uiItController._summoner[objectIndex].GetComponent<Summons>().rangeObject.SetActive(false);
                _uiItController.gameDataManager.SaveSummerInfo(_spritesName,_uiItController._summoner[objectIndex].GetComponent<Summons>()._playerinfo);


                _isChange = false;
                Color currentColor = frameImage.GetComponent<Image>().color;

                string unitImage = "ui/1";
                frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage);
                frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                _changeTime = 0.5f;
            }
        }

        public void OnMouseDrag()
        {
            if(UiController.Instance.sceneMode == 0 && "Summon".Equals(_uiItController._summoner[objectIndex].tag)) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
               // _uiItController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                _uiItController._summoner[objectIndex].transform.position = mousePosition;
            }
        }

        public void summonEnforce() {
            // UiController.Instance.goldUseSet(price);
            
            // NowSummonInfo.playerLevel += 1;

            // _uiItController.gameDataManager.SaveSummerInfo(_spritesName,NowSummonInfo);
            // NowSummonInfo = _uiItController.gameDataManager.LoadSummerInfo(objectIndex ,_uiItController._unitObject);

            // _uiItController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo);

        }
    }
}