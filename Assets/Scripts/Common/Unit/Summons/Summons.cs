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



        public float nextTime = 0.0f;  // 다음 공격 시간
     
        // 소환수 공격 범위
        private float RangeNextTime = 0.0f;  // 범위 표시시간
        float spriteScale; // 스케일 값
        public GameObject rangeObject;


        // 발사체 이펙트
        public GameObject bulletDotAni;
        public GameObject shotgunAni;
        public GameObject shotArea;

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

            bulletDotAni = Resources.Load<GameObject>("Prefabs/Bullet/SummonBullet");
            shotgunAni = Resources.Load<GameObject>("Prefabs/Bullet/Shotgun");
            shotArea = Resources.Load<GameObject>("Prefabs/Bullet/ShotArea");


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
        // void Update()
        // {
        //     if(UiController.Instance.sceneMode == 1) { // 저녁시간에만 몬스터 스캔
        //         if(nearestTarget != null) {
        //             activateStatus = "attack";
        //         } else {
        //             activateStatus = "move";
        //         }
        //     }
        // }


        // 물리 판정이 아닌 단순 거리 계산 공격
        void targetsAttack() {
            if(nearestTarget != null) {
                gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                nearestTarget = null;
            }
        }

        // 타겟 대상 스캔
        public void scanRadar() {
               
            Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, _attackRange);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy")) {
                    nearestTarget = collider.transform;
                    if(nearestTarget.position.x < transform.GetComponent<Rigidbody2D>().position.x) {
                        transform.rotation = Quaternion.Euler(0, 180f, 0);
                    } else {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    ApplyDamage(collider.gameObject);
                    nearestTarget = null;
                    return;
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
                
                Vector3 len = nearestTarget.transform.position - transform.position;
                float angle = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;

                switch(activePlayerinfo.attackType)
                {
                    case "CDN": //확산 공격
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                        shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().physicsAttack = _physicsAttack;  //물리공격력
                        shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().magicAttack = _magicAttack;  //마법 공격력
                        shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().energyAttack = _energyAttack; //에너지 공격력 
                        Quaternion rotation = Quaternion.Euler(0, 0, angle);
                        nearestTarget = null;
                        
                        Instantiate(shotgunAni, transform.position , rotation);
                        break;
                    case "FSP": //관통 공격
                        bulletDotAni.GetComponent<SummonBullet>()._bulletSpeed = 2.0f;
                        bulletDotAni.GetComponent<SummonBullet>().attackType = activePlayerinfo.attackType;
                        bulletDotAni.GetComponent<SummonBullet>().initialPosition = transform.position;
                        bulletDotAni.GetComponent<SummonBullet>().physicsAttack = _physicsAttack;  //물리공격력
                        bulletDotAni.GetComponent<SummonBullet>().magicAttack = _magicAttack;  //마법 공격력
                        bulletDotAni.GetComponent<SummonBullet>().energyAttack = _energyAttack; //에너지 공격력 

                        bulletDotAni.transform.rotation = Quaternion.Euler(0, 0, angle);

                        Instantiate(bulletDotAni, transform.position , bulletDotAni.transform.rotation);
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                        break;
                    case "CWP": //영역 공격
                        shotArea.GetComponent<SummonBullet>().attackType = activePlayerinfo.attackType;
                        shotArea.GetComponent<SummonBullet>().initialPosition = transform.position;
                        shotArea.GetComponent<SummonBullet>().physicsAttack = _physicsAttack;  //물리공격력
                        shotArea.GetComponent<SummonBullet>().magicAttack = _magicAttack;  //마법 공격력
                        shotArea.GetComponent<SummonBullet>().energyAttack = _energyAttack; //에너지 공격력 

                        Instantiate(shotArea, transform.position , transform.rotation);
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                        break;
                    case "FSR"://원거리 폭발
                        GameObject bulletEx = Resources.Load<GameObject>("Prefabs/Bullet/SummonBullet");
                        bulletEx.GetComponent<SummonBullet>()._bulletSpeed = 2.0f;
                        bulletEx.GetComponent<SummonBullet>().range = _attackRange;
                        bulletEx.GetComponent<SummonBullet>().targetPosition = nearestTarget.position;
                        bulletEx.GetComponent<SummonBullet>().attackType = activePlayerinfo.attackType;
                        bulletEx.GetComponent<SummonBullet>().initialPosition = transform.position;
                        bulletEx.GetComponent<SummonBullet>().physicsAttack = _physicsAttack;  //물리공격력
                        bulletEx.GetComponent<SummonBullet>().magicAttack = _magicAttack;  //마법 공격력
                        bulletEx.GetComponent<SummonBullet>().energyAttack = _energyAttack; //에너지 공격력 
                        bulletEx.transform.rotation = Quaternion.Euler(0, 0, angle);

                        Instantiate(bulletEx, transform.position , bulletEx.transform.rotation);
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
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
