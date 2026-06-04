using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianRecoveryZone : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.RecoverPlayerFromAbyss();
            }
        }
    }
}
