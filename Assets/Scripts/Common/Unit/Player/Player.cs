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

         //총알 프리팹
        [SerializeField]
        private GameObject _bulletPrefab;


        // 능력치
        public PlayerInfo _playerinfo;

        private Rigidbody2D _rigidbody;
        private Vector2 _movementInput;


        //총알 속도
        [SerializeField]
        private float _bulletSpeed;

        private Vector3 initialPosition; // 초기 위치

        public float _timeBetweenShots; // 딜레이 타임
        private bool _waitFire;
        private float _lastFireTime;


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
            _playerinfo = inPlayerinfo;

            _timeBetweenShots = _playerinfo.attackSpeed;
            if (UiController.Instance._imagePlayHp != null && HpHeartImage[0] != null)
            {
                UiController.Instance._imagePlayHp.sprite = HpHeartImage[0]; // Image 컴포넌트의 Sprite 파일을 교체
            }
            if (UiController.Instance._playerHp.text != null)
            {
                maxHp = _playerinfo.health;
                UiController.Instance._playerHp.text = _playerinfo.health.ToString(); // Text 컴포넌트의 내용을 변경
            }
        }

        private void FixedUpdate() {
            if (isFalling)
            {  
                StartCoroutine(damageShake());
            } 

            if(!"die".Equals(playerState) && !"tutorial".Equals(playerState)) {
                // 공격 타이밍 계산
                if(_waitFire) {
                    float timeSinceLastFire = Time.time - _lastFireTime;

                    if(timeSinceLastFire >= _timeBetweenShots) {
                        _lastFireTime = Time.time;
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
            // 발사 위치와 회전 설정
            Vector3 spawnPosition = transform.position; // 발사 위치는 캐릭터의 위치로 설정
            Quaternion spawnRotation = transform.rotation; // 발사 회전은 캐릭터의 회전으로 설정

            // 좌우 방향에 따라 총알 발사 방향 설정
            Vector3 bulletDirection = transform.right; // 기본적으로 우측 방향으로 설정
            if (transform.localScale.x < 0) // 스케일이 -1인 경우(좌우가 뒤집혔을 경우)
            {
                bulletDirection = -transform.right; // 좌측 방향으로 설정
            }


            _bulletPrefab.GetComponent<Bullet>().attack = _playerinfo.attack;
            _bulletPrefab.GetComponent<Bullet>().range = _playerinfo.attackRange;
            _bulletPrefab.GetComponent<Bullet>().initialPosition = initialPosition;

            // 프리팹으로부터 새로운 미사일 게임 오브젝트 생성
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation);
            // 미사일로부터 리지드바디 2D 컴포넌트 가져옴
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            

            // 미사일을 전방으로 발사
            rb.AddForce(bulletDirection * _bulletSpeed, ForceMode2D.Impulse);
        }


        // 이동 키 이벤트
        private void OnMove(InputValue inputValue) {
            _movementInput = inputValue.Get<Vector2>();
        }

        // 공격 키 이벤트
        private void OnFire(InputValue inputValue) {
            if(!"wait".Equals(playerState) && !"tutorial".Equals(playerState)) {
                if(inputValue.isPressed) {
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


        private void OnTriggerEnter2D(Collider2D collision) {

            if(!"die".Equals(playerState) ) {
                if(collision.GetComponent<Enemy>()) {
                    Vector2 pushDirection = (_rigidbody.position - (Vector2)collision.transform.position).normalized;
                    _rigidbody.AddRelativeForce(pushDirection * 300f);

                    isFalling = true;
                    _playerinfo.health = _playerinfo.health - collision.GetComponent<Enemy>()._attack;
                    
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
                    }
                    UiController.Instance._playerHp.text = _playerinfo.health.ToString(); 

                }
            }
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
    }
}