using UnityEngine;

namespace CyberGuardian
{
    [CreateAssetMenu(menuName = "Cyber Guardian/Difficulty Profile", fileName = "DifficultyProfile")]
    public sealed class DifficultyProfile : ScriptableObject
    {
        public string displayName = "Normal";
        [Min(15f)] public float startingTime = 87f;
        [Min(0)] public int startingScore = 100;
        [Min(0)] public int startingTokens = 15;
        [Range(1, 100)] public int startingShield = 100;
        [Range(0, 100)] public int startingVirusStrength = 20;
        [Min(1)] public int requiredRouteOrbs = 7;

        [Header("Correct Answer Reward")]
        [Min(0)] public int correctScoreReward = 75;
        [Min(0)] public int correctTokenReward = 1;
        [Min(0)] public int correctShieldReward = 6;

        [Header("Wrong Answer Penalty")]
        [Min(0)] public int wrongShieldDamage = 15;
        [Min(0)] public int wrongVirusGain = 12;

        [Header("Submit Route Reward")]
        [Min(0)] public int routeScoreReward = 250;
        [Min(0)] public int routeVirusDamage = 30;
    }
}
