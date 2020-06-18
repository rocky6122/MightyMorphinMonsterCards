using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ChampNet;

public class WinnerScreen : MonoBehaviour
{
    public TMP_Text winnersName;
    public TextMeshProUGUI winnerShadow;

    // Start is called before the first frame update
    void Start()
    {
        if (PersistentDataManager.GetWinnerName() == PersistentDataManager.GetUserName())
        {
            winnersName.text = "You Lose!";
            winnerShadow.text = "You Lose!";
        }
        else
        {
            winnersName.text = "You Win!";
            winnerShadow.text = "You Win!";
        }
    }

    private void OnApplicationQuit()
    {
        ChampNetManager.StopNetworkConnection();
    }
}
