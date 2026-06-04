using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianEnemy : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public int health = 2;
        public int touchDamage = 10;
        public float speed = 1.8f;
        public float patrolDistance = 2.4f;
        public Transform visualRoot;

        private SpriteRenderer spriteRenderer;
        private Vector3 origin;
        private Vector3 baseVisualScale;
        private int direction = 1;

        private void Awake()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            spriteRenderer = visualRoot.GetComponentInChildren<SpriteRenderer>();
            origin = transform.position;
            baseVisualScale = visualRoot.localScale;
        }

        private void Update()
        {
            transform.position += Vector3.right * direction * speed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x - origin.x) > patrolDistance)
            {
                direction *= -1;
                if (visualRoot != null)
                {
                    visualRoot.localScale = new Vector3(Mathf.Abs(baseVisualScale.x) * direction, baseVisualScale.y, baseVisualScale.z);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.35f, 0.40f, 1f);
                CancelInvoke(nameof(RestoreColor));
                Invoke(nameof(RestoreColor), 0.12f);
            }

            if (health <= 0)
            {
                if (game != null)
                {
                    game.EnemyDefeated(this);
                }

                Destroy(gameObject);
            }
        }

        private void RestoreColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (game != null && collision.collider.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.DamagePlayer(touchDamage, "Enemy contact");
            }
        }
    }
}
