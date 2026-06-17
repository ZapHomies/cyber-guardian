using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianRecoveryZone : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryRecover(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryRecover(other);
        }

        private void TryRecover(Collider2D other)
        {
            if (game != null && other != null && other.GetComponentInParent<CyberGuardianPlayerController>() != null)
            {
                game.FallIntoElectricRiver();
            }
        }
    }
}
