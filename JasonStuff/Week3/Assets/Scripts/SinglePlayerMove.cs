using UnityEngine;

public class SinglePlayerMove : MonoBehaviour
{
   // private InputSystem_Actions controls;
    public float turnSpeed = 200.0f;
    public float moveSpeed = 50.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //  controls = new InputSystem_Actions();
        // controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
      //  float horizontal = controls.Player.Move.ReadValue<Vector2>().x;
       // float vertical = controls.Player.Move.ReadValue<Vector2>().y;
      //  transform.rotation *= Quaternion.AngleAxis(horizontal * turnSpeed * Time.deltaTime, Vector3.up);
       // transform.position += vertical * moveSpeed * Time.deltaTime * transform.forward;
    }
}
