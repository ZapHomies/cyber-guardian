using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianDamageZone : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public int damage = 15;
        public bool destroyOnHit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void TryDamage(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.DamagePlayer(damage, name);
                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
