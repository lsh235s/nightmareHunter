using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    public class TutorialTarget : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject player;

        private float changeInterval = 0.1f; // 스프라이트를 교체할 간격을 설정합니다.
        private float timer = 0f; // 시간을 추적하는 타이머 변수입니다.
        private int index = 0; // 현재 스프라이트의 인덱스를 추적하는 변수입니다.


        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag == "Player") {
                canvas.GetComponent<GameSunManager>().eventFlag = false;
                canvas.GetComponent<GameSunManager>()._tutory.SetActive(false);
                canvas.GetComponent<GameSunManager>().skipButton();
            }
            if(collision.gameObject.tag == "Bullet") {
                canvas.GetComponent<GameSunManager>().eventFlag = false;
                canvas.GetComponent<GameSunManager>()._tutory2.SetActive(false);
                canvas.GetComponent<GameSunManager>().skipButton();
                player.GetComponent<Player>().playerState = "tutorial";
            }
            
            if(collision.gameObject.tag == "Summon") {
                canvas.GetComponent<GameSunManager>().eventFlag = false;
                canvas.GetComponent<GameSunManager>()._tutory3.SetActive(false);
                canvas.GetComponent<GameSunManager>().skipButton();
                player.GetComponent<Player>().playerState = "tutorial";
            }
            
            
        }


        private void Update()
        {
            if(gameObject.GetComponent<SpriteRenderer>() != null) {
                timer += Time.deltaTime; // 매 프레임마다 경과 시간을 더합니다.

                if (timer >= changeInterval) // 경과 시간이 지정된 간격을 초과하면 스프라이트를 교체합니다.
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = UiController.Instance._pointSprite[index]; // 현재 인덱스의 스프라이트로 교체합니다.
                    index = (index + 1) % UiController.Instance._pointSprite.Length; // 다음 인덱스로 이동하되, 배열의 끝에 도달하면 0으로 돌아갑니다.
                    timer = 0f; // 타이머를 리셋합니다.
                }
            }

        }


        private void OnCollisionEnter2D(Collision2D collision) {
        }
    }
}
