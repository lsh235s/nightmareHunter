using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class LogoInit : MonoBehaviour
    {
        public Camera camera;
        public SkeletonAnimation logoAnimation;
        public float fadeDuration = 1f;

        void Start()
        {
            StartCoroutine(FadeInStart());
            // 로고 애니메이션이 끝나면 NextScene 함수를 호출하도록 설정
            logoAnimation.AnimationState.Complete += NextScene;
            logoAnimation.AnimationState.SetAnimation(0, "animation", true);
        }

        void NextScene(Spine.TrackEntry trackEntry)
        {
            // 다음 씬으로 전환
            SceneMoveManager.SceneMove("GameInits");
        }

        public IEnumerator FadeInStart()
        {
            float startTime = Time.time;

            while (Time.time - startTime < fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                camera.backgroundColor = Color.Lerp(Color.black, Color.white, t);
                yield return null;
            }

            // 페이드 인 완료 시 흰색으로 설정
            camera.backgroundColor = Color.white;
        }
    }
}
