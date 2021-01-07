using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpreadEventArgs : EventArgs
{
   public Touch Finger1 { get; private set; }
   public Touch Finger2 { get; private set; }

    public float DistanceDiff { get; private set; }

    public GameObject HitObject { get; private set; }

    public SpreadEventArgs(Touch f1, Touch f2, float dist, GameObject obj)
    {
        Finger1 = f1;
        Finger2 = f2;
        DistanceDiff = dist;
        HitObject = obj;
    }
}
