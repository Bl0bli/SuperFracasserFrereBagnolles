using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/CardEffect/StatModifierEffect")]
    public class StatModifierEffect : CardEffect
    {
        [Header("Modificateur")]
        public StatType Stats;
        public ModifierMode Mode;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.Stats == null)
            {
                Debug.LogWarning("[StatModifierEffect] " + target.name + " n'a pas de StatBlock.", target);
                return;
            }

            StatModifier modifier = new StatModifier
            {
                Stats = Stats,
                Modifier = Mode,
                Value = _power,
                Duration = _duration,
                Source = this
            };

            target.Stats.AddModifier(modifier);
        }
    }
}
