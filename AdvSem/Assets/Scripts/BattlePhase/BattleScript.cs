////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  BattleScript : class | Written by John Imgrund                                                                //
//                                                                                                                //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using ChampNet;
using UnityEngine.UI;

public class BattleScript : MonoBehaviour
{
    private CardHand hand;

    CardDisplay playerOneChampion;
    CardDisplay playerTwoChampion;

    CardInfoHolder benchOneChampion;
    CardInfoHolder benchTwoChampion;

    public GameObject cardPrefab;
    public GameObject slotOne;
    public GameObject slotTwo;
    private Transform realSlotOne;
    private Transform realSlotTwo;

    CardDisplay cardOne;
    CardDisplay cardTwo;

    CardInfoHolder[] champions;

    public Slider playerOneSlider;
    public Slider playerTwoSlider;
    private float playerOneMaxTime;
    private float playerTwoMaxTime;
    private float playerOneTime;
    private float playerTwoTime;

    int playerNum;

    const float MAGIC_NUMBER = 2.4f;
    const float MAGIC_MULTIPLIER = 10f;
    const float BASE_SPEED = 4.0f;

    //public float playerHealth;
    //public float enemyHealth;

    int finalAttack;
    int finalDefense;

    AudioManagerScript audioManager;

    //Asset Bundle
    private AssetBundle monsterCards;

    /////////////////////////////////////////////////////////////
    //Damage Multiplier Array[element, Resistance]
    // 
    //Elements: Normal, Water, Fire, Earth, Air, Lightning
    //
    //Resistances: Normal, Bronze, Steel, Brass, Pewter, Electrum
    //
    /////////////////////////////////////////////////////////////

    float[,] DamageMultiplierArray = new float[6, 6]
    { //Normal, Bronze, Steel, Brass, Pewter, Electrum
        { 1, 1, 1, 1, 1, 1 },       //Normal
        { 1, 1, 1.5f, 1, 1, 0.5f }, //Water
        { 1, 1.5f, 0.5f, 1, 1, 1 }, //Fire
        { 1, 0.5f, 1, 1, 1.5f, 1 }, //Earth
        { 1, 1, 1, 1.5f, 0.5f, 1 }, //Air
        { 1, 1, 1, 0.5f, 1, 1.5f }  //Lightning
    };

    private void Awake()
    {
        monsterCards = AssetBundleManager.GetAssetBundle("monstercard");
    }

    // Start is called before the first frame update
    void Start()
    {
        hand = FindObjectOfType<CardHand>();

        //playerOneChampion = GameObject.Find("Primary Slot").GetComponent<CardDisplay>().GetCardInfo();

        // playerTwoChampion = GameObject.Find("Enemy Slot").GetComponent<CardDisplay>().GetCardInfo();

        //playerHealth = playerOneChampion.health;
        // enemyHealth = playerTwoChampion.health;

        champions = PersistentDataManager.GetChampions();

        playerNum = PersistentDataManager.GetPlayerNumber();

        playerOneTime = playerTwoTime = 0f;

        if (playerNum == 1)
        {
            realSlotOne = slotOne.transform;
            realSlotTwo = slotTwo.transform;
        }
        else
        {
            realSlotTwo = slotOne.transform;
            realSlotOne = slotTwo.transform;

            Slider tempSlider = playerOneSlider;
            playerOneSlider = playerTwoSlider;
            playerTwoSlider = tempSlider;
        }

        //CardDisplay card = Instantiate(cardPrefab, slotOne.transform).GetComponent<CardDisplay>();

        SetBenchChampions();

        audioManager = FindObjectOfType<AudioManagerScript>();

        audioManager.Play("Battle Theme");

        //Set default healths to insane numbers so the game doesnt instantly assume someones dead
        //playerOneChampion.SetHealth(1000);
        //playerTwoChampion.SetHealth(1000);
    }

    private void OnEnable()
    {
        Draggable.ActionStartClick += SwapFromHand;
    }

    private void OnDisable()
    {
        Draggable.ActionStartClick -= SwapFromHand;
    }

