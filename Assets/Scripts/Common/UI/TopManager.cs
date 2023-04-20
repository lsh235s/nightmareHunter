using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace nightmareHunter {
    public class TopManager : MonoBehaviour
    {
        public TopSystemValue topSystemValue = new TopSystemValue();
      
        public static GameDataManager gameDataManager ;

        void Awake() {
            if(gameDataManager == null) {
                gameDataManager =  new GameDataManager();
            }
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
            if("GameSun".Equals(sceneName)) {
                topSystemValue.sceneMode = 0;
            }   
            else {
                topSystemValue.sceneMode = 1;
            }
        }
    }

    [System.Serializable]
    public class TopSystemValue {
       public int sceneMode;  //0: sun, 1: moon
    }
}