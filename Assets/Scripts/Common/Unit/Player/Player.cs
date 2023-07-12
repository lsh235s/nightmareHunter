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
        public GameObject bulletPoint;
        public GameObject bulletTargetSpirte;

         //총알 프리팹
        [SerializeField]
        private GameObject _bulletPrefab;


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
            Vector3 len = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bulletPoint.transform.position;
            float angle = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
            

            // 총알 방향 설정
            bulletPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
            // 과녁 위치 설정
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3 directions = (targetPosition - bulletPoint.transform.position).normalized;


            bulletTargetSpirte.transform.position = bulletPoint.transform.position + directions * _playerinfo.attackRange ;


            if (isFalling)
            {  
                StartCoroutine(damageShake());
            } 

            if(!"die".Equals(playerState) && !"tutorial".Equals(playerState)) {
                // 공격 타이밍 계산
                if(_waitFire) {
                    nextTime = nextTime + Time.deltaTime;
                    Debug.Log("nextTime://"+nextTime+"/"+_playerinfo.attackDelayTime+"/"+_waitFire);
                    if(nextTime >= _playerinfo.attackDelayTime) {
                        nextTime = 0.0F;
                        _waitFire = false;
                    }
                }

                // 유닛의 이동 관련 처리
                SetPlayerVelocity();
                // 좌우 방향 확인
                RotateInDirectionOfInput();
            }
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
            _bulletPrefab.GetComponent<Bullet>().attack = _playerinfo.attack;
            _bulletPrefab.GetComponent<Bullet>().range = _playerinfo.attackRange;
            _bulletPrefab.GetComponent<Bullet>().initialPosition = initialPosition;
            _bulletPrefab.GetComponent<Bullet>()._bulletSpeed = _playerinfo.attackSpeed;
            
            GameObject bullet = Instantiate(_bulletPrefab, bulletPoint.transform.position, bulletPoint.transform.rotation);

            if(_playerinfo.weaponID != 0) {
                _playerinfo.weaponAmount = _playerinfo.weaponAmount - 1;
                Debug.Log("_playerinfo.weaponAmount://"+_playerinfo.weaponAmount);
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
                    Debug.Log("OnFire:/"+_waitFire);
                    if(!_waitFire) {
                        initialPosition = transform.position;
                        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("gun",true);
                        gameObject.GetComponent<AudioSource>().clip = playSound[0];
                        gameObject.GetComponent<AudioSource>().Play();
                        FireBullet();
                        _waitFire = true;
                    }
                    
                }
            }
        }


        // 무기교체
        public void WeaponChange(int weaponID) {
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