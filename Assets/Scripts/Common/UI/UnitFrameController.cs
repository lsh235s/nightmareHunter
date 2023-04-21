using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace nightmareHunter {
    public class UnitFrameController : TopManager
    {
        
        [SerializeField]
        private Image frameImage;
        [SerializeField]
        private GameObject summon;

        private bool _isChange = false;
        private float _changeTime = 0.3f;

        private int frameName = 1;
        private GameObject existSummon;

        UnitObject _unitObject;

        void Start() {
            if(topSystemValue.sceneMode == 0 && summon != null) {
                _unitObject = GameObject.Find("Canvas").GetComponent<GameSunManager>()._unitObject;
            }
        }

        void Update()
        {
            if(_isChange) {
                _changeTime -= Time.deltaTime;
                if(_changeTime <= 0) {
                    if(frameName < 3) {
                        frameName++;
                    } else {
                        frameName = 1;
                    }
                    string unitImage = "ui/"+frameName.ToString();
                    frameImage.sprite = Resources.Load<Sprite>(unitImage) ;
                    _changeTime = 0.5f;
                }
            }
        }

    
        public void OnMouseEnter()
        {
            if(topSystemValue.sceneMode == 0) {
                _isChange = true;
            }
        }

        public void OnMouseExit()
        {
            if(topSystemValue.sceneMode == 0) {
                _isChange = false;
                string unitImage = "ui/1";
                frameImage.sprite = Resources.Load<Sprite>(unitImage);
                _changeTime = 0.5f;
            }
        }


        public void OnMouseBeginDrag()
        {
            if(topSystemValue.sceneMode == 0) {
                if(summon != null && existSummon == null) {
                    // 마우스 좌표를 월드 좌표로 변환
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // 오브젝트의 위치 설정
                    summon.transform.position = mousePosition;

                    // Cube 오브젝트 생성
                    existSummon = Instantiate(summon);
                    existSummon.GetComponent<Summons>().playerDataLoad(gameDataManager.LoadSummerInfo("Exorcist",_unitObject)); 
                  
                    existSummon.GetComponent<Collider2D>().isTrigger = true;
                    
                    // 생성한 Cube 오브젝트 활성화
                    existSummon.SetActive(true);
                }
            }
        }

        public void OnMouseEndDrag()
        {
            if(topSystemValue.sceneMode == 0) {
                existSummon.transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                existSummon.GetComponent<Summons>()._playerinfo.positionInfo = existSummon.transform.position.ToString();
                gameDataManager.SaveSummerInfo("Exorcist",existSummon.GetComponent<Summons>()._playerinfo);
            }
        }

        public void OnMouseDrag()
        {
            if(topSystemValue.sceneMode == 0) {
                // 마우스 좌표를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                existSummon.transform.GetChild(0).GetComponent<Animator>().SetBool("idle",true);
                // 생성한 Cube 오브젝트 위치 변경
                existSummon.transform.position = mousePosition;
            }
        }
    }
}