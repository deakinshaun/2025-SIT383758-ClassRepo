using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float amplitude = 1.0f;
    public float frequency = 1.0f;

    void Update()
    {
        transform.position = new Vector3(0.0f, 0.0f, amplitude * Mathf.Sin(2.0f * Mathf.PI * frequency * Time.time));
    }
}
