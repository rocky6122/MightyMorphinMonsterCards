using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartLookUp : MonoBehaviour
{
    public void ToggleChart()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
