using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CardDeck : MonoBehaviour
{
    public List<CardInfo> allCards;
    private TextMeshProUGUI countdown;

    public Color32 redColor = Color.red;
    public Color32 blackColor = Color.black;

    private Image deckImage;

    private bool countdownWarning;

    private void Start()
    {
        countdown = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        deckImage = GetComponent<Image>();
        ShuffleCards();
        countdownWarning = false;
    }

    private void ShuffleCards()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < allCards.Count; i++)
        {
            int randomIndex = i + (int)(rand.NextDouble() * (allCards.Count - i));
            CardInfo card = allCards[randomIndex];
            allCards[randomIndex] = allCards[i];
            allCards[i] = card;
        }
    }

    public void UpdateCountdown(int seconds)
    {
        if (allCards.Count > 0)
        {
            if (seconds > 9)
            {
                countdown.text = ":" + seconds.ToString();
            }
            else
            {
                countdown.text = ":0" + seconds.ToString();
            }

            bool warning = (seconds < 3);

            if (warning && !countdownWarning)
            {
                countdownWarning = true;

                deckImage.color = redColor;
            }
            else if (!warning && countdownWarning)
            {
                countdownWarning = false;

                deckImage.color = blackColor;
            }
        }
        else
        {
            //countdown.gameObject.SetActive(false);
            deckImage.color = blackColor;
        }

    }

    public CardInfo DrawTopCard()
    {
        if (allCards.Count > 0)
        {
            CardInfo topCard = allCards[0];
            allCards.RemoveAt(0);

            return topCard;
        }

        return null;
    }
}
