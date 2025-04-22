using UnityEngine;

public class TurnScript : MonoBehaviour
{
    public float turnSpeed = 100.0f;

    private InputSystem_Actions controls;
    void Start()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }

    void Update()
    {
        float h = controls.Player.Move.ReadValue<Vector2>().x;
        transform.rotation *= Quaternion.AngleAxis(h * turnSpeed * Time.deltaTime, Vector3.up);
    }
}