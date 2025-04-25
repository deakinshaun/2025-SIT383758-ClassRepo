using UnityEngine;

public class TurnScript : MonoBehaviour
{
    private InputSystem_Actions controls;
    private float turnSpeed=100f;

    void Start()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float h = controls.Player.Move.ReadValue<Vector2>().x;
        transform.rotation *= Quaternion.AngleAxis(h*turnSpeed*Time.deltaTime, Vector3.up);
    }
}
