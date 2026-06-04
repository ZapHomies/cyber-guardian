using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianBossArenaTrigger : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.EnterBossMode();
                gameObject.SetActive(false);
            }
        }
    }
}
