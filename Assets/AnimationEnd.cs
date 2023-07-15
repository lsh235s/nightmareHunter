using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnd : MonoBehaviour
{
        // 애니메이션 이벤트로 호출될 함수입니다.
    public void EndAnimation()
    {
        // 애니메이션 실행이 끝났을 때 오브젝트를 종료합니다.
       Destroy(gameObject);
    }
}
