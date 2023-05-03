using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class Summons : MonoBehaviour
    {
        public float scanRange;
        public LayerMask targetLayer;
        public RaycastHit2D[] targets;
        public Transform nearestTarget;

        public PlayerInfo _playerinfo;
        public Vector2 bathPosition; 

        public int nowStatgeTime;


        void Start()
        {
            _playerinfo = new PlayerInfo();
            _playerinfo.playerLevel = 1;
            _playerinfo.health = 10;
            _playerinfo.attack = 5;
            _playerinfo.attackRange = 1;
            _playerinfo.move = 1;
            _playerinfo.attackSpeed =1;
        }

        public void playerDataLoad(PlayerInfo inPlayerinfo) {
            _playerinfo = inPlayerinfo;
        }

        // Start is called before the first frame update
        void FixedUpdate()
        {
            if(nowStatgeTime == 1) { // 저녁시간에만 몬스터 스캔
                scanRadar();
                targetsAttack();
            }
        }

        void targetsAttack() {
            if(nearestTarget != null) {
                gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("atk",true);
                nearestTarget = null;
            }
        }

        // 타겟 대상 스캔
        void scanRadar() {
            targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0,targetLayer);
            nearestTarget = GetNearest();
        }
        
        Transform GetNearest() {
            Transform result = null;
            float diff = 1.5f;

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

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.GetComponent<Enemy>()) {
                collision.GetComponent<Enemy>().DamageProcess(_playerinfo.attack);
            }
        }

    }
}
