////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  CardDisplay : class | Written by Parker Staszkiewicz                                                          //
//  
//  
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public struct CardInfoHolder
{
    public string name;
    public Color32 backgroundColor;
    public string description;
    public Sprite artwork;
    public string artist;

    public int attack;
    public int defense;
    public int health;
    public int speed;

    public Element element;
    public Resistance resistance;
}

public class CardDisplay : MonoBehaviour
{
    private CardInfoHolder cardInfo;
    public CardInfo info;

    public GameObject monsterCardBlock;

    // General
    [Header("General")]
    public TextMeshProUGUI cardName;
    public Image cardImage;

    // Monster
    [Header("Monster Card Block")]
    public TextMeshProUGUI attackStat;
    public TextMeshProUGUI defenseStat;
    public TextMeshProUGUI healthStat;
    public TextMeshProUGUI speedStat;
    public TextMeshProUGUI elementStat;
    public TextMeshProUGUI resistanceStat;

    // Element

    // Alloy

    public void SetUpCard(CardInfo info)
    {
        this.info = info;
        cardInfo.name = info.name;
        cardInfo.backgroundColor = info.backgroundColor;
        cardInfo.description = info.description;
        cardInfo.artwork = info.artwork;
        cardInfo.artist = info.artist;

        cardName.text = cardInfo.name;
        cardImage.sprite = cardInfo.artwork;

        monsterCardBlock.SetActive(false);

        if (info is MonsterCardInfo correctInfo)
        {
            cardInfo.attack = correctInfo.attack;
            cardInfo.defense = correctInfo.defense;
            cardInfo.health = correctInfo.health;
            cardInfo.speed = correctInfo.speed;

            cardInfo.element = correctInfo.element;
            cardInfo.resistance = correctInfo.resistance;

            attackStat.text = cardInfo.attack.ToString();
            defenseStat.text = cardInfo.defense.ToString();
            healthStat.text = cardInfo.health.ToString();
            speedStat.text = cardInfo.speed.ToString();
            elementStat.text = cardInfo.element.ToString();
            resistanceStat.text = cardInfo.resistance.ToString();

            monsterCardBlock.SetActive(true);
        }
    }

    public void ChangeElement(Element e)
    {
        cardInfo.element = e;

        // Update Card Visual
        elementStat.text = cardInfo.element.ToString();
    }

    public void ChangeResistance(Resistance r)
    {
        cardInfo.resistance = r;

        // Update Card Visual
        resistanceStat.text = cardInfo.resistance.ToString();
    }

    public void SetHealth(int value)
    {
        cardInfo.health = value;

        healthStat.text = cardInfo.health.ToString();
    }

    //Returns the current card to get the stats from it (probably a better solution out there)
    public CardInfoHolder GetCardInfo()
    {
        return cardInfo;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
