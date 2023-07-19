using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Spine.Unity;
using TMPro;

namespace nightmareHunter {
    public class Enemy : MonoBehaviour
    {
        public GameObject clientTarget;
        public GameObject playerTarget;
        public int waypointType;
        public GameObject[] wayPointBaseList;

        [SerializeField]
        private GameObject _skeletonObject;

        List<List<Transform>> _waypointList = new List<List<Transform>>();
        bool isLive;
        int waypointIndex = 0; //웨이포인트 인덱스
        public int targetNum = -1; //클라이언트 타겟 일경우 이동 인덱스
        private Rigidbody2D _rigidbody;

        
        private SkeletonMecanim skeletonMecanim;
        private Animator _animator;

        // 몬스터 고정 능력치 
        public float _initSpeed;
        public float _initHp;
        public float _initAttack;
        public float _initAttackSpeed;
        public float _initAttackRange;


        // 몬스터 가변 능력치 
        public int _monsterId;
        public float _speed;
        public float _hp;
        public float _attack;
        public float _attackSpeed;
        private float lastAttackTime = 0.0f; // 이전 공격 시간
        public float _attackRange;
        public int _integer;
        public string _spritesName;
        public Vector2 _positionInfo;
        public bool isDead = false;

        private Coroutine damageCoroutine;

        // 이펙트 게임 오브젝트
        GameObject instantiatedPrefab;
        
        public Animator anim;  // 몬스터 애니메이션 제어
        bool isFalling = false; // 데이지 피격 여부
        
        Vector3 NextTargetPosition;  // 다음 목표지점 위치
        private float distanceCheck; // 현재 위치의 이동 범위
        int distanceCheckCount = 0;  // 현재 위치 머무른 횟수

        UnitController _uiItController;

        private NavMeshAgent agent;

        //열거형으로 정해진 상태값을 사용
        enum State
        {
            Idle,  //보통 상태
            Run,  //이동 상태
            ClientTracking, // 의뢰인 추적 상태
            ClientAttack, // 의뢰인 공격
            Tracking, //주인공 추적 상태
            PlayerAttack, //플레이어 공격
            Bored, // 지루함
            pause, // 일시정지
            Die //죽음
        }
  
        //상태 처리
        State state;
       
        public Dictionary<string, bool> skillList = new Dictionary<string, bool>();
    

        private void Start() {
            isLive = true;
            _rigidbody = GetComponent<Rigidbody2D>();
            skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
            _animator = _skeletonObject.GetComponent<Animator>();
            instantiatedPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect1"));
            instantiatedPrefab.SetActive(false); 

            skillList.Add("AttackUp", false);
            skillList.Add("AttackSpeedUp", false);
            skillList.Add("MoveSpeedUp", false);
            skillList.Add("ClientTargetFix", false);
            

            for(int i=0; i < wayPointBaseList.Length; i++) {
                Transform parentTransform = wayPointBaseList[i].transform;
                List<Transform> inputPoint = new List<Transform>();

                foreach (Transform childTransform in parentTransform)
                {
                    inputPoint.Add(childTransform);
                }  

                _waypointList.Add(inputPoint); 
            }

            if(_monsterId == 1) {
                _animator.SetTrigger("Attack");
                gameObject.GetComponent<EnemySkill>().skillUse("TellerCry");
            } else if (_monsterId == 3) {
                gameObject.GetComponent<EnemySkill>().skillUse("Cloaking");
            }
            _animator.SetTrigger("Idle");

            agent = GetComponent<NavMeshAgent>();
            
        }

        public void initState(PlayerInfo playerinfo) {
            _speed = playerinfo.move;
            // if(_speed <= 0) {
            //     _speed = 1.0f;
            // }
            _hp = playerinfo.health;
            _attack = playerinfo.attack;
            _attackSpeed = playerinfo.attackSpeed;
            _attackRange = playerinfo.attackRange;
            _integer = playerinfo.reward;
            _spritesName = playerinfo.spritesName;

            _initSpeed = _speed;
            _initHp = _hp;
            _initAttack = _attack;
            _initAttackSpeed = _attackSpeed;
            _initAttackRange = _attackRange;

        }

 

        private void FixedUpdate() {
            if(state != State.Die) {
                if (isFalling)
                {  
                    StartCoroutine(damageShake());
                }    
                buffJudge();

                EnemyActJudge();
               
            }
        }

