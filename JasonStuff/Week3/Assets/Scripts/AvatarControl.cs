using UnityEngine;
using Fusion;

public class AvatarControl : NetworkBehaviour
{
    public float turnSpeed = 200.0f;
    public float moveSpeed = 50.0f;

    public override void FixedUpdateNetwork()

    {
         if (GetInput (out InputNetworkData inputNetworkData))
        {
           // Debug.Log("Input Received");
            transform.rotation *= Quaternion.AngleAxis(inputNetworkData.turnAmount * turnSpeed * Time.deltaTime, Vector3.up);
            transform.position += inputNetworkData.moveAmount * moveSpeed * Time.deltaTime * transform.forward;
        }
    }
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
