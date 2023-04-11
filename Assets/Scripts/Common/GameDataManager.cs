using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace nightmareHunter {
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager instance;

        UnitObject unitObject = new UnitObject();
        StateMonsterBatch stateMonsterBatch = new StateMonsterBatch();

        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }
        
        public void PlayerInitTest() {
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.playerLevel = 1;
            playerInfo.health = 100;
            playerInfo.attack = 10;
            playerInfo.attackRange = 1;
            playerInfo.move = 5;
            playerInfo.attackSpeed = 1;
            playerInfo.spritesName = "hunter";

            SavePlayerInfo(playerInfo);
        }

        public void SavePlayerInfo(PlayerInfo playerInfo) {
            string json = JsonConvert.SerializeObject(playerInfo);
            Debug.Log(json);

            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;
            Debug.Log(filePath);
            System.IO.File.WriteAllText(filePath, json);
        }

        public PlayerInfo LoadPlayerInfo() {
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

        
        public UnitObject MonsterWave() {
            

            return stateMonsterBatch;
        }

    }
}