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
        public GameObject bulletDot; // 총알
        public float _bulletSpeed; // 총알 속도
        public GameObject MainObject;

        private int targetCount = 0; // 타겟 카운트
        Rigidbody2D rigidbody2D;

        private bool activeflag = false;


        void Start() {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void EffectAnimation() {
            activeflag = true;
            GameObject instantiatedPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect2"),  transform);
            instantiatedPrefab.transform.localScale = new Vector3(15f, 15f, 1f);
            instantiatedPrefab.SetActive(true);
            bulletDot.SetActive(false);
        
            StartCoroutine(objectEnd(instantiatedPrefab));
        }


        void Update () {
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

        IEnumerator objectEnd(GameObject instantiatedPrefab) {
            yield return new WaitForSeconds(1.0f);
            Destroy(bulletDot);
            Destroy(MainObject);
            Destroy(instantiatedPrefab);
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            Debug.Log(collision.gameObject.tag);
            if(collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Debug.Log(collision.gameObject.tag);
                Destroy(gameObject);
            }
            if(!activeflag) {
                if(collision.GetComponent<Enemy>()) {
                    if(!collision.GetComponent<Enemy>().isDead) {    
                        if(targetCount == 0) {
                            collision.GetComponent<Enemy>().DamageProcess(attack);
                            targetCount++;
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
