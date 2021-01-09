using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.UI;

public class NotifsHandler : MonoBehaviour
{
    public Text PrevScore;
    private int prevScore;

    public void NotifChannel()
    {
        //Channel init data
        string ChID = "Proggers";
        string title = "Score Champ";
        Importance importance = Importance.Default;
        string desc = "Proggy notifs through channel";

        //Notif panel struct
        AndroidNotificationChannel ch = new AndroidNotificationChannel(ChID, title, desc, importance);
        
        //Registry entry
        AndroidNotificationCenter.RegisterNotificationChannel(ch);
    }

    private void Awake()
    {
        NotifChannel();
    }

    //Send notif function/s
    public void SendScoreNotif(int score)
    {
        //Data
        string header = "Your Score:";
        string msg = score.ToString();

        //Timeline
        System.DateTime deployNotifTime = System.DateTime.Now.AddSeconds(2);

        //Notif go pewpew
        AndroidNotification newNotif = new AndroidNotification(header, msg, deployNotifTime);

        //Prev Score data
        newNotif.IntentData = score.ToString();

        //TODO:Gura Icon stuff
        newNotif.SmallIcon = "gura";
        newNotif.LargeIcon = "guralarge";

        AndroidNotificationCenter.SendNotification(newNotif, "Proggers");
    }

    public void parseData()
    {
        Debug.Log("Attempting to parse notif");
        AndroidNotificationIntentData cringeFactor = AndroidNotificationCenter.GetLastNotificationIntent();

        if (cringeFactor == null)
        {
            Debug.Log("No notif data found");
            PrevScore.text = "0";
            prevScore = 0;
        }

        else
        {
            Debug.Log("Notif data found");
            PrevScore.text = cringeFactor.Notification.IntentData;
            prevScore = int.Parse(cringeFactor.Notification.IntentData);
        }
    }

    public int getprevScore()
    {
        return prevScore;
    }
}
