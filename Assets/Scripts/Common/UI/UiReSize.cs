using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        GameObject[] dynamicFont = GameObject.FindGameObjectsWithTag("DynamicFont");
        GameObject[] dynamicGrid = GameObject.FindGameObjectsWithTag("DynamicGrid");

        foreach (GameObject obj in dynamicLayers)
        {
            BaseCanvsWidth(obj);
        }

        foreach (GameObject obj in dynamicImages)
        {
            FitImageAspectRatio(obj);
        }

        foreach (GameObject obj in dynamicFont)
        {
            FitFontSize(obj);
        }

        foreach (GameObject obj in dynamicGrid)
        {
            FitGridSize(obj);
        }
    }


    void FitGridSize(GameObject objectName) {
        FitImageAspectRatio(objectName);

        GridLayoutGroup _gridLayoutGroup = objectName.GetComponent<GridLayoutGroup>();

        float rateWidth = (float)Screen.width / baseWidth;
        float rateHeight = (float)Screen.height / baseHeight;

        float gridWidth = (float)rateWidth * _gridLayoutGroup.cellSize.x;
        float gridHeight = (float)rateHeight * _gridLayoutGroup.cellSize.y;

        float gridSpaceX = (float)rateWidth * _gridLayoutGroup.spacing.x;
        float gridSpaceY = (float)rateHeight * _gridLayoutGroup.spacing.y;

        _gridLayoutGroup.cellSize = new Vector2(gridWidth, gridHeight);
        _gridLayoutGroup.spacing = new Vector2(gridSpaceX, gridSpaceY);
    }

    void FitFontSize(GameObject objectName) {
        TextMeshProUGUI text = objectName.GetComponent<TextMeshProUGUI>();
        float rateWidth = (float)Screen.width / baseWidth;
        float fontSize = (float)rateWidth * text.fontSize;
        text.fontSize = (int)fontSize;
        FitImageAspectRatio(objectName);
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
