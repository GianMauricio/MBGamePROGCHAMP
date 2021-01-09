using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Notifications.Android;

public class MainMenuScript : MonoBehaviour
{
    //TODO:Change the scene callback name when publishing
    public void StartGame()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        SceneManager.LoadScene("gameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
