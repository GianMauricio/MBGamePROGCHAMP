using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]

public enum RotationDirections
{
    CW,CCW
}
public class RotateEventArgs : EventArgs
{
    public Touch Finger1 { get; private set; }
    public Touch Finger2 { get; private set; }

    public float Angle { get; private set; }

    public RotationDirections RotationDirection { get; private set; }

    public GameObject HitObject { get; private set; }

    public RotateEventArgs(Touch f1, Touch f2, float A,RotationDirections dir, GameObject obj)
    {
        Finger1 = f1;
        Finger2 = f2;
        Angle = A;
        RotationDirection = dir;
        HitObject = obj;
    }
}

