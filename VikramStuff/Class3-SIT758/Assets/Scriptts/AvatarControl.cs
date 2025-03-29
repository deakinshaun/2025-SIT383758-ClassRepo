using UnityEngine;
using Fusion;
public class AvatarControl : NetworkBehaviour
{
    private float _turnSpeed = 80;
    private float _moveSpeed = 3;
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out InputNetworkData inputNetworkData))
        {
            Debug.Log("Got Input");
            transform.rotation *= Quaternion.AngleAxis(inputNetworkData.turnAmount * _turnSpeed * Time.deltaTime, Vector3.up);
            transform.position += inputNetworkData.moveAmount * _moveSpeed * Time.deltaTime * transform.forward;

        }
    }
}
