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

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(_loadingControl.FadeInStart());
            stroyStage = PlayerPrefs.GetInt("FirstTimeLaunch", 0);

            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();
            _talkObject = GameObject.Find("Canvas/ChatGroup/LeftSet").GetComponent<Transform>();
            _ChatGroup = GameObject.Find("Canvas/ChatGroup");
            _chatWindowText = GameObject.Find("Canvas/ChatGroup/ChatWindow/StoryText").GetComponent<TextMeshProUGUI>();
            _chatArrowBtn = GameObject.Find("Canvas/ChatGroup/LeftSet/ChatArrowBtn").GetComponent<Button>();
            _chatArrowBtn.onClick.AddListener(skipButton);

            for (int i = 0; i < _talkObject.childCount; i++)
            {
                if(_talkObject.GetChild(i).gameObject.GetComponent<SkeletonGraphic>()) {
                    _tailkGraphicList.Add(_talkObject.GetChild(i).name, _talkObject.GetChild(i).gameObject);
                    
                    _tailkGraphicList[_talkObject.GetChild(i).name].SetActive(false);
                }
            }
            
           
            if(stroyStage > -1) {
                storyStart(stroyStage);
            } else {
                _ChatGroup.SetActive(false);
            }
        }

        void skipButton() {
            stroyStage++;
            foreach (KeyValuePair<string, GameObject> entry in _tailkGraphicList)
            {
                entry.Value.SetActive(false);
            }
            PlayerPrefs.SetInt("FirstTimeLaunch", stroyStage);
            storyStart(stroyStage);
        }

        void storyStart(int inStroyStage) {
            //스켈레톤 사용법
            //hunterGraphic.initialFlipX = true;  좌우반전 
            //hunterGraphic.Initialize(true); 재시작

            if("story".Equals(storyObject.storyContentList[inStroyStage].contentType)) {
                _tailkGraphicList[storyObject.storyContentList[inStroyStage].leftCharacter].SetActive(true);
                _tailkGraphicList[storyObject.storyContentList[inStroyStage].leftCharacter].GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, storyObject.storyContentList[inStroyStage].characterAnimation, true);
            }
            
            _chatWindowText.text = storyObject.storyContentList[inStroyStage].content;
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

      
    }
}
