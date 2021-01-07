using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        GestureManager.Instance.OnTwoFingerPan += OnTwoFingerPan;
    }

    private void OnDisable()
    {
        GestureManager.Instance.OnTwoFingerPan -= OnTwoFingerPan;
    }
    private void OnTwoFingerPan(object sender, TwoFingerPanEventArgs e)
    {
        Vector2 d1 = e.Finger1.deltaPosition;
        Vector2 d2 = e.Finger2.deltaPosition;

        Vector2 ave = (d1 + d2) / 2;

        Vector3 change = ((Vector3)(ave / Screen.dpi))* Speed;

        transform.position += change;
    }

   
}
