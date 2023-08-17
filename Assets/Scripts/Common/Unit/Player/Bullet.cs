using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class Bullet : MonoBehaviour
    {
        //총알 사거리
        public int weaponType;
        public string weaponAttackType;
        public float physicsAttack;
        public float magicAttack;
        public float range; // 사거리
        public Vector3 initialPosition; // 초기 위치
        public float _bulletSpeed; // 총알 속도

        public GameObject bulletDotAni;
        public GameObject shotgunAni;

        private int targetCount = 0; // 타겟 카운트
        Rigidbody2D rigidbody2D;

        private bool activeflag = false;

        private float delayTime = 0.0f;

        void Start() {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void setWeaponEffect(int weaponID) {
            bulletDotAni.SetActive(true);
            shotgunAni.SetActive(false);
            if(weaponID == 1) {
                bulletDotAni.GetComponent<SkeletonAnimation>().AnimationName = "bullet1";
            } else if(weaponID == 2) {
                bulletDotAni.SetActive(false);
                shotgunAni.SetActive(true);
                //shotgunAni.GetComponent<Animator>().Play("ShotgunEffect");
            } else if(weaponID == 3) {
                bulletDotAni.GetComponent<SkeletonAnimation>().AnimationName =  "bullet1";
            } else {
                bulletDotAni.GetComponent<SkeletonAnimation>().AnimationName =  "bullet2";
            }
        }


        void EffectAnimation() {
          
            if(weaponAttackType.Equals("NORMAL")) {
                activeflag = true;
                bulletDotAni.SetActive(false);
                Destroy(gameObject);
            }
        
            
        }


        void Update () {
           // Debug.Log("weaponAttackType : " + weaponAttackType);
            if(weaponAttackType.Equals("DIFFUS")) {
                delayTime += Time.deltaTime;
                if(delayTime >= 1.0f) {
                    delayTime = 0.0f;
                    Destroy(gameObject);
                }
            } else {
                if (Vector3.Distance(transform.position, initialPosition) >= range)
                {
                    Destroy(gameObject); // 총알 삭제
                } else {
                    if(!activeflag) {
                        // 일정한 속도로 위쪽으로 이동
                        transform.Translate(Vector2.right * _bulletSpeed * Time.deltaTime);
                    }
                }
            }
        }




        private void OnTriggerEnter2D(Collider2D collision) {
            if(weaponType != 2 && collision.gameObject.tag != "Untagged" && collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Destroy(gameObject);
            }
            if(!activeflag) {
                if(collision.GetComponent<Enemy>()) {
                    if(!collision.GetComponent<Enemy>().isDead) {    
                        if(targetCount == 0 && weaponAttackType.Equals("NORMAL")) {
                            collision.GetComponent<Enemy>().DamageProcess(physicsAttack,0.0f,0.0f);
                            targetCount++;
                            EffectAnimation();
                           
                        } else {
                            collision.GetComponent<Enemy>().DamageProcess(physicsAttack,0.0f,0.0f);
                            EffectAnimation();
                        }
                    }
                }
            }

            if(collision.gameObject.tag == "Tutorial") {
                Destroy(gameObject);
            }
        }
    }
}
