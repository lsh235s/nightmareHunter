using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class AttackRangeController : MonoBehaviour
    {
        private float RangeNextTime = 0.0f;  // 범위 표시시간
        public float spriteScale; // 스케일 값

        // Update is called once per frame
        void Update()
        {
            RangeNextTime = RangeNextTime + (Time.deltaTime * 2);
            if(RangeNextTime > spriteScale) {
                RangeNextTime = 0.0f;
            } 

            GetComponent<SpriteRenderer>().transform.localScale = new Vector3(RangeNextTime, RangeNextTime, 1f); // 스케일 값을 적용
        }

    }
}
