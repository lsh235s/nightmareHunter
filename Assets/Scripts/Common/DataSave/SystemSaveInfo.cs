using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nightmareHunter {
    /// <summary>
    /// 유저 게임 정보
    /// </summary>
    [System.Serializable]

    public class SystemSaveInfo 
    {
        public int money { get; set; }
        public int storyNum { get; set; }
    }
}
