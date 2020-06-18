////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  NotficationSystem : class | Written by John Imgrund                                                           //
//  Show the user the number of incoming Notifications from the chat system                                       //
//                                                                                                                //
//  Inpsired by Project Buckstein ChatSystem                                                                      //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using ChampNet;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ChatSystem : MonoBehaviour
{
    public RectTransform contentWindow;
    private int rectHeight;

    public TMP_InputField inputField;

    public GameObject chatBoxPrefab;

    private const int X_OFFSET = 6;
    private const int NEW_MESSAGE_SPACE = 12;
    private const int MAX_MESSAGES_PER_BOX = 7;
    private int yOffset;
    private int verticalSeparation;
    private int endPos;

    private int numMessages;

    private List<ChatMessage> messages;

    public void InitializeChatBox()
    {
        messages = new List<ChatMessage>();

       rectHeight = 180;

        yOffset = -6;
        verticalSeparation = 24;

        endPos = 0;

        numMessages = 0;

        inputField.characterLimit = 40;
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    public bool IsTyping()
    {
        return inputField.isFocused;
    }

    public void IsOpen(bool open)
    {
        gameObject.SetActive(open);

        if (open)
        {
            ActivateInput();
        }
    }

    public void ActivateInput()
    {
        inputField.ActivateInputField();
    }

    public void DeactivateInput()
    {
        inputField.DeactivateInputField();
    }

    public void SendTypedMessage()
    {
        if (!(inputField.text == ""))
        {
            ChampNetManager.SendChatMessage(inputField.text);

            inputField.text = "";
        }

        ActivateInput();
    }

    public void AddMessage(string username, string message)
    {
        ++numMessages;
        
        //Create new message and fill it with info
        ChatMessage newMessage;
        newMessage.userName = username;
        newMessage.message = message;

        //Add message to message list
        messages.Add(newMessage);

        //Create Object for message to sit on
        GameObject messageObject = Instantiate(chatBoxPrefab, contentWindow);

        RectTransform messageTransform = messageObject.GetComponent<RectTransform>();

        messageTransform.anchoredPosition = new Vector3(X_OFFSET, yOffset, 0);

        TextMeshProUGUI gui = messageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        gui.text = "[" + username + "]: " + message;

        yOffset -= verticalSeparation;

        if (numMessages > MAX_MESSAGES_PER_BOX)
        {
            rectHeight += NEW_MESSAGE_SPACE;

            contentWindow.sizeDelta = new Vector2(0, rectHeight);


            float difference = Mathf.Abs(endPos - contentWindow.anchoredPosition.y);
            endPos += NEW_MESSAGE_SPACE;

            if (difference <= 10)
            {
                contentWindow.anchoredPosition = new Vector3(0, endPos);
            }
        }
    }
}
