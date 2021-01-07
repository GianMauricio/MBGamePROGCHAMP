using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public GameHandler gameHandler;
    public Image timerBar;

    // Update is called once per frame
    void Update()
    {
        timerBar.fillAmount = 1 - (gameHandler.CurrentTime / gameHandler.MaxTime);

        if(timerBar.fillAmount < 0.25f)
        {
            timerBar.color = Color.green;
        }
        else if(timerBar.fillAmount < 0.50f)
        {
            timerBar.color = Color.yellow;
        }
        else
        {
            timerBar.color = Color.red;
        }
    }
}
