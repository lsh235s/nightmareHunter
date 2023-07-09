using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    /// <summary>
    /// 유저 게임 정보
    /// </summary>
    [System.Serializable]
    public class WeaponInfo
    {
        public int id { get; set; }
        public string WeaponName { get; set; }
        public int Level { get; set; }
        public float Attack { get; set; }
        public float LevAttack { get; set; }
        public float AttackRange { get; set; }
        public float LevAttackRange { get; set; }
        public float Move { get; set; }
        public float LevMove { get; set; }
        public float AttackSpeed { get; set; }
        public float LevAttackSpeed { get; set; }
        public float Amount { get; set; }
        public float LevAmount { get; set; }
        public float ExistTime { get; set; }
   
    }
}