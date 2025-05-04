using UnityEngine;

namespace Works.Week_5.Scripts
{
    public class PlaneController : MonoBehaviour
    {
        [Header("Throttle")]
        public float acceleration = 10f;
        public float deceleration = 10f;
        public float maxSpeed = 50f;
        public float minSpeed = 0f;
        public float currentSpeed = 10f;
        
        [Header("Rotation (deg/sec)")]
        public float pitchSpeed = 45f;  
        public float rollSpeed  = 45f;
        
        
        void Update()
        {
            // throttle up/down
            if (Input.GetKey(KeyCode.LeftShift))  currentSpeed += acceleration * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftControl)) currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

            // forward motion
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

            // pitch & roll
            float pitch = Input.GetAxis("Vertical")   * pitchSpeed * Time.deltaTime;
            float roll  = -Input.GetAxis("Horizontal") * rollSpeed  * Time.deltaTime;
            transform.Rotate(pitch, 0f, roll, Space.Self);
        }
        
        
    }
}