////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  Draggable : class | Written by Parker Staszkiewicz                                                            //
//  Uses Interface methods to handle dragging a UI element across the screen.                                     //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static GameObject objBeingDragged;

    AudioManagerScript audioManager;

    public static event Action<Draggable> ActionStartDrag = delegate { }; //empty delegate to avoid a null reference
    public static event Action<Draggable> ActionEndDrag = delegate { };
    public static event Action<Draggable> ActionStartClick = delegate { };

    public bool onlyClick = false;
    private bool canBeClicked = true;

    private CanvasGroup canvasGroup;
    private Transform draggingTransform;
    private Transform handPanel;

    private float scaleFactor = 1.5f;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManagerScript>();

        draggingTransform = GameObject.Find("DraggingTransform").transform;
        handPanel = GameObject.Find("Hand").transform;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onlyClick || !canBeClicked)
        {
            return;
        }

        objBeingDragged = gameObject;

        transform.SetParent(draggingTransform);

        canvasGroup.blocksRaycasts = false;

        transform.localScale *= scaleFactor;

        ActionStartDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onlyClick || !canBeClicked)
        {
            return;
        }

        transform.position = Input.mousePosition;

        audioManager.Play("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onlyClick || !canBeClicked)
        {
            return;
        }

        objBeingDragged = null;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == draggingTransform)
        {
            transform.SetParent(handPanel);
        }

        transform.localScale /= scaleFactor;

        ActionEndDrag(this);

        audioManager.Play("Drop");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ActionStartClick(this);
    }

    public void CannotClick()
    {
        canBeClicked = false;
    }
}
