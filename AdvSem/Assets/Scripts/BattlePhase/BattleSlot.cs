using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSlot : MonoBehaviour
{
    private GameObject championBench;
    public int index;

    private GameObject champion;
    private GameObject mainChampion;


    // Start is called before the first frame update
    void Start()
    {
        //Find the main Champion
        championBench = GameObject.Find("ChampionBench");

        //Pop a champion off the championBench List and add it to the ChampionSlot
        //mainChampion = championBench.GetComponent<ChampionBench>().getChampion(index);
    }
}
