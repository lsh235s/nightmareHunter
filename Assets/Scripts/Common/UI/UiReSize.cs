using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiReSize : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform rectTransform;
    private Image UiImage;

    private float baseWidth = 578f;
    private float baseHeight = 325f;


    void Awake() {
        
        GameObject[] dynamicLayers = GameObject.FindGameObjectsWithTag("DynamicLayer");
        GameObject[] dynamicImages = GameObject.FindGameObjectsWithTag("DynamicImage");

        foreach (GameObject obj in dynamicLayers)
        {
            BaseCanvsWidth(obj);
        }

        foreach (GameObject obj in dynamicImages)
        {
            FitImageAspectRatio(obj);
        }
    }

    void BaseCanvsWidth(GameObject objectName) {
        rectTransform = objectName.GetComponent<RectTransform>();

        float rateWidth = (float)Screen.width / baseWidth;

        rectTransform.sizeDelta = new Vector2(Screen.width, rectTransform.sizeDelta.y);
    }

    void FitImageAspectRatio(GameObject objectName)
    {
        rectTransform = objectName.GetComponent<RectTransform>();

        float rateWidth = (float)Screen.width / baseWidth;
        float rateHeight = (float)Screen.height / baseHeight;

        float imageWidth = (float)rateWidth * rectTransform.sizeDelta.x;
        float imageHeight = (float)rateHeight * rectTransform.sizeDelta.y;
        
        float imageX = (float)rateWidth * rectTransform.anchoredPosition.x;
        float imageY = (float)rateHeight * rectTransform.anchoredPosition.y;


        rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        rectTransform.anchoredPosition = new Vector2(imageX, imageY);

    }


}
