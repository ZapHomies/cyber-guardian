using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianBossProjectile : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Vector2 velocity = new Vector2(-8f, 0f);
        public int damage = 12;
        public float lifetime = 4f;
        public string damageSource = "Boss packet attack";

        private void Update()
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            if (velocity.sqrMagnitude > 0.01f)
            {
                transform.right = velocity.normalized;
            }

            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<CyberGuardianBossShieldBlock>() != null)
            {
                Destroy(gameObject);
            }
            else if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.DamagePlayer(damage, damageSource);
                Destroy(gameObject);
            }
            else if (!other.isTrigger && other.GetComponent<CyberGuardianBossShieldBlock>() == null)
            {
                Destroy(gameObject);
            }
        }
    }
}
