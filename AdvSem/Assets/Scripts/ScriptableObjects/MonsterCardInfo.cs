////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  MonsterCardInfo : Scriptable Object | Written by Parker Staszkiewicz                                          //
//  Derives from CardInfo class; holds data types specific to Monster cards.                                      //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Monster")]
public class MonsterCardInfo : CardInfo
{
    public int attack;
    public int defense;
    public int health;
    public int speed;

    public Element element;
    public Resistance resistance;
}
