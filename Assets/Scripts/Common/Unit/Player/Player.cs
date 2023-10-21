using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;

namespace nightmareHunter {
    public class Player : MonoBehaviour
    {
        public string playerState;

        // 발사체 이펙트
        public GameObject bulletDotAni;
        public GameObject shotgunAni;


        // 능력치
        public PlayerInfo _playerinfo;
        public PlayerInfo _playerBaseInfo;

        private Rigidbody2D _rigidbody;
        private Vector2 _movementInput;


        private Vector3 initialPosition; // 초기 위치

        private bool _waitFire;
        private float nextTime;


        // 주인공 스켈레톤
        [SerializeField]
        private GameObject _skeletonObject;


        [SerializeField]
        private Sprite[] HpHeartImage;

        private float maxHp;

        // 사운드
        [SerializeField]
        private AudioClip[] playSound;

        bool isFalling = false;

        private SkeletonMecanim skeletonMecanim;
        
        private Animator _animator;
        
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();

            skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
            _animator = _skeletonObject.GetComponent<Animator>();

            bulletDotAni = Resources.Load<GameObject>("Prefabs/Bullet/Bullet");
            shotgunAni = Resources.Load<GameObject>("Prefabs/Bullet/Shotgun");
        }

        public void playerDataLoad(PlayerInfo inPlayerinfo) {

            _playerinfo = GameDataManager.Instance.PlayWeaponSet(0,inPlayerinfo);

            if (UiController.Instance._imagePlayHp != null && HpHeartImage[0] != null)
            {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[0]; // Image 컴포넌트의 Sprite 파일을 교체
            }
            if (UiController.Instance._playerHp.text != null)
            {
                maxHp = _playerinfo.health;
                UiController.Instance._playerHp.text = _playerinfo.health.ToString(); // Text 컴포넌트의 내용을 변경
            }

            _playerBaseInfo = _playerinfo;
        }

        private void FixedUpdate() {
            // 과녁 위치 설정 총알 거리 및 위치 포인트 표시 제거
            //Vector3 mousePosition = Input.mousePosition;
            //mousePosition.z = -Camera.main.transform.position.z;
            //Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //Vector3 directions = (targetPosition - bulletPoint.transform.position).normalized;
            //bulletTargetSpirte.transform.position = bulletPoint.transform.position + directions * _playerinfo.attackRange ;

            if (isFalling)
            {  
                StartCoroutine(damageShake());
            } 
            
            if(!"die".Equals(playerState) && !"tutorial".Equals(playerState) ) {
                // 공격 타이밍 계산

                if("wait".Equals(playerState)) {
                    _waitFire = false;
                }
                
                if(_waitFire ) {
                    if(nextTime >= _playerinfo.attackDelayTime) {
                        nextTime = 0.0F;
                        FireBullet();
                    }
                }

                if(nextTime != 0.0F || _waitFire == true) {
                    nextTime = nextTime + Time.deltaTime;
                    if(nextTime >= _playerinfo.attackDelayTime) {
                        if( _waitFire == false) {
                           nextTime = 0.0F;
                        }
                    }
                }

                // 유닛의 이동 관련 처리
                SetPlayerVelocity();
                // 좌우 방향 확인
                RotateInDirectionOfInput();
            }
        }

        public void playerStateChange() {
           _animator.SetFloat("move", 0.0F);
        }


