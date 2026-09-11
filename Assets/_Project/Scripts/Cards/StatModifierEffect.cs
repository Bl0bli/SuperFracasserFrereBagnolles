using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/CardEffect/StatModifierEffect")]
    public class StatModifierEffect : CardEffect
    {
        [Header("Modificateur")]
        [Tooltip("Statistique touchee.")]
        [FormerlySerializedAs("Stats")]
        [SerializeField] private StatType _stat = StatType.MoveSpeed;

        [Tooltip("Additive : la valeur s'ajoute a la stat de base. " +
                 "Multiply : la valeur est un facteur, 0.2 divise la stat par 5, " +
                 "1.5 l'augmente de moitie. Ne jamais laisser 0 en Multiply.")]
        [FormerlySerializedAs("Mode")]
        [SerializeField] private ModifierMode _mode = ModifierMode.Multiply;

        [FormerlySerializedAs("_power")]
        [SerializeField] private float _value = 0.5f;

        [Tooltip("Duree en secondes. Une carte ne pose jamais de modificateur permanent : " +
                 "une valeur nulle est ramenee au minimum.")]
        [SerializeField, Min(0.1f)] private float _duration = 5f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.Stats == null)
            {
                Debug.LogWarning("[StatModifierEffect] " + target.name + " n'a pas de StatBlock.", target);
                return;
            }
            
            target.Stats.AddModifier(new StatModifier
            {
                Stats = _stat,
                Modifier = _mode,
                Value = _value,
                Duration = Mathf.Max(0.1f, _duration),
                Source = this
            });
        }

        public override float RemainingOn(Actor target)
        {
            if (target == null || target.Stats == null) return 0f;

            float remaining = 0f;
            var modifiers = target.Stats.Modifiers;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (ReferenceEquals(modifiers[i].Source, this)) remaining = Mathf.Max(remaining, modifiers[i].Duration);
            }

            return remaining;
        }
    }
}
