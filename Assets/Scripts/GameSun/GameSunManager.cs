using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;

namespace nightmareHunter {
    public class GameSunManager : MonoBehaviour
    {
        [SerializeField]
        GameObject _playGameObject;   // 플레이어 오브젝트
        [SerializeField]
        StoryObject storyObject; // 저장된 스토리 오브젝트

        [SerializeField]
        LoadingControl _loadingControl; // 로딩 컨트롤
        

        UiController _uiController; // UI 컨트롤러
        Transform _talkObject; // 대화 캐릭터 오브젝트 대화 좌우 반전용
        GameObject _ChatGroup; // 대화 창 오브젝트
        Dictionary<string,GameObject> _tailkGraphicList = new Dictionary<string, GameObject>(); // 대화 그래픽 리스트
        TextMeshProUGUI _chatWindowText;  // 대화창 텍스트

        Button _chatArrowBtn; // 대화창 화살표 버튼

        int stroyStage; // 스토리 스테이지

        public bool eventFlag = false; // 이벤트 플래그

        public GameObject _tutory; // 튜토리얼 도착지점
        public GameObject _tutory2; // 튜토리얼 표적
        public GameObject _tutory3; // 소환수 배치

        GameObject _unitFrame; // 유닛 프레임 오브젝트

        // Start is called before the first frame update
        void Start()
        {     
            canvasInit();

            stroyStage = _uiController.systemSaveInfo.storyNum; //최종 진행된 스토리 스테이지
           
            if(stroyStage > -1) {
                _uiController.timePause = false;
                storyStart(stroyStage);
                if(stroyStage < 9) {
                    _playGameObject.transform.position = new Vector2(-3f, 0.7f);
                }
                if(stroyStage >= 40) {
                    _unitFrame.SetActive(true);
                }
            } else {
                _unitFrame.SetActive(true);
                _ChatGroup.SetActive(false);
            }
        }

        void canvasInit() {
            _loadingControl.FadeActive();

            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();
            _talkObject = GameObject.Find("Canvas/ChatGroup/LeftSet").GetComponent<Transform>();
            _ChatGroup = GameObject.Find("Canvas/ChatGroup");
            _chatWindowText = GameObject.Find("Canvas/ChatGroup/ChatWindow/StoryText").GetComponent<TextMeshProUGUI>();
            _chatArrowBtn = GameObject.Find("Canvas/ChatGroup/LeftSet/ChatArrowBtn").GetComponent<Button>();
            _unitFrame = GameObject.Find("Canvas/Mercenary");
            _chatArrowBtn.onClick.AddListener(skipButton);

            for (int i = 0; i < _talkObject.childCount; i++)
            {
                if(_talkObject.GetChild(i).gameObject.GetComponent<SkeletonGraphic>()) {
                    _tailkGraphicList.Add(_talkObject.GetChild(i).name, _talkObject.GetChild(i).gameObject);
                    
                    _tailkGraphicList[_talkObject.GetChild(i).name].SetActive(false);
                }
            }

            _unitFrame.SetActive(false);

            StartCoroutine(_loadingControl.FadeInStart());
        }

        public void skipButton() {
            
            foreach (KeyValuePair<string, GameObject> entry in _tailkGraphicList)
            {
                entry.Value.SetActive(false);
            }
            if(!eventFlag) {      
                _playGameObject.GetComponent<Player>().playerState = "tutorial";
                _ChatGroup.SetActive(true);
                stroyStage++;

                _uiController.systemSaveInfo.storyNum = stroyStage;
                _uiController.SystemDataSave();
                storyStart(stroyStage);
            } else {
                if(_uiController.systemSaveInfo.storyNum == 7)
                {
                    _playGameObject.GetComponent<Player>().playerState = "active";
                }
                _ChatGroup.SetActive(false);
            }
        }

        void storyStart(int inStroyStage) {
            //스켈레톤 사용법
            //hunterGraphic.initialFlipX = true;  좌우반전 
            //hunterGraphic.Initialize(true); 재시작

            if(storyObject.storyContentList.Count > inStroyStage) {
                _chatWindowText.text = storyObject.storyContentList[inStroyStage].content;
                if("story".Equals(storyObject.storyContentList[inStroyStage].contentType)) {
                    if(storyObject.storyContentList[inStroyStage].leftCharacter != "") {
                        _tailkGraphicList[storyObject.storyContentList[inStroyStage].leftCharacter].SetActive(true);
                        _tailkGraphicList[storyObject.storyContentList[inStroyStage].leftCharacter].GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, storyObject.storyContentList[inStroyStage].characterAnimation, true);
                    }
                } else if("tutorial".Equals(storyObject.storyContentList[inStroyStage].contentType)) {
                    eventFlag = true;

                    evnetAction(storyObject.storyContentList[inStroyStage].event_stage_id);
                }
            } else {
                _playGameObject.GetComponent<Player>().playerState = "wait";
               _ChatGroup.SetActive(false);
            }
        }

        // 대화 이미지 오브젝트 좌우 변경
        void talkObjectChangeLocation() {

            for (int i = 0; i < _talkObject.childCount; i++)
            {
                RectTransform rectTransform = _talkObject.GetChild(i).gameObject.GetComponent<RectTransform>();
                
                rectTransform.anchorMin  = new Vector2((rectTransform.anchorMin.x * -1) + 1, 0);
                rectTransform.anchorMax  = new Vector2((rectTransform.anchorMax.x * -1) + 1, 0);

                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x * -1, rectTransform.anchoredPosition.y);
            }
        }

        void evnetAction(int stageId) {
            switch  (stageId) {
                case 1 :
                    _tutory.SetActive(true);
                    _playGameObject.GetComponent<Player>().playerState = "wait";
                break;
                case 2 :
                    _tutory2.SetActive(true);
                break;
                case 3 :
                    _uiController.systemSaveInfo.money = 100;
                    _uiController._gold.text = _uiController.systemSaveInfo.money.ToString();
                    _uiController.SystemDataSave();
                    eventFlag = false;
                break;
                case 4 :
                    _loadingControl.FadeActive();
                    _playGameObject.transform.position = new Vector2(2.3f, 0.3f);
                    StartCoroutine(_loadingControl.FadeInStart());
                    eventFlag = false;
                    skipButton();
                break;
                case 5 :
                    _tutory3.SetActive(true);
                    _unitFrame.SetActive(true);
                break;
                case 6 :
                    _playGameObject.GetComponent<Player>().playerState = "wait";
                    _ChatGroup.SetActive(false);
                    _uiController.skipTime();
                    _uiController.timePause = true;
                break;
            }
        }
      
    }
}
