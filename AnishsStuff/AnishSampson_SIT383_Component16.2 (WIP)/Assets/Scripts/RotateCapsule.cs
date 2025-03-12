using UnityEngine;

public class RotateCapsule : MonoBehaviour
{
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(0.5f, new Vector3(1, 0, 0.5f));
    }
}