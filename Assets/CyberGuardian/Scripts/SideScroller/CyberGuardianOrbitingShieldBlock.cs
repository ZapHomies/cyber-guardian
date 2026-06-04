using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianOrbitingShieldBlock : MonoBehaviour
    {
        public Transform center;
        public float radius = 3f;
        public float angularSpeed = 28f;
        public float phaseDegrees;
        public float verticalSquash = 0.72f;
        public float pulseAmplitude = 0.12f;
        public float pulseSpeed = 2.2f;

        private Vector3 baseScale;

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private void Update()
        {
            if (center == null)
            {
                return;
            }

            float angle = (phaseDegrees + Time.time * angularSpeed) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius * verticalSquash, 0f);
            transform.position = center.position + offset;

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + phaseDegrees * Mathf.Deg2Rad) * pulseAmplitude;
            transform.localScale = baseScale * pulse;
        }
    }
}
