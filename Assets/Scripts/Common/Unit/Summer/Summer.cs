using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class Summer : MonoBehaviour
    {
        public string summerStatus;
        public float scanRange;
        public LayerMask targetLayer;
        public RaycastHit2D[] targets;
        public Transform nearestTarget;

        // Start is called before the first frame update
        void FixedUpdate()
        {
            if(!"sun".Equals(summerStatus)) {
                Debug.Log("몬스터 탐지 시작!!");
                targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0,targetLayer);
                GetNearest();
            }
        }

        Transform GetNearest() {
            Transform result = null;
            float diff = 50;

            foreach (RaycastHit2D target in targets)
            {
                Vector3 myPost = transform.position;
                Vector3 targetPos = target.transform.position;

                float curDiff = Vector3.Distance(myPost,targetPos);

                if(curDiff < diff) {
                    diff = curDiff;
                    result = target.transform;
                }
            }

            return result;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
