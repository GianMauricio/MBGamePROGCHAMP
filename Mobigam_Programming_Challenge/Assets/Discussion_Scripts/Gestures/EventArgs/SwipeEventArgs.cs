using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum Directions {
    UP,DOWN,LEFT,RIGHT
}

public class SwipeEventArgs : EventArgs
{
    private Vector2 swipePosition;
    private Vector2 rawDirection;
    private Directions direction;
    private GameObject hitObject;

    public SwipeEventArgs(Vector2 pos, Vector2 rawDir, Directions dir, GameObject hit) {

        swipePosition = pos;
        rawDirection = rawDir;
        direction = dir;
        hitObject = hit;
    }

    public Vector2 SwipePosition
    {
        get { 
            return swipePosition;
        }
    }

    public Vector2 RawDirection
    {
        get
        {
            return rawDirection;
        }
    }

    public Directions Direction
    {
        get
        {
            return direction;
        }
    }

     public GameObject HitObject
    {
        get
        {
            return hitObject;
        }
    }
   
}

