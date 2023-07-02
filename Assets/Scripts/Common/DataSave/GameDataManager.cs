using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace nightmareHunter {
    public class GameDataManager : MonoBehaviour
    {
        string[] summonList = new string[] {"Hunter","Exorcist"};

  

        public void SavePlayerInfo(PlayerInfo playerInfo) {
            string json = JsonConvert.SerializeObject(playerInfo);

            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            System.IO.File.WriteAllText(filePath, json);
        }

        public  PlayerInfo LoadPlayerInfo() {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            // JSON 파일 로드
            string jsonString = File.ReadAllText(filePath);

            PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

            for(int i = 0; i < unitObjectList.Count; i++) {
                if (unitObjectList[i]["UnitType"].ToString().Equals("0")) {
                    playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                    playerInfo.attack =  float.Parse(unitObjectList[i]["Attack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttack"].ToString()));
                    playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackRange"].ToString()))) * 0.1f;
                    playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMove"].ToString()))) *0.1f;
                    playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString()))) * 0.1f;
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                }
            }


            return playerInfo;
        }

        public List<PlayerInfo> SummerListLoad() {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            List<PlayerInfo> existTargetInfo = new List<PlayerInfo>();
            for(int i = 0; i < summonList.Length; i++) {
                string fileName = summonList[i] + ".json";
                string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;


                string jsonString = File.ReadAllText(filePath);
                PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

                for(int j = 0; j < unitObjectList.Count; j++) {
                    if ("2".Equals(unitObjectList[j]["UnitType"].ToString()) && summonList[i].Equals(unitObjectList[j]["SpritesName"].ToString())) {
                        playerInfo.health = float.Parse(unitObjectList[j]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[j]["LevHealth"].ToString()));
                        playerInfo.attack =  float.Parse(unitObjectList[j]["Attack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[j]["LevAttack"].ToString()));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[j]["AttackRange"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[j]["LevAttackRange"].ToString()))) * 0.1f;
                        playerInfo.move =  (float.Parse(unitObjectList[j]["Move"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[j]["LevMove"].ToString())) * 0.1f);
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[j]["LevAttackSpeed"].ToString()))) * 0.1f;
                        playerInfo.spritesName = unitObjectList[j]["SpritesName"].ToString();
                    }
                }

                if(playerInfo.summonsExist) {
                    existTargetInfo.Add(playerInfo);
                }
            }

            return existTargetInfo;
        }

        public void SaveSummerInfo(string intPlayerInfo, PlayerInfo playerInfo) {

            string json = JsonConvert.SerializeObject(playerInfo);

            string fileName = intPlayerInfo + ".json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            System.IO.File.WriteAllText(filePath, json);
        }

        public PlayerInfo LoadSummerInfo(int intPlayerInfo, UnitObject unitObject) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            string fileName; 
            PlayerInfo playerInfo = new PlayerInfo();
            if(intPlayerInfo > summonList.Length -1) {
                fileName = "none";
            } else {
                fileName = summonList[intPlayerInfo] +".json";
            }
            if (!"none".Equals(fileName)) {
                string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

                // JSON 파일 로드
                string jsonString = File.ReadAllText(filePath);
                playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

                for(int i = 0; i < unitObjectList.Count; i++) {
                    if("2".Equals(unitObjectList[i]["UnitType"]) && summonList[intPlayerInfo].Equals(unitObjectList[i]["SpritesName"]) ) {
                        playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                        playerInfo.attack =  float.Parse(unitObjectList[i]["Attack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttack"].ToString()));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackRange"].ToString()))) * 0.1f;
                        playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMove"].ToString())) * 0.1f);
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString()))) * 0.1f;
                        playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                    }
                }

            }
            Debug.Log("playerInfo : " + playerInfo.attackRange);
            return playerInfo;
        }

        public PlayerInfo LoadMonsterInfo( Dictionary<string, object> stateMonster) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerLevel = (int)stateMonster["Level"];

            for(int i = 0; i < unitObjectList.Count; i++) {
                if ("1".Equals(unitObjectList[i]["UnitType"].ToString()) && int.Parse(unitObjectList[i]["Id"].ToString()) == (int)stateMonster["MonsterId"]) {
                    playerInfo.health =  float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                    playerInfo.attack =  float.Parse(unitObjectList[i]["Attack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttack"].ToString()));
                    playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackRange"].ToString())))  * 0.1f;
                    playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMove"].ToString()))) * 0.1f;
                    playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString()))) * 0.1f;
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                }
            }

            
            return playerInfo;
        }




        public void SaveSystemInfo(SystemSaveInfo systemSaveInfo) {
            string json = JsonConvert.SerializeObject(systemSaveInfo);

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

    }
}