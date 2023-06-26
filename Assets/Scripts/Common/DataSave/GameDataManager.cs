using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace nightmareHunter {
    public class GameDataManager : MonoBehaviour
    {
        string[] summonList = new string[] {"Hunter","Exorcist"};

        public void HunterInit() {
            PlayerInfo playerInfo = new PlayerInfo();


            playerInfo.id = 0;
            playerInfo.playerLevel = 0;
            playerInfo.health = 0;
            playerInfo.attack = 0;
            playerInfo.attackRange = 0;
            playerInfo.move = 0;
            playerInfo.attackSpeed = 0;
            playerInfo.positionInfo = "";
            playerInfo.spritesName = "Hunter";
            playerInfo.reward = 0;
            playerInfo.summonsExist = false;


            SaveSummerInfo("Hunter",playerInfo);
        }

        public void SavePlayerInfo(PlayerInfo playerInfo) {
            string json = JsonConvert.SerializeObject(playerInfo);

            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            System.IO.File.WriteAllText(filePath, json);
        }

        public PlayerInfo LoadPlayerInfo(UnitObject unitObject) {
            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            // JSON 파일 로드
            string jsonString = File.ReadAllText(filePath);

            PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

            for(int i = 0; i < unitObject.unitList.Count; i++) {
                if (unitObject.unitList[i].unitType == 0) {
                    playerInfo.health = unitObject.unitList[i].health + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_health);
                    playerInfo.attack = unitObject.unitList[i].attack + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attack);
                    playerInfo.attackRange = unitObject.unitList[i].attackRange + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackRange);
                    playerInfo.move = unitObject.unitList[i].move + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_move);
                    playerInfo.attackSpeed = unitObject.unitList[i].attackSpeed + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackSpeed);
                    playerInfo.spritesName = unitObject.unitList[i].spritesName;
                }
            }

            return playerInfo;
        }

        public List<PlayerInfo> SummerListLoad(UnitObject unitObject) {
            List<PlayerInfo> existTargetInfo = new List<PlayerInfo>();
            for(int i = 0; i < summonList.Length; i++) {
                string fileName = summonList[i] + ".json";
                string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;


                string jsonString = File.ReadAllText(filePath);
                PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

                for(int j = 0; j < unitObject.unitList.Count; j++) {
                    if (unitObject.unitList[j].unitType == 2 && summonList[i].Equals(unitObject.unitList[j].spritesName)) {
                        playerInfo.health = unitObject.unitList[j].health + ((playerInfo.playerLevel-1) * unitObject.unitList[j].lev_health);
                        playerInfo.attack = unitObject.unitList[j].attack + ((playerInfo.playerLevel-1) * unitObject.unitList[j].lev_attack);
                        playerInfo.attackRange = unitObject.unitList[j].attackRange + ((playerInfo.playerLevel-1) * unitObject.unitList[j].lev_attackRange);
                        playerInfo.move = unitObject.unitList[j].move + ((playerInfo.playerLevel-1) * unitObject.unitList[j].lev_move);
                        playerInfo.attackSpeed = unitObject.unitList[j].attackSpeed + ((playerInfo.playerLevel-1) * unitObject.unitList[j].lev_attackSpeed);
                        playerInfo.spritesName = unitObject.unitList[j].spritesName;
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
            string fileName; 
            PlayerInfo playerInfo = new PlayerInfo();
            if(intPlayerInfo > summonList.Length -1) {
                fileName = "none";
            } else {
                fileName = summonList[intPlayerInfo] +".json";
            }
            Debug.Log("fileName : " + fileName);
            if (!"none".Equals(fileName)) {
                string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

                // JSON 파일 로드
                string jsonString = File.ReadAllText(filePath);
                playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

                for(int i = 0; i < unitObject.unitList.Count; i++) {
                     Debug.Log("unitType : " + unitObject.unitList[i].unitType);
                     Debug.Log("unitType : " + unitObject.unitList[i].spritesName);
                    if (unitObject.unitList[i].unitType == 2 && summonList[intPlayerInfo].Equals(unitObject.unitList[i].spritesName)) {
                        playerInfo.health = unitObject.unitList[i].health + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_health);
                        playerInfo.attack = unitObject.unitList[i].attack + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attack);
                        playerInfo.attackRange = unitObject.unitList[i].attackRange + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackRange);
                         Debug.Log("playerInfoDetail : " + playerInfo.attackRange);
                        playerInfo.move = unitObject.unitList[i].move + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_move);
                        playerInfo.attackSpeed = unitObject.unitList[i].attackSpeed + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackSpeed);
                        playerInfo.spritesName = unitObject.unitList[i].spritesName;
                    }
                }
            }
            Debug.Log("playerInfo : " + playerInfo.attackRange);
            return playerInfo;
        }

        public PlayerInfo LoadMonsterInfo( UnitObject unitObject, Dictionary<string, object> stateMonster) {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerLevel = (int)stateMonster["Level"];
            
            for(int i = 0; i < unitObject.unitList.Count; i++) {
                if (unitObject.unitList[i].unitType == 1 && unitObject.unitList[i].id == (int)stateMonster["MonsterId"]) {
                    playerInfo.health = unitObject.unitList[i].health + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_health);
                    playerInfo.attack = unitObject.unitList[i].attack + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attack);
                    playerInfo.attackRange = unitObject.unitList[i].attackRange + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackRange);
                    playerInfo.move = unitObject.unitList[i].move + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_move);
                    playerInfo.attackSpeed = unitObject.unitList[i].attackSpeed + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackSpeed);
                    playerInfo.reward = unitObject.unitList[i].reward;
                    playerInfo.spritesName = unitObject.unitList[i].spritesName;
                }
            }
            return playerInfo;
        }


        //         public PlayerInfo LoadMonsterInfo( UnitObject unitObject, StateMonster stateMonster) {
        //     PlayerInfo playerInfo = new PlayerInfo();
        //     playerInfo.playerLevel = stateMonster.level;
            
        //     for(int i = 0; i < unitObject.unitList.Count; i++) {
        //         if (unitObject.unitList[i].unitType == 1 && unitObject.unitList[i].id == stateMonster.monsterId) {
        //             playerInfo.health = unitObject.unitList[i].health + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_health);
        //             playerInfo.attack = unitObject.unitList[i].attack + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attack);
        //             playerInfo.attackRange = unitObject.unitList[i].attackRange + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackRange);
        //             playerInfo.move = unitObject.unitList[i].move + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_move);
        //             playerInfo.attackSpeed = unitObject.unitList[i].attackSpeed + ((playerInfo.playerLevel-1) * unitObject.unitList[i].lev_attackSpeed);
        //             playerInfo.reward = unitObject.unitList[i].reward;
        //             playerInfo.spritesName = unitObject.unitList[i].spritesName;
        //         }
        //     }
        //     return playerInfo;
        // }


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