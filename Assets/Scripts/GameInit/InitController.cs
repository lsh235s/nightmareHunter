using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nightmareHunter {
    public class InitController : MonoBehaviour
    {
        Button SkipButton;
        Button StartButton;
        Button ContinueButton;
        Button SettingButton;
        Button ExitButton;

        [SerializeField]
        private GameObject _storyPanel;
        [SerializeField]
        private Animator _backGroundAnimation;

        [SerializeField]
        LoadingControl _loadingControl;

        // Start is called before the first frame update
        void Start()
        {
            SkipButton = GameObject.Find("Canvas/StoryPanel/SkipButton").GetComponent<Button>();
            StartButton = GameObject.Find("Canvas/MainPanel/ButtonList/StartButton").GetComponent<Button>();
            ContinueButton = GameObject.Find("Canvas/MainPanel/ButtonList/ContinueButton").GetComponent<Button>();
            SettingButton = GameObject.Find("Canvas/MainPanel/ButtonList/SettingButton").GetComponent<Button>();
            ExitButton = GameObject.Find("Canvas/MainPanel/ButtonList/ExitButton").GetComponent<Button>();

            SkipButton.onClick.AddListener(skipOnClick);
            StartButton.onClick.AddListener(startOnClick);
            ContinueButton.onClick.AddListener(ContinueOnClick);
            SettingButton.onClick.AddListener(SettingOnClick);
            ExitButton.onClick.AddListener(ExitOnClick);

            AudioManager.Instance.BackGroundPlay("bgm_title");

            StartCoroutine(_loadingControl.FadeInStart());
            StartCoroutine(storyPanelStop()); 
        }


        public void skipOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _storyPanel.SetActive(false);
        }
        public void startOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            _backGroundAnimation.SetBool("start",true);
        }    

        public void ContinueOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            SceneMoveManager.SceneMove("GameMoon");
        }    
        public void SettingOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
           // GameObject.Find("Canvas/StoryPanel").SetActive(false);
        }    
        public void ExitOnClick() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            Debug.Log("Exit");
        }

        public void pageMoveGameSun() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            SceneMoveManager.SceneMove("GameSun");
        }

        private IEnumerator storyPanelStop() {
            AudioManager.Instance.playSoundEffect(AudioManager.Instance.buttonSound,gameObject.GetComponent<AudioSource>());
            yield return new WaitForSeconds(2);
            _storyPanel.SetActive(false);
        }


    }
}
