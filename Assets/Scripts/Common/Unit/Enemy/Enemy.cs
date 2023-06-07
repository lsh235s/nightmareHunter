using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;

namespace nightmareHunter {
    public class Enemy : MonoBehaviour
    {
        public Rigidbody2D _target;
        public int waypointType;
        public GameObject[] wayPointBaseList;

        [SerializeField]
        private GameObject _skeletonObject;


        List<List<Transform>> _waypointList = new List<List<Transform>>();
        bool isLive;
        public string activateStatus = "stop";
        int waypointIndex = 0;
        private Rigidbody2D _rigidbody;

        
        private SkeletonMecanim skeletonMecanim;
        private Animator _animator;


        // 몬스터 능력치 
        public float _speed;
        public float _hp;
        public float _attack;
        public float _attackSpeed;
        public float _attackRange;
        public int _integer;
        public string _spritesName;
        public Vector2 _positionInfo;
        public bool isDead = false;

        private Coroutine damageCoroutine;

        bool isFalling = false;

        // 이펙트 게임 오브젝트
        GameObject instantiatedPrefab;

        private void Awake() {
            isLive = true;
            _rigidbody = GetComponent<Rigidbody2D>();
            skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
            _animator = _skeletonObject.GetComponent<Animator>();
            instantiatedPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect1"),  transform);
            instantiatedPrefab.SetActive(false); 

            for(int i=0; i < wayPointBaseList.Length; i++) {
                Transform parentTransform = wayPointBaseList[i].transform;
                List<Transform> inputPoint = new List<Transform>();

                foreach (Transform childTransform in parentTransform)
                {
                    inputPoint.Add(childTransform);
                }  

                _waypointList.Add(inputPoint); 
            }
            

            activateStatus = "move";
        }

        public void initState(PlayerInfo playerinfo) {
           

            _speed = playerinfo.move;
            if(_speed <= 0) {
                _speed = 1.0f;
            }
            _hp = playerinfo.health;
            _attack = playerinfo.attack;
            _attackSpeed = playerinfo.attackSpeed;
            _attackRange = playerinfo.attackRange;
            _integer = playerinfo.reward;
            _spritesName = playerinfo.spritesName;
        }

 

        private void FixedUpdate() {
            if(!isLive)
                return;

            if (isFalling)
            {  
                StartCoroutine(damageShake());
            }    

            if("move".Equals(activateStatus)) {
                MonsterMoveProcess();
            }
        }

        void MonsterMoveProcess() {
            // if(waypointIndex > 0) {
            //     waypointType = Random.Range(0, 3);
            // }
            _animator.SetFloat("move", 1f);
    

            if(waypointIndex < _waypointList[waypointType].Count) {
                transform.position = Vector2.MoveTowards (transform.position, _waypointList[waypointType][waypointIndex].transform.position, _speed * Time.fixedDeltaTime);
        
                if(transform.position == _waypointList[waypointType][waypointIndex].transform.position) {
                    waypointIndex += 1;
                }
            }
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
            if(!"die".Equals(activateStatus)) {
                int randomNumber = Random.Range(0, 4);

                if(randomNumber == 0) {
                    Vector3 rePosition = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                    instantiatedPrefab.transform.position = rePosition;

                    if(_target.position.x < _rigidbody.position.x) {
                        instantiatedPrefab.transform.rotation = Quaternion.Euler(0, 180f, 0);
                    } else {
                        instantiatedPrefab.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    instantiatedPrefab.SetActive(true);   
                    float clipLength = instantiatedPrefab.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                    StartCoroutine(objectEnd(clipLength));
                }
              
                _hp = _hp - damage;

                isFalling = true;
                gameObject.GetComponent<AudioSource>().clip = AudioManager.Instance.playSound[_spritesName+"_0"];
                gameObject.GetComponent<AudioSource>().Play();

                if(_hp <= 0) {
                    UiController.Instance.integerAddSet(_integer);
                    _animator.SetTrigger("die");
                    activateStatus = "die";
                    StartCoroutine(MonsterDie()); 
                }
            }
        }

        IEnumerator objectEnd(float clipLength ) {
            yield return new WaitForSeconds(clipLength);
            instantiatedPrefab.SetActive(false); 
            yield return null;
        }


        //대상 일정 공속 공격
        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.GetComponent<Target>()) {
                activateStatus = "targetAttack";
                damageCoroutine = StartCoroutine(ApplyDamage(collision.gameObject));
            }
        }

        private IEnumerator MonsterDie() {
            yield return new WaitForSeconds(1.5f);
            isDead = true;
            Destroy(gameObject);
        }


        // 주인공 충돌 처리
        public void MonsterAttackProcess() {
            StartCoroutine(MonsterAttack());
        }

        private IEnumerator MonsterAttack() {
            _animator.SetTrigger("atk");
            if(!"targetAttack".Equals(activateStatus)) {
                activateStatus = "attack";
                yield return new WaitForSeconds(1.0f);
                activateStatus = "move";
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
            while ("targetAttack".Equals(activateStatus))
            {
                yield return new WaitForSeconds(_attackSpeed);
                _animator.SetTrigger("atk");
                collisionObject.GetComponent<Target>().DamageProcess(_attack);
            }
        }

        private IEnumerator damageShake() {
            skeletonMecanim.transform.position  += new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            Color endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  += new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  -= new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);

            isFalling = false;

            yield return null;
        }
    }
}