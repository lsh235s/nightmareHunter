using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class Summons : MonoBehaviour
    {
        public int summonerId;
        public int summonerBatchKeyId;
        //소환수 모션
        private SkeletonMecanim skeletonMecanim;
        public LayerMask targetLayer;
        public RaycastHit2D[] targets;
        public Transform nearestTarget;


        public PlayerInfo basePlayerinfo;
        public PlayerInfo activePlayerinfo;
        // 소환수 능력치 
        string activateStatus = "stop";
        public float _speed;
        public float _hp;
        public float _attack;
        public float _physicsAttack;
        public float _magicAttack;
        public float _energyAttack;
        public float _attackSpeed;
        public float _attackRange;
        public string _attackType;
        public int _integer;
        public string _positionString;
        public Vector2 _positionInfo;
        public bool summonsExist;
        public bool summonsBatchIng;



        private float nextTime = 0.0f;  // 다음 공격 시간
     
        // 소환수 공격 범위
        private float RangeNextTime = 0.0f;  // 범위 표시시간
        float spriteScale; // 스케일 값
        public GameObject rangeObject;
        public GameObject summonBullet;

        UnitController _uiItController;

        public Dictionary<string, bool> skillList = new Dictionary<string, bool>();

        void Start() {
            skillList.Add("AttackUp", false);
            skillList.Add("AttackSpeedUp", false);
            skillList.Add("AttackSpeedDown", false);
            skillList.Add("MoveSpeedUp", false);
            skillList.Add("ClientTargetFix", false);
            skillList.Add("Cloaking", false);

            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();
            skeletonMecanim = gameObject.transform.GetChild(0).GetComponent<SkeletonMecanim>();

            if(summonerId == 1) {
                Color endColor = new Color32(0, 0, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 2) {
                Color endColor = new Color32(0, 255, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 3) {
                Color endColor = new Color32(0, 0, 255, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 5) {
                Color endColor = new Color32(100, 0, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 6) {
                Color endColor = new Color32(0, 100, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 7) {
                Color endColor = new Color32(0, 0, 100, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            }
        }


        public void playerDataLoad(PlayerInfo inPlayerinfo) {
            activateStatus = "move";

            _speed = inPlayerinfo.move;
            if(_speed <= 0) {
                _speed = 1.0f;
            }
            _hp = inPlayerinfo.health;
            _physicsAttack = inPlayerinfo.physicsAttack;
            _magicAttack = inPlayerinfo.magicAttack;
            _energyAttack = inPlayerinfo.energyAttack;
            _attackSpeed = inPlayerinfo.attackSpeed;
            _attackRange = inPlayerinfo.attackRange;
            _integer = inPlayerinfo.reward;
            _attackType = inPlayerinfo.attackType;

            spriteScale = (_attackRange * 10.0f) + _attackRange; // 스케일 값을 계산
            rangeObject.GetComponent<AttackRangeController>().spriteScale = spriteScale;

            if(UiController.Instance.sceneMode == 1) { // 저녁시간에만 몬스터 스캔
                rangeObject.SetActive(false);
            }
           
            Debug.Log("activePlayerinfo : " + inPlayerinfo.attackType);
           
        }

        // Start is called before the first frame update
        void FixedUpdate()
        {
            if(UiController.Instance.sceneMode == 1) { // 저녁시간에만 몬스터 스캔
                nextTime = nextTime + Time.deltaTime;
                    
                if(nextTime > _attackSpeed) {
                    scanRadar();
                    nextTime = 0.0F;
                }

                if(nearestTarget != null) {
                    activateStatus = "attack";
                    ApplyDamage(nearestTarget.gameObject);
                    nearestTarget = null;
                    
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


        private void ApplyDamage(GameObject collisionObject)
        {
            if(collisionObject.GetComponent<Enemy>() != null) {
                summonBullet.GetComponent<SummonBullet>().attackType = activePlayerinfo.attackType;
                
                Vector3 direction = ( nearestTarget.transform.position - transform.position).normalized;
                summonBullet.transform.rotation = Quaternion.LookRotation(direction);

                switch(activePlayerinfo.attackType)
                {
                    case "CDN":
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                        summonBullet.GetComponent<SummonBullet>().setWeaponEffect(activePlayerinfo.attackType);
                        nearestTarget = null;
                        
                        Instantiate(summonBullet, transform.position , summonBullet.transform.rotation);
                        break;
                    case "FSP":
                        summonBullet.GetComponent<SummonBullet>()._bulletSpeed = 2.0f;
                        summonBullet.GetComponent<SummonBullet>().initialPosition = transform.position;
                        summonBullet.GetComponent<SummonBullet>().targetMonster = nearestTarget.transform;
                        summonBullet.GetComponent<SummonBullet>().physicsAttack = _physicsAttack;  //물리공격력
                        summonBullet.GetComponent<SummonBullet>().magicAttack = _magicAttack;  //마법 공격력
                        summonBullet.GetComponent<SummonBullet>().energyAttack = _energyAttack; //에너지 공격력 
                        summonBullet.GetComponent<SummonBullet>().setWeaponEffect(activePlayerinfo.attackType);

                        GameObject bullet = Instantiate(summonBullet, transform.position , summonBullet.transform.rotation);
                        break;
                    default:
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                        collisionObject.GetComponent<Enemy>().DamageProcess(_physicsAttack, _magicAttack, _energyAttack);
                        nearestTarget = null;
                        break;

                }
      
            }
        }

        
        private void OnTriggerExit2D(Collider2D collision)
        {
            Color endColor = new Color32(255, 255, 255, 255);

            if(summonerId == 1) {
                endColor = new Color32(0, 0, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 2) {
                endColor = new Color32(0, 255, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 3) {
                endColor = new Color32(0, 0, 255, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 5) {
                endColor = new Color32(100, 0, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 6) {
                endColor = new Color32(0, 100, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            } else if(summonerId == 7) {
                endColor = new Color32(0, 0, 100, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
            }

            skeletonMecanim.skeleton.SetColor(endColor);
            summonsExist = true;
        }

        // 배치시 소환수 위치 설정
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if("Wall".Equals(collision.tag)) {
                Color endColor = new Color32(255, 0, 0, 255);
                skeletonMecanim.skeleton.SetColor(endColor);
                summonsExist = false;
            }
        }

        
        public void OnMouseDown()
        {
            if(UiController.Instance.sceneMode == 0) {
                GameDataManager.Instance.DeleteData("Id", summonerBatchKeyId.ToString());
                Destroy(gameObject);
            }
        }
    }
}
