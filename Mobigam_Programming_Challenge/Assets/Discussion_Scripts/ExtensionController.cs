using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionController : MonoBehaviour
{
    public GameObject ExtendedPanel;

    private DeviceOrientation currentOrientation;
    private DeviceOrientation prevOrientation = DeviceOrientation.LandscapeRight;

    // Update is called once per frame
    void Update()
    {
        currentOrientation = Input.deviceOrientation;
        if(prevOrientation != currentOrientation)
        {
            if(currentOrientation == DeviceOrientation.LandscapeLeft ||
                currentOrientation == DeviceOrientation.LandscapeRight)
            {
                ExtendedPanel.SetActive(true);
            }
            else if(currentOrientation == DeviceOrientation.Portrait ||
                    currentOrientation == DeviceOrientation.PortraitUpsideDown)
            {
                ExtendedPanel.SetActive(false);
            }
        }
        prevOrientation = currentOrientation;
    }
}
