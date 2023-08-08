using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace nightmareHunter {
    public class GameDataManager : MonoBehaviour
    {

        public static GameDataManager Instance { get; private set; }

        string[] summonList = new string[] {"hunter", "priest", "exorcist", "monk", "shaman", "gostbuste"};
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
            string fileName = "PlayerInfo.json";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            // JSON 파일 로드
            string jsonString = File.ReadAllText(filePath);

            PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(jsonString);

            for(int i = 0; i < unitObjectList.Count; i++) {
                if (unitObjectList[i]["UnitType"].ToString().Equals("0")) {
                    playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                    playerInfo.physicsAttack =  WeaponLoadInfo[0].PhysicsAttack + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevPhysicsAttack);
                    playerInfo.magicAttack =  WeaponLoadInfo[0].MagicAttack + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevMagicAttack);
                    playerInfo.attackRange =  (WeaponLoadInfo[0].AttackRange + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevAttackRange)) * 0.1f;
                    playerInfo.move =  (WeaponLoadInfo[0].Move + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevMove)) * 0.1f;
                    playerInfo.attackSpeed =  WeaponLoadInfo[0].AttackSpeed + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevAttackSpeed);
                    playerInfo.attackDelayTime =  WeaponLoadInfo[0].AttackDelayTime + ((playerInfo.playerLevel-1) * WeaponLoadInfo[0].LevAttackDelayTime);
                    playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                }
            }
            return playerInfo;
        }

        public PlayerInfo PlayWeaponSet(int weaponID, PlayerInfo playerInfo) {
            playerInfo.weaponID = weaponID;
            playerInfo.physicsAttack = WeaponLoadInfo[weaponID].PhysicsAttack + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevPhysicsAttack);
            playerInfo.magicAttack = WeaponLoadInfo[weaponID].MagicAttack + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevMagicAttack);
            playerInfo.attackRange = (WeaponLoadInfo[weaponID].AttackRange + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackRange)) * 0.1f;
            playerInfo.move = (WeaponLoadInfo[weaponID].Move + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevMove)) * 0.1f;
            playerInfo.attackSpeed = WeaponLoadInfo[weaponID].AttackSpeed + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackSpeed);
            playerInfo.attackDelayTime = WeaponLoadInfo[weaponID].AttackDelayTime + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAttackDelayTime);
            playerInfo.weaponAttackType = WeaponLoadInfo[weaponID].WeaponAttackType;
            playerInfo.weaponAmount = WeaponLoadInfo[weaponID].Amount + ((playerInfo.playerLevel-1) * WeaponLoadInfo[weaponID].LevAmount);
            
            return playerInfo;
        }

        public List<PlayerInfo> SummerListLoad() {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");
            List<Dictionary<string, object>> summerBatchList = CSVReader.Read("SummonBatch");
            List<PlayerInfo> existTargetInfo = new List<PlayerInfo>();
            Debug.Log("playerInfo : SummerListLoad" );

            for(int i=0; i < summerBatchList.Count; i++) {
                PlayerInfo playerInfo = new PlayerInfo();
                for(int j = 0; j < unitObjectList.Count; j++) {

                   Debug.Log("playerInfo : " +unitObjectList[j]["UnitType"].ToString()+"/"+summerBatchList[i]["SummonId"].ToString() +"/"+unitObjectList[j]["Id"].ToString());
                    if ("2".Equals(unitObjectList[j]["UnitType"].ToString()) && summerBatchList[i]["SummonId"].ToString().Equals(unitObjectList[j]["Id"].ToString())) {
                        playerInfo.id =  int.Parse(unitObjectList[j]["Id"].ToString());
                        playerInfo.health = float.Parse(unitObjectList[j]["Health"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevHealth"].ToString()));
                        playerInfo.physicsAttack =  float.Parse(unitObjectList[j]["PhysicsAttack"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevPhysicsAttack"].ToString()));
                        playerInfo.magicAttack =  float.Parse(unitObjectList[j]["MagicAttack"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevMagicAttack"].ToString()));
                        playerInfo.pysicsDefense =  float.Parse(unitObjectList[j]["PhysicsDefense"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevPhysicsDefense"].ToString()));
                        playerInfo.magicDefense =  float.Parse(unitObjectList[j]["MagicDefense"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevMagicDefense"].ToString()));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[j]["AttackRange"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevAttackRange"].ToString()))) ;
                        playerInfo.move =  (float.Parse(unitObjectList[j]["Move"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevMove"].ToString())) * 0.1f);
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[j]["AttackSpeed"].ToString()) + ((int.Parse(summerBatchList[i]["Level"].ToString())-1) * float.Parse(unitObjectList[j]["LevAttackSpeed"].ToString()))) * 0.1f;
                        playerInfo.spritesName = unitObjectList[j]["SpritesName"].ToString();
                        playerInfo.positionInfoX = summerBatchList[i]["PositionInfoX"].ToString();
                        playerInfo.positionInfoY = summerBatchList[i]["PositionInfoY"].ToString();
                        playerInfo.positionInfoZ = summerBatchList[i]["PositionInfoZ"].ToString();
                        Debug.Log("playerInfo : " +unitObjectList[j]["SpritesName"].ToString()+"/"+ playerInfo.attackRange+"/"+playerInfo.positionInfoX+"/"+playerInfo.positionInfoY+"/"+playerInfo.positionInfoZ);
                    }
                }
                existTargetInfo.Add(playerInfo);
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
                    if(2 == int.Parse(unitObjectList[i]["UnitType"].ToString()) && intPlayerInfo == int.Parse(unitObjectList[i]["Id"].ToString()) ) {
                        playerInfo.health = float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                        playerInfo.physicsAttack =  float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsAttack"].ToString()));
                        playerInfo.magicAttack =  float.Parse(unitObjectList[i]["MagicAttack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicAttack"].ToString()));
                        playerInfo.pysicsDefense =  float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsDefense"].ToString()));
                        playerInfo.magicDefense =  float.Parse(unitObjectList[i]["MagicDefense"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicDefense"].ToString()));
                        playerInfo.attackRange =  (float.Parse(unitObjectList[i]["AttackRange"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackRange"].ToString()))) * 0.1f;
                        playerInfo.move =  (float.Parse(unitObjectList[i]["Move"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMove"].ToString())) * 0.1f);
                        playerInfo.attackSpeed =  (float.Parse(unitObjectList[i]["AttackSpeed"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevAttackSpeed"].ToString()))) * 0.1f;
                        playerInfo.spritesName = unitObjectList[i]["SpritesName"].ToString();
                    }
                }

            }
            return playerInfo;
        }

        public PlayerInfo LoadMonsterInfo( Dictionary<string, object> stateMonster) {
            List<Dictionary<string, object>> unitObjectList = CSVReader.Read("UnitObject");

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerLevel = (int)stateMonster["Level"];

            for(int i = 0; i < unitObjectList.Count; i++) {
                if ("1".Equals(unitObjectList[i]["UnitType"].ToString()) && int.Parse(unitObjectList[i]["Id"].ToString()) == (int)stateMonster["MonsterId"]) {
                    playerInfo.health =  float.Parse(unitObjectList[i]["Health"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevHealth"].ToString()));
                    playerInfo.physicsAttack =  float.Parse(unitObjectList[i]["PhysicsAttack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsAttack"].ToString()));
                    playerInfo.magicAttack =  float.Parse(unitObjectList[i]["MagicAttack"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicAttack"].ToString()));
                    playerInfo.pysicsDefense =  float.Parse(unitObjectList[i]["PhysicsDefense"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevPhysicsDefense"].ToString()));
                    playerInfo.magicDefense =  float.Parse(unitObjectList[i]["MagicDefense"].ToString()) + ((playerInfo.playerLevel-1) * float.Parse(unitObjectList[i]["LevMagicDefense"].ToString()));
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