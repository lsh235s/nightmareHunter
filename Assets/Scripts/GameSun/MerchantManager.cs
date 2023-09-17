using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class MerchantManager : MonoBehaviour
    {
        public GameObject merchant;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OpenMerchant() {
            Debug.Log("Open Merchant");
            merchant.SetActive(true);
        }
    }
}
