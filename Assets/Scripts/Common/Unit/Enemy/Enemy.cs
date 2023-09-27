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
        public float _initphysicsAttack;
        public float _initMagicAttack;
        public float _initpysicsDefense;
        public float _initMagicDefense;
        public float _initAttackSpeed;
        public float _initAttackRange;

        // 몬스터 가변 능력치 
        public int _monsterId;
        public float _speed;
        public float _hp;
        public float _physicsAttack;
        public float _magicAttack;
        public float _pysicsDefense;
        public float _magicDefense;
        public float _attackSpeed;
        private float lastAttackTime = 0.0f; // 이전 공격 시간
        public float _attackRange;
        public float _attackRange2;
        public float _attackRange3;
        public float _attackRange4;
        public float _attackRange5;
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
            Attack, // 공격
            Tracking, //주인공 추적 상태
            Bored, // 지루함
            pause, // 일시정지
            Die //죽음
        }
  
        //상태 처리
        State state;
        public string stateName;
       
        public Dictionary<string, bool> skillList = new Dictionary<string, bool>();
    

        private void Start() {
            isLive = true;
            _rigidbody = GetComponent<Rigidbody2D>();
            skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
            _animator = _skeletonObject.GetComponent<Animator>();
            instantiatedPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect1"));
            instantiatedPrefab.SetActive(false); 

            skillList.Clear();

            if(skillList.Count == 0) {
                skillList.Add("AttackUp", false);
                skillList.Add("AttackSpeedUp", false);
                skillList.Add("MoveSpeedUp", false);
                skillList.Add("ClientTargetFix", false);
                skillList.Add("PlayerTargetFix", false);
                skillList.Add("PhysicsResistance", false);
                skillList.Add("MagicResistance", false);
                skillList.Add("Cloaking", false);
                skillList.Add("Split", false);
                skillList.Add("SummonAttackSpeddDown", false);
            }


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
            } else if (_monsterId == 2) {
                gameObject.GetComponent<EnemySkill>().skillUse("PlayerTarget");
            } else if (_monsterId == 3) {
                gameObject.GetComponent<EnemySkill>().skillUse("Split");    
            } else if (_monsterId == 5) {
                gameObject.GetComponent<EnemySkill>().skillUse("PhysicsResistance");
            } else if (_monsterId == 6) {
                gameObject.GetComponent<EnemySkill>().skillUse("MagicResistance");
            } else if (_monsterId == 13) {
                state = State.ClientTracking;
                stateName = "ClientTracking";
                gameObject.GetComponent<EnemySkill>().skillUse("ClientTargetFix");
            }
            
            _animator.SetTrigger("Idle");
            stateName = "Idle";

            agent = GetComponent<NavMeshAgent>();


            if(_monsterId == 2) {
                Color endColor = new Color32(0, 255, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 3) {
                Color endColor = new Color32(0, 0, 255, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 13) {
                Color endColor = new Color32(0, 0, 180, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 4) {
                Color endColor = new Color32(0, 100, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } 

            if(_monsterId != 1 && _monsterId != 13) {
                transform.position = _waypointList[waypointType][waypointIndex].transform.position;
            }
            
            
        }

        public void initState(PlayerInfo playerinfo) {
            _speed = playerinfo.move;
            // if(_speed <= 0) {
            //     _speed = 1.0f;
            // }
            _hp = playerinfo.health;
            _physicsAttack = playerinfo.physicsAttack; 
            _magicAttack = playerinfo.magicAttack;
            _pysicsDefense = playerinfo.pysicsDefense;
            _magicDefense = playerinfo.magicDefense;
            _attackSpeed = playerinfo.attackSpeed;
            _attackRange = playerinfo.attackRange;
            _integer = playerinfo.reward;
            _spritesName = playerinfo.spritesName;

            _initSpeed = _speed;
            _initHp = _hp;
            _initphysicsAttack = _physicsAttack;
            _initMagicAttack = _magicAttack;
            _initpysicsDefense = _pysicsDefense;
            _initMagicDefense = _magicDefense;
            _initAttackSpeed = _attackSpeed;
            _initAttackRange = _attackRange;

        }

 

        private void FixedUpdate() {
            if(state != State.Die) {
                agent.speed = _speed;
                if (isFalling)
                {  
                    StartCoroutine(damageShake());
                }    
                buffJudge();

                EnemyActJudge();
               
                // if (_monsterId == 2) { //클로킹 스킬 제거
                //     foundMonk();
                // }
            } else {
                agent.speed = 0;
                agent.isStopped = true;
            }
        }

        // private void foundMonk() {
        //     Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, 4f);
        //     foreach (Collider2D collider in colliders)
        //     {
        //         gameObject.GetComponent<EnemySkill>().skillUse("Cloaking");
        //         if (collider.CompareTag("Summon"))
        //         {
        //             if(collider.gameObject.GetComponent<Summons>() != null) {
        //                 if(collider.gameObject.GetComponent<Summons>().summonerId == 3){
        //                     gameObject.GetComponent<EnemySkill>().skillEnd("Cloaking");
        //                 }
        //             }
        //         }
        //     }
        // }

        // 버프처리
        private void buffJudge() {
            foreach (KeyValuePair<string, bool> kvp in skillList)
            {
                if (kvp.Value == true)
                {
                    switch(kvp.Key)
                    {
                        case "AttackUp":
                            _physicsAttack = _initphysicsAttack + (_initphysicsAttack * 2f);
                            break;
                        case "AttackSpeedUp":
                            _attackSpeed = _initSpeed + (_initSpeed * 2f);
                            break;
                        case "MoveSpeedUp":
                            _speed = _initSpeed + (_initSpeed * 2f);
                            break;
                        case "ClientTargetFix":
                            state = State.ClientTracking;
                            stateName = "ClientTracking";
                            break;
                        case "PlayerTargetFix":
                            NextTargetPosition = playerTarget.transform.position ;
                            state = State.Tracking;
                            stateName = "Tracking";
                            break;
                        case "Cloaking":
                            if(isFalling == false) {
                                Color endColor = new Color32(0, 0, 0, 50);
                                skeletonMecanim.skeleton.SetColor(endColor);
                            }
                            break;
                        case "SummonAttackSpeddDown":
                            Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, 4f);
                            foreach (Collider2D collider in colliders)
                            {
                                if (collider.CompareTag("Summon"))
                                {
                                    // 특정 태그를 가진 오브젝트를 찾았을 때의 동작을 수행합니다.
                                    // 이 예제에서는 콘솔에 오브젝트의 이름을 출력합니다.
                                    if(collider.gameObject.GetComponent<Summons>() != null) {
                                        collider.gameObject.GetComponent<Summons>().skillList["AttackSpeedDown"] = true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

        }

        private void EnemyActJudge() {
            if(_monsterId != 1) {
                AttackRadar();  // 공격 대상 판단 

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
            } else {
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    _animator.SetTrigger("Attack");
                    gameObject.GetComponent<EnemySkill>().skillUse("TellerCry");
                    _animator.SetTrigger("Idle");
                    state = State.Attack;
                    stateName = "Attack";

                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f; 
                }
            }
         
        }


        private void AttackRadar() {
            if(state != State.Die) {
                float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
                float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);
_attackRange5 = ClientDistance;
                if(PlayerDistance <= (_attackRange * 2)) {
                    NextTargetPosition = playerTarget.transform.position ;
                    state = State.Tracking;
                    stateName = "Tracking";
                    _animator.SetTrigger("Run");
                    stateName = "Run";
                } 

                if(PlayerDistance <= _attackRange) {
                    state = State.Attack;
                    stateName = "Attack";
                    UpdateAttack("Player");
                } else if(ClientDistance - 0.1f <= _attackRange) {
                    Debug.Log("ClientDistance : " + ClientDistance + " / " + _attackRange);
                    state = State.Attack;
                    stateName = "Attack";
                    UpdateAttack("Client");
                } else if(ClientDistance > 1.0f && PlayerDistance > 1.0f && state == State.Attack ) {
                    lastAttackTime = 0.0f;
                    stateName = "Idle";
                    state = State.Idle;
                }
            } else {
                agent.speed = 0;
                agent.isStopped = true;
            }
        }

        private void UpdateClientTracking() {
            float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
            float TrackDistance = 0.0f;
          _attackRange4 = ClientDistance;
            NextTargetPosition = clientTarget.transform.position;
            if(ClientDistance <= _attackRange) {
                if(lastAttackTime == 0.0f) {
                    _animator.SetTrigger("Attack");
                    // 공격 실행
                    state = State.Idle;
                    stateName = "Idle";
                    StartCoroutine(MonsterDie());
                    clientTarget.GetComponent<Target>().DamageProcess(_physicsAttack); 
                }
                // 공격 타이머를 증가시킴
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f;
                }
            } else {
                lastAttackTime = 0.0f;
                agent.SetDestination(NextTargetPosition); 
            }
        }

        private void UpdateTracking() {
            if(state != State.Die && _monsterId != 1) {
                float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
                float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);
                _attackRange3 = ClientDistance;
                if(PlayerDistance > (_attackRange * 2) && skillList["PlayerTargetFix"] == false) {
                    NextTargetPosition = _waypointList[waypointType][waypointIndex].transform.position;
                    lastAttackTime = 0.0f;
                    state = State.Idle;
                    stateName = "Idle";
                    _animator.SetTrigger("Run");
                    stateName = "Run";
                }  

                if(PlayerDistance <= _attackRange) {
                   state = State.Attack;
                   stateName = "Attack";
                   UpdateAttack("Player");
                } else if(ClientDistance  <= _attackRange) {
                   state = State.Attack;
                   stateName = "Attack";
                   UpdateAttack("Client");
                }

                if(state == State.Tracking) {
                    agent.SetDestination(NextTargetPosition); 
                }
            } else {
                agent.speed = 0;
                agent.isStopped = true;
            }
        }

        private void UpdateAttack(string target) 
        {   
            float ClientDistance = Vector3.Distance(transform.position, clientTarget.transform.position);
            float PlayerDistance = Vector3.Distance(transform.position, playerTarget.transform.position);
            _attackRange2 = ClientDistance;
            if(ClientDistance > 1.0f && PlayerDistance > 1.0f && state == State.Attack) {
                lastAttackTime = 0.0f;
                state = State.Idle;
                stateName = "Idle";
            } else {
                if(lastAttackTime == 0.0f) {
                     _animator.SetTrigger("Attack");
                     stateName = "Attack";

                    if("Player".Equals(target)) {
                        // 공격 실행
                        playerTarget.GetComponent<Player>().OnEventPlayerDamage(_physicsAttack,transform.position); 
                    } else if("Client".Equals(target)) {
                        // 공격 실행
                        state = State.Idle;
                        stateName = "Idle";
                        StartCoroutine(MonsterDie());
                        clientTarget.GetComponent<Target>().DamageProcess(_physicsAttack); 
                    }
        
                }
                // 공격 타이머를 증가시킴
                lastAttackTime += Time.deltaTime;

                // 공격 타이머가 공격 속도보다 크거나 같은지 확인
                if (lastAttackTime >= _attackSpeed)
                {
                    // 공격 타이머 초기화
                    lastAttackTime = 0.0f;
                }
                if(state != State.Die) {
                    agent.SetDestination(NextTargetPosition);
                } else {
                    agent.speed = 0;
                    agent.isStopped = true;
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
                    stateName = "Run";
                } else {
                    agent.speed = 0;
                    agent.isStopped = true;
                }
            } else if((Mathf.Round(distanceCheck * 1000f) / 1000f) == (Mathf.Round(distance * 1000f) / 1000f)) {
                distanceCheckCount++;

            } else {
                distanceCheck = distance;
                distanceCheckCount = 0;
            }

            if(state != State.Die) {
                agent.SetDestination(NextTargetPosition); 
            } else {
                agent.speed = 0;
                agent.isStopped = true;
            }
        }

        private void UpdateRun()
        {
            int randomNumber = Random.Range(0, 1000);

            float distance = Vector3.Distance(transform.position, NextTargetPosition);
            if (distance <= 0.3f)
            {
                if(state != State.Die) {
                    state = State.Idle;
                    _animator.SetTrigger("Idle");
                    stateName = "Idle";
                }
            } 

            if(state != State.Die) {
                if (agent.isActiveAndEnabled) {
                  //  Debug.Log(_monsterId+"/agent.isActiveAndEnabled" + agent.isActiveAndEnabled);
                  //  Debug.Log(NextTargetPosition);
                    agent.SetDestination(NextTargetPosition);
                } 
            } else {
                agent.speed = 0;
                agent.isStopped = true;
            }
        }

        private void UpdateIdle()
        {
            if(_waypointList[waypointType].Count - 1 > waypointIndex) { 
                waypointIndex += 1;
            }
            NextTargetPosition = _waypointList[waypointType][waypointIndex].transform.position;
            if(state != State.Die) {
                state = State.Run;
            } 
        }

        IEnumerator objectEnd(GameObject weaponDamageEffect) {
            yield return new WaitForSeconds(1.0f);
            Destroy(weaponDamageEffect);
            yield return null;
        }



        public void DamageProcess(float physicsAttack, float magicAttack, float energyAttack) {
            if(state != State.Die) { 
                Vector3 rePosition = new Vector3(transform.position.x, transform.position.y , transform.position.z);
                GameObject weaponDamageEffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/DamageEffect2"),  rePosition, Quaternion.identity);
                //instantiatedPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                weaponDamageEffect.SetActive(true);

                StartCoroutine(objectEnd(weaponDamageEffect));


                // 4/1 확률로 몬스터 밀림
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

                DamageCount(physicsAttack, magicAttack, energyAttack);

                isFalling = true;
                // 피격 사운드 처리
                if(_spritesName.Equals("Teller") &&  _spritesName.Equals("wanderer") ) {
                    if(AudioManager.Instance.playSound[_spritesName+"_0"] != null) {
                        gameObject.GetComponent<AudioSource>().clip = AudioManager.Instance.playSound[_spritesName+"_0"];
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                }
               

                if(_hp <= 0) {
                    Collider2D collider = gameObject.GetComponent<Collider2D>();
                    collider.isTrigger = true;
                    _animator.SetTrigger("Die");
                    state = State.Die;
                    stateName = "Die";

                    //무기 드랍
                    int dropRate = Random.Range(0, 5);
                    int weaponNum = Random.Range(0, 3);

                    if( dropRate == 0) {
                        GameObject weaPonItem = Instantiate(UiController.Instance.dropItem, transform.position, transform.rotation);
                        weaPonItem.GetComponent<WeaponItem>().SetWeaponType(weaponNum + 1);
                    }
          
                    
                    //재화 증가
                    UiController.Instance.integerUseSet(_integer,"+");
                    
                    if(skillList["Split"]) {
                        skillAllEnd();
                        state = State.Idle;
                        stateName = "Idle";
                        skillList["ClientTargetFix"] = true;

                        for(int i =0; i < 3; i++) {
                            _monsterId = 13;
                            GameObject copy = Instantiate(gameObject);
                            copy.GetComponent<Enemy>()._hp = _initHp;
                            copy.GetComponent<Enemy>().isDead = false; 
                            copy.GetComponent<Enemy>().transform.position = new Vector3(transform.position.x + (i * 0.5f), transform.position.y + (i * 0.5f), transform.position.z);

                            Color endColor = new Color32(0, 0, 255, 255);
                            copy.transform.Find("Grapics").GetComponent<SkeletonMecanim>().skeleton.SetColor(endColor);
                            GameObject.Find("Canvas").GetComponent<GameMoonManager>().SplitSkillAdd(3 ,copy);
                        }

                    } else {
                        skillAllEnd();
                    }
                    
                    StartCoroutine(MonsterDie()); 
                }
            }
        }

        void skillAllEnd() {
            skillList["AttackUp"] = false;
            skillList["AttackSpeedUp"] = false;
            skillList["MoveSpeedUp"] = false;
            skillList["ClientTargetFix"] = false;
            skillList["PlayerTargetFix"] = false;
            skillList["PhysicsResistance"] = false;
            skillList["MagicResistance"] = false;
            skillList["Cloaking"] = false;
            skillList["Split"] = false;
            skillList["SummonAttackSpeddDown"] = false;
        }

        void DamageCount (float physicsAttack, float magicAttack, float energyAttack) {
            Debug.Log("physicsAttack : " + skillList.Count);
            if(skillList["PhysicsResistance"]) {  //물리 내성 -50% 데미지
                physicsAttack = physicsAttack - Mathf.Round(physicsAttack/2);
            } else {
                physicsAttack = physicsAttack - _pysicsDefense;
            }

            if(skillList["MagicResistance"]) { //마법 내성 -50% 데미지
                magicAttack = magicAttack - Mathf.Round(magicAttack/2);   
            } else {
                magicAttack = magicAttack - _magicDefense;
            }

            if(physicsAttack < 0) {
                physicsAttack = 0;
            }
            if(magicAttack < 0) {
                magicAttack = 0;
            }
            _hp = _hp - (physicsAttack + magicAttack + energyAttack);

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
            skeletonMecanim.transform.position  -= new Vector3(0,0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            skeletonMecanim.transform.position  += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);


            if(_monsterId == 2) {
                endColor = new Color32(0, 255, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 3) {
                endColor = new Color32(0, 0, 255, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 13) {
                endColor = new Color32(0, 0, 180, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(_monsterId == 4) {
                endColor = new Color32(0, 100, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } 

            isFalling = false;

            yield return null;
        }
    }
}