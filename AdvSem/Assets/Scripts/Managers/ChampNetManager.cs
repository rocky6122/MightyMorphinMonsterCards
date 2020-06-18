////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Mighty Morphin Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019        //
//                                                                                                                //
//  ChampNet : namespace | Written by Parker Staszkiewicz                                                         //
//  The ChampNet namespace ensures that anything defined by our plug-in will not conflict with any Unity          //
//  definitions.                                                                                                  //
//                                                                                                                //
//  CHAMPNET_MESSAGE_ID : enum | Written by John Imgrund                                                          //
//  These message IDs correspond to values within the DLL plug-in and are used for determing what message         //
//  was sent.                                                                                                     //
//                                                                                                                //
//  ChampNetManager : static class | Written by Parker Staszkiewicz and John Imgrund                              //
//  A collection of static extern functions for accessing the DLL plug-in as well as static wrapper functions     //
//  for calling the extern functions within other classes.                                                        //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace ChampNet
{
    public enum CHAMPNET_MESSAGE_ID
    {
        ID_MESSAGE_DATA = 136,
        ID_PLAYER_NUM,
        ID_START_GAME,
        ID_DRAW_CARD,
        ID_BATTLE_PHASE,
        ID_CHAMPION_DATA,
        ID_ATTACK,
        ID_END_GAME,
    }

    public struct ChatMessage
    {
        public string userName;
        public string message;
    }

    public static class ChampNetManager
    {
        #region DLL Imports

        #region Init and Destroy
        [DllImport("ChampNet_x64")]
        static extern void InitializeHostNetworking(string userName);

        [DllImport("ChampNet_x64")]
        static extern void InitializeClientNetworking(string userName, string serverIP);

        [DllImport("ChampNet_x64")]
        static extern void StopNetworking();
        #endregion

        #region Polling
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("ChampNet_x64")]
        static extern int ReceiveMessageType();

        [DllImport("ChampNet_x64")]
        static extern int GetCurrentClientNum();

        [DllImport("ChampNet_x64")]
        static extern void PopReceiver();
        #endregion

        #region ChatMessage
        [DllImport("ChampNet_x64")]
        static extern void AddChatMessageToSender(string message);

        [DllImport("ChampNet_x64")]
        static extern IntPtr ChatMessageUserName();

        [DllImport("ChampNet_x64")]
        static extern IntPtr ChatMessageMessage();

        #endregion

        #region PlayerNumMessage

        [DllImport("ChampNet_x64")]
        static extern int PlayerNumMessage();

        [DllImport("ChampNet_x64")]
        static extern void GetMissingChampion(int playerNum);

        #endregion

        #region ChampionData

        [DllImport("ChampNet_x64")]
        static extern void AddChampionDataToSender(string monsterName, int monsterHealth, int monsterSpeed, int element, int alloy, int playerNum);

        [DllImport("ChampNet_x64")]
        static extern IntPtr ChampionDataName();

        [DllImport("ChampNet_x64")]
        static extern int ChampionHealth();

        [DllImport("ChampNet_x64")]
        static extern int ChampionElement();

        [DllImport("ChampNet_x64")]
        static extern int ChampionAlloy();

        [DllImport("ChampNet_x64")]
        static extern int ChampionsPlayerNum();

        #endregion

        #region AttackMessage

        [DllImport("ChampNet_x64")]
        static extern int AttackingPlayer();

        #endregion

        #region EndGame

        [DllImport("ChampNet_x64")]
        static extern void AddEndGameMessageToSender();

        [DllImport("ChampNet_x64")]
        static extern IntPtr EndGameMessage();

        #endregion

        #endregion

        #region Wrapper Functions

        static bool networkIsActive = false;

        public static int GetMessageType()
        {
            return ReceiveMessageType();
        }

        public static void PopMessage()
        {
            PopReceiver();
        }

        public static int GetClientNum()
        {
            return GetCurrentClientNum();
        }

        public static void InitClient(string username, string serverIP)
        {
            InitializeClientNetworking(username, serverIP);
            networkIsActive = true;
        }

        public static void StopNetworkConnection()
        {
            if (networkIsActive)
            {
                StopNetworking();
            }
        }


        public static void SendChatMessage(string msg)
        {
            AddChatMessageToSender(msg);
        }

        public static ChatMessage GetChatMessage()
        {
            ChatMessage chatMsg;

            chatMsg.userName = Marshal.PtrToStringAnsi(ChatMessageUserName());
            chatMsg.message = Marshal.PtrToStringAnsi(ChatMessageMessage());

            PopReceiver();

            return chatMsg;
        }

        public static int GetPlayerNum()
        {
            int playerNum;

            playerNum = PlayerNumMessage();

            PopReceiver();

            return playerNum;
        }

        public static void RequestMissingChampion(int playerNum)
        {
            GetMissingChampion(playerNum);
        }

        public static void SendChampion(CardInfoHolder card)
        {
            //LOAD AND SEND DATA
            AddChampionDataToSender(card.name, card.health, card.speed, (int)card.element, (int)card.resistance, PersistentDataManager.GetPlayerNumber());
        }

        public static string GetChampionName()
        {
            string monsterName;

            monsterName = Marshal.PtrToStringAnsi(ChampionDataName());

            return monsterName;
        }

        public static int GetChampionHealth()
        {
            int health;

            health = ChampionHealth();

            return health;
        }

        public static int GetChampionElement()
        {
            int element;

            element = ChampionElement();

            return element;
        }

        public static int GetChampionAlloy()
        {
            int alloy;

            alloy = ChampionAlloy();

            return alloy;
        }

        public static int GetChampionsPlayer()
        {
            int playerNum;

            playerNum = ChampionsPlayerNum();

            return playerNum;
        }



        public static int GetAttackingPlayer()
        {
            int attackingPlayerNum;

            attackingPlayerNum = AttackingPlayer();

            PopReceiver();

            return attackingPlayerNum;
        }

        public static void SendEndGameMessage()
        {
            AddEndGameMessageToSender();
        }

        public static string GetWinner()
        {
            string winnerName;

            winnerName = Marshal.PtrToStringAnsi(EndGameMessage());

            PopReceiver();

            return winnerName;
        }

        #endregion
    }
}