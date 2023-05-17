using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class Bullet : MonoBehaviour
    {
        //총알 사거리
        public float attack;
        public float range; // 사거리
        public Vector3 initialPosition; // 초기 위치
        public GameObject effectObject; // 타겟
        public GameObject bulletDot; // 총알
        public GameObject MainObject;
        public Animator logoAnimation;

        private int targetCount = 0; // 타겟 카운트
        Rigidbody2D rigidbody2D;

        private bool activeflag = false;


        void Start() {
            rigidbody2D = GetComponent<Rigidbody2D>();
            effectObject.SetActive(false);
        }

        void EffectAnimation() {
            if(logoAnimation != null) {
                bulletDot.SetActive(false);
                effectObject.SetActive(true);
                logoAnimation.SetTrigger("active");
                activeflag = true;
                StartCoroutine(objectEnd());
            }
        }


        void Update (){
             if (Vector3.Distance(transform.position, initialPosition) >= range)
            {
                 Destroy(gameObject); // 총알 삭제
            }
        }

        IEnumerator objectEnd() {
            yield return new WaitForSeconds(1.0f);
            Destroy(bulletDot);
            Destroy(effectObject);
            Destroy(logoAnimation);
            Destroy(MainObject);
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Destroy(gameObject);
            }
            if(!activeflag) {
                if(collision.GetComponent<Enemy>()) {
                    if(!collision.GetComponent<Enemy>().isDead) {    
                        if(targetCount == 0) {
                            collision.GetComponent<Enemy>().DamageProcess(attack);
                            targetCount++;
                        }
                        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                        EffectAnimation();
                    }
                }
            }

            if(collision.gameObject.tag == "Tutorial") {
                Destroy(gameObject);
            }
        }
    }
}
