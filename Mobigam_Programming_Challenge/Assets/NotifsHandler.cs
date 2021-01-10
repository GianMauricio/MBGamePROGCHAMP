using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.UI;

public class NotifsHandler : MonoBehaviour
{
    public Text PrevScore;
    private int prevScore;
    private bool hasPrevScore = false;

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
        AndroidNotificationIntentData cringeFactor = AndroidNotificationCenter.GetLastNotificationIntent();

        if (cringeFactor == null)
        {
            PrevScore.text = "0";
            prevScore = 0;
            hasPrevScore = false;
        }

        else
        {
            PrevScore.text = cringeFactor.Notification.IntentData;
            prevScore = int.Parse(cringeFactor.Notification.IntentData);
            hasPrevScore = true;
        }
    }

    public int getprevScore()
    {
        return prevScore;
    }

    public bool checkPrevScore()
    {
        return hasPrevScore;
    }
}
