using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class SummonBullet : MonoBehaviour
    {
        public float range; // 사거리
        public Vector3 initialPosition; // 초기 위치
        public Vector3 targetPosition; // 타겟 위치
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
            if("CWP".Equals(attackType))
            {
                delayTime += Time.deltaTime;
                if(delayTime >= 1.0f) {
                    delayTime = 0.0f;
                    Destroy(gameObject);
                }
            } 
            if("FSR".Equals(attackType)) {
                if (Vector3.Distance(transform.position, initialPosition) >= range)
                {
                    Destroy(gameObject); // 총알 삭제
                } else {
                    // 타겟 위치와 현재 위치의 차이 계산
                    Vector3 targetDirection = (targetPosition - transform.position).normalized;

                    // 타겟 방향으로 회전
                    float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    // 총알 이동
                    transform.Translate(targetDirection * _bulletSpeed * Time.deltaTime);
                }
            }
            if("FSP".Equals(attackType))
            {
                
                // 일정한 속도로 위쪽으로 이동
                transform.Translate(Vector3.right  * _bulletSpeed * Time.deltaTime);
                Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            
                if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    Destroy(gameObject);
                }
                return;
            }
            
        }


        private void OnTriggerEnter2D(Collider2D collision) {
            if(!"FSP".Equals(attackType) && collision.gameObject.tag != "Untagged" && collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Summon" ) {
                Destroy(gameObject);
            }

            if(collision.GetComponent<Enemy>()) {
                if(!collision.GetComponent<Enemy>().isDead) {  
                    if("FSR".Equals(attackType)) {
                        GameObject exflow = Resources.Load<GameObject>("Prefabs/Bullet/ShotExflow");
                        exflow.GetComponent<SummonBullet>().attackType = "CWP";
                        exflow.GetComponent<SummonBullet>().initialPosition = transform.position;
                        exflow.GetComponent<SummonBullet>().physicsAttack = physicsAttack;  //물리공격력
                        exflow.GetComponent<SummonBullet>().magicAttack = magicAttack;  //마법 공격력
                        exflow.GetComponent<SummonBullet>().energyAttack = energyAttack; //에너지 공격력 
                        Instantiate(exflow, transform.position , transform.rotation);
                        Destroy(gameObject);
                    } else {
                        collision.GetComponent<Enemy>().DamageProcess(physicsAttack,magicAttack,energyAttack);
                    }
                }
            }
        }    

    }
}