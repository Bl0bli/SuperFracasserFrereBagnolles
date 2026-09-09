using UnityEngine;

namespace Game
{
    public struct StatModifier
    {
        public StatType Stats;
        public ModifierMode Modifier;
        public float Value;
        public float Duration;
        //public Actor Source ? TODO implémenter la source
    }
}
