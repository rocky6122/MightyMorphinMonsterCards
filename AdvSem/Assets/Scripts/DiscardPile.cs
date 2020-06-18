using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardPile : MonoBehaviour, IDropHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        DiscardACard(Draggable.objBeingDragged);
        Draggable.objBeingDragged = null;
    }

    public void DiscardACard(GameObject card)
    {
        card.transform.SetParent(transform);

        card.GetComponent<RectTransform>().localPosition = Vector2.zero;

        card.GetComponent<Animator>().SetBool("Burn Card", true);
    }
}
