using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameMoonManager : MonoBehaviour
    {
 
        [SerializeField]
        GameObject[] wayPointList; 

        private List<GameObject>[] _monsterList;
        public GameObject[] _monsters;


        UiController _uiController;

        void Start()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UiController>();

            AudioManager.Instance.BackGroundPlay("bgm_game");
            //몬스터 배치
            monsterInit();

        }         

        

        // 몬스터 배치 관련
        void monsterInit() {
            _monsterList = new List<GameObject>[_monsters.Length];

            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();
            }

            for (int i = 0; i < _uiController._stateMonsterBatch.stateMonsterList.Count; i++) {
                MonsterGet(_uiController._stateMonsterBatch.stateMonsterList[i]);
            }

        }


        public GameObject MonsterGet(StateMonster stateMonster) {
            GameObject select = null;

            if(_monsterList.Length > stateMonster.monsterId )
            {

                foreach (GameObject item in _monsterList[stateMonster.monsterId])
                {
                    if (!item.activeSelf) {
                        item.GetComponent<Enemy>()._target = _uiController._playGameObject.GetComponent<Rigidbody2D>();
                        item.GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                        item.transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                        item.GetComponent<Enemy>().initState(_uiController.gameDataManager.LoadMonsterInfo(_uiController._unitObject,stateMonster)); 
                        select = item;
                        break;
                    }
                }

                if (!select) {
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>()._target = _uiController._playGameObject.GetComponent<Rigidbody2D>();
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                    _monsters[stateMonster.monsterId].transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().initState(_uiController.gameDataManager.LoadMonsterInfo(_uiController._unitObject,stateMonster));
                    select = Instantiate(_monsters[stateMonster.monsterId]);
                    _monsterList[stateMonster.monsterId].Add(select);
                }
            } else {
                Debug.Log("not find monsterID");
            }

            return select;
        }
    }
}