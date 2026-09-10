using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/CardEffect/DamageEffect")]
    public class DamageEffect : CardEffect
    {
        [Header("Degats")]
        [SerializeField, Min(0)] private int _damage = 20;

        [Tooltip("Impulsion de recul. La vitesse communiquee vaut cette force divisee " +
                 "par la masse de la cible.")]
        [SerializeField, Min(0f)] private float _knockback = 0f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.Health == null || !target.Health.IsAlive) return;
            
            Vector2 direction = target.transform.position - collector.transform.position;
            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;

            target.Health.TakeDamage(new DamageInfos
            {
                Amount = _damage,
                Type = DamageType.Melee,
                Source = collector,
                Direction = direction.normalized,
                Knockback = _knockback
            });
        }
    }
}
