using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class GameSunManager : MonoBehaviour
    {
        [SerializeField]
        GameObject _playGameObject; 
        [SerializeField]
        SkeletonGraphic skeletonGraphic;
        
        UiController _uiController;

        // Start is called before the first frame update
        void Start()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();

            skeletonGraphic.AnimationState.SetAnimation(0, "idle_talk", true);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

      
    }
}
