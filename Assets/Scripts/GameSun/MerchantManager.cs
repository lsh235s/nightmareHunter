using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class MerchantManager : MonoBehaviour
    {
        public GameObject _merchantPanel;
        public void OnMouseDown()
        {
            if( UiController.Instance.systemSaveInfo.day != 0) {
                _merchantPanel.active = true;

               Transform merchantSub = _merchantPanel.transform.Find("MerchantSub");
                if (merchantSub != null)
                {
                    merchantSub.gameObject.SetActive(false);
                }
            }
        }
    }
}
