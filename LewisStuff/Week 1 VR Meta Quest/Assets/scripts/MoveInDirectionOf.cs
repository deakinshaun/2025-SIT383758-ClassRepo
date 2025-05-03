using UnityEngine;

public class MoveInDirectionOf : MonoBehaviour
{
    [Tooltip("Controller input used to trigger movement")]
    public OVRInput.RawButton button = OVRInput.RawButton.LThumbstickUp;

    [Tooltip("Speed of movement.")]
    public float speed = 10.0f;

    [Tooltip("Object used to determine direction of travel")]
    public GameObject pointer;

    [Tooltip("Constraint vector used to filter axes of movement. The normal to the plane that movement is projected to.")]
    public Vector3 constrainDirection = new Vector3(0, 1, 0);

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(button))
        {
            this.transform.position += speed * Time.deltaTime * Vector3.ProjectOnPlane(pointer.transform.forward, constrainDirection);
        }
    }
}
