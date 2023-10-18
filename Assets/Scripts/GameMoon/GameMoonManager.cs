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
        GameObject stageBackGround; // 배경
 
        [SerializeField]
        GameObject wayPointGroup; 
        [SerializeField]
        GameObject[] wayPointList; 


        [SerializeField]
        LoadingControl _loadingControl; // 로딩 컨트롤

        private List<GameObject>[] _monsterList;
        Dictionary<int, int> monsterActiveCnt = new Dictionary<int, int>();
        public GameObject[] _monsters;

        string appearStageTimer = "";


        UnitController _uiItController;
        bool stageClear = false;
        
        int teller = 0;
        int wanderer = 0;


        void Awake() {
           
            _uiItController = GameObject.Find("Canvas").GetComponent<UnitController>();
        }

        void Start()
        { 
           
            UiController.Instance.LoadStart();
            string stageName = "Prefabs/Stage/" + UiController.Instance.systemSaveInfo.stageId;
            //string wayPoint = "Prefabs/Waypoint/WayPoint" + UiController.Instance.systemSaveInfo.stageId;
           // GameObject wayPointLoad = Instantiate(Resources.Load<GameObject>(wayPoint));
            wayPointList[0] = wayPointGroup.transform.Find("WaypointA").gameObject;
            wayPointList[1] = wayPointGroup.transform.Find("WaypointB").gameObject;
            wayPointList[2] = wayPointGroup.transform.Find("WaypointC").gameObject;
            wayPointList[3] = wayPointGroup.transform.Find("WaypointD").gameObject;
            wayPointList[4] = wayPointGroup.transform.Find("Directlypoint").gameObject;
           // stageBackGround = Instantiate(Resources.Load<GameObject>(stageName));
            stageBackGround.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = stageBackGround.GetComponent<BackgroundController>().backGroundSprite[1];

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
                        int retrunGold = 0;
                        for (int i = 0; i < _uiItController._summonList.Length; i++) {
                            for (int j = 0; j < _uiItController._summonList[i].Count; j++) {
                               retrunGold = retrunGold + (_uiItController._summonList[i][j].GetComponent<Summons>().activePlayerinfo.goldCash / 2);
                            }
                        }
                        UiController.Instance.goldUseSet(retrunGold,"+");
                        GameDataManager.Instance.GameDataInit();

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

            if(UiController.Instance._timerText.text != appearStageTimer) {
                appearStageTimer = UiController.Instance._timerText.text;
                string appearTimer = appearStageTimer; 

                if (appearStageTimer.Length > 0 && appearStageTimer[0] == '0')
                {
                    appearTimer = appearStageTimer.Substring(1);
                }

                for (int i = 0; i < _uiItController.DevelMonsterBatch.Count; i++) {
                    string appearCsvTimer = _uiItController.DevelMonsterBatch[i]["AppearTimer"].ToString();
                    if (appearCsvTimer[0] == '0' && appearCsvTimer[1] != ':')
                    {
                        appearCsvTimer = appearCsvTimer.Substring(1);
                    }

                    Debug.Log("//////asd:"+appearTimer +"/"+ _uiItController.DevelMonsterBatch[i]["AppearTimer"]+"/"+ appearCsvTimer);
                    if(appearTimer.Equals(appearCsvTimer) && UiController.Instance.systemSaveInfo.stageId == (int)_uiItController.DevelMonsterBatch[i]["Stage"]) {
                        listNum = monsterActiveCnt[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]];

                        if(!_monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].activeSelf && _monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].GetComponent<Enemy>().isDead == false) {
                            _monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].SetActive(true);
                           
                            if((int)_uiItController.DevelMonsterBatch[i]["MonsterId"] == 1) {
                                _monsterList[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]][listNum].transform.position = _uiItController._targetGameObject.transform.position;
                            }
                            monsterActiveCnt[(int)_uiItController.DevelMonsterBatch[i]["MonsterId"]] = listNum + 1;
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
            int stageMonsterCnt = 0;
            for(int i = 0; i < _uiItController.DevelMonsterBatch.Count; i++) {
                if(UiController.Instance.systemSaveInfo.stageId == (int)_uiItController.DevelMonsterBatch[i]["Stage"]) {
                    stageMonsterCnt++;
                }
            }

            _uiItController.monsterBuildCount = stageMonsterCnt;
            _monsterList = new List<GameObject>[_monsters.Length];  //몬스터 종류 배열 생성


            for (int i = 0; i < _monsterList.Length; i++) {
                _monsterList[i] = new List<GameObject>();  //몬스터 종류별 배열 생성
                
                monsterActiveCnt.Add(i, 0);  //몬스터 종류별 출현 카운트
            }

            for (int i = 0; i <  _uiItController.DevelMonsterBatch.Count; i++) {
                if(UiController.Instance.systemSaveInfo.stageId == (int)_uiItController.DevelMonsterBatch[i]["Stage"]) {
                    MonsterGet(_uiItController.DevelMonsterBatch[i]);
                }
            }
        }

        
        public GameObject MonsterGet(Dictionary<string, object> stateMonster)  {
            GameObject select = null;


            if(_monsterList.Length > (int)stateMonster["MonsterId"] && UiController.Instance.systemSaveInfo.stageId == (int)stateMonster["Stage"])
            {
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>()._monsterId = (int)stateMonster["MonsterId"];
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().clientTarget = _uiItController._targetGameObject;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().playerTarget = _uiItController._playGameObject;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().waypointType = (int)stateMonster["MoveType"];

                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().wayPointBaseList = wayPointList;
                _monsters[(int)stateMonster["MonsterId"]].GetComponent<Enemy>().initState(GameDataManager.Instance.LoadMonsterInfo(stateMonster));
                
                select = Instantiate(_monsters[(int)stateMonster["MonsterId"]]);

                if((int)stateMonster["MonsterId"] == 1) {
                    _monsters[(int)stateMonster["MonsterId"]].transform.position = _uiItController._targetGameObject.transform.position;
                } else {
                    _monsters[(int)stateMonster["MonsterId"]].transform.position = wayPointList[(int)stateMonster["MoveType"]].transform.GetChild(0).gameObject.transform.position;
                }

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