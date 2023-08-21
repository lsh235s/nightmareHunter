using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace nightmareHunter {
    public class EnemySkill : MonoBehaviour
    {
        [SerializeField]
        private GameObject _skeletonObject;
        private SkeletonMecanim skeletonMecanim;
    
        public void skillUse(string skillName) {
            switch (skillName) {
                //텔러 울부짖기
                //모든 원더러가 의뢰자를 공격하며 공격력과 이동속도가 증가한다, 탤러가 죽을때 까지 지속
                case "TellerCry": 
                    GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");

                    // 검색된 GameObject에 대한 작업 수행
                    foreach (GameObject targetGameObject in gameObjectsWithTag)
                    {
                        if(targetGameObject.GetComponent<Enemy>()._monsterId == 0) {
                            targetGameObject.GetComponent<Enemy>().skillList["AttackUp"] = true;
                            targetGameObject.GetComponent<Enemy>().skillList["MoveSpeedUp"] = true;
                            targetGameObject.GetComponent<Enemy>().skillList["ClientTargetFix"] = true;
                            targetGameObject.GetComponent<Enemy>().targetNum = -1;
                        } 
                    }
                    break;
                case "PlayerTarget":
                    gameObject.GetComponent<Enemy>().skillList["PlayerTargetFix"] = true;
                    break;
                case "PhysicsResistance":
                    gameObject.GetComponent<Enemy>().skillList["physicsResistance"] = true;
                    break;
                case "MagicResistance":
                    gameObject.GetComponent<Enemy>().skillList["MagicResistance"] = true;
                    break;
                case "Split":
                    gameObject.GetComponent<Enemy>().skillList["Split"] = true;
                    break;
                case "Cloaking":
                    skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
                    Color endColor = new Color32(0, 0, 0, 50);
                    skeletonMecanim.skeleton.SetColor(endColor);
                    gameObject.GetComponent<Enemy>().skillList["Cloaking"] = true;
                    break;
                case "StillerSlow":
                    gameObject.GetComponent<Enemy>().skillList["SlowSkill"] = true;
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
                    foreach (GameObject targetGameObject in gameObjectsWithTag)
                    {
                        if(targetGameObject.GetComponent<Enemy>()._monsterId == 0) {
                            targetGameObject.GetComponent<Enemy>().skillList["AttackUp"] = false;
                            targetGameObject.GetComponent<Enemy>().skillList["MoveSpeedUp"] = false;
                            targetGameObject.GetComponent<Enemy>().skillList["ClientTargetFix"] = false;
                            targetGameObject.GetComponent<Enemy>().stateMod("Idle");
                        } 
                    }
                    break;
                case "PlayerTarget":
                    gameObject.GetComponent<Enemy>().skillList["PlayerTargetFix"] = false;
                    break;
                case "Split":
                    gameObject.GetComponent<Enemy>().skillList["Split"] = false;
                    break;
                case "Cloaking":
                    skeletonMecanim = _skeletonObject.GetComponent<SkeletonMecanim>();
                    Color endColor = new Color32(255, 255, 255, 255);
                    skeletonMecanim.skeleton.SetColor(endColor);
                    gameObject.GetComponent<Enemy>().skillList["Cloaking"] = false;
                    break;
                case "PhysicsResistance":
                    gameObject.GetComponent<Enemy>().skillList["PhysicsResistance"] = false;
                    break;
                case "MagicResistance":
                    gameObject.GetComponent<Enemy>().skillList["MagicResistance"] = false;
                    break;
                case "StillerSlow":
                    gameObject.GetComponent<Enemy>().skillList["SlowSkill"] = false;
                    break;
            }
        }
    }
}
