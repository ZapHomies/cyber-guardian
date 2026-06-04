using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianTurret : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public GameObject projectilePrefab;
        public Transform muzzle;
        public Vector2 direction = Vector2.left;
        public float projectileSpeed = 7.5f;
        public float fireInterval = 1.6f;
        public float initialDelay = 0.5f;

        private float fireTimer;

        private void Awake()
        {
            fireTimer = initialDelay;
        }

        private void Update()
        {
            if (projectilePrefab == null || game == null)
            {
                return;
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f)
            {
                return;
            }

            fireTimer = fireInterval;
            Vector3 spawnPosition = muzzle != null ? muzzle.position : transform.position + (Vector3)(direction.normalized * 0.45f);
            GameObject shot = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            CyberGuardianBossProjectile projectile = shot.GetComponent<CyberGuardianBossProjectile>();
            if (projectile != null)
            {
                projectile.game = game;
                projectile.velocity = direction.normalized * projectileSpeed;
                projectile.damage = 9;
                projectile.lifetime = 3.2f;
            }
        }
    }
}
