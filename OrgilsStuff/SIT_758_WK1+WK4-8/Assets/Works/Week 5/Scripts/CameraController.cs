using UnityEngine;

namespace Works.Week_5.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;    

        [Header("Position")]
        public Vector3 offset = new Vector3(0f, 5f, -10f);
        public float positionSmoothTime = 0.2f;
        private Vector3 positionVelocity = Vector3.zero;

        [Header("Rotation")]
        public float rotationSmoothTime = 0.1f;
        private Vector3 currentRotation;
        private Vector3 rotationVelocity = Vector3.zero;

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desiredPos = target.TransformPoint(offset);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref positionVelocity,
                positionSmoothTime
            );
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion desiredRot = Quaternion.LookRotation(directionToTarget, Vector3.up);

            Vector3 desiredEuler = desiredRot.eulerAngles;
            currentRotation = Vector3.SmoothDamp(
                currentRotation,
                desiredEuler,
                ref rotationVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }
}