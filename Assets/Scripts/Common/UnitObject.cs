using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitObject", menuName = "ScriptableObjects/UnitObjects", order = 1)]
public class UnitObject : ScriptableObject
{
    public List<UnitList> unitList = new List<UnitList>(); //Output

    public int numberOfCardsInDatabase; // total amount of cards in Awake not including 0***
}


[System.Serializable]
public class UnitList 
{
    [Header("Card Details")]
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;
    public int rarity;
    public string seriesName;

    [Header("Card Appearance")]
    public Sprite thisImage;
    public Sprite backgroundImage;
    public Sprite rarityImage;

    public Color32 color;


    public UnitList()
    {

    }

    public UnitList(int Id, string CardName, int Cost,int Power, string CardDescription, Sprite ThisImage, Sprite BackgroundImage, Color32 Color, int Rarity, Sprite RarityImage, string SeriesName)
    {
        id = Id;
        cardName = CardName;
        backgroundImage = BackgroundImage;
        thisImage = ThisImage;
        rarityImage = RarityImage;
        cost = Cost;
        power = Power;
        cardDescription = CardDescription;
        rarity = Rarity;
        seriesName = SeriesName;
        color = Color;
       
    }
}