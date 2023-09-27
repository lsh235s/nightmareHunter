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
        public float PhysicsAttack { get; set; }
        public float MagicAttack { get; set; }
        public float LevPhysicsAttack { get; set; }
        public float LevMagicAttack { get; set; }
        public float AttackRange { get; set; }
        public float LevAttackRange { get; set; }
        public float Move { get; set; }
        public float LevMove { get; set; }
        public float AttackSpeed { get; set; }
        public float LevAttackSpeed { get; set; }
        public float AttackDelayTime { get; set; }
        public float LevAttackDelayTime { get; set; }
        public int Amount { get; set; }
        public int LevAmount { get; set; }
        public float ExistTime { get; set; }
        public string WeaponAttackType { get; set; }

   
    }
}