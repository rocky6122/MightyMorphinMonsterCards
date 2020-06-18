using UnityEngine;
using System.Collections;

public class MorphingPhase : MonoBehaviour
{
    private CardDeck deck;
    private CardHand hand;

    public ChampionSlot[] champions;

    private readonly float timeBetweenDraws = 6f;
    private float timerToDraw = 0f;

    private void Start()
    {
        deck = FindObjectOfType<CardDeck>();

        hand = FindObjectOfType<CardHand>();

        FindObjectOfType<AudioManagerScript>().Play("Morphing Theme");
        FindObjectOfType<AudioManagerScript>().Play("Shuffle");

        for (int i = 0; i < 5; i++)
        {
            DrawCardFromDeck();
        }
    }

    private void Update()
    {
        timerToDraw += Time.deltaTime;

        deck.UpdateCountdown(Mathf.RoundToInt(timeBetweenDraws - timerToDraw));

        if (timerToDraw >= timeBetweenDraws)
        {
            timerToDraw = 0f;
        }
    }

    public void DrawCardFromDeck()
    {
        CardInfo drawnCard = deck.DrawTopCard();

        if (drawnCard != null)
        {
            hand.AddCardFromDeck(drawnCard);
        }
    }

    public CardInfoHolder GetInfoAtSlot(int index)
    {
        return champions[index].GetCardInfo();
    }
}
