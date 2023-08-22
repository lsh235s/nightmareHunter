using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace nightmareHunter {
    public class GameMoonManager : MonoBehaviour
    {

        Texture2D NormalIcon;
        Texture2D AttckIcon;
 
        [SerializeField]
        GameObject[] wayPointList; 

        [SerializeField]
        LoadingControl _loadingControl; // 로딩 컨트롤

        private List<GameObject>[] _monsterList;
        public GameObject[] _monsters;

        string appearStageTimer = "";


        UnitController _uiItController;
        bool stageClear = false;


        void Awake() {
           
            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();
        }

        void Start()
        { 
            UiController.Instance.LoadStart();
            AudioManager.Instance.BackGroundPlay("bgm_game");
            _uiItController.GameStart();
            _loadingControl.FadeActive();
            StartCoroutine(_loadingControl.FadeInStart());

            NormalIcon = Resources.Load<Texture2D>("ui/pointer"); 
            AttckIcon = Resources.Load<Texture2D>("ui/pointer2");

            Cursor.SetCursor(NormalIcon, new Vector2(NormalIcon.width / 3, 0), CursorMode.Auto);
            
            //몬스터 배치
           monsterInit();
        }         

        void Update() {
            monsterInitappear();
            summonerScan();

            if(appearStageTimer != "" ) { 
                if(!stageClear) {
                    if(AreAllMonstersDead(_monsterList)) {
                        stageClear = true;
                        UiController.Instance.stageClear();
                    }
                } 
            }

            if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(AttckIcon, new Vector2(AttckIcon.width / 5, 0), CursorMode.Auto);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(NormalIcon, new Vector2(NormalIcon.width / 3, 0), CursorMode.Auto);
            }
        }

        //모든 소환수 공격 가능여부 체크
        void summonerScan() {
            for (int i = 0; i < _uiItController._summonList.Length; i++) {
                for (int j = 0; j < _uiItController._summonList[i].Count; j++) {
                    _uiItController._summonList[i][j].GetComponent<Summons>().nextTime = _uiItController._summonList[i][j].GetComponent<Summons>().nextTime + Time.deltaTime;
                    if(_uiItController._summonList[i][j].GetComponent<Summons>().nextTime  > _uiItController._summonList[i][j].GetComponent<Summons>()._attackSpeed) {
                        _uiItController._summonList[i][j].GetComponent<Summons>().scanRadar();
                        _uiItController._summonList[i][j].GetComponent<Summons>().nextTime = 0.0F;
                    }
                }
            }
        }

        void monsterInitappear() {
            int listNum = 0;
            int teller = 0;
            int wanderer = 0;

            if(UiController.Instance._timerText.text != appearStageTimer) {
                appearStageTimer = UiController.Instance._timerText.text;

                for (int i = 0; i < _uiItController.DevelMonsterBatch.Count; i++) {
                    listNum = 0;
                    if(appearStageTimer.Equals(_uiItController.DevelMonsterBatch[i]["AppearTimer"])) {
                        if((int)_uiItController.DevelMonsterBatch[i]["MonsterId"] == 0) {
                            listNum = wanderer;
                            wanderer++;
                        } else if((int)_uiItController.DevelMonsterBatch[i]["MonsterId"] == 1) {
                            listNum = teller;
                            teller++;
                        }

                        if(!_monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].activeSelf && _monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].GetComponent<Enemy>().isDead == false) {
                            _monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].SetActive(true);
                        }
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
            _uiItController.monsterBuildCount = _uiItController.DevelMonsterBatch.Count;
            _monsterList = new List<GameObject>[_monsters.Length];

            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();
            }

            for (int i = 0; i <  _uiItController.DevelMonsterBatch.Count; i++) {
                MonsterGet(_uiItController.DevelMonsterBatch[i]);
            }
        }

        
        public GameObject MonsterGet(Dictionary<string, object> stateMonster)  {
            GameObject select = null;

            if(_monsterList.Length > (int)stateMonster["MonsterId"])
            {
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>()._monsterId = (int)stateMonster["MonsterId"];
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().clientTarget = _uiItController._targetGameObject;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().playerTarget = _uiItController._playGameObject;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().waypointType = (int)stateMonster["MoveType"];
                if((int)stateMonster["MonsterId"] == 1) {
                    _monsters[(int)stateMonster["MonsterId"]].transform.position = _uiItController._targetGameObject.transform.position;
                } else {
                    _monsters[(int)stateMonster["MonsterId"]].transform.position = wayPointList[(int)stateMonster["MoveType"]].transform.GetChild(0).gameObject.transform.position;
                }
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().wayPointBaseList = wayPointList;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().initState(GameDataManager.Instance.LoadMonsterInfo(stateMonster));
                
                select = Instantiate(_monsters[(int)stateMonster["MonsterId"]]);

                _monsterList[(int)stateMonster["MonsterId"]].Add(select);

                select.SetActive(false);
            } else {
                Debug.Log("not find monsterID");
            }

            return select;
        }

        public void SplitSkillAdd(int monsterID, GameObject target) {
            _monsterList[monsterID].Add(target);
        }
    }
}