using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/CardEffect/StatModifierEffect")]
    
    public class StatModifierEffect : CardEffect
    {
        public StatType Stats;
        public ModifierMode Mode;
        public override void Apply(Actor target)
        {
            StatModifier modifier = new StatModifier();
            modifier.Duration = _duration;
            modifier.Value = _power;
            modifier.Modifier = Mode;
            modifier.Stats = Stats;
            
            target.Stats.AddModifier(modifier);
        }
    }
}
