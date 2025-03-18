using UnityEngine;

public class SinglePlayerMovement : MonoBehaviour
{
//    public float turnSpeed = 10.0f;
//    public float moveSpeed = 1.0f;
//    private InputSystem_Actions controls;

    void Start()
    {
        /*
        controls = new InputSystem_Actions();
        controls.Enable();
        */
    }

    void Update()
    {
        /*
        float h = controls.Player.Move.ReadValue<Vector2>().x;
        float v = controls.Player.Move.ReadValue<Vector2>().y;
        transform.rotation *= Quaternion.AngleAxis(h * turnSpeed * Time.deltaTime, Vector3.up);
        transform.position += v * moveSpeed * Time.deltaTime * transform.forward;
        */
    }
}
