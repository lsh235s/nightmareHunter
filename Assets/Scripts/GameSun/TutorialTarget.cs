using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    public class TutorialTarget : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject player;

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

        private void OnCollisionEnter2D(Collision2D collision) {
        }
    }
}
