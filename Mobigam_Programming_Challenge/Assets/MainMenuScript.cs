using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    //TODO:Change the scene callback name when publishing
    public void StartGame()
    {
        SceneManager.LoadScene("gameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
