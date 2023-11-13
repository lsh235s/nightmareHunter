using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

namespace nightmareHunter {
    public class GameDataManager : MonoBehaviour
    {

        public static GameDataManager Instance { get; private set; }

        string[] summonList = new string[] {"hunter","priest","archer","icewitch","exorcist","monk","shaman","gostbuste"};
        public Dictionary<int, WeaponInfo> WeaponLoadInfo = new Dictionary<int, WeaponInfo>(); // 무기 정보


  
        void Awake()
        {
            // 이미 인스턴스가 있는지 확인합니다.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // 중복되는 인스턴스가 있는 경우, 이 게임 객체를 파괴합니다.
                Destroy(this.gameObject);
            }

        }



        public void LoadWeaponInfo() {
            if(WeaponLoadInfo.Count == 0 ) {
                List<Dictionary<string, object>> WeaponObjectList = CSVReader.Read("PlayerWeapon");

                for(int i = 0; i < WeaponObjectList.Count; i++) {
                    WeaponInfo weaponInfo = new WeaponInfo();
                    weaponInfo.id = int.Parse(WeaponObjectList[i]["Id"].ToString());
                    weaponInfo.WeaponName = WeaponObjectList[i]["WeaponName"].ToString();
                    weaponInfo.Level = int.Parse(WeaponObjectList[i]["Level"].ToString());
                    weaponInfo.PhysicsAttack = float.Parse(WeaponObjectList[i]["PhysicsAttack"].ToString());
                    weaponInfo.MagicAttack = float.Parse(WeaponObjectList[i]["MagicAttack"].ToString());
                    weaponInfo.LevPhysicsAttack = float.Parse(WeaponObjectList[i]["LevPhysicsAttack"].ToString());
                    weaponInfo.LevMagicAttack = float.Parse(WeaponObjectList[i]["LevMagicAttack"].ToString());
                    weaponInfo.AttackRange = float.Parse(WeaponObjectList[i]["AttackRange"].ToString());
                    weaponInfo.LevAttackRange = float.Parse(WeaponObjectList[i]["LevAttackRange"].ToString());
                    weaponInfo.Move = float.Parse(WeaponObjectList[i]["Move"].ToString());
                    weaponInfo.LevMove = float.Parse(WeaponObjectList[i]["LevMove"].ToString());
                    weaponInfo.AttackSpeed = float.Parse(WeaponObjectList[i]["AttackSpeed"].ToString());
                    weaponInfo.LevAttackSpeed = float.Parse(WeaponObjectList[i]["LevAttackSpeed"].ToString());
                    weaponInfo.Amount = int.Parse(WeaponObjectList[i]["Amount"].ToString());
                    weaponInfo.LevAmount = int.Parse(WeaponObjectList[i]["LevAmount"].ToString());
                    weaponInfo.ExistTime = float.Parse(WeaponObjectList[i]["ExistTime"].ToString());
                    weaponInfo.WeaponAttackType = WeaponObjectList[i]["WeaponAttackType"].ToString();
                    weaponInfo.AttackDelayTime = float.Parse(WeaponObjectList[i]["AttackDelayTime"].ToString());
                    weaponInfo.LevAttackDelayTime = float.Parse(WeaponObjectList[i]["LevAttackDelayTime"].ToString());

                    WeaponLoadInfo.Add(i, weaponInfo);
                }
            }
        }

        public void SavePlayerInfo(PlayerInfo playerInfo) {
            string json = JsonConvert.SerializeObject(playerInfo);

            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            System.IO.File.WriteAllText(filePath, json);
        }

