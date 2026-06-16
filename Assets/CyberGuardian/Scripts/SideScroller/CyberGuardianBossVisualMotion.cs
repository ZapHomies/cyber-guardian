using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianBossVisualMotion : MonoBehaviour
    {
        public Vector2 amplitude = new Vector2(0.16f, 0.06f);
        public float speed = 1.2f;
        public float phase;
        public float tiltDegrees = 2.0f;
        public bool liftOnly;

        private Vector3 baseLocalPosition;
        private Quaternion baseLocalRotation;

        private void OnEnable()
        {
            baseLocalPosition = transform.localPosition;
            baseLocalRotation = transform.localRotation;
        }

        private void Update()
        {
            float time = Time.time * speed + phase;
            float horizontal = Mathf.Sin(time);
            float verticalWave = Mathf.Cos(time * 1.17f);
            float vertical = liftOnly ? Mathf.Abs(verticalWave) : verticalWave;
            transform.localPosition = baseLocalPosition + new Vector3(horizontal * amplitude.x, vertical * amplitude.y, 0f);
            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, horizontal * tiltDegrees);
        }
    }
}
