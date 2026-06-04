using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianSwingingTrap : MonoBehaviour
    {
        public float angle = 34f;
        public float speed = 1.25f;
        public float phase;

        private void Update()
        {
            float swing = Mathf.Sin((Time.time + phase) * speed) * angle;
            transform.localRotation = Quaternion.Euler(0f, 0f, swing);
        }
    }
}
