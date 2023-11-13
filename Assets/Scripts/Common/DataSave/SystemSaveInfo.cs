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
        // 골드
        public int money { get; set; }
        // 정수
        public int integer { get; set; }
        // 타겟 Hp
        public int targetHP { get; set; }
        // 스테이지 진행 단계
        public int stageId { get; set; }
        // 스토리 진행 번호
        public int storyNum { get; set; }
        // 각 스테이지 마다 밤 모드 진행 단계
        public int day { get; set; }
    }
}
