using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace nightmareHunter {
    public class Enemy : MonoBehaviour
    {
        public Rigidbody2D _target;
        public GameObject waypoints;


        List<Transform> _waypointList = new List<Transform>();
        bool isLive;
        string activateStatus = "stop";
        int waypointIndex = 0;

        [SerializeField]
        private Animator _animator;

        private Rigidbody2D _rigidbody;


        // 몬스터 능력치 
        public float _speed;
        public float _hp;
        public float _attack;
        public float _attackSpeed;
        public float _attackRange;
        public int _integer;
        public Vector2 _positionInfo;

        private Coroutine damageCoroutine;


        private void Awake() {
            isLive = true;
            _rigidbody = GetComponent<Rigidbody2D>();
            Transform parentTransform = waypoints.transform;

            foreach (Transform childTransform in parentTransform)
            {
                _waypointList.Add(childTransform);
            }   

        }

        public void initState(PlayerInfo playerinfo) {
            activateStatus = "move";

            _speed = playerinfo.move;
            if(_speed <= 0) {
                _speed = 1.0f;
            }
            _hp = playerinfo.health;
            _attack = playerinfo.attack;
            _attackSpeed = playerinfo.attackSpeed;
            _attackRange = playerinfo.attackRange;
            _integer = playerinfo.reward;
        }

 

        private void FixedUpdate() {
            if(!isLive)
                return;

            MonsterMoveProcess();
        }

        void MonsterMoveProcess() {
            //if("move".Equals(activateStatus)) {
                _animator.SetFloat("move", 1f);
     
                if(waypointIndex < _waypointList.Count) {
                    transform.position = Vector2.MoveTowards (transform.position, _waypointList[waypointIndex].transform.position, _speed * Time.fixedDeltaTime);
            
                    if(transform.position == _waypointList[waypointIndex].transform.position) {
                        waypointIndex += 1;
                    }
                }
            //}
        }

        private void LateUpdate() {
            if(!isLive)
                return;

            if(_target.position.x < _rigidbody.position.x) {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        
        }

        public void DamageProcess(float damage) {
            _hp = _hp - damage;

            if(_hp <= 0) {
                activateStatus = "dead";
                UiController.Instance.integerAddSet(_integer);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.GetComponent<Target>()) {
                activateStatus = "attack";
                StartCoroutine(ApplyDamage(collision.gameObject));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(!"dead".Equals(activateStatus)) {    
                if(collision.GetComponent<Target>()) 
                {
                    activateStatus = "move";
                    if(activateStatus != null) {
                        StopCoroutine(damageCoroutine);
                    }
                }
            }
        }


        private IEnumerator ApplyDamage(GameObject collisionObject)
        {
            while ("attack".Equals(activateStatus))
            {
                yield return new WaitForSeconds(_attackSpeed);
                collisionObject.GetComponent<Target>().DamageProcess(_attack);
            }
        }
    }
}