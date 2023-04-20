using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameSunManager : TopManager
    {
        [SerializeField]
        GameObject _playGameObject; 
        

        public UnitObject _unitObject;
        // Start is called before the first frame update
        void Start()
        {
            playerInit();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void playerInit() {
            _playGameObject.GetComponent<Player>().playerDataLoad(gameDataManager.LoadPlayerInfo(_unitObject)); 
        }
    }
}
