////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  AudioManagerScript : class | Written by John Imgrund                                                          //
//  Managers all of the sounds in game via a list of Sound objects.                                               //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public Sound[] soundList;

    public static AudioManagerScript instance;

    void Awake()
    {
        //Check to see if an audio manager already exists in the scene
        //Audio Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Allow the audioManager to persist scenes
        //DontDestroyOnLoad(gameObject);


        foreach (Sound clip in soundList)
        {
            //Create AudioSource for sound
            clip.soundSource = gameObject.AddComponent<AudioSource>();

            //Add in sound and volume
            clip.soundSource.clip = clip.soundClip;
            clip.soundSource.volume = clip.volume;
            clip.soundSource.loop = clip.loop;
        }
    }

    //Plays selected sound
    public void Play(string soundName)
    {
        //Create finalSound object
        Sound finalSound = null;

        //Check to see if the sound is in the soundList
        foreach (Sound clip in soundList)
        {
            if (clip.name == soundName)
            {
                finalSound = clip;
            }
        }

        //If no sound matches the sound name, return
        if (finalSound == null)
        {
            Debug.LogWarning("Error! No sound with name " + soundName + " exists in the soundList. Please check spelling.");
            return;
        }

        //Play sound
        finalSound.soundSource.Play();
    }
}
