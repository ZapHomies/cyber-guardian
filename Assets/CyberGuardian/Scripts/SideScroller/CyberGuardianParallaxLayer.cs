using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianParallaxLayer : MonoBehaviour
    {
        public Camera targetCamera;
        public Vector2 factor = new Vector2(0.18f, 0.08f);

        private Vector3 startPosition;
        private Vector3 cameraStartPosition;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            startPosition = transform.position;
            cameraStartPosition = targetCamera != null ? targetCamera.transform.position : Vector3.zero;
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                return;
            }

            Vector3 cameraDelta = targetCamera.transform.position - cameraStartPosition;
            transform.position = startPosition + new Vector3(cameraDelta.x * factor.x, cameraDelta.y * factor.y, 0f);
        }
    }
}
