using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameMoonManager : MonoBehaviour
    {
        // Start is called before the first frame update
        GameObject _playGameObject; 
        void Start()
        {
            GameDataManager gameDataManager = new GameDataManager();
            _playGameObject = GameObject.Find("Player");

            _playGameObject.GetComponent<Player>()._playerinfo = gameDataManager.LoadPlayerInfo(); 

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}