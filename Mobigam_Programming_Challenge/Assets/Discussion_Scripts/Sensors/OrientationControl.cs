using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationControl : MonoBehaviour
{
    DeviceOrientation lastOrientaion;

    // Update is called once per frame
    void Update()
    {
        lastOrientaion = Input.deviceOrientation;
    }

    public void FingerDown()
    {
        int _intOrientation = (int)lastOrientaion;

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        if (_intOrientation > 4) _intOrientation = (int)DeviceOrientation.LandscapeLeft;

        Screen.orientation = (ScreenOrientation) _intOrientation;
    }

    public void FingerUp()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
