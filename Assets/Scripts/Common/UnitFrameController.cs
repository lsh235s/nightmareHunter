using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitFrameController : MonoBehaviour
{
    
    [SerializeField]
    private Image frameImage;

    private bool _isChange = false;
    private float _changeTime = 0.3f;

    private int frameName = 1;


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
}
