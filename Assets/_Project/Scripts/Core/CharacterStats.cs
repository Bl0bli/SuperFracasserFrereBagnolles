using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("Vitesse maximale, en unites par seconde.")]
        [Range(1, 30)] public float MoveSpeed = 8f;

        [Tooltip("Gain de vitesse par seconde tant que le stick est pousse. " +
                 "Plus c'est haut, plus le demarrage est sec.")]
        [Range(1, 100)] public float Acceleration = 30f;

        [Tooltip("Perte de vitesse par seconde quand le stick est relache. " +
                 "Bas = le vehicule roule longtemps sur son erre.")]
        [Range(1, 100)] public float Deceleration = 20f;

        [Tooltip("Part de la vitesse perpendiculaire a la direction voulue annulee a chaque pas " +
                 "de physique. 1 = changement de cap instantane, 0.1 = patinoire.")]
        [Range(0f, 1f)] public float Grip = 0.2f;

        [Tooltip("Vitesse de rotation du sprite vers la direction voulue, en degres par seconde. " +
                 "Purement visuel : n'influence pas la trajectoire.")]
        [Range(1, 1440)] public float TurnRate = 720f;

        [Header("Health Stats")]
        [Range(1, 500)] public int MaxHealth = 100;

        [Header("Combat Stats")]
        [Range(1, 500)] public int Damage = 10;
        [Range(0.1f, 10)] public float AttackCooldown = 1f;
    }
}
