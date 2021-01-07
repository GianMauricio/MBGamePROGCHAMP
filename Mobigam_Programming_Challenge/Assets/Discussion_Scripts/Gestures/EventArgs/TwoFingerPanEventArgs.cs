using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TwoFingerPanEventArgs : EventArgs
{
    public Touch Finger1
    {
        get;private set;
    }

    public Touch Finger2
    {
        get;private set;
    }

    public TwoFingerPanEventArgs(Touch f1, Touch f2)
    {
        Finger1 = f1;
        Finger2 = f2;
    }
}
