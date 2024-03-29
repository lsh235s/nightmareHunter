using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    public class WeaponItem : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] weaponSprites;
        public int weaponType;
        int weaponImage;

        private bool isBlinking = false; // 깜박이는 중인지 여부

        private float stateTime = 7f; // 파괴되기까지의 대기 시간

        private float elapsedTime = 0f; // 경과한 시간
        private bool increasing = true; // 알파 값 증가 여부

        private void Start() {
        }


        public void SetWeaponType(int type) {
            weaponType = type;
            weaponImage = type - 1;

            gameObject.GetComponent<SpriteRenderer>().sprite = weaponSprites[weaponImage];
        }

        private void Update()
        {
            float alpha = gameObject.GetComponent<SpriteRenderer>().color.a; 
            elapsedTime += Time.deltaTime; // 경과 시간 증가
            if (elapsedTime >= stateTime)
            {
                Destroy(gameObject); // 오브젝트 파괴
            } else {
                if(elapsedTime <= 3) {
                    if (increasing)
                    {
                        alpha += 1f * Time.deltaTime; // 알파 값 증가
                        if (alpha >= 1f)
                        {
                            alpha = 1f;
                            increasing = false;
                        }
                    }
                    else
                    {
                        alpha -= 1f * Time.deltaTime; // 알파 값 감소
                        if (alpha <= 0f)
                        {
                            alpha = 0f;
                            increasing = true;
                        }
                    }
                }
                

                Color newColor = gameObject.GetComponent<SpriteRenderer>().color;
                newColor.a = alpha;
                gameObject.GetComponent<SpriteRenderer>().color = newColor;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.tag == "Player") {
                Debug.Log(collision.gameObject.tag);
                collision.gameObject.GetComponent<Player>().WeaponChange(weaponType);
                
                Destroy(gameObject);
            }

        }
    }
}
