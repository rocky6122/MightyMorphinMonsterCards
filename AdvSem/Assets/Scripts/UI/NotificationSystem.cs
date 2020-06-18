////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Mighty Morphin' Monster Cards | Programmed and Designed by Parker Staszkiewicz and John Imgrund (c) 2019      //
//                                                                                                                //
//  NotficationSystem : class | Written by John Imgrund                                                           //
//  Show the user the number of incoming Notifications from the chat system                                       //
//                                                                                                                //
//  Inpsired by Project Buckstein ChatSystem                                                                      //
//                                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    public TextMeshProUGUI numberOfNotifications;

    int incomingNotifications;

    public void ResetNotifications()
    {
        incomingNotifications = 0;
        EnableNotificationPanel();
    }

    public void AddNotification()
    {
        ++incomingNotifications;
        EnableNotificationPanel();
    }

    public void EnableNotificationPanel()
    {
        numberOfNotifications.text = incomingNotifications.ToString();

        gameObject.SetActive(true);
    }

    public void DisableNotificationPanel()
    {
        gameObject.SetActive(false);
    }
}
