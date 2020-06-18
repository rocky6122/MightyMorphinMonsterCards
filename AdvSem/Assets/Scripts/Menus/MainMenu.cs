////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  MainMenu : class | Written by John Imgrund                                                                    //
//  Used in MenuScene. Has functions for button in the scene.                                                     //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Quits the game.
    /// </summary>
    /// 

    public Animator anim;

    private void Start()
    {
        FindObjectOfType<AudioManagerScript>().Play("Menu Theme");
    }

    public void TransitionAnimation(int animation)
    {
        anim.SetInteger("transition", animation);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
