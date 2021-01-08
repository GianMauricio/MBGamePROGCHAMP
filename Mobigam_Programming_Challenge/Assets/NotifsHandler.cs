using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotifsHandler : MonoBehaviour
{
    public void NotifChannel()
    {
        //Channel init data
        string ChID = "Proggers";
        string title = "Score Champ";
        Importance importance = Importance.Default;
        string desc = "Poggy notifs through channel";

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
        System.DateTime deployNotifTime = System.DateTime.Now.AddSeconds(10);

        //Notif go pewpew
        AndroidNotification newNotif = new AndroidNotification(header, msg, deployNotifTime);
    }
}
