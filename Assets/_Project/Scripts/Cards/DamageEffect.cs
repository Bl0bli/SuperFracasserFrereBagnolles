using UnityEngine;

namespace Game
{

    [CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/CardEffect/DamageEffect")]
    public class DamageEffect : CardEffect
    {
        [Header("Degats")]
        [SerializeField] private float _knockback = 0f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.Health == null || !target.Health.IsAlive) return;

            Vector2 direction = target.transform.position - collector.transform.position;
            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;

            target.Health.TakeDamage(new DamageInfos
            {
                Amount = Mathf.RoundToInt(_power),
                Type = DamageType.Melee,
                Source = collector,
                Direction = direction.normalized,
                Knockback = _knockback
            });
        }
    }
}
