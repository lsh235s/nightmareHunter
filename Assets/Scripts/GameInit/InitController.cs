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
        private GameObject _storyPanel;
        [SerializeField]
        private Animator _backGroundAnimation;

        [SerializeField]
        LoadingControl _loadingControl;
        bool isSkip = false;


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

            StartCoroutine(_loadingControl.FadeInStart());
            StartCoroutine(storyPanelStop()); 
            testText.text = Path.Combine(Application.persistentDataPath, "/Plugin/SaveData/SystemData.json")+"//"+Application.dataPath; 
        }


        void Update() {
            if(_skipText != null) {
                float alpha = Mathf.PingPong(Time.time * 3f, 1);
                _skipText.color = new Color(_skipText.color.r, _skipText.color.g, _skipText.color.b, alpha);

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

            GameDataManager gameDataManager = new GameDataManager();
            gameDataManager.SaveSystemInfo(systemSaveInfo);

            gameDataManager.HunterInit();

            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _backGroundAnimation.SetBool("start",true);
        }    

        public void ContinueOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _backGroundAnimation.SetBool("start",true);
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
