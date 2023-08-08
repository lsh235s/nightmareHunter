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
            DevelMonsterBatch =  CSVReader.Read("StateMonsterBatch");

        }

        public void GameStart()
        {
            baseDataLoad();
            //소환수 배치
            SummonerInit();
            //플레이어 능력치 load
            PlayerInit();
        }

        void baseDataLoad() {
            GameDataManager.Instance.LoadWeaponInfo();
        }

        // 플레이어 배치 관련
        void PlayerInit() {
            _playGameObject.GetComponent<Player>().playerDataLoad(GameDataManager.Instance.LoadPlayerInfo()); 
        }

        // 저장된 소환수 배치 관련
        void SummonerInit() {
           List<PlayerInfo> existTargetInfo = GameDataManager.Instance.SummerListLoad();
           for(int i = 0; i < existTargetInfo.Count; i++)
           {
                SummonerGet(existTargetInfo[i]);
           }
        }
        
        public void SummonerGet(PlayerInfo PlayerInfo) {
            GameObject select = null;
            
            Debug.Log("PlayerInfo.id:"+PlayerInfo.id);
            select = _summoner[PlayerInfo.id];
            select.GetComponent<Summons>().playerDataLoad(PlayerInfo);
            
            Vector3 vector = new Vector3(float.Parse(PlayerInfo.positionInfoX), float.Parse(PlayerInfo.positionInfoY), float.Parse(PlayerInfo.positionInfoZ));
          
            _summoner[PlayerInfo.id] = Instantiate(select);   
            _summoner[PlayerInfo.id].tag = "Summon";
            _summoner[PlayerInfo.id].GetComponent<Collider2D>().isTrigger = true;
            _summoner[PlayerInfo.id].transform.position = vector;
         
        }

    }
}
