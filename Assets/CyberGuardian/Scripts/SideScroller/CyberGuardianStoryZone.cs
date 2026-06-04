using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianStoryZone : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public string storyTitle = "SECTOR";
        [TextArea(2, 4)] public string storyBody = "Data route unlocked.";
        public float duration = 4.2f;
        public bool oneShot = true;

        private bool shown;

        private void Reset()
        {
            Collider2D zoneCollider = GetComponent<Collider2D>();
            zoneCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((oneShot && shown) || other == null || other.GetComponent<CyberGuardianPlayerController>() == null)
            {
                return;
            }

            shown = true;
            if (game != null)
            {
                game.ShowStory(storyTitle, storyBody, duration);
            }
        }
    }
}
