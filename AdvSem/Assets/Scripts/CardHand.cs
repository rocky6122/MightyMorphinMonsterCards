using UnityEngine;
using System.Collections;

public class CardHand : MonoBehaviour
{
    private readonly int maxHandLimit = 5;
    private int currentHandCount;

    public GameObject cardDisplayPrefab;

    public DiscardPile discard;

    private void Start()
    {
        currentHandCount = 0;
    }

    private void OnEnable()
    {
        Draggable.ActionEndDrag += UpdateHand;
    }

    private void OnDisable()
    {
        Draggable.ActionEndDrag -= UpdateHand;
    }

    private void UpdateHand(Draggable draggable)
    {
        currentHandCount = transform.childCount;

        while (currentHandCount > maxHandLimit)
        {
            GameObject card = transform.GetChild(transform.childCount - 1).gameObject;

            // BURN CARD
            --currentHandCount;
            discard.DiscardACard(card);
        }
    }

    public void AddCardFromDeck(CardInfo info)
    {
        CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponent<CardDisplay>();

        card.SetUpCard(info);

        if (CanTakeNewCard())
        {
            UpdateHand(null);
        }
        else
        {
            discard.DiscardACard(card.gameObject);
        }
    }

    public void AddCardToHand(CardDisplay card)
    {
        card.transform.SetParent(transform);

        UpdateHand(null);
    }

    public CardDisplay GetCardFromHand()
    {
        CardDisplay cardToReturn;

        if (transform.childCount == 0)
        {
            return null;
        }

        cardToReturn = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<CardDisplay>();

        return cardToReturn;
    }

    public bool CanTakeNewCard()
    {
        return currentHandCount < maxHandLimit;
    }
}
