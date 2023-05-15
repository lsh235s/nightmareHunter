using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class Bullet : MonoBehaviour
    {
        //총알 사거리
        public float attack;
        public float range; // 사거리
        public Vector3 initialPosition; // 초기 위치

        private int targetCount = 0; // 타겟 카운트

        void Update (){
             if (Vector3.Distance(transform.position, initialPosition) >= range)
            {
                 Destroy(gameObject); // 총알 삭제
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Destroy(gameObject);
            }

            if(collision.GetComponent<Enemy>()) {
                if(targetCount == 0) {
                    collision.GetComponent<Enemy>().DamageProcess(attack);
                    targetCount++;
                }
                Destroy(gameObject);
            }

            if(collision.gameObject.tag == "Tutorial") {
                Destroy(gameObject);
            }
        }
    }
}