        public PlayerInfo LoadPlayerInfo() {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");


            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerLevel = 1;

            for(int i = 0; i < unitObjectList.Count; i++) {
                if (unitObjectList[i]["UnitType"].ToString().Equals("0")) {
                    playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + (float.Parse(unitObjectList[i]["Health"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString())));
                    playerInfo.physicsAttack =  float.Parse(WeaponLoadInfo[0].PhysicsAttack.ToString()) + (float.Parse(WeaponLoadInfo[0].PhysicsAttack.ToString()) * ((playerInfo.playerLevel-1) * float.Parse(WeaponLoadInfo[0].LevPhysicsAttack.ToString())));
                    playerInfo.magicAttack =  float.Parse(WeaponLoadInfo[0].MagicAttack.ToString()) + (float.Parse(WeaponLoadInfo[0].MagicAttack.ToString()) * ((playerInfo.playerLevel-1) * float.Parse(WeaponLoadInfo[0].LevMagicAttack.ToString())));
                    playerInfo.attackRange =  (WeaponLoadInfo[0].AttackRange + (WeaponLoadInfo[0].AttackRange * ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevAttackRange))) * 0.1f;
                    playerInfo.move =  (WeaponLoadInfo[0].Move + (WeaponLoadInfo[0].Move * ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevMove))) * 0.1f;
                    playerInfo.attackSpeed =  float.Parse(WeaponLoadInfo[0].AttackSpeed.ToString()) + (float.Parse(WeaponLoadInfo[0].AttackSpeed.ToString()) * ((playerInfo.playerLevel-1) * float.Parse(WeaponLoadInfo[0].LevAttackSpeed.ToString())));
                    playerInfo.attackDelayTime =  WeaponLoadInfo[0].AttackDelayTime + (WeaponLoadInfo[0].AttackDelayTime * ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevAttackDelayTime));
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                }
            }
            return playerInfo;
        }

        public PlayerInfo PlayWeaponSet(int weaponID, PlayerInfo playerInfo) {
            playerInfo.weaponID = weaponID;
            playerInfo.physicsAttack = WeaponLoadInfo[weaponID].PhysicsAttack + (WeaponLoadInfo[weaponID].PhysicsAttack * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevPhysicsAttack));
            playerInfo.magicAttack = WeaponLoadInfo[weaponID].MagicAttack + (WeaponLoadInfo[weaponID].MagicAttack * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevMagicAttack));
            playerInfo.attackRange = (WeaponLoadInfo[weaponID].AttackRange + (WeaponLoadInfo[weaponID].AttackRange * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackRange))) * 0.1f;
            playerInfo.move = (WeaponLoadInfo[weaponID].Move + (WeaponLoadInfo[weaponID].Move * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevMove))) * 0.1f;
            playerInfo.attackSpeed = WeaponLoadInfo[weaponID].AttackSpeed + (WeaponLoadInfo[weaponID].AttackSpeed * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackSpeed));
            playerInfo.attackDelayTime = WeaponLoadInfo[weaponID].AttackDelayTime + (WeaponLoadInfo[weaponID].AttackDelayTime * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackDelayTime));
            playerInfo.weaponAttackType = WeaponLoadInfo[weaponID].WeaponAttackType;
            playerInfo.weaponAmount = WeaponLoadInfo[weaponID].Amount + (WeaponLoadInfo[weaponID].Amount * ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAmount));
            
            return playerInfo;
        }


        // 맵 시작시 소환 수 로드
        public List<PlayerInfo> SummerListLoad() {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            List<Dictionary<string, object>> summerBatchList = CSVReader.Read("SummonBatch");
            List<Dictionary<string, object>> summerLevel = CSVReader.Read("SummonLevel");
            List<PlayerInfo> existTargetInfo = new List<PlayerInfo>();

            for(int i=0; i < summerBatchList.Count; i++) {
                PlayerInfo playerInfo = new PlayerInfo();
                for(int j = 0; j < unitObjectList.Count; j++) {

                  // Debug.Log("playerInfo : " +unitObjectList[j]["UnitType"].ToString()+"/"+summerBatchList[i]["SummonId"].ToString() +"/"+unitObjectList[j]["Id"].ToString());
                    if ("2".Equals(unitObjectList[j]["UnitType"].ToString()) && summerBatchList[i]["SummonId"].ToString().Equals(unitObjectList[j]["Id"].ToString())) {
                     
                        playerInfo.keyId =  int.Parse(summerBatchList[i]["Id"].ToString());
                        playerInfo.id =  int.Parse(unitObjectList[j]["Id"].ToString());
                        int summonLevel = int.Parse(summerLevel[playerInfo.id]["Level"].ToString());
                        playerInfo.playerLevel = summonLevel;
                        
                        summonLevel = summonLevel -1;
                        playerInfo.goldCash = int.Parse(summerLevel[playerInfo.id]["GoldCash"].ToString()) + (int.Parse(summerLevel[playerInfo.id]["GoldCash"].ToString()) * (summonLevel* int.Parse(summerLevel[playerInfo.id]["LevGoldCash"].ToString())));
                        playerInfo.levGoldCash = int.Parse(summerLevel[playerInfo.id]["LevGoldCash"].ToString());
                        playerInfo.integerCash = int.Parse(summerLevel[playerInfo.id]["IntegerCash"].ToString()) + (int.Parse(summerLevel[playerInfo.id]["IntegerCash"].ToString()) * (summonLevel* int.Parse(summerLevel[playerInfo.id]["LevIntegerCash"].ToString())));
                        playerInfo.levIntegerCash = int.Parse(summerLevel[playerInfo.id]["LevIntegerCash"].ToString());
                        playerInfo.health = float.Parse(unitObjectList[j]["Health"].ToString()) + (float.Parse(unitObjectList[j]["Health"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevHealth"].ToString())));
                        playerInfo.physicsAttack =  float.Parse(unitObjectList[j]["PhysicsAttack"].ToString()) + (float.Parse(unitObjectList[j]["PhysicsAttack"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevPhysicsAttack"].ToString())));
                        playerInfo.magicAttack =  float.Parse(unitObjectList[j]["MagicAttack"].ToString()) + (float.Parse(unitObjectList[j]["MagicAttack"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevMagicAttack"].ToString())));
                        playerInfo.pysicsDefense =  float.Parse(unitObjectList[j]["PhysicsDefense"].ToString()) + (float.Parse(unitObjectList[j]["PhysicsDefense"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevPhysicsDefense"].ToString())));
                        playerInfo.magicDefense =  float.Parse(unitObjectList[j]["MagicDefense"].ToString()) + (float.Parse(unitObjectList[j]["MagicDefense"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevMagicDefense"].ToString())));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[j]["AttackRange"].ToString()) + (float.Parse(unitObjectList[j]["AttackRange"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevAttackRange"].ToString())))) ;
                        playerInfo.move =  (float.Parse(unitObjectList[j]["Move"].ToString()) + (float.Parse(unitObjectList[j]["Move"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevMove"].ToString()))))  * 0.1f;
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) + (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) * (summonLevel * float.Parse(unitObjectList[j]["LevAttackSpeed"].ToString())))) * 0.1f;
                        playerInfo.attackType = unitObjectList[j]["AttackType"].ToString();
                        playerInfo.spritesName = unitObjectList[j]["SpritesName"].ToString();
                        playerInfo.positionInfoX = summerBatchList[i]["PositionInfoX"].ToString();
                        playerInfo.positionInfoY = summerBatchList[i]["PositionInfoY"].ToString();
                        playerInfo.positionInfoZ = summerBatchList[i]["PositionInfoZ"].ToString();
                      //  Debug.Log("playerInfo : "+summonLevel+"/" +unitObjectList[j]["SpritesName"].ToString()+"/"+ playerInfo.attackRange+"/"+playerInfo.positionInfoX+"/"+playerInfo.positionInfoY+"/"+playerInfo.positionInfoZ);
                        existTargetInfo.Add(playerInfo);
                    }
                }
               
            }


            return existTargetInfo;
        }

        // 소환수 늘력치 재 조회
        public PlayerInfo loadSummerInfo(PlayerInfo inPlayerInfo, int summonerId ,int nowLevel) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            List<Dictionary<string, object>> summerLevel = CSVReader.Read("SummonLevel");
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo = inPlayerInfo;

            for(int j = 0; j < unitObjectList.Count; j++) {
                if ("2".Equals(unitObjectList[j]["UnitType"].ToString())) {
                        playerInfo.playerLevel = nowLevel;
                        playerInfo.goldCash = int.Parse(summerLevel[summonerId]["GoldCash"].ToString()) + (int.Parse(summerLevel[summonerId]["GoldCash"].ToString()) * (nowLevel* int.Parse(summerLevel[summonerId]["LevGoldCash"].ToString())));
                        playerInfo.levGoldCash = int.Parse(summerLevel[summonerId]["LevGoldCash"].ToString());
                        playerInfo.integerCash = int.Parse(summerLevel[summonerId]["IntegerCash"].ToString()) + (int.Parse(summerLevel[summonerId]["IntegerCash"].ToString()) * (nowLevel* int.Parse(summerLevel[summonerId]["LevIntegerCash"].ToString())));
                        playerInfo.levIntegerCash = int.Parse(summerLevel[summonerId]["LevIntegerCash"].ToString());
                        playerInfo.health = float.Parse(unitObjectList[j]["Health"].ToString()) + (float.Parse(unitObjectList[j]["Health"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevHealth"].ToString())));
                        playerInfo.physicsAttack =  float.Parse(unitObjectList[j]["PhysicsAttack"].ToString()) + (float.Parse(unitObjectList[j]["PhysicsAttack"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevPhysicsAttack"].ToString())));
                        playerInfo.magicAttack =  float.Parse(unitObjectList[j]["MagicAttack"].ToString()) + (float.Parse(unitObjectList[j]["MagicAttack"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevMagicAttack"].ToString())));
                        playerInfo.pysicsDefense =  float.Parse(unitObjectList[j]["PhysicsDefense"].ToString()) + (float.Parse(unitObjectList[j]["PhysicsDefense"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevPhysicsDefense"].ToString())));
                        playerInfo.magicDefense =  float.Parse(unitObjectList[j]["MagicDefense"].ToString()) + (float.Parse(unitObjectList[j]["MagicDefense"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevMagicDefense"].ToString())));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[j]["AttackRange"].ToString()) + (float.Parse(unitObjectList[j]["AttackRange"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevAttackRange"].ToString())))) ;
                        playerInfo.move =  (float.Parse(unitObjectList[j]["Move"].ToString()) + (float.Parse(unitObjectList[j]["Move"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevMove"].ToString()))) ) * 0.1f;
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) + (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) * (nowLevel * float.Parse(unitObjectList[j]["LevAttackSpeed"].ToString())))) * 0.1f;
    
                }
            }
      

            return playerInfo;
        }


        // 소환수 배치시 정보 저장
        public int SaveSummerInfo(string intPlayerInfo, PlayerInfo playerInfo) {
            List<Dictionary<string, object>> dataList = LoadData("SummonBatch");

            string maxid = "0";
            if(dataList.Count > 0) {
                maxid = dataList[dataList.Count -1]["Id"].ToString();
            }
           
            int playId = int.Parse(maxid) + 1;
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Id", playId);
            data.Add("Level", playerInfo.playerLevel);
            data.Add("SummonId", Array.IndexOf(summonList, intPlayerInfo));
            data.Add("PositionInfoX", playerInfo.positionInfoX);
            data.Add("PositionInfoY", playerInfo.positionInfoY);
            data.Add("PositionInfoZ", playerInfo.positionInfoZ);

            dataList.Add(data);
            SaveData(dataList);

            return playId;
        }


        // 소환수 강화시 정보 저장
        public int SaveSummerlevelInfo(string intPlayerInfo, PlayerInfo playerInfo) {
            List<Dictionary<string, object>> dataList = LoadData("SummonBatch");

            string maxid = "0";
            if(dataList.Count > 0) {
                maxid = dataList[dataList.Count -1]["Id"].ToString();
            }
           
            int playId = int.Parse(maxid) + 1;
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Id", playId);
            data.Add("Level", playerInfo.playerLevel);
            data.Add("SummonId", Array.IndexOf(summonList, intPlayerInfo));
            data.Add("PositionInfoX", playerInfo.positionInfoX);
            data.Add("PositionInfoY", playerInfo.positionInfoY);
            data.Add("PositionInfoZ", playerInfo.positionInfoZ);

            dataList.Add(data);
            SaveData(dataList);

            return playId;
        }


        public int UpdateCsvData(string targetName)
        {
            int result = 0;
            string filePath = Application.dataPath + "/Plugin/SaveData/SummonLevel.csv";
            List<string[]> csvData = new List<string[]>();

            // 기존 CSV 파일 데이터 읽기
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    csvData.Add(values);
                }
            }

            // 업데이트할 데이터 찾아서 수정
            for (int i = 1; i < csvData.Count; i++) // 첫 번째 행은 헤더
            {
                if (csvData[i][1] == targetName) // 두 번째 컬럼이 이름인 경우
                {
                    result = int.Parse(csvData[i][2].ToString()) + 1;
                    csvData[i][2] = result.ToString(); // 세 번째 컬럼 업데이트
                    break;
                }
            }

            // 수정된 데이터를 다시 파일에 쓰기
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string[] values in csvData)
                {
                    string line = string.Join(",", values);
                    writer.WriteLine(line);
                }
            }
            return result;
        }

        // 소환수 능력치 조회
        public PlayerInfo LoadSummerInfo(string summonName) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            List<Dictionary<string, object>> summerLevel = CSVReader.Read("SummonLevel");
            
            PlayerInfo playerInfo = new PlayerInfo();

            for(int i = 0; i < unitObjectList.Count; i++) {
                if(2 == int.Parse(unitObjectList[i]["UnitType"].ToString()) && summonName.Equals((unitObjectList[i]["SpritesName"].ToString())) ) {
                    playerInfo.id = int.Parse(unitObjectList[i]["Id"].ToString());
                    int summonLevel = int.Parse(summerLevel[playerInfo.id]["Level"].ToString());
                    
                    playerInfo.playerLevel = summonLevel;
                    summonLevel = summonLevel -1;
                    playerInfo.goldCash = int.Parse(summerLevel[playerInfo.id]["GoldCash"].ToString()) + (int.Parse(summerLevel[playerInfo.id]["GoldCash"].ToString()) * (summonLevel* int.Parse(summerLevel[playerInfo.id]["LevGoldCash"].ToString())));
                    playerInfo.levGoldCash = int.Parse(summerLevel[playerInfo.id]["LevGoldCash"].ToString());
                    playerInfo.integerCash = int.Parse(summerLevel[playerInfo.id]["IntegerCash"].ToString()) + (int.Parse(summerLevel[playerInfo.id]["IntegerCash"].ToString()) * (summonLevel* int.Parse(summerLevel[playerInfo.id]["LevIntegerCash"].ToString())));
                    playerInfo.levIntegerCash = int.Parse(summerLevel[playerInfo.id]["LevIntegerCash"].ToString());
                    playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + (float.Parse(unitObjectList[i]["Health"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevHealth"].ToString())));
                    playerInfo.physicsAttack =  float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) + (float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevPhysicsAttack"].ToString())));
                    playerInfo.magicAttack =  float.Parse(unitObjectList[i]["MagicAttack"].ToString()) + (float.Parse(unitObjectList[i]["MagicAttack"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevMagicAttack"].ToString())));
                    playerInfo.pysicsDefense =  float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) + (float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevPhysicsDefense"].ToString())));
                    playerInfo.magicDefense =  float.Parse(unitObjectList[i]["MagicDefense"].ToString()) + (float.Parse(unitObjectList[i]["MagicDefense"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevMagicDefense"].ToString())));
                    playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + (float.Parse(unitObjectList[i]["AttackRange"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevAttackRange"].ToString()))));
                    playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + (float.Parse(unitObjectList[i]["Move"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevMove"].ToString()))) ) * 0.1f;
                    playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) * (summonLevel* float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString())))) * 0.1f;
                    playerInfo.attackType = unitObjectList[i]["AttackType"].ToString();
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                    
                  //  Debug.Log("LoadSummerInfo : "+summonLevel+"/" +unitObjectList[i]["SpritesName"].ToString()+"/"+ playerInfo.attackRange+"/"+playerInfo.positionInfoX+"/"+playerInfo.positionInfoY+"/"+playerInfo.positionInfoZ);
                }
            }

           return playerInfo; 
        }

        public List<Dictionary<string, object>> MonsterBatch(int stageId, int day) {
            Debug.Log("stageId : " + stageId + "/day : " + day);
            List<Dictionary<string, object>> monsterBatchList = CSVReader.Read("StateMonsterBatch"+stageId);
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

            for(int i = 0; i < monsterBatchList.Count; i++) {
                if(day == int.Parse(monsterBatchList[i]["Day"].ToString())) {
                    dataList.Add(monsterBatchList[i]);
                }
            }
            return dataList; 
        }

        public PlayerInfo LoadMonsterInfo( Dictionary<string, object> stateMonster) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerLevel = (int)stateMonster["Level"];

            for(int i = 0; i < unitObjectList.Count; i++) {
                if ("1".Equals(unitObjectList[i]["UnitType"].ToString()) && int.Parse(unitObjectList[i]["Id"].ToString()) == (int)stateMonster["MonsterId"]) {
                    playerInfo.health =  float.Parse(unitObjectList[i]["Health"].ToString()) + (float.Parse(unitObjectList[i]["Health"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString())));
                    playerInfo.physicsAttack =  float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) + (float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsAttack"].ToString())));
                    playerInfo.magicAttack =  float.Parse(unitObjectList[i]["MagicAttack"].ToString()) + (float.Parse(unitObjectList[i]["MagicAttack"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicAttack"].ToString())));
                    playerInfo.pysicsDefense =  float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) + (float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsDefense"].ToString())));
                    playerInfo.magicDefense =  float.Parse(unitObjectList[i]["MagicDefense"].ToString()) + (float.Parse(unitObjectList[i]["MagicDefense"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicDefense"].ToString())));
                    playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + (float.Parse(unitObjectList[i]["AttackRange"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackRange"].ToString()))))  * 0.1f;
                    playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + (float.Parse(unitObjectList[i]["Move"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMove"].ToString())))) * 0.1f;
                    playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) * ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString())))) * 0.1f;
                    playerInfo.reward =  int.Parse(unitObjectList[i]["Reward"].ToString()) ;
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                }
            }

            
            return playerInfo;
        }

        // 소환수 배치 초기화
        public void GameDataInit() {
            List<Dictionary<string, object>> dataList = LoadData("SummonBatch");

            for(int i=0; i < dataList.Count; i++) {
                if(i > 0) {
                    DeleteData("Id", dataList[i]["Id"].ToString());
                }
            }


            //소환수 레벨 초기화
            string filePath = Application.dataPath + "/Plugin/SaveData/SummonLevel.csv";
            List<string[]> csvData = new List<string[]>();

            // 기존 CSV 파일 데이터 읽기
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    csvData.Add(values);
                }
            }
            // 업데이트할 데이터 찾아서 수정
            for (int i = 1; i < csvData.Count; i++) // 첫 번째 행은 헤더
            {
                csvData[i][2] = "1"; // 세 번째 컬럼 업데이트
            }

            // 수정된 데이터를 다시 파일에 쓰기
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string[] values in csvData)
                {
                    string line = string.Join(",", values);
                    writer.WriteLine(line);
                }
            }
        }

        public void SaveSystemInfo(SystemSaveInfo systemSaveInfo) {
            string json = JsonConvert.SerializeObject(systemSaveInfo);
         //   Debug.Log("json:"+json);

            string fileName = "SystemData.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            System.IO.File.WriteAllText(filePath, json);
        }

        
        public SystemSaveInfo LoadSystemSaveInfo() {
            SystemSaveInfo systemSaveInfo = new SystemSaveInfo();
            
            string fileName = "SystemData.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            string jsonString = File.ReadAllText(filePath);
            systemSaveInfo = JsonConvert.DeserializeObject<SystemSaveInfo>(jsonString);
            return systemSaveInfo;
        }


        /// <summary>
        /// Json 데이터를 읽고 쓰기 위한 기능
        /// </summary>
        /// <returns></returns>

   

        // 데이터 리스트 추가하기
        public void SaveData(List<Dictionary<string, object>> existingData)
        {
            string filePath = Application.dataPath + "/Plugin/SaveData/SummonBatch.csv" ;


            using (StreamWriter sw = new StreamWriter(filePath))
            {
                if (existingData.Count > 0)
                {
                    // CSV 파일의 헤더 작성
                    var firstDictionary = existingData[0];
                    var headers = new List<string>();

                    foreach (var key in firstDictionary.Keys)
                    {
                        headers.Add(key);
                    }

                    sw.WriteLine(string.Join(",", headers));

                    // 데이터 작성
                    foreach (var data in existingData)
                    {
                        string[] values = new string[headers.Count];
                        for (int i = 0; i < headers.Count; i++)
                        {
                            values[i] = data[headers[i]].ToString();
                        }
                        sw.WriteLine(string.Join(",", values));
                    }
                }
            }
        }

        public List<Dictionary<string, object>> LoadData(string fileName)
        {
            string filePath = Application.dataPath + "/Plugin/SaveData/"+fileName+".csv";
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                string headerLine = sr.ReadLine();
                string[] headers = headerLine.Split(',');

                while (!sr.EndOfStream)
                {
                    string dataLine = sr.ReadLine();
                    string[] values = dataLine.Split(',');

                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        data[headers[i]] = values[i];
                    }

                    dataList.Add(data);
                }
            }

            return dataList;
        }

        public void DeleteData(string columnName, string value)
        {
            List<Dictionary<string, object>> dataList = LoadData("SummonBatch");
            dataList.RemoveAll(data => data.ContainsKey(columnName) && data[columnName].ToString() == value);
            SaveData(dataList);
        }

    }
}