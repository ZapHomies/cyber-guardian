using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianRotator : MonoBehaviour
    {
        public float degreesPerSecond = 180f;

        private void Update()
        {
            transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
        }
    }
}
