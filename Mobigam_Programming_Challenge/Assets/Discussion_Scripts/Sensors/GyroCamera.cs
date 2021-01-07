using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }

        else
        {
            Debug.Log("Does no have gyro");
        }
    }

    private void FixedUpdate()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Vector3 rot = Input.gyro.rotationRate;

            rot.x *= -1;
            rot.y *= -1;

            transform.Rotate(rot);
        }
     }

}
