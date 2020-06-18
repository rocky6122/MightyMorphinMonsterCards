////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  PersistantDataManager : class | Written by John Imgrund and Park Staszkiewicz                                 //
//  Contain the data that will be transfered from scene to scene                                                  //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using ChampNet;

public static class PersistentDataManager
{
    //Persistant Data Variables
    private static string userName;

    private static string winnerName;

    private static int playerNumber;

    private static int numberOfNotifications;

    private static List<ChatMessage> messages = new List<ChatMessage>();

    private static CardInfoHolder[] playerChampions = new CardInfoHolder[3];

    private static int championIndex = 0;


    //Add champion Cards here to continue from scene to scene

    //Data Setters
    public static void SetUserName(string name)
    {
        userName = name;
    }

    public static void SetMessage(ChatMessage message)
    {
        messages.Add(message);
    }

    public static void setPlayerNumber(int num)
    {
        playerNumber = num;
    }

    public static void setWinnerName(string name)
    {
        winnerName = name;
    }

    public static void SetChampion(CardInfoHolder info)
    {
        playerChampions[championIndex] = info;

        ++championIndex;
    }

    //Data Getters
    public static string GetUserName()
    {
        return userName;
    }

    public static ChatMessage GetMessage(int index)
    {
        return messages[index];
    }

    public static int GetMessageCount()
    {
        return messages.Count;
    }

    public static int GetPlayerNumber()
    {
        return playerNumber;
    }

    public static string GetWinnerName()
    {
        return winnerName;
    }

    public static CardInfoHolder[] GetChampions()
    {
        return playerChampions;
    }
}
