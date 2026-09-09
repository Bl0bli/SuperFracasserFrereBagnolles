using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [Header("Movement Stats")]
        [Range(1, 30)] public float MoveSpeed = 8f;
        [Range(0.1f, 50)] public float Acceleration = 20f;
        [Range(1, 360)] public float TurnRate = 180f;

        [Tooltip("Part de la vitesse laterale annulee a chaque pas de physique. " +
                 "1 = sur des rails, 0.1 = savonnette.")]
        [Range(0f, 1f)] public float Grip = 0.85f;

        [Header("Health Stats")]
        [Range(1, 500)] public int MaxHealth = 100;

        [Header("Combat Stats")]
        [Range(1, 500)] public int Damage = 10;
        [Range(0.1f, 10)] public float AttackCooldown = 1f;
    }
}
