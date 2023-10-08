using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class AnimationEnd : MonoBehaviour
    {
            // 애니메이션 이벤트로 호출될 함수입니다.
        public void EndAnimation()
        {
            // 애니메이션 실행이 끝났을 때 오브젝트를 종료합니다.
             // 부모 GameObject 찾기
            Transform parent = transform.parent;

            if (parent != null)
            {
                // 부모 GameObject 파괴
                Destroy(parent.gameObject);
            }
            else
            {
                // 부모 GameObject가 없는 경우 현재 GameObject를 파괴
                Destroy(gameObject);
            }
        }

    

        //오프닝 애니메이션 이벤트로 호출될 함수입니다.
        public void OpeningAnimation()
        {
            SceneMoveManager.SceneMove("GameInits");
        }

    }
}
