using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameMoonManager : MonoBehaviour
    {
        // Start is called before the first frame update
        GameObject _playGameObject; 

        public GameObject[] _monsters;
        List<GameObject>[] _monsterList;
        
        GameDataManager gameDataManager = new GameDataManager();

        void Start()
        {
            monsterInit();

            _playGameObject = GameObject.Find("Player");

            _playGameObject.GetComponent<Player>()._playerinfo = gameDataManager.LoadPlayerInfo(); 

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void monsterInit() {
            _monsterList = new List<GameObject>[_monsters.Length];

            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();
            }


            UnitObject stateMonsterBatch = gameDataManager.MonsterWave();
            Debug.Log("stateMonsterBatch://"+stateMonsterBatch.unitList.Count);

        }

        public GameObject Get(int index) {
            GameObject select = null;

            foreach (GameObject item in _monsterList[index])
            {
                if (!item.activeSelf) {
                    select = item;
                    break;
                }
            }

            if (!select) {
                select = Instantiate(_monsters[index]);
                _monsterList[index].Add(select);
            }

            return select;
        }
    }
}