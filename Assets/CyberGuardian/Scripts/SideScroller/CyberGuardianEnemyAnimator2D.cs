using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianEnemyAnimator2D : MonoBehaviour
    {
        public Transform visualRoot;
        public Transform torso;
        public Transform head;
        public Transform leftClaw;
        public Transform rightClaw;
        public Transform glitchTrail;
        public SpriteRenderer coreRenderer;
        public SpriteRenderer eyeRenderer;

        private Vector3 visualBasePosition;
        private PoseState torsoBase;
        private PoseState headBase;
        private PoseState leftClawBase;
        private PoseState rightClawBase;
        private PoseState glitchTrailBase;
        private float phase;

        private void Awake()
        {
            if (visualRoot == null)
            {
                CyberGuardianEnemy enemy = GetComponent<CyberGuardianEnemy>();
                visualRoot = enemy != null ? enemy.visualRoot : transform;
            }

            phase = Random.Range(0f, 6.28f);
            visualBasePosition = visualRoot != null ? visualRoot.localPosition : Vector3.zero;
            torsoBase = Capture(torso);
            headBase = Capture(head);
            leftClawBase = Capture(leftClaw);
            rightClawBase = Capture(rightClaw);
            glitchTrailBase = Capture(glitchTrail);
        }

        private void LateUpdate()
        {
            if (visualRoot == null)
            {
                return;
            }

            float wave = Mathf.Sin(Time.time * 5.6f + phase);
            float twitch = Mathf.Sin(Time.time * 12.0f + phase);
            visualRoot.localPosition = visualBasePosition + new Vector3(twitch * 0.012f, wave * 0.035f, 0f);
            Apply(torso, torsoBase, Vector3.zero, wave * 4.0f);
            Apply(head, headBase, Vector3.up * (wave * 0.018f), twitch * 4.5f);
            Apply(leftClaw, leftClawBase, Vector3.zero, -18f + wave * 12f);
            Apply(rightClaw, rightClawBase, Vector3.zero, 18f - wave * 12f);
            Apply(glitchTrail, glitchTrailBase, new Vector3(-Mathf.Abs(twitch) * 0.035f, -Mathf.Abs(wave) * 0.025f, 0f), twitch * 8f);

            float pulse = 0.55f + Mathf.Sin(Time.time * 8.0f + phase) * 0.45f;
            if (coreRenderer != null)
            {
                coreRenderer.color = Color.Lerp(new Color(1f, 0.06f, 0.48f, 0.70f), new Color(1f, 0.45f, 0.85f, 1f), pulse);
            }

            if (eyeRenderer != null)
            {
                eyeRenderer.color = Color.Lerp(new Color(1f, 0.04f, 0.23f, 1f), Color.white, pulse);
            }
        }

        private static PoseState Capture(Transform target)
        {
            if (target == null)
            {
                return default;
            }

            return new PoseState(target.localPosition, target.localRotation);
        }

        private static void Apply(Transform target, PoseState basePose, Vector3 positionOffset, float zRotationOffset)
        {
            if (target == null)
            {
                return;
            }

            target.localPosition = basePose.LocalPosition + positionOffset;
            target.localRotation = basePose.LocalRotation * Quaternion.Euler(0f, 0f, zRotationOffset);
        }

        private readonly struct PoseState
        {
            public readonly Vector3 LocalPosition;
            public readonly Quaternion LocalRotation;

            public PoseState(Vector3 localPosition, Quaternion localRotation)
            {
                LocalPosition = localPosition;
                LocalRotation = localRotation;
            }
        }
    }
}
