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
        private Image loadImage;
        [SerializeField]
        private GameObject _storyPanel;
        [SerializeField]
        private Animator _backGroundAnimation;

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
            StartCoroutine(FadeInStart()); 
        }

        public void skipOnClick() {
            _storyPanel.SetActive(false);
        }
        public void startOnClick() {
            _backGroundAnimation.SetBool("start",true);
            Invoke("pageMoveGameSun", 1.5f);
            
        }    
        public void ContinueOnClick() {
            SceneMoveManager.SceneMove("GameMoon");
        }    
        public void SettingOnClick() {
           // GameObject.Find("Canvas/StoryPanel").SetActive(false);
        }    
        public void ExitOnClick() {
            Debug.Log("Exit");
        }


        void pageMoveGameSun() {
            SceneMoveManager.SceneMove("GameSun");
        }
        

            //페이드 아웃
        private IEnumerator FadeInStart()
        {
            GameObject.Find("Canvas").transform.Find("Load").gameObject.SetActive(true);
            for (float f = 1f; f > 0; f -= 0.005f)
            {
                Color c = loadImage.GetComponent<Image>().color;
                c.a = f;
                loadImage.GetComponent<Image>().color = c;
                yield return null;
            }
            
            GameObject.Find("Canvas/Load").SetActive(false);
            yield return new WaitForSeconds(1);
            _storyPanel.SetActive(false);
        }
    }
}
