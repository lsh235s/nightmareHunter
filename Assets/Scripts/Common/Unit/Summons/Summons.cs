using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class Summons : MonoBehaviour
    {
        public LayerMask targetLayer;
        public RaycastHit2D[] targets;
        public Transform nearestTarget;


        public PlayerInfo _playerinfo;
        // 소환수 능력치 
        string activateStatus = "stop";
        public float _speed;
        public float _hp;
        public float _attack;
        public float _attackSpeed;
        public float _attackRange;
        public int _integer;
        public string _positionString;
        public Vector2 _positionInfo;
        public bool summonsExist;
        public string summonsName;


        private float nextTime = 0.0f;  // 다음 공격 시간
     

        public CircleCollider2D collider2D;
        public GameObject rangeObject;

     

        public void playerDataLoad(PlayerInfo inPlayerinfo) {
            activateStatus = "move";

            _speed = inPlayerinfo.move;
            if(_speed <= 0) {
                _speed = 1.0f;
            }
            _hp = inPlayerinfo.health;
            _attack = inPlayerinfo.attack;
            _attackSpeed = inPlayerinfo.attackSpeed;
            _attackRange = inPlayerinfo.attackRange;
            _integer = inPlayerinfo.reward;
            _positionString = inPlayerinfo.positionInfo;
            _playerinfo = inPlayerinfo;

            collider2D.radius = _attackRange;
         
            float spriteScale = (_attackRange * 10.0f) + _attackRange; // 스케일 값을 계산
            Debug.Log("spriteScale : " + spriteScale);
            rangeObject.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(spriteScale, spriteScale, 1f); // 스케일 값을 적용
            
        }

        // Start is called before the first frame update
        void FixedUpdate()
        {
            if(UiController.Instance.sceneMode == 1) { // 저녁시간에만 몬스터 스캔
                scanRadar();
                if(nearestTarget != null) {
                    activateStatus = "attack";
                    nextTime = nextTime + Time.deltaTime;
                    if(nextTime > _attackSpeed) {
                        ApplyDamage(nearestTarget.gameObject);
                        nextTime = 0.0F;
                        nearestTarget = null;
                    }
                } else {
                    activateStatus = "move";
                }
            }
        }


        // 물리 판정이 아닌 단순 거리 계산 공격
        void targetsAttack() {
            if(nearestTarget != null) {
                gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                nearestTarget = null;
            }
        }

        // 타겟 대상 스캔
        void scanRadar() {
            targets = Physics2D.CircleCastAll(transform.position, _attackRange, Vector2.zero, 0,targetLayer);
 
            nearestTarget = GetNearest();

            if(nearestTarget != null) {
                if(nearestTarget.position.x < transform.GetComponent<Rigidbody2D>().position.x) {
                    transform.rotation = Quaternion.Euler(0, 180f, 0);
                } else {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
        
        Transform GetNearest() {
            Transform result = null;
            float diff = _attackRange;

            foreach (RaycastHit2D target in targets)
            {
                Vector3 myPost = transform.position;
                Vector3 targetPos = target.transform.position;

     
                float curDiff = Vector3.Distance(myPost,targetPos);
                if(curDiff < diff) {
                    diff = curDiff;
                    result = target.transform;
                }
            }

            return result;
        }


        // private void OnTriggerEnter2D(Collider2D collision) {
        //     if(nearestTarget != null) {
        //         activateStatus = "attack";
        //         damageCoroutine = StartCoroutine(ApplyDamage(nearestTarget.gameObject));
        //     }
        // }

        // private void OnTriggerExit2D(Collider2D collision)
        // {
        //     if(!"dead".Equals(activateStatus)) {    
        //          if(collision.GetComponent<Enemy>()) {
        //             activateStatus = "move";
        //             if(activateStatus != null) {
        //                 StopCoroutine(damageCoroutine);
        //             }
        //         }
        //     }
        // }

        private void ApplyDamage(GameObject collisionObject)
        {
            if(collisionObject.GetComponent<Enemy>() != null) {
                gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                collisionObject.GetComponent<Enemy>().DamageProcess(_attack);
                nearestTarget = null;
            }
        }

    }
}
