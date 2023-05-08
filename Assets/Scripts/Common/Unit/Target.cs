using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private UiController TargetHpCanvas;

        public void DamageProcess(float damage) {
            TargetHpCanvas.TargetHP(damage);
        }
    }
}

