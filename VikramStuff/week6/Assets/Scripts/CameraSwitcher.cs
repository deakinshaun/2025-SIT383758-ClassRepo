using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraSwitcher : MonoBehaviour
{
    public ARCameraManager cameraManager;

    private void Start()
    {
        // Set a default facing direction
        cameraManager.requestedFacingDirection = CameraFacingDirection.World;
    }

    public void ToggleCamera()
    {
        if (cameraManager.requestedFacingDirection == CameraFacingDirection.World)
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
        }
        else
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.World;
        }
    }
}
