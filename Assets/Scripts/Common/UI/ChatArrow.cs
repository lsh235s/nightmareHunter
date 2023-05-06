using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatArrow : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites; 

    float changeInterval = 0.5f;
    private int currentSpriteIndex;
    private Image imageComponent;

    void Start() {
        imageComponent = GetComponent<Image>();

        if (sprites.Length > 0)
        {
            currentSpriteIndex = 0;
            imageComponent.sprite = sprites[currentSpriteIndex];

            
            // 일정 시간마다 이미지를 교체하는 메서드를 호출합니다.
            InvokeRepeating("ChangeImage", changeInterval, changeInterval); 
        }
    }

    void ChangeImage()
    {
        // 다음 스프라이트 인덱스를 계산합니다.
        currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;

        // 이미지를 교체합니다.
        imageComponent.sprite = sprites[currentSpriteIndex];
    }
}
