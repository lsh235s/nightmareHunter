using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace nightmareHunter {
    public class InitController : MonoBehaviour
    {
        Button SkipButton;
        Button StartButton;
        Button ContinueButton;
        Button SettingButton;
        Button ExitButton;

        TextMeshProUGUI _skipText;
        public TextMeshProUGUI testText;

        [SerializeField]
        private GameObject _backGroundPanel;
        [SerializeField]
        private GameObject _storyPanel;
        [SerializeField]
        private GameObject _opening;
        [SerializeField]
        private Animator _backGroundAnimation;

        [SerializeField]
        LoadingControl _loadingControl;
        bool isSkip = false;

        int playerAnimation = 0;
        private AnimatorStateInfo  _currentState;


        // Start is called before the first frame update
        void Start()
        {
            SkipButton = GameObject.Find("Canvas/StoryPanel/SkipButton").GetComponent<Button>();
            StartButton = GameObject.Find("Canvas/MainPanel/ButtonList/StartButton").GetComponent<Button>();
            ContinueButton = GameObject.Find("Canvas/MainPanel/ButtonList/ContinueButton").GetComponent<Button>();
            SettingButton = GameObject.Find("Canvas/MainPanel/ButtonList/SettingButton").GetComponent<Button>();
            ExitButton = GameObject.Find("Canvas/MainPanel/ButtonList/ExitButton").GetComponent<Button>();
            _skipText = GameObject.Find("Canvas/StoryPanel/SkipButton/SkipText").GetComponent<TextMeshProUGUI>();

            SkipButton.onClick.AddListener(skipOnClick);
            StartButton.onClick.AddListener(startOnClick);
            ContinueButton.onClick.AddListener(ContinueOnClick);
            SettingButton.onClick.AddListener(SettingOnClick);
            ExitButton.onClick.AddListener(ExitOnClick);

            AudioManager.Instance.BackGroundPlay("bgm_title");

            _backGroundAnimation.SetBool("start", false);

            StartCoroutine(_loadingControl.FadeInStart());
            StartCoroutine(storyPanelStop()); 
            testText.text = Path.Combine(Application.persistentDataPath, "/Plugin/SaveData/SystemData.json")+"//"+Application.dataPath; 
        }


        void Update() {
            if(_skipText != null) {
                float alpha = Mathf.PingPong(Time.time * 3f, 1);
                _skipText.color = new Color(_skipText.color.r, _skipText.color.g, _skipText.color.b, alpha);
            }

            if(playerAnimation == 1) {
                // 현재 재생 중인 애니메이션 상태 정보 가져오기
                _currentState = _backGroundAnimation.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] currentClipInfo = _backGroundAnimation.GetCurrentAnimatorClipInfo(0);

                if ("title_2".Equals(currentClipInfo[0].clip.name) && _currentState.normalizedTime >= 0.91f )
                {
                    backGroundAnimationEnd();
                }
            } 
        }


        public void skipOnClick() {
            if(isSkip) {
                AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
                _storyPanel.SetActive(false);
            }
        }
        public void startOnClick() {
            SystemSaveInfo systemSaveInfo = new SystemSaveInfo(); 
            systemSaveInfo.integer = 0;
            systemSaveInfo.money = 0;
            systemSaveInfo.stageId = 0;
            systemSaveInfo.storyNum = 0;
            systemSaveInfo.targetHP = 1000;

            GameDataManager.Instance.SaveSystemInfo(systemSaveInfo);
            GameDataManager.Instance.GameDataInit();

            PlayerInfo summonsInfo = new PlayerInfo();

            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _backGroundAnimation.SetBool("start",true);
            _backGroundAnimation.Play("title2");
            playerAnimation = 1;
        }    

        public void ContinueOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _backGroundAnimation.SetBool("start",true);
            _backGroundAnimation.Play("title_2");
            playerAnimation = 1;
            //SceneMoveManager.SceneMove("GameMoon");
        }    
        public void SettingOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
           // GameObject.Find("Canvas/StoryPanel").SetActive(false);
        }    
        public void ExitOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }


        public void backGroundAnimationEnd() {
            
            int score = PlayerPrefs.GetInt("FirstGameStart", 0);

            if(score == 0) {
                playerAnimation = 2;
                GameObject.Find("Canvas/MainPanel/ButtonList").SetActive(false);
                // 오프닝 시작
                OpeningStart();
            } else {
                playerAnimation = 3;
                pageMoveGameSun();
            }
        }

        
        void OpeningStart() {
            //PlayerPrefs.SetInt("FirstGameStart", 1);
            //PlayerPrefs.Save();
            
            _opening.SetActive(true);
            _backGroundPanel.SetActive(false);
        }
        
        
        public void pageMoveGameSun() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            SceneMoveManager.SceneMove("GameSun");
        }

        private IEnumerator storyPanelStop() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            yield return new WaitForSeconds(7);
            _skipText.text = "건너뛰기..";
            isSkip = true;
            yield return new WaitForSeconds(3);
            _storyPanel.SetActive(false);
        }


    }
}
