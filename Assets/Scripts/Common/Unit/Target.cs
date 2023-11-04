using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


namespace nightmareHunter {
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private UiController TargetHpCanvas;
        public Sprite[] _clientHpImage;

        
        public SkeletonMecanim skeletonMecanim;
        public GameObject whilwind;
        public GameObject clinetTarget;
        Animator _animator;

        bool isFalling = false;

        void Start() {
            _animator = clinetTarget.GetComponent<Animator>();
        }

        public void DamageProcess(float damage) {
            UiController.Instance.TargetHP(damage,_clientHpImage);
            isFalling = true;
            _animator.SetTrigger("dam");

            Debug.Log("TargetHpCanvas._clientHp : " + UiController.Instance._clientHp.text);
            if(int.Parse(UiController.Instance._clientHp.text) <= 0) {
                clinetTarget.SetActive(false);
                whilwind.SetActive(true);
                StartCoroutine(gameEnd()); 
            }
        }

        private IEnumerator gameEnd() {
            if(AudioManager.Instance.playSound.ContainsKey("over")){
                gameObject.GetComponent<AudioSource>().clip = AudioManager.Instance.playSound["over"];
                gameObject.GetComponent<AudioSource>().Play();
            };
            yield return new WaitForSeconds(2.0f);
            SceneMoveManager.SceneMove("GameInits");
        }

        void Update() {
            if (isFalling)
            {  
                StartCoroutine(damageShake());
            }   
        }


        private IEnumerator damageShake() {
            skeletonMecanim.transform.position  += new Vector3(0.01f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            Color endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0.01f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0.01f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  += new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0.01f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);

            isFalling = false;

            yield return null;
        }
    }
}

