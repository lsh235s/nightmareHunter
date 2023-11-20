using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;


namespace nightmareHunter {
    public class WeaponItem : MonoBehaviour
    {

        private SkeletonAnimation skeletonAnimation;
        private Material skeletonMaterial;
        
        public int weaponType = 0;

        private bool isBlinking = false; // 깜박이는 중인지 여부

        private float stateTime = 7f; // 파괴되기까지의 대기 시간

        private float elapsedTime = 0f; // 경과한 시간
        private bool increasing = true; // 알파 값 증가 여부

        byte blinkCount = 255; // 깜박이를 위한 수치
        public int stoneValue = 0;



        private void Start() {
        }


        public void SetWeaponType(int type) {
            skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
             // SkeletonAnimation에 사용된 Material 가져오기
            skeletonMaterial = skeletonAnimation.GetComponent<Renderer>().material;    

            weaponType = type;

             if(type == 1) {
                skeletonAnimation.timeScale = 0.0f;
                skeletonAnimation.AnimationState.SetAnimation(0, "Pistol_1", false);
            } else if(type == 2) {
                skeletonAnimation.timeScale = 0.0f;
                skeletonAnimation.AnimationState.SetAnimation(0, "shotgun_1", false);
            } else if(type == 3) {
                skeletonAnimation.timeScale = 0.0f;
                skeletonAnimation.AnimationState.SetAnimation(0, "machinegun_1", false);
            } else if(type == 4) {
                stateTime = 2f;
            } else {
                skeletonAnimation.timeScale = 0.0f;
                skeletonAnimation.AnimationState.SetAnimation(0, "shotgun_2", false);
            }
            
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime; // 경과 시간 증가
            if (elapsedTime >= stateTime)
            {
                if(stoneValue > 0 ){
                    //재화 증가
                    UiController.Instance.integerUseSet(stoneValue,"+");
                }
                Destroy(gameObject); // 오브젝트 파괴
            } else {
                if(elapsedTime >= 4) {
                    if (increasing)
                    {
                        blinkCount += 1; // 알파 값 증가
                        if (blinkCount >= 255)
                        {
                            blinkCount = 255;
                            increasing = false;
                        }
                    }
                    else
                    {
                        blinkCount -= 1; // 알파 값 감소
                        if (blinkCount <= 0f)
                        {
                            blinkCount = 0;
                            increasing = true;
                        }
                    }
                }

                Color endColor = new Color32(255, 255, 255, blinkCount);
                
                if(skeletonMaterial != null){
                    skeletonMaterial.color = endColor; 
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag == "Player") {
                if(weaponType != 4) {
                    collision.gameObject.GetComponent<Player>().WeaponChange(weaponType);
                }

                if(stoneValue > 0 ){
                    //재화 증가
                    UiController.Instance.integerUseSet(stoneValue,"+");
                }
                Destroy(gameObject);
            }

        }
    }
}
