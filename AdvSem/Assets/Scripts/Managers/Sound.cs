////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  Sound : class | Written by John Imgrund                                                                       //
//  Container for sound effects and sound info.                                                                   //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

[System.Serializable]
public class Sound
{
    //Audio Objects
    public AudioClip soundClip;

    [HideInInspector]
    public AudioSource soundSource;

    //Variables
    public string name;

    public bool loop;

    [Range(0f, 1f)]
    public float volume;
}
