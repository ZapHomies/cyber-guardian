using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianCharacterAnimator2D : MonoBehaviour
    {
        public CyberGuardianPlayerController player;
        public Rigidbody2D body;
        public Transform visualRoot;
        public Transform torso;
        public Transform head;
        public Transform cape;
        public Transform blade;
        public Transform leftArm;
        public Transform rightArm;
        public Transform leftLeg;
        public Transform rightLeg;
        public SpriteRenderer coreRenderer;
        public SpriteRenderer visorRenderer;

        private Vector3 visualBasePosition;
        private PoseState torsoBase;
        private PoseState headBase;
        private PoseState capeBase;
        private PoseState bladeBase;
        private PoseState leftArmBase;
        private PoseState rightArmBase;
        private PoseState leftLegBase;
        private PoseState rightLegBase;

        private void Awake()
        {
            if (player == null)
            {
                player = GetComponent<CyberGuardianPlayerController>();
            }

            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (visualRoot == null && player != null)
            {
                visualRoot = player.visualRoot;
            }

            visualBasePosition = visualRoot != null ? visualRoot.localPosition : Vector3.zero;
            torsoBase = Capture(torso);
            headBase = Capture(head);
            capeBase = Capture(cape);
            bladeBase = Capture(blade);
            leftArmBase = Capture(leftArm);
            rightArmBase = Capture(rightArm);
            leftLegBase = Capture(leftLeg);
            rightLegBase = Capture(rightLeg);
        }

        private void LateUpdate()
        {
            if (visualRoot == null)
            {
                return;
            }

            float horizontal = player != null ? player.HorizontalInput : 0f;
            Vector2 velocity = player != null ? player.Velocity : (body != null ? body.linearVelocity : Vector2.zero);
            bool grounded = player == null || player.IsGroundedForAnimation;
            bool boosting = player != null && player.IsBoosting;
            bool bossMode = player != null && player.InBossMode;
            float runAmount = grounded ? Mathf.Clamp01(Mathf.Abs(horizontal)) : 0f;
            float runCycle = Time.time * 13.5f;
            float idleCycle = Time.time * 3.2f;
            float stride = Mathf.Sin(runCycle);
            float idleBob = Mathf.Sin(idleCycle) * 0.018f;
            float runBob = Mathf.Abs(stride) * 0.045f;
            float airBob = grounded ? 0f : Mathf.Clamp(velocity.y * 0.006f, -0.035f, 0.035f);

            visualRoot.localPosition = visualBasePosition + Vector3.up * (idleBob + runBob * runAmount + airBob);

            float bodyLean = boosting ? -13f : (!grounded ? Mathf.Clamp(-velocity.y * 0.7f, -10f, 10f) : -4.5f * horizontal);
            Apply(torso, torsoBase, Vector3.zero, bodyLean);
            Apply(head, headBase, Vector3.up * (idleBob * 0.35f), bodyLean * 0.35f);
            Apply(cape, capeBase, Vector3.left * (boosting ? 0.10f : 0.035f * runAmount), boosting ? 11f : -8f * horizontal - 5f);

            if (grounded)
            {
                Apply(leftLeg, leftLegBase, Vector3.zero, stride * 17f * runAmount);
                Apply(rightLeg, rightLegBase, Vector3.zero, -stride * 17f * runAmount);
            }
            else
            {
                Apply(leftLeg, leftLegBase, Vector3.zero, -18f);
                Apply(rightLeg, rightLegBase, Vector3.zero, 14f);
            }

            float armSwing = grounded ? -stride * 13f * runAmount : 10f;
            Apply(leftArm, leftArmBase, Vector3.zero, armSwing - (boosting ? 8f : 0f));
            Apply(rightArm, rightArmBase, Vector3.zero, -armSwing + (bossMode ? -18f : 0f));
            Apply(blade, bladeBase, Vector3.zero, bossMode ? 10f + Mathf.Sin(Time.time * 8f) * 4f : -5f + Mathf.Sin(Time.time * 5f) * 2f);

            float pulse = 0.55f + Mathf.Sin(Time.time * (bossMode ? 10f : 6f)) * 0.45f;
            if (coreRenderer != null)
            {
                coreRenderer.color = Color.Lerp(new Color(0.05f, 0.95f, 1f, 1f), Color.white, pulse);
            }

            if (visorRenderer != null)
            {
                visorRenderer.color = Color.Lerp(new Color(0.35f, 1f, 1f, 1f), new Color(0.95f, 1f, 1f, 1f), pulse);
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
