using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateMonsterBatch", menuName = "ScriptableObjects/StateMonsterBatch", order = 1)]
public class StateMonsterBatch : ScriptableObject
{
    public List<StateMonster> stateMonsterList = new List<StateMonster>(); //Output

}


[System.Serializable]
public class StateMonster
{
    [Header("unit State")]
    public int id;
    public int level;
    public int monsterId;
    public int moveType;
    public string appearTimer;
   
    public StateMonster()
    {

    }

    public StateMonster(int Id,int Level,int MonsterId,int MoveType,string AppearTimer)
    {
        id = Id;
        level = Level;
        monsterId = MonsterId;
        moveType = MoveType;
        appearTimer = AppearTimer;

    }
}