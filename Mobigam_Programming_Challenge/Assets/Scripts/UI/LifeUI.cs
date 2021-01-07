using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public GameHandler gameHandler;

    public Text currLifeText;
    public Text maxLifeText;

    // Update is called once per frame
    void Update()
    {
        currLifeText.text = gameHandler.CurrentLife.ToString();
        maxLifeText.text = gameHandler.MaxLife.ToString();
    }
}
