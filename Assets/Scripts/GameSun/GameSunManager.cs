using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameSunManager : MonoBehaviour
    {
        [SerializeField]
        GameObject _playGameObject; 
        
        public UiController _uiController;

        // Start is called before the first frame update
        void Start()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

      
    }
}
