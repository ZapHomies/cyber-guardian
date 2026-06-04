using UnityEngine;

namespace CyberGuardian
{
    public enum CyberGuardianPowerUpType
    {
        Health,
        Boost,
        Firewall,
        Overclock
    }

    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianPowerUp : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public CyberGuardianPowerUpType type = CyberGuardianPowerUpType.Boost;
        public int amount = 25;

        private bool collected;

        private void Reset()
        {
            Collider2D powerCollider = GetComponent<Collider2D>();
            powerCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || other == null || other.GetComponent<CyberGuardianPlayerController>() == null)
            {
                return;
            }

            collected = true;
            if (game != null)
            {
                game.ApplyPowerUp(this);
            }

            gameObject.SetActive(false);
        }
    }
}
