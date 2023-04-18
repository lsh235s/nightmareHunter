using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameSunManager : MonoBehaviour
    {
        [SerializeField]
        GameObject _playGameObject; 
        

        
        GameDataManager gameDataManager = new GameDataManager();

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
