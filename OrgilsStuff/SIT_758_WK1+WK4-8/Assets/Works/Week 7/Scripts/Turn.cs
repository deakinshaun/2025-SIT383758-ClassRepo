using UnityEngine;

public class Turn : MonoBehaviour
{
    [SerializeField] private float turnSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float turnDir = 0;
        if (Input.GetKey(KeyCode.D))
        {
            turnDir += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnDir += -1;
        }

        transform.rotation = Quaternion.Euler(0, turnSpeed * turnDir * Time.deltaTime, 0) * transform.rotation;
    }
}