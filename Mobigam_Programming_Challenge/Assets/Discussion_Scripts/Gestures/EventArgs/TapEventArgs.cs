using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TapEventArgs : EventArgs
{
	
	private Vector2 tapPosition;
	private GameObject tappedobject;

	public TapEventArgs(Vector2 pos, GameObject obj = null)
	{

		tapPosition = pos;
		tappedobject = obj;
	}

	public Vector2 TapPosition
	{
		get
		{
			return tapPosition;
		}

	}

	public GameObject TappedObject
	{
		get
		{
			return tappedobject;
		}

	}
}
