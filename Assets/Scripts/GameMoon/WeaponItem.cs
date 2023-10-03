using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;


namespace nightmareHunter {
    public class WeaponItem : MonoBehaviour
    {

        private SkeletonMecanim skeletonMecanim;
        private Animator animator;
        
        public int weaponType;

        private bool isBlinking = false; // 깜박이는 중인지 여부

        private float stateTime = 7f; // 파괴되기까지의 대기 시간

        private float elapsedTime = 0f; // 경과한 시간
        private bool increasing = true; // 알파 값 증가 여부

        byte blinkCount = 255; // 깜박이를 위한 수치



        private void Start() {
            skeletonMecanim = gameObject.GetComponent<SkeletonMecanim>();
            animator = skeletonMecanim.GetComponent<Animator>();
        }


        public void SetWeaponType(int type) {
            weaponType = type;

            animator.SetInteger("GunType", type);
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime; // 경과 시간 증가
            if (elapsedTime >= stateTime)
            {
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

                skeletonMecanim.skeleton.SetColor(endColor);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag == "Player") {
                Debug.Log(collision.gameObject.tag);
                collision.gameObject.GetComponent<Player>().WeaponChange(weaponType);
                
                Destroy(gameObject);
            }

        }
    }
}
