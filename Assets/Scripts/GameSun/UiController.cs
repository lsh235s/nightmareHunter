using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    TextMeshProUGUI _timerText;
    float _countSec = 0.0f;
    int _Min = 0;
    int _Hour = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _timerText = GameObject.Find("Canvas/UIGroup/Pause/PauseValue/Timer").GetComponent<TextMeshProUGUI>(); 
    }

    // Update is called once per frame
    void Update()
    {
        _countSec += Time.deltaTime;

        if(_countSec >= 2.5f) {
            _countSec = 0;
            _Min += 10;
        }

        if(_Min >= 60) {
            _Hour++;
            _Min = 0;
        }

        
        _timerText.text = string.Format("{0:00}:{1:00}", _Hour, _Min);
    }
}