    private void Update()
    {
        if (playerOneChampion == null || playerTwoChampion == null)
        {
            return;
        }

        ShowTimes();

        //Check to see if a your champion has died, if it has then destroy the card and grab the next available one from the hand
        if (playerNum == 1)
        {
            if (playerOneChampion.GetCardInfo().health <= 0)
            {
                //For now just override the dead champion
                SummonNewChampionOnDeath(1);
            }
        }
        else
        {
            if (playerTwoChampion.GetCardInfo().health <= 0)
            {
                //For now just override the dead champion
                SummonNewChampionOnDeath(2);
            }
        }

        //Check to see if all your champions are dead
    }

    private void ShowTimes()
    {
        playerOneTime += Time.deltaTime;
        playerTwoTime += Time.deltaTime;

        playerOneSlider.value = (playerOneTime / playerOneMaxTime);
        playerTwoSlider.value = (playerTwoTime / playerTwoMaxTime);
    }

    public void SetPlayerOneChampion(CardDisplay card)
    {
        if (playerOneChampion != null)
        {
            Destroy(playerOneChampion.gameObject);
        }

        playerOneChampion = card;

        //Make sure card is in the right place
        playerOneChampion.transform.SetParent(realSlotOne);

        playerOneChampion.transform.localPosition = Vector2.zero;

        playerOneMaxTime = playerOneChampion.GetCardInfo().speed * .01f * BASE_SPEED;
        playerOneTime = 0f;

        Debug.Log("Player One Champion Data Set");
    }

    public void SetPlayerTwoChampion(CardDisplay card)
    {
        if (playerTwoChampion != null)
        {
            Destroy(playerTwoChampion.gameObject);
        }

        playerTwoChampion = card;

        //Make sure card is in the right place
        playerTwoChampion.transform.SetParent(realSlotTwo);

        playerTwoChampion.transform.localPosition = Vector2.zero;

        playerTwoMaxTime = playerTwoChampion.GetCardInfo().speed * .01f * BASE_SPEED;
        playerTwoTime = 0f;

        Debug.Log("Player Two Champion Data Set");
    }

    public void SetBenchChampions()
    {
        //TODO
        //create first benched card

        string championName = champions[1].name;

        Vector2 cardDefaultScale = new Vector2(1, 1);

        if (championName != null)
        {
            cardOne = Instantiate(cardPrefab, hand.transform).GetComponent<CardDisplay>();

            MonsterCardInfo cardInfo = monsterCards.LoadAsset<MonsterCardInfo>(championName);

            cardOne.SetUpCard(cardInfo);

            cardOne.ChangeElement(champions[1].element);

            cardOne.ChangeResistance(champions[1].resistance);

            cardOne.SetHealth(champions[1].health);

            cardOne.GetComponent<Draggable>().onlyClick = true;

            cardOne.GetComponent<RectTransform>().localScale = cardDefaultScale;

            championName = champions[2].name;

            Debug.Log(championName);

            if (championName != null)
            {
                //Create second benched card
                cardTwo = Instantiate(cardPrefab, hand.transform).GetComponent<CardDisplay>();

                cardInfo = monsterCards.LoadAsset<MonsterCardInfo>(championName);

                cardTwo.SetUpCard(cardInfo);

                cardTwo.ChangeElement(champions[2].element);

                cardTwo.ChangeResistance(champions[2].resistance);

                cardTwo.SetHealth(champions[2].health);

                cardTwo.GetComponent<Draggable>().onlyClick = true;

                cardTwo.GetComponent<RectTransform>().localScale = cardDefaultScale;

                //Add Card to Hand
            }
        }
        else
            return;

        
    }

    public void PlayerOneAttack()
    {
        //Testing

        if (playerOneChampion == null)
        {
            Debug.Log("ASKING FOR CARD");
            ChampNetManager.RequestMissingChampion(1);
            return;
        }
            
        if(playerTwoChampion == null)
        {
            Debug.Log("ASKING FOR CARD");
            ChampNetManager.RequestMissingChampion(2);
            return;
        }

        float playerAttack = playerOneChampion.GetCardInfo().attack;

        float enemyDefense = playerTwoChampion.GetCardInfo().defense;

        float multiplier = CalculateMultiplier((int)playerOneChampion.GetCardInfo().element, (int)playerTwoChampion.GetCardInfo().resistance);

        int finalDamage = (int)(((MAGIC_NUMBER * playerAttack) / enemyDefense) * (MAGIC_MULTIPLIER * multiplier));

        Debug.Log("Damage Dealt: " + finalDamage);

        int newHealth = playerTwoChampion.GetCardInfo().health;
        
        newHealth -= finalDamage;

        Debug.Log("New Health value: " + newHealth);

        //Update health on screen
        playerTwoChampion.SetHealth(newHealth);

        playerOneTime = 0f;

        //play sound
        audioManager.Play("Clash");
    }

