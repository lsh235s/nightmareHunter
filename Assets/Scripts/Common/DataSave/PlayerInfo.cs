using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    /// <summary>
    /// 유저 게임 정보
    /// </summary>
    [System.Serializable]
    public class PlayerInfo
    {
        public int keyId { get; set; } 
        public int id { get; set; }
        public int playerLevel { get; set; }
        public float health { get; set; }
        public float physicsAttack { get; set; }
        public float magicAttack { get; set; }
        public float energyAttack { get; set; }
        public float pysicsDefense { get; set; }
        public float magicDefense { get; set; }
        public float attackRange { get; set; }
        public float move { get; set; }
        public float attackSpeed { get; set; }
        public string positionInfoX { get; set; }
        public string positionInfoY { get; set; }
        public string positionInfoZ { get; set; }
        public string spritesName { get; set; }
        public int reward { get; set; }
        public bool summonsExist { get; set; }
        public int weaponID { get; set; }
        public int weaponAmount { get; set; }
        public float attackDelayTime { get; set; }
        public string weaponAttackType { get; set; }
        public string attackType { get; set; }
   
    }
}