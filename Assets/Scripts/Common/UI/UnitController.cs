using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace nightmareHunter {
    public class UnitController : MonoBehaviour
    {
        // 저장 데이터
        public UnitObject _unitObject;

        //운영 시 몬스터 배치 정보
        public StateMonsterBatch _stateMonsterBatch ;
        //개발 시 몬스터 배치 정보
        public List<Dictionary<string, object>> DevelMonsterBatch  ;

        // 저장 기능 관련
        public GameDataManager gameDataManager ;
        // 스테이지 정보 저장
        
        public GameObject[] _summoner;

        // Start is called before the first frame update
        public GameObject _playGameObject; 
        public GameObject _targetGameObject;

        //스테이지에 배치된 몬스터 수
        public int monsterBuildCount = 0;
        // 플레이어 제거 수
        public int monsterKillCount = 0;

        
        // Start is called before the first frame update
        void Awake() {           
            gameDataManager = new GameDataManager();

           
            DevelMonsterBatch =  CSVReader.Read("StateMonsterBatch");

        }

        public void GameStart()
        {
            //소환수 배치
            SummonerInit();
            //플레이어 능력치 load
            PlayerInit();
        }


        // 플레이어 배치 관련
        void PlayerInit() {
            _playGameObject.GetComponent<Player>().playerDataLoad(gameDataManager.LoadPlayerInfo()); 
        }

        // 소환수 배치 관련
        void SummonerInit() {
           List<PlayerInfo> existTargetInfo = gameDataManager.SummerListLoad();
           for(int i = 0; i < existTargetInfo.Count; i++)
           {
                SummonerGet(existTargetInfo[i]);
           }
        }
        
        public void SummonerGet(PlayerInfo PlayerInfo) {
            GameObject select = null;
            
            select = _summoner[PlayerInfo.id];
            select.GetComponent<Summons>().playerDataLoad(PlayerInfo);
            string[] vectorValues = PlayerInfo.positionInfo.Trim('(', ')').Split(',');
            
            Vector3 vector = new Vector3(float.Parse(vectorValues[0]), float.Parse(vectorValues[1]), float.Parse(vectorValues[2]));
          
            _summoner[PlayerInfo.id] = Instantiate(select);   
            _summoner[PlayerInfo.id].tag = "Summon";
            _summoner[PlayerInfo.id].GetComponent<Collider2D>().isTrigger = true;
            _summoner[PlayerInfo.id].transform.position = vector;
         
        }

    }
}
