using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private UiController TargetHpCanvas;
        public Sprite[] _clientHpImage;

        public void DamageProcess(float damage) {
            UiController.Instance.TargetHP(damage,_clientHpImage);
        }
    }
}

