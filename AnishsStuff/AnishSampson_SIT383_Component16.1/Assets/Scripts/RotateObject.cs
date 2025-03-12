using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    void Update ()
    {
       transform.rotation *= Quaternion.AngleAxis(0.15f, new Vector3(1, -1, 1));
    }
}