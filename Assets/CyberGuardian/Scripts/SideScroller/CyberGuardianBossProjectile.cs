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

        private void Update()
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.DamagePlayer(damage, "Boss packet attack");
                Destroy(gameObject);
            }
            else if (!other.isTrigger && other.GetComponent<CyberGuardianBossShieldBlock>() == null)
            {
                Destroy(gameObject);
            }
        }
    }
}
