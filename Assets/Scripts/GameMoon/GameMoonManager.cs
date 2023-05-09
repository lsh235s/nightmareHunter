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


        UnitController _uiItController;

        void Awake() {
            UiController.Instance.LoadStart();
            
            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();
        }

        void Start()
        {
            AudioManager.Instance.BackGroundPlay("bgm_game");
            _uiItController.GameStart();
            //몬스터 배치
            monsterInit();
        }         

        // 몬스터 배치 관련
        void monsterInit() {
            _monsterList = new List<GameObject>[_monsters.Length];

            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();
            }

            for (int i = 0; i < _uiItController._stateMonsterBatch.stateMonsterList.Count; i++) {
                MonsterGet(_uiItController._stateMonsterBatch.stateMonsterList[i]);
            }

        }


        public GameObject MonsterGet(StateMonster stateMonster) {
            GameObject select = null;

            if(_monsterList.Length > stateMonster.monsterId )
            {

                foreach (GameObject item in _monsterList[stateMonster.monsterId])
                {
                    if (!item.activeSelf) {
                        item.GetComponent<Enemy>()._target = _uiItController._playGameObject.GetComponent<Rigidbody2D>();
                        item.GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                        item.transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                        item.GetComponent<Enemy>().initState(_uiItController.gameDataManager.LoadMonsterInfo(_uiItController._unitObject,stateMonster)); 
                        select = item;
                        break;
                    }
                }

                if (!select) {
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>()._target = _uiItController._playGameObject.GetComponent<Rigidbody2D>();
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                    _monsters[stateMonster.monsterId].transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().initState(_uiItController.gameDataManager.LoadMonsterInfo(_uiItController._unitObject,stateMonster));
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