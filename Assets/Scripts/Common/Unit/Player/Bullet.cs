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


        void Update (){
             if (Vector3.Distance(transform.position, initialPosition) >= range)
            {
                 Destroy(gameObject); // 총알 삭제
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.GetComponent<Enemy>()) {
                collision.GetComponent<Enemy>().DamageProcess(attack);
                Destroy(gameObject);
            }

            if(collision.gameObject.tag == "Tutorial") {
                Destroy(gameObject);
            }
        }
    }
}
