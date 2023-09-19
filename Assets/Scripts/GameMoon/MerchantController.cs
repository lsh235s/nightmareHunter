using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace nightmareHunter {
    public class MerchantController : MonoBehaviour
    {
        Button SaleButton;
        Button StoryButton;
        Button CancelButton;


        Button SubAddButton;
        Button SubSubButton;
        Button SubSellButton;
        Button SubCancelButton;

        TextMeshProUGUI gemText;
        int gemValue;
        TextMeshProUGUI goldText;
        int goldValue;

        GameObject merchantCanvas;
        GameObject merchantSub;

        // Start is called before the first frame update
        void Start()
        {
            SaleButton = transform.Find("MerchantCanvas/MerchantButtonList/Button1").gameObject.GetComponent<Button>();
            StoryButton = transform.Find("MerchantCanvas/MerchantButtonList/Button2").gameObject.GetComponent<Button>();
            CancelButton = transform.Find("MerchantCanvas/MerchantButtonList/Button3").gameObject.GetComponent<Button>();

            SubAddButton = transform.Find("MerchantCanvas/MerchantSub/PanelBack/UpButton").gameObject.GetComponent<Button>();
            SubSubButton = transform.Find("MerchantCanvas/MerchantSub/PanelBack/DownButton").gameObject.GetComponent<Button>();
            SubSellButton = transform.Find("MerchantCanvas/MerchantSub/PanelBack/SellButton").gameObject.GetComponent<Button>();
            SubCancelButton = transform.Find("MerchantCanvas/MerchantSub/PanelBack/CancelButton").gameObject.GetComponent<Button>();

            gemText = transform.Find("MerchantCanvas/MerchantSub/PanelBack/GemText/Text").gameObject.GetComponent<TextMeshProUGUI>();
            goldText = transform.Find("MerchantCanvas/MerchantSub/PanelBack/GoldText/Text").gameObject.GetComponent<TextMeshProUGUI>();

            merchantCanvas = transform.Find("MerchantCanvas").gameObject;
            merchantSub = transform.Find("MerchantCanvas/MerchantSub").gameObject;

            
            SaleButton.onClick.AddListener(saleOnClick);
            StoryButton.onClick.AddListener(storyOnClick);
            CancelButton.onClick.AddListener(cancelOnClick);

            SubAddButton.onClick.AddListener(subAddOnClick);
            SubSubButton.onClick.AddListener(subSubOnClick);
            SubSellButton.onClick.AddListener(subSellOnClick);
            SubCancelButton.onClick.AddListener(subCancelOnClick);

            gemValue = 0;
            goldValue = 0;
        }
        
        void saleOnClick() {
            merchantSub.SetActive(true);

            //gemText.text = UiController.Instance._integer.text;
            //gemValue = int.Parse(gemText.text);
        }

        void storyOnClick() {
        }

        void cancelOnClick() { 
            merchantSub.SetActive(false);
            merchantCanvas.SetActive(false);
        }

        void subAddOnClick() {
            if(int.Parse(UiController.Instance._integer.text) > gemValue) {
                gemValue = gemValue + 1;
                goldValue = gemValue * 100;
                gemText.text = gemValue.ToString();
                goldText.text = goldValue.ToString();
            }
        }

        void subSubOnClick() {
            if(gemValue > 0) {
                gemValue = gemValue - 1;
                goldValue = gemValue * 100;
                gemText.text = gemValue.ToString();
                goldText.text = goldValue.ToString();
            }
        }

        void subSellOnClick() {
            UiController.Instance.integerUseSet(gemValue,"-");
            UiController.Instance.goldUseSet(goldValue, "+");
            initValse();
        }

        void subCancelOnClick() {
            initValse();
            merchantSub.SetActive(false);
        }

        void initValse() {
            gemValue = 0;
            goldValue = 0;
            gemText.text = gemValue.ToString();
            goldText.text = goldValue.ToString();
        }

    }
}