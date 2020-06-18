using UnityEngine;
using System;

public class MorphResultSlot : MonoBehaviour
{
    public static event Action ActionCardTaken = delegate { };

    private GameObject obj = null;
    public GameObject cardDisplayPrefab;

    private void OnEnable()
    {
        Draggable.ActionStartDrag += TakeCard;
    }

    private void OnDisable()
    {
        Draggable.ActionStartDrag -= TakeCard;
    }

    public CardDisplay SpawnCard(CardInfo cardInfo)
    {
        CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponent<CardDisplay>();

        card.SetUpCard(cardInfo);

        obj = card.gameObject;

        return card;
    }

    public void DestroyCard()
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void TakeCard(Draggable draggedCard)
    {
        if (obj != null && obj == draggedCard.gameObject)
        {
            ActionCardTaken();
            obj = null;
        }
    }
}
