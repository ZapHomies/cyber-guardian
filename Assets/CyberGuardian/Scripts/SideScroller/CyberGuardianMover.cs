using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianMover : MonoBehaviour
    {
        public Vector3 localOffset = new Vector3(0f, -1.5f, 0f);
        public float speed = 1f;
        public float pauseTime = 0.25f;

        private Vector3 origin;
        private float waitTimer;

        private void Awake()
        {
            origin = transform.position;
        }

        private void Update()
        {
            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            float t = Mathf.PingPong(Time.time * speed, 1f);
            Vector3 target = Vector3.Lerp(origin, origin + localOffset, Mathf.SmoothStep(0f, 1f, t));
            if ((transform.position - target).sqrMagnitude < 0.0004f && (t < 0.02f || t > 0.98f))
            {
                waitTimer = pauseTime;
            }

            transform.position = target;
        }
    }
}
