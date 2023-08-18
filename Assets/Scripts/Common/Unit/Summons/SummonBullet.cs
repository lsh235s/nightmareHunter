using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class SummonBullet : MonoBehaviour
    {
        public float range; // 사거리
        public Vector3 initialPosition; // 초기 위치
        private float delayTime = 0.0f;
        public float _bulletSpeed; // 총알 속도
        public float _bulletDamage; // 총알 데미지
        public string attackType; //발사체 유형
        public float physicsAttack;  //물리공격력
        public float magicAttack;  //마법 공격력
        public float energyAttack; //에너지 공격력

        
        private bool activeflag = false;

        Rigidbody2D rigidbody2D;
        
        
        // Start is called before the first frame update
        void Start()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }


        // Update is called once per frame
        void Update()
        {
            if("CDN".Equals(attackType))
            {
                delayTime += Time.deltaTime;
                if(delayTime >= 1.0f) {
                    delayTime = 0.0f;
                    Destroy(gameObject);
                }
            } 
            if("FSP".Equals(attackType))
            {
                
                // 일정한 속도로 위쪽으로 이동
                transform.Translate(Vector3.forward  * _bulletSpeed * Time.deltaTime);
                Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            
                if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    Destroy(gameObject);
                }
                return;
            }
            if("CWP".Equals(attackType))
            {
                delayTime += Time.deltaTime;
                if(delayTime >= 1.0f) {
                    delayTime = 0.0f;
                    Destroy(gameObject);
                }
            } 
        }


        private void OnTriggerEnter2D(Collider2D collision) {
            if(!"FSP".Equals(attackType) && collision.gameObject.tag != "Untagged" && collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Destroy(gameObject);
            }

            if(collision.GetComponent<Enemy>()) {
                if(!collision.GetComponent<Enemy>().isDead) {    
                    switch(attackType) {
                        case "CDN" :
                            collision.GetComponent<Enemy>().DamageProcess(physicsAttack,magicAttack,energyAttack);
                        break;
                        case "FSP" :
                            collision.GetComponent<Enemy>().DamageProcess(physicsAttack,magicAttack,energyAttack);
                        break;
                    }
                }
            }
        }    

    }
}