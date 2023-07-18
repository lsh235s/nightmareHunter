using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nightmareHunter {
    public class EnemySkill : MonoBehaviour
    {
    
        public void skillUse(string skillName) {
            switch (skillName) {
                //텔러 울부짖기
                //모든 원더러가 의뢰자를 공격하며 공격력과 이동속도가 증가한다, 탤러가 죽을때 까지 지속
                case "TellerCry": 
                    GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");

                    // 검색된 GameObject에 대한 작업 수행
                    foreach (GameObject gameObject in gameObjectsWithTag)
                    {
                        if(gameObject.GetComponent<Enemy>()._monsterId == 0) {
                            gameObject.GetComponent<Enemy>().skillList["AttackUp"] = true;
                            gameObject.GetComponent<Enemy>().skillList["MoveSpeedUp"] = true;
                            gameObject.GetComponent<Enemy>().skillList["ClientTargetFix"] = true;
                            gameObject.GetComponent<Enemy>().targetNum = -1;
                        } 
                    }
                    break;
                case "Cloaking":
                    break;
            }
        }

        public void skillEnd(string skillName) {
            switch (skillName) {
                //텔러 울부짖기 종료
                //탤러가 죽을때 종료 시킨다.
                case "TellerCry": 
                    GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");

                    // 검색된 GameObject에 대한 작업 수행
                    foreach (GameObject gameObject in gameObjectsWithTag)
                    {
                        if(gameObject.GetComponent<Enemy>()._monsterId == 0) {
                            gameObject.GetComponent<Enemy>().skillList["AttackUp"] = false;
                            gameObject.GetComponent<Enemy>().skillList["MoveSpeedUp"] = false;
                            gameObject.GetComponent<Enemy>().skillList["ClientTargetFix"] = false;
                            gameObject.GetComponent<Enemy>().stateMod("Idle");
                        } 
                    }
                    break;
                case "Cloaking":
                    break;
            }
        }
    }
}
