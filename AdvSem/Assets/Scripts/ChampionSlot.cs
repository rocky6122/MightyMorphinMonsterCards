using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ChampionSlot : MonoBehaviour, IDropHandler
{
    public GameObject obj = null;

    private void OnEnable()
    {
        Draggable.ActionStartDrag += RemoveObj;
    }

    private void OnDisable()
    {
        Draggable.ActionStartDrag -= RemoveObj;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedCard = Draggable.objBeingDragged;

        CardInfo info = draggedCard.GetComponent<CardDisplay>().info;

        if (obj == null && info is MonsterCardInfo)
        {
            obj = draggedCard;
            obj.transform.SetParent(transform);
            obj.transform.position = transform.position;
        }
    }

    private void RemoveObj(Draggable draggedObject)
    {
        if (obj != null && obj == draggedObject.gameObject)
        {
            obj = null;
        }
    }
    
    public CardInfoHolder GetCardInfo()
    {
        if (obj != null)
        {
            CardDisplay display = obj.GetComponent<CardDisplay>();

            if (display != null)
            {
                return display.GetCardInfo();
            }

        }

        return new CardInfoHolder();
    }
}
