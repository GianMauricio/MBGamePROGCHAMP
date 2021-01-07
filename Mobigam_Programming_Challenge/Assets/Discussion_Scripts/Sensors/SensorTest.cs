using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorTest : MonoBehaviour
{
    public float MinChange = 0.1f;
    public float Speed = 5;

    Quaternion initialRotaion;

    public OnScreenStick stick;
    private void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            initialRotaion = QuaternionToUnity(Input.gyro.attitude);
        }

        else
        {
            Debug.Log("Does no have gyro");
        }
    }

    public float minAngle = 15;
    private void FixedUpdate()
    {
        /*
         //Gyroscope
        Vector3 diffVector = QuaternionToUnity(Input.gyro.attitude).eulerAngles - initialRotaion.eulerAngles;

        float z = diffVector.z;

        if (Mathf.Abs(z) >= minAngle)
        {
            float change = -z * Time.deltaTime * 0.5f;

            transform.Translate(z, 0, 0);
        }
        
        //Accelorometer
        float x = Input.acceleration.x;
        if (Mathf.Abs(x) >= MinChange)
        {
           
            float change = x * Speed * Time.deltaTime;

            transform.Translate(change, 0, 0);
        }
        */
        Vector2 stickValue = stick.JoystickVector;
        Vector3 changed = stickValue * Speed * Time.deltaTime;

        transform.Translate(changed);
    }

    Quaternion QuaternionToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Debug.DrawRay(transform.position + Vector3.up, Input.acceleration, Color.cyan);
        }
    }
}
