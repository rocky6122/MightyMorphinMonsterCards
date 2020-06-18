////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  MorphingSlot : class | Written by Parker Staszkiewicz                                                         //
//  Uses IDropHandler interface to detect when a Draggable object has be released on this object.                 //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MorphingSlot : MonoBehaviour, IDropHandler
{
    public static event Action ActionSlotChange = delegate { };

    public GameObject obj = null;

    private void OnEnable()
    {
        Draggable.ActionStartDrag += RemoveObj; // Subscribe to Draggable event
    }

    private void OnDisable()
    {
        Draggable.ActionStartDrag -= RemoveObj; // Unsubscribe to Draggable event
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (obj == null)
        {
            obj = Draggable.objBeingDragged;
            obj.transform.SetParent(transform);
            obj.transform.position = transform.position;

            ActionSlotChange();
        }
    }

    public void DestroyCard()
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void RemoveObj(Draggable draggedObject)
    {
        if (obj != null && obj == draggedObject.gameObject)
        {
            obj = null;

            ActionSlotChange();
        }
    }
}