        private void buffJudge() {
            foreach (KeyValuePair<string, bool> kvp in skillList)
            {
                if (kvp.Value == true)
                {
                    switch(kvp.Key)
                    {
                        case "AttackUp":
                            _attack = _initAttack + (_initAttack * 2f);
                            break;
                        case "AttackSpeedUp":
                            _attackSpeed = _initSpeed + (_initSpeed * 2f);
                            break;
                        case "MoveSpeedUp":
                            _speed = _initSpeed + (_initSpeed * 2f);
                            break;
                        case "ClientTargetFix":
                            state = State.ClientTracking;
                            break;
                    }
                }
            }

        }

        private void EnemyActJudge() {
            
            agent.speed = _speed;
            if(_monsterId != 1) {
                if(state == State.Idle || state == State.Run || state == State.Bored) {
                    AttackRadar();  // 공격 대상 판단 
                }

                if (state == State.Idle) // 대기중 일때 다음 이동 지점 판단
                {
                    UpdateIdle();
                }
                else if (state == State.Run) // 목표지점 이동중 일 때 정상적인 이동 판단
                {
                    UpdateRun();
                }
                else if (state == State.ClientTracking) // 의뢰인만 추적
                {
                    UpdateClientTracking();
                }
                else if (state == State.Tracking)  // 근처에 주인공이 존재시 추적 판단.
                {
                    UpdateTracking();
                }
                else if (state == State.Bored) // 대기 중 일때 다음 행동 판단
                {
                    UpdateBored();
                }
                else if (state == State.PlayerAttack) // 플레이어를 공격중 일때 판단
                {
                    UpdatePlayerAttack();
                }
                else if (state == State.ClientAttack) // 클라이언트를 공격중 일때 판단
                {
                    UpdateClientAttack();
                }
            } else {
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    _animator.SetTrigger("Attack");
                    gameObject.GetComponent<EnemySkill>().skillUse("TellerCry");
                    _animator.SetTrigger("Idle");

                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f; 
                }
            }
         
        }


