using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace nightmareHunter {
    public class ClientTarget : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] HpHeartImage;
        [SerializeField]
        private Image HpCanvas;
        [SerializeField]
        private TextMeshProUGUI HpText;

        private float ClientHp = 1000;
        private int maxHp = 1000;


        // Start is called before the first frame update
        void Start()
        {
            if (HpCanvas != null && HpHeartImage[0] != null)
            {
                HpCanvas.sprite = HpHeartImage[0]; // Image 컴포넌트의 Sprite 파일을 교체
            }
            if (HpText != null)
            {
                HpText.text = ClientHp.ToString(); // Text 컴포넌트의 내용을 변경
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        private void OnCollisionEnter2D(Collision2D collision) {

            Collider2D otherCollider = collision.collider;

            if(otherCollider.GetComponent<Enemy>()) {
                ClientHp = ClientHp - otherCollider.GetComponent<Enemy>()._physicsAttack;
                if(ClientHp < 0) {
                    ClientHp = 0;
                }

                float hpRate = (float)ClientHp / (float)maxHp * 100;

                if(hpRate > 80 && hpRate == 100.0f) {
                    HpCanvas.sprite = HpHeartImage[0];
                } else if (hpRate > 50.0f && hpRate <= 80.0f) {
                    HpCanvas.sprite = HpHeartImage[1];
                } else if (hpRate > 25.0f && hpRate <= 50.0f) {
                    HpCanvas.sprite = HpHeartImage[2];
                } else if (hpRate > 10.0f && hpRate <= 25.0f) {
                    HpCanvas.sprite = HpHeartImage[3];
                } else if (hpRate > 0.0f && hpRate <= 10.0f) {
                    HpCanvas.sprite = HpHeartImage[4];
                } else if (hpRate <= 0.0f) {
                    HpCanvas.sprite = HpHeartImage[5];
                }
                HpText.text = ClientHp.ToString(); 
            }
        }
    }
}
