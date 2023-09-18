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
            _merchantPanel.active = true;
        }
    }
}