        private void AttackRadar() {
            if(state != State.Die) {
                float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
                float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);

                if(PlayerDistance <= (_attackRange * 2)) {
                    NextTargetPosition = playerTarget.transform.position ;
                    state = State.Tracking;
                    _animator.SetTrigger("Run");
                } 

                if(PlayerDistance <= _attackRange) {
                    state = State.PlayerAttack;
                } else if(ClientDistance <= _attackRange) {
                    state = State.ClientAttack;
                } else if(ClientDistance > 1.0f && PlayerDistance > 1.0f && (state == State.PlayerAttack || state == State.ClientAttack)) {
                    lastAttackTime = 0.0f;
                    state = State.Idle;
                }
            }
        }

        private void UpdateClientTracking() {
            float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
            float TrackDistance = 0.0f;
          
            if(targetNum == -1) {
                for(int i = 0; i < _waypointList[4].Count; i++) {
                    if(i == 0) {
                        TrackDistance = Vector3.Distance(transform.position, _waypointList[4][i].transform.position);
                        targetNum = 0;
                    } else {
                        if(TrackDistance > Vector3.Distance(transform.position, _waypointList[4][i].transform.position)) {
                            TrackDistance = Vector3.Distance(transform.position, _waypointList[4][i].transform.position);
                            targetNum = i;
                        }
                    }
                }
            } else {
                float distance = Vector3.Distance(transform.position, NextTargetPosition);
                if (distance <= 0.5f)
                {
                    if(_waypointList[4].Count -1 > targetNum) {
                        targetNum++;
                    }
                } else if((Mathf.Round(distanceCheck * 1000f) / 1000f) == (Mathf.Round(distance  * 1000f) / 1000f)) {
                    distanceCheckCount++;
                    // if(distanceCheckCount > 4) {
                    //     if(_waypointList[4].Count -1 > targetNum) {
                    //         targetNum++;
                    //     }
                    // }
                } else {
                    distanceCheck = distance;
                    distanceCheckCount = 0;
                }
            }
            
            NextTargetPosition = _waypointList[4][targetNum].transform.position;
            if(ClientDistance <= _attackRange) {
                state = State.ClientAttack;
            }
            if(state == State.ClientTracking) {
               agent.SetDestination(NextTargetPosition); 
            }
        }

        private void UpdateTracking() {
            if(state != State.Die && _monsterId != 1) {
                float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
                float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);

                if(PlayerDistance > (_attackRange * 2)) {
                    NextTargetPosition = _waypointList[waypointType][waypointIndex].transform.position;
                    lastAttackTime = 0.0f;
                    state = State.Bored;
                    _animator.SetTrigger("Run");
                } 

                if(PlayerDistance <= _attackRange) {
                    state = State.PlayerAttack;
                } else if(ClientDistance <= _attackRange) {
                    state = State.ClientAttack;
                }

                if(state == State.Tracking) {
                    agent.SetDestination(NextTargetPosition); 
                }
            }
        }

        private void UpdatePlayerAttack()
        {   
            float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
            float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);

            if(ClientDistance > 1.0f && PlayerDistance > 1.0f && (state == State.PlayerAttack || state == State.ClientAttack)) {
                lastAttackTime = 0.0f;
                state = State.Idle;
            } else {
                if(lastAttackTime == 0.0f) {
                     _animator.SetTrigger("Attack");
                    // 공격 실행
                    playerTarget.GetComponent<Player>().OnEventPlayerDamage(_attack,transform.position); 
        
                }
                // 공격 타이머를 증가시킴
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f;
                }
            }
        }

        private void UpdateClientAttack()
        {
            skillList["ClientTargetFix"] = false;
            float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
            float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);

            if(ClientDistance > 1.0f && PlayerDistance > 1.0f && (state == State.PlayerAttack || state == State.ClientAttack)) {
                lastAttackTime = 0.0f;
                state = State.Idle;
            } else {
                if(lastAttackTime == 0.0f) {
                   _animator.SetTrigger("Attack");
                    // 공격 실행
                    clientTarget.GetComponent<Target>().DamageProcess(_attack); 
          
                }
                // 공격 타이머를 증가시킴
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f;
                }
            }
        }

        private void UpdateBored() {
            float distance = Vector3.Distance(transform.position, NextTargetPosition);
            if (distance <= 0.3f)
            {
                NextTargetPosition = _waypointList[waypointType][waypointIndex].transform.position;
                if(state != State.Die) {
                    state = State.Run;
                    _animator.SetTrigger("Run");
                }
            } else if((Mathf.Round(distanceCheck * 1000f) / 1000f) == (Mathf.Round(distance * 1000f) / 1000f)) {
                distanceCheckCount++;
                // if(distanceCheckCount > 4) {
                //     Vector3 randomDirection = Random.insideUnitSphere;
                //     // 방향 벡터를 정규화하여 단위 벡터로 만듦
                //     randomDirection.Normalize();
                //     NextTargetPosition = transform.position + randomDirection * 0.5f;
                //     distanceCheckCount = 0;
                // }
            } else {
                distanceCheck = distance;
                distanceCheckCount = 0;
            }

            if(state == State.Bored) {
                agent.SetDestination(NextTargetPosition); 
            }
        }

        private void UpdateRun()
        {
            int randomNumber = Random.Range(0, 1000);

            // if(randomNumber == 0) { //랜덤으로 딴짓을 하게 만든다.
            //     Vector3 randomDirection = Random.insideUnitSphere;
            //     // 방향 벡터를 정규화하여 단위 벡터로 만듦
            //     randomDirection.Normalize();
            //     NextTargetPosition = transform.position + randomDirection * 0.5f;
            // } else {
                //남은 거리가 0.3f 라면 목적지에 도착 한것으로 판단
                float distance = Vector3.Distance(transform.position, NextTargetPosition);
                if (distance <= 0.3f)
                {
                    if(state != State.Die) {
                        state = State.Idle;
                        _animator.SetTrigger("Idle");
                    }
                } else if((Mathf.Round(distanceCheck * 1000f) / 1000f) == (Mathf.Round(distance * 1000f) / 1000f)) {
                    distanceCheckCount++;
                    // if(distanceCheckCount > 4) {
                    //     Vector3 randomDirection = Random.insideUnitSphere;
                    //     // 방향 벡터를 정규화하여 단위 벡터로 만듦
                    //     randomDirection.Normalize();
                    //     NextTargetPosition = transform.position + randomDirection * 0.5f;
                    //     distanceCheckCount = 0;
                    // }
                    if(state != State.Die) {
                        state = State.Bored;
                    }
                } else {
                    distanceCheck = distance;
                    distanceCheckCount = 0;
                }

         //   }
            if(state == State.Run) {
                agent.SetDestination(NextTargetPosition); 
            }
        }

        private void UpdateIdle()
        {
            int randomNumber = 0;

            // 첫번째 목적지가 아닐 경우 이동 할지 움직일지 결정
            if(waypointIndex > 0) {
                randomNumber = Random.Range(0, 3);
            }
 
            if(randomNumber == 0) {
                if(_waypointList[waypointType].Count - 1 > waypointIndex) { 
                    waypointIndex += 1;
                }
                NextTargetPosition = _waypointList[waypointType][waypointIndex].transform.position;
                if(state != State.Die) {
                    state = State.Run;
                }
                _animator.SetTrigger("Run");
            } else {
                Vector3 randomDirection = Random.insideUnitSphere;
                // 방향 벡터를 정규화하여 단위 벡터로 만듦
                randomDirection.Normalize();
                NextTargetPosition = transform.position + randomDirection * 0.3f;
                if(state != State.Die) {
                    state = State.Bored;
                }
            }
        }

        IEnumerator objectEnd(GameObject weaponDamageEffect) {
            yield return new WaitForSeconds(1.0f);
            Destroy(weaponDamageEffect);
            yield return null;
        }



        public void DamageProcess(float damage) {
            if(state != State.Die) { 
                Vector3 rePosition = new Vector3(transform.position.x, transform.position.y , transform.position.z);
                GameObject weaponDamageEffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect2"),  rePosition, Quaternion.identity);
                //instantiatedPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                weaponDamageEffect.SetActive(true);

                StartCoroutine(objectEnd(weaponDamageEffect));

                int randomNumber = Random.Range(0, 4);

                if(randomNumber == 0) {
                    rePosition = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                    instantiatedPrefab.transform.position = rePosition;

                    if(clientTarget.transform.position.x < _rigidbody.position.x) {
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
                if(_spritesName.Equals("Teller") &&  _spritesName.Equals("wanderer") ) {
                    if(AudioManager.Instance.playSound[_spritesName+"_0"] != null) {
                        gameObject.GetComponent<AudioSource>().clip = AudioManager.Instance.playSound[_spritesName+"_0"];
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                }
               

                if(_hp <= 0) {
                    Debug.Log("몬스터 사망 처리");
                    int weaponNum = Random.Range(0, 3);

                    GameObject weaPonItem = Instantiate(UiController.Instance.dropItem, transform.position, transform.rotation);
                    weaPonItem.GetComponent<WeaponItem>().SetWeaponType(weaponNum + 1);

                    UiController.Instance.integerAddSet(_integer);
                    Collider2D collider = gameObject.GetComponent<Collider2D>();
                    collider.isTrigger = true;
                    state = State.Die;
                    _animator.SetTrigger("Die");
                    if(_monsterId == 1) {
                        gameObject.GetComponent<EnemySkill>().skillEnd("TellerCry");
                    }
                    StartCoroutine(MonsterDie()); 
                }
            }
        }

        public void stateMod(string stateVal) {
            switch(stateVal)
            {
                case "Idle":
                    state = State.Idle;
                    break;
            }
        }

        IEnumerator objectEnd(float clipLength ) {
            yield return new WaitForSeconds(clipLength);
            instantiatedPrefab.SetActive(false); 
            yield return null;
        }


        // //대상 일정 공속 공격
        // private void OnTriggerEnter2D(Collider2D collision) {
        //     if(collision.GetComponent<Target>()) {
        //         activateStatus = "targetAttack";
        //         damageCoroutine = StartCoroutine(ApplyDamage(collision.gameObject));
        //     }
        // }

        
        // private void OnTriggerExit2D(Collider2D collision)
        // {
        //     if(!"dead".Equals(activateStatus)) {    
        //         if(collision.GetComponent<Target>()) 
        //         {
        //             activateStatus = "move";
        //             if(activateStatus != null) {
        //                 StopCoroutine(damageCoroutine);
        //             }
        //         }
        //     }
        // }


        //         private IEnumerator ApplyDamage(GameObject collisionObject)
        // {
        //     while ("targetAttack".Equals(activateStatus))
        //     {
        //         yield return new WaitForSeconds(_attackSpeed);
        //         _animator.SetTrigger("atk");
        //         collisionObject.GetComponent<Target>().DamageProcess(_attack);
        //     }
        // }


        private IEnumerator MonsterDie() {
            yield return new WaitForSeconds(1.5f);
            isDead = true;
            gameObject.SetActive(false);
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

            if (_monsterId == 3) {
                gameObject.GetComponent<EnemySkill>().skillUse("Cloaking");
            }

            yield return null;
        }
    }
}