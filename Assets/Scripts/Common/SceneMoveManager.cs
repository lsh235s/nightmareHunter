using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace nightmareHunter {

    [System.Serializable]
    public class SceneMoveManager 
    {
        public static void SceneMove (string sceneName) { 
            SceneManager.LoadScene(sceneName);
        }
    }
}