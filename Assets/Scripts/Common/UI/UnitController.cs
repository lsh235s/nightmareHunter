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
        public List<GameObject>[] _summonList;

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
            _summonList = new List<GameObject>[_summoner.Length];

            for (int i = 0; i < _summoner.Length; i++) {
                _summonList[i] = new List<GameObject>();
            }

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
                Debug.Log("소환수 배치 ID : " + existTargetInfo[i].id);
                SummonerGet(existTargetInfo[i]);
           }
        }
        
        public void SummonerGet(PlayerInfo PlayerInfo) {
            GameObject inputObject = null;
            
            _summoner[PlayerInfo.id].GetComponent<Summons>().playerDataLoad(PlayerInfo);
            
            Debug.Log("소환수 배치 ID : " + PlayerInfo.positionInfoX +"/"+ PlayerInfo.positionInfoY +"/"+ PlayerInfo.positionInfoZ);
            Vector3 vector = new Vector3(float.Parse(PlayerInfo.positionInfoX), float.Parse(PlayerInfo.positionInfoY), float.Parse(PlayerInfo.positionInfoZ));
            

            _summoner[PlayerInfo.id].GetComponent<Collider2D>().isTrigger = true;
            inputObject = Instantiate(_summoner[PlayerInfo.id]);   

            _summonList[PlayerInfo.id].Add(inputObject);

            if (_summonList[PlayerInfo.id].Count > 0)
            {
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].GetComponent<Collider2D>().isTrigger = true;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].transform.position = vector;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].tag = "Summon";
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].name = PlayerInfo.spritesName + "_" + PlayerInfo.keyId;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].GetComponent<Summons>().summonerBatchKeyId = PlayerInfo.keyId;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].GetComponent<Summons>().basePlayerinfo = PlayerInfo;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].GetComponent<Summons>().activePlayerinfo = PlayerInfo;
                _summonList[PlayerInfo.id][_summonList[PlayerInfo.id].Count - 1].SetActive(true);
            }
        
        }

    }
}
