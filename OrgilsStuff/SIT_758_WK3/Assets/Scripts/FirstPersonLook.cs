using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple FPP (First Person Perspective) camera rotation script.
/// Like those found in most FPS (First Person Shooter) games.
/// </summary>
public class FirstPersonLook : NetworkBehaviour {

    public float Sensitivity {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    [Range(0f, 90f)][SerializeField] float xRotationLimit = 88f;
    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; 
    const string yAxis = "Mouse Y";

    [SerializeField] private InputActionAsset inputActionMap;
    [SerializeField] private Transform cameraTf;
    InputAction lookAction;
    private void Start()
    {
        lookAction = inputActionMap.FindAction("Player/Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        
        if(!Object.HasInputAuthority) return;
        
        rotation.x += lookAction.ReadValue<Vector2>().x * sensitivity;
        rotation.y += lookAction.ReadValue<Vector2>().y * sensitivity;
        
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        rotation.x = Mathf.Clamp(rotation.x, -xRotationLimit, xRotationLimit);
        
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuat;
        cameraTf.localRotation = yQuat;
    }
}