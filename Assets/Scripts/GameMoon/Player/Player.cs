using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity;

namespace nightmareHunter {
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float _rotationSpeed;

        [SerializeField]
        private Animator _animator;

        public PlayerInfo _playerinfo;

        private Rigidbody2D _rigidbody;
        private Vector2 _movementInput;
        private Vector2 _smoothedMovementInput;
        private Vector2 _movementInputSmoothVelocity;


        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            SetPlayerVelocity();
            RotateInDirectionOfInput();
        }

        private void SetPlayerVelocity() {

            Vector2 nextVec = _movementInput * _playerinfo.move * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + nextVec );
            _rigidbody.velocity = Vector2.zero;
        }

        private void RotateInDirectionOfInput() {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            _animator.SetFloat("move", Mathf.Abs(horizontalInput));

            if (_movementInput != Vector2.zero) {
            // Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
                //Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                // // 좌우 방향에 따라 이동 방향 벡터를 설정합니다.
                if (horizontalInput > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (horizontalInput < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 180f, 0);
                }
                //_rigidbody.MoveRotation(targetRotation);
            }

        }

        private void OnMove(InputValue inputValue) {
            _movementInput = inputValue.Get<Vector2>();
        }
    }
}