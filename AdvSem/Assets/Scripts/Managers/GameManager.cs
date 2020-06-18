using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using ChampNet;

public class GameManager : MonoBehaviour
{
    //variables
    int incomingMessage;

    const string ASSET_PATH = "Assets/Cards/";
    const string END_OF_PATH = ".asset";

    public GameObject cardDisplayPrefab;
    GameObject canvasObject;

    public ChatSystem chat;
    public NotificationSystem notifications;
    public BattleScript battler;
    public MorphingPhase morpher;
    public ChartLookUp lookUp;

    Transform playerOneTransform;
    Transform playerTwoTransform;

    private bool chatOpen;

    private AssetBundle monsterCards;

    // Start is called before the first frame update
    void Start()
    {
        AssetBundleManager.LoadAssetBundle("monstercard");

        monsterCards = AssetBundleManager.GetAssetBundle("monstercard");

        if (monsterCards == null)
        {
            Debug.LogWarning("Monster Card Bundle Null");
        }

        Scene current = SceneManager.GetActiveScene();
        if ( current.name != "WaitingScene")
        {
            //Intialize the ChatSystem
            chat.InitializeChatBox();
            chat.IsOpen(false);
            chatOpen = false;

            AddMessageToChat("HOST", "Welcome to the game!");
        }

        if (current.name == "Battle Phase")
        {
            playerOneTransform = GameObject.Find("Player One Slot").transform;
            playerTwoTransform = GameObject.Find("Player Two Slot").transform;

            int messageCount = PersistentDataManager.GetMessageCount();

            for(int i = 0; i < messageCount; ++i)
            {
                ChatMessage newMessage = PersistentDataManager.GetMessage(i);

                //Add Message to chatsystem
                chat.AddMessage(newMessage.userName, newMessage.message);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleNetworking();

        HandleInput();

        if (Input.GetKeyDown(KeyCode.P) && !chat.IsTyping())
        {
            MonsterCardInfo monster = monsterCards.LoadAsset<MonsterCardInfo>("Gnome on the Range");

            AddMessageToChat("SYSTEM", monster.name);

            Debug.Log(monster);
        }
    }

    private void OnApplicationQuit()
    {
        ChampNetManager.StopNetworkConnection();
    }

    void HandleInput()
    {
        //Check for chat System
        if (Input.GetKeyDown(KeyCode.T) && !chat.IsTyping())
        {
            if (chatOpen)
            {
                chat.IsOpen(false);
                notifications.ResetNotifications();
                chatOpen = false;
            }
            else
            {
                chat.IsOpen(true);
                notifications.DisableNotificationPanel();
                chatOpen = true;
            }
        }

        //Check for Chart LookUp
        if (Input.GetKeyDown(KeyCode.M) && !chat.IsTyping())
        {
            if (lookUp == null)
            {
                return;
            }

            lookUp.ToggleChart();
        }
    }

    //Handle all incoming ChampNet Packets
    void HandleNetworking()
    {
        incomingMessage = ChampNetManager.GetMessageType();

        switch (incomingMessage)
        {
            case -1:
                {
                    //No Message To Receive
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_MESSAGE_DATA:
                {
                    //Chat Message
                    ChatMessage msg = ChampNetManager.GetChatMessage();

                    PersistentDataManager.SetMessage(msg);

                    AddMessageToChat(msg.userName, msg.message);
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_PLAYER_NUM:
                {
                    PersistentDataManager.setPlayerNumber(ChampNetManager.GetPlayerNum());
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_START_GAME:
                {
                    ChampNetManager.PopMessage();
                    SceneManager.LoadScene(2); //Morphing Phase Scene
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_DRAW_CARD:
                {
                    morpher.DrawCardFromDeck(); //Draw Card

                    ChampNetManager.PopMessage();
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_BATTLE_PHASE:
                {
                    // Transfer Data from Morphing scene to persistent data

                    for (int i = 0; i < 3; i++)
                    {
                        PersistentDataManager.SetChampion(morpher.GetInfoAtSlot(i));
                    }

                    //Grab primary champion to send to combat
                    ChampNetManager.SendChampion(morpher.GetInfoAtSlot(0));

                    ChampNetManager.PopMessage();

                    SceneManager.LoadScene(3); //BattlePhase Scene
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_CHAMPION_DATA:
                {
                    //Check which players Champion it is
                    int player = ChampNetManager.GetChampionsPlayer();

                    string championName = ChampNetManager.GetChampionName();

                    MonsterCardInfo cardInfo = monsterCards.LoadAsset<MonsterCardInfo>(championName);


                    if (player == 1) //Set player one champion
                    {
                        //create card
                        CardDisplay card = Instantiate(cardDisplayPrefab, playerOneTransform).GetComponent<CardDisplay>();

                        card.SetUpCard(cardInfo);

                        card.ChangeElement((Element)ChampNetManager.GetChampionElement());

                        card.ChangeResistance((Resistance)ChampNetManager.GetChampionAlloy());

                        card.SetHealth(ChampNetManager.GetChampionHealth());

                        card.GetComponent<Draggable>().onlyClick = true;

                        //send to slot
                        battler.SetPlayerOneChampion(card);

                    }
                    else if (player == 2) //Set player two champion
                    {
                        //create card
                        CardDisplay card = Instantiate(cardDisplayPrefab, playerTwoTransform).GetComponent<CardDisplay>();

                        card.SetUpCard(cardInfo);
                        card.ChangeElement((Element)ChampNetManager.GetChampionElement());

                        card.ChangeResistance((Resistance)ChampNetManager.GetChampionAlloy());

                        card.SetHealth(ChampNetManager.GetChampionHealth());

                        card.GetComponent<Draggable>().onlyClick = true;

                        //send to slot
                        battler.SetPlayerTwoChampion(card);
                    }
                    else
                    {
                        Debug.LogWarning("Champion Player Number invalid! Number is: " + player);
                    }

                    ChampNetManager.PopMessage();
                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_ATTACK:
                {
                    //Attack with specific person
                    int attacker = ChampNetManager.GetAttackingPlayer();

                    if (attacker == 1) //Player One is attacking
                    {
                        battler.PlayerOneAttack();
                    }
                    else if (attacker == 2) //Player two is attacking
                    {
                        battler.PlayerTwoAttack();
                    }
                    else
                    {
                        Debug.LogWarning("Attacking Player Number invalid! Number is: " + attacker);
                    }

                }
                break;
            case (int)CHAMPNET_MESSAGE_ID.ID_END_GAME:
                {
                    //Set winners name in persistant data
                    PersistentDataManager.setWinnerName(ChampNetManager.GetWinner());

                    SceneManager.LoadScene(4); //End Scene
                }
                break;


        }
    }

    //Add incoming message to the chat
    private void AddMessageToChat(string username, string message)
    {
        chat.AddMessage(username, message);

        if (!chat.IsActive())
        {
            notifications.AddNotification();
        }
    }

    private void OpenChat(bool open)
    {
        if (open)
        {
            notifications.ResetNotifications();
            notifications.DisableNotificationPanel();
        }

        chat.IsOpen(open);
    }
}