        private void SetPlayerVelocity() {
            Vector2 nextVec = _movementInput * _playerinfo.move * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + nextVec );
            _rigidbody.velocity = Vector2.zero;
        }

        private void RotateInDirectionOfInput() {
            float horizontalInput = Input.GetAxisRaw("Horizontal") +Input.GetAxisRaw("Vertical");
            _animator.SetFloat("move", Mathf.Abs(horizontalInput));

            if (_movementInput != Vector2.zero) {
                // // 좌우 방향에 따라 이동 방향 벡터를 설정합니다.
                if (horizontalInput > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (horizontalInput < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 180f, 0);
                }
            }

        }


        // 공격 처리
        private void FireBullet() {
            initialPosition = transform.position;
            string weaponType = "PW0";
            if(_playerinfo.weaponID == 1) {
                weaponType = "PW1";
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().timeScale = 1.0f;
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Pistol_1", false);
            } else if(_playerinfo.weaponID == 2) {
                weaponType = "PW2";
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().timeScale = 1.0f;
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "shotgun_1", false);
            } else if(_playerinfo.weaponID == 3) {
                weaponType = "PW3";
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().timeScale = 1.0f;
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "machinegun_1", false);
            } else {
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().timeScale = 1.0f;
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "shotgun_2", false);
            }
            gameObject.GetComponent<AudioSource>().clip = playSound[0];
            gameObject.GetComponent<AudioSource>().Play();

            
            Vector3 playPosition = new Vector3(GameObject.Find("ShotPoint").transform.position.x, GameObject.Find("ShotPoint").transform.position.y, GameObject.Find("ShotPoint").transform.position.z);
           // 오차를 보정할 위치를 정의합니다.
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y  , Input.mousePosition.z);

            Vector3 len = Camera.main.ScreenToWorldPoint(mousePosition) - playPosition;

            float angle = Mathf.Atan2(len.y , len.x) * Mathf.Rad2Deg;

            GameObject.Find("ShotPoint").transform.rotation = Quaternion.Euler(0, 0, angle);


            if(_playerinfo.weaponID != 2) {
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().attackType = weaponType;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().physicsAttack = _playerinfo.physicsAttack;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().magicAttack = _playerinfo.magicAttack;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().energyAttack = 0.0f;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().range = _playerinfo.attackRange;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().initialPosition = initialPosition;
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>().targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                bulletDotAni.transform.GetChild(0).GetComponent<SummonBullet>()._bulletSpeed = _playerinfo.attackSpeed;
                bulletDotAni.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


                GameObject bullet = Instantiate(bulletDotAni, playPosition, Quaternion.AngleAxis(angle , Vector3.forward));
                
                if(_playerinfo.weaponID != 0) {
                    bullet.transform.GetChild(0).GetComponent<SkeletonAnimation>().AnimationName = "bullet1";
                }
            } else {
                shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().attackType = "CDN"; //발사체 유형
                shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().physicsAttack = _playerinfo.physicsAttack;  //물리공격력
                shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().magicAttack = _playerinfo.magicAttack;  //마법 공격력
                shotgunAni.transform.Find("ShotgunAni").GetComponent<SummonBullet>().energyAttack = 0.0f; //에너지 공격력 

                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                
                Instantiate(shotgunAni, playPosition, rotation);
            }

            if(_playerinfo.weaponID != 0) {
                _playerinfo.weaponAmount = _playerinfo.weaponAmount - 1;
                if(_playerinfo.weaponAmount <= 0) {
                    _playerinfo = GameDataManager.Instance.PlayWeaponSet(0,_playerBaseInfo);
                }
            }
        }


        // 이동 키 이벤트
        private void OnMove(InputValue inputValue) {
            _movementInput = inputValue.Get<Vector2>();
        }

        // 공격 키 이벤트
        private void OnFire(InputValue inputValue) {
            if(!"wait".Equals(playerState) && !"tutorial".Equals(playerState)) {
                if(inputValue.isPressed) {
                    if(nextTime == 0.0F) {
                        FireBullet();
                    }
                    nextTime = nextTime + 0.01f;
                     _waitFire = true;
                } else {
                    _waitFire = false;
                }
            }
        }


        // 무기교체
        public void WeaponChange(int weaponID) {
            if(weaponID == 1) {
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Pistol_1", true );
                 
            } else if(weaponID == 2) {
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "shotgun_1", true );
            } else if(weaponID == 3) {
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "machinegun_1", true );
            } else {
                gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "shotgun_2", true );
            }
            gameObject.transform.GetChild(1).GetComponent<SkeletonAnimation>().timeScale = 0.0f;
            _playerinfo = GameDataManager.Instance.PlayWeaponSet(weaponID, _playerBaseInfo);
        }



        public void OnEventPlayerDamage(float attackDamage, Vector2 enemyPosition) {
            Vector2 pushDirection = (_rigidbody.position - enemyPosition).normalized;
            _rigidbody.AddRelativeForce(pushDirection * 300f);

            isFalling = true;
            _playerinfo.health = _playerinfo.health - attackDamage;
            
            if(_playerinfo.health < 0) {
                _playerinfo.health = 0;
            }

            float hpRate = (float)_playerinfo.health / (float)maxHp * 100;
            
            if(hpRate > 80 && hpRate == 100.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[0];
            } else if (hpRate > 50.0f && hpRate <= 80.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[1];
            } else if (hpRate > 25.0f && hpRate <= 50.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[2];
            } else if (hpRate > 10.0f && hpRate <= 25.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[3];
            } else if (hpRate > 0.0f && hpRate <= 10.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[4];
            } else if (hpRate <= 0.0f) {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[5];
                _animator.SetTrigger("die");
                playerState = "die";
                StartCoroutine(gameEnd()); 
            }
            UiController.Instance._playerHp.text = _playerinfo.health.ToString(); 
        }


        // private void OnTriggerEnter2D(Collider2D collision) {

        //     Collider2D otherCollider = collision.GetComponent<Collider>();
        //     if(!"die".Equals(playerState) ) {
        //         if(otherCollider.GetComponent<Enemy>()) {
        //             _playerinfo.health = _playerinfo.health - otherCollider.GetComponent<Enemy>()._attack;
        //             if(_playerinfo.health < 0) {
        //                 _playerinfo.health = 0;
        //             }

        //             float hpRate = (float)_playerinfo.health / (float)maxHp * 100;
                    
        //             if(hpRate > 80 && hpRate == 100.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[0];
        //             } else if (hpRate > 50.0f && hpRate <= 80.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[1];
        //             } else if (hpRate > 25.0f && hpRate <= 50.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[2];
        //             } else if (hpRate > 10.0f && hpRate <= 25.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[3];
        //             } else if (hpRate > 0.0f && hpRate <= 10.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[4];
        //             } else if (hpRate <= 0.0f) {
        //                 UiController.Instance._imagePlayHp.sprite = HpHeartImage[5];
        //                 _animator.SetTrigger("die");
        //                 playerState = "die";
        //             }
        //             UiController.Instance._playerHp.text = _playerinfo.health.ToString(); 
        //         }
        //     }
        // }

        private IEnumerator damageShake() {
            _skeletonObject.transform.localPosition  += new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            Color endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  -= new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  -= new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  += new Vector3(0.02f, 0, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  -= new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 0, 0, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            _skeletonObject.transform.localPosition  -= new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.02f);
            endColor = new Color32(255, 255, 255, 255);
            skeletonMecanim.skeleton.SetColor(endColor);
            isFalling = false;
            _skeletonObject.transform.localPosition = new Vector3(0, 0, 0);
            yield return null;
        }

        private IEnumerator gameEnd() {
            yield return new WaitForSeconds(2.0f);
            SceneMoveManager.SceneMove("GameInits");
        }
    }
}