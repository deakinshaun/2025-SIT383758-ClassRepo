using UnityEngine;
using Fusion;

public class AvatarControl : NetworkBehaviour
{
    public float turnSpeed = 10.0f;
    public float moveSpeed = 1.0f;

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (GetInput (out InputNetworkData inputNetworkData))
        {
            // Debug.Log("Got input");
            transform.rotation *= Quaternion.AngleAxis(inputNetworkData.turnAmount * turnSpeed  * Time.deltaTime, Vector3.up);
            transform.position += inputNetworkData.moveAmount * moveSpeed * Time.deltaTime * transform.forward;
        }
    }
}
