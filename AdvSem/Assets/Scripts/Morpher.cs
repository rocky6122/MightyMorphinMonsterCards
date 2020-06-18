////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  Morpher : class | Written by Parker Staszkiewicz                                                              //
//  Every time a Morph Slot updates, the morpher will update itself and generate a new card if there are          //
//  two cards in the slots which fit a card recipe.                                                               //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

public class Morpher : MonoBehaviour
{
    [SerializeField]
    private MorphingSlot[] slots;
    private MorphResultSlot resultSlot;

    public List<Recipe> recipes;

    private void Start()
    {
        slots = FindObjectsOfType<MorphingSlot>();
        resultSlot = FindObjectOfType<MorphResultSlot>();
    }

    private void OnEnable()
    {
        MorphingSlot.ActionSlotChange += CheckSlots; // Subscribe to Morphingslot event
        MorphResultSlot.ActionCardTaken += ClearMorphSlots;
    }

    private void OnDisable()
    {
        MorphingSlot.ActionSlotChange -= CheckSlots; // Unsubscribe to Morphingslot event
        MorphResultSlot.ActionCardTaken -= ClearMorphSlots;
    }

    private void CheckSlots()
    {
        // If both slots have a valid gameobject, then check them
        if (slots[0].obj && slots[1].obj)
        {
            CardInfo resultInfo = GenerateResult(out Element finalElement, out Resistance finalResistance);

            if (resultInfo != null)
            {
                CardDisplay card = resultSlot.SpawnCard(resultInfo);
                card.ChangeElement(finalElement);
                card.ChangeResistance(finalResistance);
            }
        }
        // If we've removed a card, we need to get rid of previous results
        else
        {
            resultSlot.DestroyCard();
        }
    }

    private void ClearMorphSlots()
    {
        foreach (MorphingSlot slot in slots)
        {
            slot.DestroyCard();
        }
    }

    private CardInfo GenerateResult(out Element e, out Resistance r)
    {
        // Create a Card Pair using the card info in the slots
        CardPair pair;
        CardInfo slotZeroCard = slots[0].obj.GetComponent<CardDisplay>().info;
        CardInfoHolder zeroInfo = slots[0].obj.GetComponent<CardDisplay>().GetCardInfo();
        CardInfo slotOneCard = slots[1].obj.GetComponent<CardDisplay>().info;
        CardInfoHolder oneInfo = slots[1].obj.GetComponent<CardDisplay>().GetCardInfo();

        // Check Morphing with Element
        if (slotZeroCard.GetType() == typeof(MonsterCardInfo) && slotOneCard.GetType() == typeof(ElementCardInfo))
        {
            MonsterCardInfo monster = (MonsterCardInfo)slotZeroCard;
            ElementCardInfo element = (ElementCardInfo)slotOneCard;

            e = element.type;
            r = zeroInfo.resistance;
            return monster;
        }
        else if (slotOneCard.GetType() == typeof(MonsterCardInfo) && slotZeroCard.GetType() == typeof(ElementCardInfo))
        {
            MonsterCardInfo monster = (MonsterCardInfo)slotOneCard;
            ElementCardInfo element = (ElementCardInfo)slotZeroCard;

            e = element.type;
            r = oneInfo.resistance;
            return monster;
        }
        // Check Morphing with Resistance
        else if (slotZeroCard.GetType() == typeof(MonsterCardInfo) && slotOneCard.GetType() == typeof(ResistanceCardInfo))
        {
            MonsterCardInfo monster = (MonsterCardInfo)slotZeroCard;
            ResistanceCardInfo resistance = (ResistanceCardInfo)slotOneCard;

            e = zeroInfo.element;
            r = resistance.resistance;
            return monster;
        }
        else if (slotOneCard.GetType() == typeof(MonsterCardInfo) && slotZeroCard.GetType() == typeof(ResistanceCardInfo))
        {
            MonsterCardInfo monster = (MonsterCardInfo)slotOneCard;
            ResistanceCardInfo resistance = (ResistanceCardInfo)slotZeroCard;

            e = oneInfo.element;
            r = resistance.resistance;
            return monster;
        }
        // Check Recipes
        else
        {
            if (String.Compare(slotZeroCard.name, slotOneCard.name) < 0) // Determining the alphabetical order of cards in slots
            {                                                            // because all Recipes have cardPairs in alphabetical order
                pair.cardZero = slotZeroCard;
                pair.cardOne = slotOneCard;
            }
            else
            {
                pair.cardZero = slotOneCard;
                pair.cardOne = slotZeroCard;
            }

            foreach (Recipe recipe in recipes)
            {
                foreach (CardPair cp in recipe.cardPairs)
                {
                    if (cp.cardZero == pair.cardZero && cp.cardOne == pair.cardOne)
                    {
                        MonsterCardInfo m = (MonsterCardInfo)recipe.result;

                        e = m.element;
                        r = m.resistance;

                        return recipe.result;
                    }
                }
            }
        }

        Debug.Log("No Result");

        e = Element.Normal;
        r = Resistance.Normal;
        return null;
    }
}
