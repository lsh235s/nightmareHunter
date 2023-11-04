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
        private string _spritesName;

        private bool _isChange = false;
        private float _changeTime = 0.3f;

        private int frameName = 1;


        UnitController _unitController;

        PlayerInfo NowSummonInfo;

        public int objectIndex;

        private GameObject selectSummon;

        [SerializeField]
        private GameObject Goldinfo;
        [SerializeField]
        private GameObject Interinfo;

        Sprite[] levelImage;

        [SerializeField]
        Image levelImageObject;

        void Start() {
            priceButton.onClick.AddListener(summonEnforce);

            _unitController = GameObject.Find("Canvas").GetComponent<UnitController>();
            levelImage = new Sprite[5];
            levelImage[0] = Resources.Load<Sprite>("ui/Summon/Level1");
            levelImage[1] = Resources.Load<Sprite>("ui/Summon/Level2");
            levelImage[2] = Resources.Load<Sprite>("ui/Summon/Level3");
            levelImage[3] = Resources.Load<Sprite>("ui/Summon/Level4");
            levelImage[4] = Resources.Load<Sprite>("ui/Summon/Level5");

            NowSummonInfo = GameDataManager.Instance.LoadSummerInfo(_spritesName);
           // Debug.Log(UiController.Instance.sceneMode+"//"+_spritesName+"//"+NowSummonInfo.goldCash);
            priceText.text = NowSummonInfo.goldCash.ToString();
            levelImageObject.sprite = levelImage[NowSummonInfo.playerLevel-1];

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
            Debug.Log("OnTriggerEnter2DOnMouseDown/"+ UiController.Instance.gameMode);
            if(UiController.Instance.sceneMode == 0) {
               // Color currentColor = frameImage.GetComponent<Image>().color;
               // frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 255f);
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
                _unitController._summoner[objectIndex].GetComponent<Summons>().rangeObject.SetActive(false);
            }
        }

        public void OnMouseEndDrag()
        {
            if(UiController.Instance.sceneMode == 0 && selectSummon.GetComponent<Summons>().summonsBatchIng == true) {
               if(selectSummon.GetComponent<Summons>().summonsExist) {
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoX = selectSummon.transform.position.x.ToString();
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoY = selectSummon.transform.position.y.ToString();
                    selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoZ = selectSummon.transform.position.z.ToString();

                    if("SuccessTutorial".Equals(UiController.Instance.gameMode)) {
                        selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoX = "1.08";
                        selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoY = "-0.16";
                        selectSummon.GetComponent<Summons>().basePlayerinfo.positionInfoZ = "0.0";
                    }

                    int playerId = GameDataManager.Instance.SaveSummerInfo(_spritesName,selectSummon.GetComponent<Summons>().basePlayerinfo);

                    _unitController._summonList[objectIndex].Add(Instantiate(selectSummon));

                    if (_unitController._summonList[objectIndex].Count > 0)
                    { 
                        GameObject AddSummon = _unitController._summonList[objectIndex][_unitController._summonList[objectIndex].Count - 1];
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
                    //Color currentColor = frameImage.GetComponent<Image>().color;

                    string unitImage = "ui/1";
                   // frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage);
                   // frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                    _changeTime = 0.5f;

                    selectSummon.SetActive(false);
                    selectSummon.GetComponent<Summons>().summonsBatchIng = false;

                    if(AudioManager.Instance.playSound.ContainsKey("place")){
                        gameObject.GetComponent<AudioSource>().clip = AudioManager.Instance.playSound["place"];
                        gameObject.GetComponent<AudioSource>().Play();
                    };

           
                } else {
                    selectSummon.SetActive(false);
                    selectSummon.GetComponent<Summons>().summonsBatchIng = false;
                    
                    _isChange = false;

                    string unitImage = "ui/1";
                   // frameImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(unitImage);
                  //  frameImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                    _changeTime = 0.5f;
                }
            }
        }

        public void OnMouseDrag()
        {
            if(UiController.Instance.sceneMode == 0 && selectSummon.GetComponent<Summons>().summonsBatchIng == true) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
               // _unitController._summoner[objectIndex].transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                selectSummon.transform.position = mousePosition;

                if("tutorial".Equals(UiController.Instance.gameMode)) {
                    selectSummon.GetComponent<Summons>().FaildSummon();
                }
            }
        }

        public void summonEnforce() {
            // 소환수 레벨업을 할 경우 해당 정보를 저장.
            if(NowSummonInfo.playerLevel < 5) {
                UiController.Instance.integerUseSet(NowSummonInfo.integerCash,"-");
            
                int summonLevel = GameDataManager.Instance.UpdateCsvData(_spritesName);
                Debug.Log("summonLevel:"+summonLevel);
                NowSummonInfo.playerLevel = summonLevel-1;
                levelImageObject.sprite = levelImage[summonLevel-1];
                

                for(int i=0; i < _unitController._summonList[objectIndex].Count; i++) {
                    _unitController._summonList[objectIndex][i].GetComponent<Summons>().summonlevelUp(NowSummonInfo, summonLevel);
                }
                
            }

        }

    }
}