using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianCheckpoint : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Transform recoveryPoint;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                Vector3 point = recoveryPoint != null ? recoveryPoint.position : transform.position;
                game.SetRecoveryPoint(point);
            }
        }
    }
}
