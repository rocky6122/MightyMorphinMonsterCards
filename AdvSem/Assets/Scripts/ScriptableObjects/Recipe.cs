////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  Recipe : Scriptable Object | Written by Parker Staszkiewicz                                                   //
//  Contains a list of card pairs and a given result, to be used by the Morpher class                             //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardPair
{
    public CardInfo cardZero;
    public CardInfo cardOne;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public List<CardPair> cardPairs;

    public CardInfo result;
}
