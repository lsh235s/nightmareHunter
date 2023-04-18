using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitFrameController : MonoBehaviour
{
    
    [SerializeField]
    private Image frameImage;
    [SerializeField]
    private GameObject summers;

    private bool _isChange = false;
    private float _changeTime = 0.3f;

    private int frameName = 1;
    private GameObject existSummers;

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
        _isChange = true;
    }

    public void OnMouseExit()
    {
        _isChange = false;
        string unitImage = "ui/1";
        frameImage.sprite = Resources.Load<Sprite>(unitImage);
        _changeTime = 0.5f;
    }


    public void OnMouseBeginDrag()
    {
        if(summers != null) {
            // 마우스 좌표를 월드 좌표로 변환
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 오브젝트의 위치 설정
            summers.transform.position = mousePosition;


            // Cube 오브젝트 생성
            existSummers = Instantiate(summers);

            // 생성한 Cube 오브젝트 활성화
            existSummers.SetActive(true);
        }
    }

    public void OnMouseEndDrag()
    {
        Debug.Log("드래그 완료");
    }

    public void OnMouseDrag()
    {
        
        // 마우스 좌표를 월드 좌표로 변환
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePosition);
        // 생성한 Cube 오브젝트 위치 변경
        existSummers.transform.position = mousePosition;
    }
}
