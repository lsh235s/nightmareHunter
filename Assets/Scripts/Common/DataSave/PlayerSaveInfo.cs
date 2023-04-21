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
        public int playerLevel { get; set; }
        public float health { get; set; }
        public float attack { get; set; }
        public float attackRange { get; set; }
        public float move { get; set; }
        public float attackSpeed { get; set; }
        public string positionInfo { get; set; }
        public string spritesName { get; set; }
        public bool summonsExit { get; set; }
   
    }
}