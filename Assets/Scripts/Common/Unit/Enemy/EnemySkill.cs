using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    Enemy enemy;

    public Dictionary<string, bool> skillList = new Dictionary<string, bool>();

    void Start()
    {
        enemy = GetComponent<Enemy>();
        skillList.Add("AttackUp", false);
        skillList.Add("AttackSpeedUp", false);
        skillList.Add("MoveSpeedUp", false);
        skillList.Add("TargetFix", false);
        skillList.Add("TellerCry", false);
    }

    public void skillUseOn(string skillName) {
        skillList[skillName] = true;
    }

    public void skillUseOff(string skillName) {
        skillList[skillName] = false;
    }

    public void skillUse(string skillName) {
        if (skillList[skillName]) {
            switch (skillName) {
                case "AttackUp":
                    enemy.attack += 10;
                    break;
                case "AttackSpeedUp":
                    enemy.attackSpeed += 0.5f;
                    break;
                case "MoveSpeedUp":
                    enemy.moveSpeed += 0.5f;
                    break;
                case "TargetFix":
                    enemy.targetFix = true;
                    break;
                case "TellerCry":
                    enemy.tellerCry = true;
                    break;
            }
        }
    }
}
