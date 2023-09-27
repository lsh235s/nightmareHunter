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
        private TextMeshProUGUI levelText;

        [SerializeField]
        private Button priceButton;

        [SerializeField]
        private string _spritesName;

        private bool _isChange = false;
        private float _changeTime = 0.3f;

        private int frameName = 1;


        UnitController _uiItController;

        PlayerInfo NowSummonInfo;

        public int objectIndex;

        private GameObject selectSummon;

        [SerializeField]
        private GameObject Goldinfo;
        [SerializeField]
        private GameObject Interinfo;

        void Start() {
            priceButton.onClick.AddListener(summonEnforce);

            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();

            NowSummonInfo = GameDataManager.Instance.LoadSummerInfo(_spritesName);
           // Debug.Log(UiController.Instance.sceneMode+"//"+_spritesName+"//"+NowSummonInfo.goldCash);
            priceText.text = NowSummonInfo.goldCash.ToString();
            levelText.text = NowSummonInfo.playerLevel.ToString();

            selectSummon = Instantiate(summon);
            selectSummon.GetComponent<Summons>().summonsBatchIng = false;
            selectSummon.SetActive(false);

            if(UiController.Instance.getSceneMode() == 0) {
                Goldinfo.SetActive(true);
                Interinfo.SetActive(false);
            } 
            else {
                Goldinfo.SetActive(false);
                Interinfo.SetActive(true);
            }
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
            } else {
                summonEnforce();
            }
        }



        public void OnMouseBeginDrag()
        {
            if(UiController.Instance.sceneMode == 0) {
     
                if(summon != null && selectSummon.GetComponent<Summons>().summonsBatchIng == false) {
                    if(NowSummonInfo.goldCash <= int.Parse(UiController.Instance._gold.text)) {
                        UiController.Instance.goldUseSet(NowSummonInfo.goldCash,"-");

                        // 마우스 좌표를 월드 좌표로 변환
                        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //    summon.GetComponent<Summons>().summonerId = objectIndex;
                        // 오브젝트의 위치 설정
                        summon.transform.position = mousePosition;

                        selectSummon.transform.position = mousePosition;
                        selectSummon.tag = "Summon";
                        selectSummon.GetComponent<Collider2D>().isTrigger = true;
                        // 생성한 Cube 오브젝트 활성화
                        selectSummon.SetActive(true);
                        selectSummon.GetComponent<Summons>().summonsBatchIng = true;
                    }
                }
            } else {
                _uiItController._summoner[objectIndex].GetComponent<Summons>().rangeObject.SetActive(false);
            }
        }

        public void OnMouseEndDrag()
        {
            if(UiController.Instance.sceneMode == 0 && selectSummon.GetComponent<Summons>().summonsBatchIng == true) {
               if(selectSummon.GetComponent<Summons>().summonsExist) {
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoX = selectSummon.transform.position.x.ToString();
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoY = selectSummon.transform.position.y.ToString();
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoZ = selectSummon.transform.position.z.ToString();
                    int playerId = GameDataManager.Instance.SaveSummerInfo(_spritesName,selectSummon.GetComponent<Summons>().basePlayerinfo);

                    _uiItController._summonList[objectIndex].Add(Instantiate(selectSummon));

                    if (_uiItController._summonList[objectIndex].Count > 0)
                    { 
                        GameObject AddSummon = _uiItController._summonList[objectIndex][_uiItController._summonList[objectIndex].Count - 1];
                        AddSummon.GetComponent<Collider2D>().isTrigger = true;
                        AddSummon.transform.position = selectSummon.transform.position;
                        AddSummon.name = _spritesName + "_" + playerId;
                        AddSummon.tag = "Summon";
                        AddSummon.SetActive(true);
                        AddSummon.GetComponent<Summons>().summonerBatchKeyId = playerId;
                        AddSummon.GetComponent<Summons>().summonsBatchIng = false;

                        AddSummon.GetComponent<Summons>().basePlayerinfo = GameDataManager.Instance.LoadSummerInfo(_spritesName);

                        AddSummon.GetComponent<Summons>().basePlayerinfo.summonsExist = true;
                        AddSummon.GetComponent<Summons>().basePlayerinfo.spritesName = _spritesName;
                        AddSummon.GetComponent<Summons>().rangeObject.SetActive(false);
                    }
                    

                    _isChange = false;
                    Color currentColor = frameImage.GetComponent<Image>().color;

                    string unitImage = "ui/1";
                    frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage);
                    frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                    _changeTime = 0.5f;

                    selectSummon.SetActive(false);
                    selectSummon.GetComponent<Summons>().summonsBatchIng = false;

           
                } else {
                    selectSummon.SetActive(false);
                    selectSummon.GetComponent<Summons>().summonsBatchIng = false;
                    
                    _isChange = false;
                    Color currentColor = frameImage.GetComponent<Image>().color;

                    string unitImage = "ui/1";
                    frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage);
                    frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                    _changeTime = 0.5f;
                }
            }
        }

        public void OnMouseDrag()
        {
            if(UiController.Instance.sceneMode == 0 && selectSummon.GetComponent<Summons>().summonsBatchIng == true) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
               // _uiItController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                selectSummon.transform.position = mousePosition;
            }
        }

        public void summonEnforce() {
            // 소환수 레벨업을 할 경우 해당 정보를 저장.
            if(int.Parse(levelText.text) < 6) {
                UiController.Instance.integerUseSet(NowSummonInfo.integerCash,"-");
            
                int summonLevel = GameDataManager.Instance.UpdateCsvData(_spritesName);
                levelText.text = summonLevel.ToString();

                for(int i=0; i < _uiItController._summonList[objectIndex].Count; i++) {
                    _uiItController._summonList[objectIndex][i].GetComponent<Summons>().summonlevelUp(NowSummonInfo, summonLevel);
                }
                
            }

            // NowSummonInfo = _uiItController.gameDataManager.LoadSummerInfo(objectIndex ,_uiItController._unitObject);

            // _uiItController._summoner[objectIndex].GetComponent<Summons>().playerDataLoad(NowSummonInfo);

        }

    }
}