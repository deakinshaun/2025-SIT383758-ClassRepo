using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 30, 0); // Y-axis rotation

    void FixedUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
