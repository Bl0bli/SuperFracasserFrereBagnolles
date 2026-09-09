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
                 "de physique. 1 = changement de cap instantane et resistance aux poussees, " +
                 "0.1 = patinoire.")]
        [Range(0f, 1f)] public float Grip = 0.2f;

        [Tooltip("Vitesse de rotation du sprite vers la direction voulue, en degres par seconde. " +
                 "Purement visuel : n'influence pas la trajectoire.")]
        [Range(1, 1440)] public float TurnRate = 720f;

        [Header("Physics")]
        [Tooltip("Masse du Rigidbody2D, appliquee au changement de role. " +
                 "C'est le rapport des masses qui decide qui pousse qui.")]
        [Range(0.1f, 50f)] public float Mass = 1f;

        [Tooltip("Frottement passif. Haut = tout elan parasite meurt vite.")]
        [Range(0f, 10f)] public float LinearDamping = 0.5f;

        [Header("Degats au contact")]
        [Tooltip("Fraction de MoveSpeed a depasser pour que le contact blesse. " +
                 "0.5 = il faut rouler a au moins la moitie de sa vitesse max.")]
        [Range(0f, 1f)] public float RamSpeedThreshold = 0.5f;

        [Tooltip("Degats infliges au contact une fois le seuil franchi. " +
                 "Quantite FIXE : au-dela du seuil, plus vite ne fait pas plus mal.")]
        [Range(0, 200)] public int RamDamage = 10;

        [Tooltip("Impulsion de recul transmise a la victime. " +
                 "La vitesse reellement communiquee vaut cette force divisee par la masse de la victime.")]
        [Range(0f, 60f)] public float KnockbackForce = 6f;

        [Header("Health Stats")]
        [Range(1, 500)] public int MaxHealth = 100;

        [Header("Combat Stats")]
        [Range(1, 500)] public int Damage = 10;
        [Range(0.1f, 10)] public float AttackCooldown = 1f;
    }
}
