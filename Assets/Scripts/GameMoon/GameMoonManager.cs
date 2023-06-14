using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class GameMoonManager : MonoBehaviour
    {
 
        [SerializeField]
        GameObject[] wayPointList; 

        [SerializeField]
        LoadingControl _loadingControl; // 로딩 컨트롤

        private List<GameObject>[] _monsterList;
        public GameObject[] _monsters;

        string appearStageTimer = "";


        UnitController _uiItController;
        bool stageClear = false;
        int monsterBuildCount = 0;

        void Awake() {
            UiController.Instance.LoadStart();
            
            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();
        }

        void Start()
        {
            AudioManager.Instance.BackGroundPlay("bgm_game");
            _uiItController.GameStart();
            _loadingControl.FadeActive();
            StartCoroutine(_loadingControl.FadeInStart());
            
            //몬스터 배치
           monsterInit();
        }         

        void Update() {
            //monsterInitappear();

            if(appearStageTimer != "" ) { 
                if(!stageClear) {
                    if(AreAllMonstersDead(_monsterList)) {
                        stageClear = true;
                        UiController.Instance.stageClear();
                    }
                } 
            }
        }

        void monsterInitappear() {
            if(UiController.Instance._timerText.text != appearStageTimer) {
                appearStageTimer = UiController.Instance._timerText.text;

                for (int i = monsterBuildCount; i < _uiItController._stateMonsterBatch.stateMonsterList.Count; i++) {
                    if(appearStageTimer == _uiItController._stateMonsterBatch.stateMonsterList[i].appearTimer) {
                        _monsterList[0][monsterBuildCount].SetActive(true);
                        monsterBuildCount++;
                    }
                }
            }
        }

        bool AreAllMonstersDead(List<GameObject>[] monsterGroups)
        {
            foreach (List<GameObject> monsterGroup in monsterGroups)
            {
                foreach (GameObject monster in monsterGroup)
                {
                    if(monster != null) {
                        Enemy monsterScript = monster.GetComponent<Enemy>();
                        
                        if (monsterScript != null && monsterScript.isDead == false)
                        {
                            // 하나라도 살아있는 몬스터가 있다면, 모두 죽지 않았다고 반환합니다.
                            return false;
                        }
                    }
                }
            }
            // 모든 몬스터 그룹의 모든 몬스터를 확인했는데, 모두 죽었다면 true를 반환합니다.
            return true;
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
                        item.GetComponent<Enemy>().clientTarget = _uiItController._targetGameObject;
                        item.GetComponent<Enemy>().playerTarget = _uiItController._playGameObject;
                        item.GetComponent<Enemy>().waypointType = stateMonster.moveType;
                        item.transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                        item.GetComponent<Enemy>().wayPointBaseList = wayPointList;
                        item.GetComponent<Enemy>().initState(_uiItController.gameDataManager.LoadMonsterInfo(_uiItController._unitObject,stateMonster)); 
                        select = item;
                        break;
                    }
                }

                if (!select) {
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().clientTarget = _uiItController._targetGameObject;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().playerTarget = _uiItController._playGameObject;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().waypointType = stateMonster.moveType;
                    _monsters[stateMonster.monsterId].transform.position = wayPointList[stateMonster.moveType].transform.GetChild(0).gameObject.transform.position;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().wayPointBaseList = wayPointList;
                    _monsters[stateMonster.monsterId].GetComponent<Enemy>().initState(_uiItController.gameDataManager.LoadMonsterInfo(_uiItController._unitObject,stateMonster));
                    select = Instantiate(_monsters[stateMonster.monsterId]);
                    _monsterList[stateMonster.monsterId].Add(select);
                }
            } else {
                Debug.Log("not find monsterID");
            }
            select.SetActive(false);

            return select;
        }
    }
}