using UnityEngine;

public class SinglePlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;


    [SerializeField] private float turnSpeed;
    [SerializeField] private float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new();
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float playerMoveX = controls.Player.Move.ReadValue<Vector2>().x;
        float playerMoveY = controls.Player.Move.ReadValue<Vector2>().y;
        transform.rotation *= Quaternion.AngleAxis(playerMoveX * turnSpeed * Time.deltaTime, Vector3.up);
        transform.position += playerMoveY * moveSpeed * Time.deltaTime * transform.forward;
    }
}