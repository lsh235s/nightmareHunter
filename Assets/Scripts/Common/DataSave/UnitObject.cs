using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitObject", menuName = "ScriptableObjects/UnitObjects", order = 1)]
public class UnitObject : ScriptableObject
{
    public List<UnitList> unitList = new List<UnitList>(); //Output

}


[System.Serializable]
public class UnitList 
{
    [Header("unit State")]
    public int id;
    public float unitType;
    public string unitName;
    public int level;
    public float health;
    public float lev_health;
    public float attack;
    public float lev_attack;
    public float attackRange;
    public float lev_attackRange;
    public float move;
    public float lev_move;
    public float attackSpeed;
    public float lev_attackSpeed;
    

    

    [Header("Monster shape")]
    public string spritesName;


    public UnitList()
    {

    }

    public UnitList(int Id,float UnitType,string UnitName,int Level,float Health,float Lev_health,float Attack,float Lev_attack,float AttackRange,float Lev_attackRange,float Move,float Lev_move,float AttackSpeed,float Lev_attackSpeed,string SpritesName)
    {
        id = Id;
        unitType = UnitType;
        unitName = UnitName;
        level = Level;
        health = Health;
        lev_health = Lev_health;
        attack = Attack;
        lev_attack = Lev_attack;
        attackRange = AttackRange;
        lev_attackRange = Lev_attackRange;
        move = Move;
        lev_move = Lev_move;
        attackSpeed = AttackSpeed;
        lev_attackSpeed = Lev_attackSpeed;
        spritesName = SpritesName; 
       
    }
}