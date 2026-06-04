using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianPlayerProjectile2D : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Vector2 velocity = new Vector2(12f, 0f);
        public int damage = 1;
        public float lifetime = 1.45f;

        private void Update()
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            transform.Rotate(0f, 0f, -560f * Time.deltaTime);
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null || other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                return;
            }

            CyberGuardianEnemy enemy = other.GetComponent<CyberGuardianEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            CyberGuardianBossShieldBlock block = other.GetComponent<CyberGuardianBossShieldBlock>();
            if (block != null && game != null)
            {
                game.PlayerProjectileHitShieldBlock(block);
                Destroy(gameObject);
                return;
            }

            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
