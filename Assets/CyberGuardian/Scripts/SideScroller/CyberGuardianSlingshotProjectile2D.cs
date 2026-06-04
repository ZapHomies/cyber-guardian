using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianSlingshotProjectile2D : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHitTarget(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryHitTarget(other);
        }

        private void TryHitTarget(Collider2D other)
        {
            if (game == null || other == null || other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                return;
            }

            CyberGuardianBossShieldBlock block = other.GetComponent<CyberGuardianBossShieldBlock>();
            if (block != null)
            {
                game.ProjectileHitShieldBlock(block);
                return;
            }

            CyberGuardianBossCore boss = other.GetComponent<CyberGuardianBossCore>();
            if (boss != null)
            {
                game.ProjectileHitBoss();
                return;
            }

            if (!other.isTrigger)
            {
                game.ProjectileHitSolid();
            }
        }
    }
}
