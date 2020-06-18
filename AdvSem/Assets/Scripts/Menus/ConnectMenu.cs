////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  ConnectMenu : class | Written by John Imgrund                                                                 //
//  Used to collect server data and connect to said server                                                        //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ChampNet;

public class ConnectMenu : MonoBehaviour
{
    public TMP_InputField serverIP;
    public TMP_InputField userName;
    private int currentInput = -1;

    public Animator anim;

    private bool isActive;

    private void OnEnable()
    {
        userName.characterLimit = 12;
    }

    public void SetInput(int num)
    {
        currentInput = num;
    }

    private void Update()
    {
        if (isActive)
        {
            if (!serverIP.isFocused && !userName.isFocused)
            {
                currentInput = -1;
            }
            else
            {
                currentInput = serverIP.isFocused ? 0 : 1;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentInput = (currentInput + 1) % 2;

                ActivateInputField(currentInput);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                JoinServer();
            }
        }
    }

    private void ActivateInputField(int num)
    {
        if (num == 0)
        {
            serverIP.Select();
        }
        else
        {
            userName.Select();
        }
    }

    /// <summary>
    /// Connects the player to the server based on the given IP address.
    /// </summary>
    public void JoinServer()
    {
        ChampNetManager.InitClient(userName.text, serverIP.text);

        PersistentDataManager.SetUserName(userName.text);

        NextScene();
    }

    public void TransitionAnimation(int animation)
    {
        anim.SetInteger("transition", animation);

        if (animation == 1)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }
    }

    public void NextScene()
    {
        if (userName.text == "" || serverIP.text == "")
        {
            AudioManagerScript.instance.Play("Death");
            return;
        }

        SceneManager.LoadScene(1);
    }
}
