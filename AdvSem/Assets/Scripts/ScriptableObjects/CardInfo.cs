////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  CardInfo : Scriptable Object | Written by Parker Staszkiewicz                                                 //
//  Base scriptable object (can not actually be created) for all cards in the game.                               //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public enum Element
{
    Normal,
    Water,
    Fire,
    Earth,
    Wind,
    Lightning
}

public enum Resistance
{
    Normal,
    Bronze,
    Steel,
    Brass,
    Pewter,
    Electrum
}

public class CardInfo : ScriptableObject
{
    public new string name;
    public Color32 backgroundColor;
    public string description;
    public Sprite artwork;
    public string artist;
}
