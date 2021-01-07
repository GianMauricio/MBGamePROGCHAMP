using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEnabler : MonoBehaviour
{
    public GameObject GridPanel;


    public void ToggleGrid()
    {
        GridPanel.SetActive(!GridPanel.activeSelf);
    }
}
