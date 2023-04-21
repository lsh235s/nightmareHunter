using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameMoonManager : TopManager
    {
        // Start is called before the first frame update
        [SerializeField]
        GameObject _playGameObject; 

        [SerializeField]
        GameObject[] wayPointList; 

        private List<GameObject>[] _monsterList;
        public GameObject[] _monsters;

        private List<GameObject>[] _summonerList;
        public GameObject[] _summoner;

        public UnitObject _unitObject;
        public StateMonsterBatch _stateMonsterBatch ;

        void Start()
        {
            //몬스터 배치
            monsterInit();
            //소환수 배치
            summonerInit();
            //플레이어 능력치 load
            playerInit();
        }         

        // 플레이어 배치 관련
        void playerInit() {
            _playGameObject.GetComponent<Player>().playerDataLoad(gameDataManager.LoadPlayerInfo(_unitObject)); 
        }

        // 소환수 배치 관련
        void summonerInit() {
            _summonerList = new List<GameObject>[_summoner.Length];

            for (int i = 0; i < _summonerList.Length; i++) {
                _summonerList[i] = new List<GameObject>();
            }

            gameDataManager.LoadPlayerInfo(_unitObject)
        }

        // 몬스터 배치 관련
        void monsterInit() {
            _monsterList = new List<GameObject>[_monsters.Length];

            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();
            }

            for (int i = 0; i < _stateMonsterBatch.stateMonsterList.Count; i++) {
                Get(_stateMonsterBatch.stateMonsterList[i]);
            }

        }

        public GameObject Get(StateMonster stateMonster) {
            GameObject select = null;

            if(_monsterList.Length > stateMonster.monsterId )
            {

                foreach (GameObject item in _monsterList[stateMonster.monsterId])
                {
                    if (!item.activeSelf) {
                        item.GetComponent<Enemy>()._target = _playGameObject.GetComponent<Rigidbody2D>();
                        item.GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                        item.transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                        item.GetComponent<Enemy>().initState(gameDataManager.LoadMonsterInfo(_unitObject,stateMonster)); 
                        select = item;
                        break;
                    }
                }

                if (!select) {
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>()._target = _playGameObject.GetComponent<Rigidbody2D>();
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().waypoints = wayPointList[stateMonster.moveType];
                    _monsters[stateMonster.monsterId].transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().initState(gameDataManager.LoadMonsterInfo(_unitObject,stateMonster));
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