    public void PlayerTwoAttack()
    {
        //Testing

        if (playerOneChampion == null)
        {
            Debug.Log("ASKING FOR CARD");
            ChampNetManager.RequestMissingChampion(1);
            return;
        }

        if (playerTwoChampion == null)
        {
            Debug.Log("ASKING FOR CARD");
            ChampNetManager.RequestMissingChampion(2);
            return;
        }

        float playerAttack = playerTwoChampion.GetCardInfo().attack;

        float enemyDefense = playerOneChampion.GetCardInfo().defense;

        float multiplier = CalculateMultiplier((int)playerTwoChampion.GetCardInfo().element, (int)playerOneChampion.GetCardInfo().resistance);

        int finalDamage = (int)(((MAGIC_NUMBER * playerAttack) / enemyDefense) * (MAGIC_MULTIPLIER * multiplier));

        Debug.Log("Damage Dealt: " + finalDamage);

        int newHealth = playerOneChampion.GetCardInfo().health;

        newHealth -= finalDamage;

        Debug.Log("New Health value: " + newHealth);

        //update health on screen
        playerOneChampion.SetHealth(newHealth);

        playerTwoTime = 0f;

        //play sound
        audioManager.Play("Clash");
    }

    public void SummonNewChampionOnDeath(int num)
    {
        if (num == 1)
        {
            //Delete Dead Card
            Destroy(playerOneChampion.gameObject);

            //Pull card info from hand
            playerOneChampion = hand.GetCardFromHand();

            if (playerOneChampion == null)
            {
                Debug.Log("End Game Message");
                ChampNetManager.SendEndGameMessage();

                PersistentDataManager.setWinnerName(PersistentDataManager.GetUserName());
                SceneManager.LoadScene(4);

                return;
            }

            //Move Card off bench to slot
            playerOneChampion.transform.SetParent(realSlotOne);

            playerOneChampion.transform.localPosition = Vector2.zero;

            ChampNetManager.SendChampion(playerOneChampion.GetCardInfo());
            
            return;
        }

        if (num == 2)
        {
            //Delete Dead Card
            Destroy(playerTwoChampion.gameObject);

            //Grabs Front Card and deletes it
            playerTwoChampion = hand.GetCardFromHand();

            if (playerTwoChampion == null)
            {
                Debug.Log("End Game Message");
                ChampNetManager.SendEndGameMessage();

                PersistentDataManager.setWinnerName(PersistentDataManager.GetUserName());
                SceneManager.LoadScene(4);

                return;
            }

            //Move Card off bench to slot
            playerTwoChampion.transform.SetParent(realSlotTwo);

            playerTwoChampion.transform.localPosition = Vector2.zero;

            ChampNetManager.SendChampion(playerTwoChampion.GetCardInfo());

            return;
        }
    }

    private void SwapFromHand(Draggable clickedCard)
    {
        CardDisplay myCard = playerNum == 1 ? playerOneChampion : playerTwoChampion;

        myCard.transform.SetParent(hand.transform);

        if (playerNum == 1)
        {
            playerOneChampion = null;
            SetPlayerOneChampion(clickedCard.GetComponent<CardDisplay>());
            ChampNetManager.SendChampion(playerOneChampion.GetCardInfo());
        }
        else
        {
            playerTwoChampion = null;
            SetPlayerTwoChampion(clickedCard.GetComponent<CardDisplay>());
            ChampNetManager.SendChampion(playerTwoChampion.GetCardInfo());
        }

        Destroy(clickedCard.gameObject);
    }

    float CalculateMultiplier(int attackerElement, int blockerResistance)
    {
        return DamageMultiplierArray[attackerElement, blockerResistance];
    }
}