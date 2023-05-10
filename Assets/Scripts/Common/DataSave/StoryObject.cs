using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StoryObject", menuName = "ScriptableObjects/StoryObject", order = 1)]
public class StoryObject : ScriptableObject
{
    public List<StoryContentList> storyContentList = new List<StoryContentList>(); //Output
}


[System.Serializable]
public class StoryContentList 
{
    [Header("Story Content")]
    public int stage_id;
    public int scenario_stage_id;
    public int event_stage_id;
    public string content;
    public string contentType;
    public string leftCharacter;
    public string rightCharacter;
    public string characterAnimation;
    

    public StoryContentList()
    {

    }

    public StoryContentList(int Stage_id, int Scenario_stage_id, int Event_stage_id, string Content,string ContentType, string LeftCharacter, string RightCharacter, string CharacterAnimation)
    {
        stage_id = Stage_id;
        scenario_stage_id = Scenario_stage_id;
        event_stage_id = Event_stage_id;
        content = Content;
        contentType = ContentType;
        leftCharacter = LeftCharacter;
        rightCharacter = RightCharacter;
        characterAnimation = CharacterAnimation;
    }
}