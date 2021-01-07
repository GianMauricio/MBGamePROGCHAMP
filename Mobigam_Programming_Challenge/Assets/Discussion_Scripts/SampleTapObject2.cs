using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTapObject2 : MonoBehaviour, ITapped
{
    public void OnTap()
    {

        Destroy(gameObject);
    }
}
