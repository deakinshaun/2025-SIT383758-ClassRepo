using System.Collections;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class AvatarControl : NetworkBehaviour
{
    public float moveSpeed = 1.0f;
    public float turnSpeed = 100.0f;

    public GameObject blobTemplate;
    public GameObject cameraObject;

    private AudioSource hornSFXsource;
    public override void Spawned()
    {
        hornSFXsource = GetComponent<AudioSource>();
        if (Object.HasInputAuthority)
        {
            cameraObject.SetActive(true);
        }
        else
        {
            cameraObject.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputNetworkData move))
        {
            transform.rotation *= Quaternion.AngleAxis (move.turnAmount * turnSpeed * Runner.DeltaTime, Vector3.up);
            transform.position += move.forwardAmount * moveSpeed * Runner.DeltaTime * transform.forward;
            
            if (move.honk)
            {
                if (!hornSFXsource.isPlaying)
                {
                    hornSFXsource.Play();
                }
            }
            else
            {
                hornSFXsource.Stop();
            }
        }
        base.FixedUpdateNetwork();
    }
}
