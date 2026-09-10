using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct StatModifier
    {
        public StatType Stats;
        public ModifierMode Modifier;
        public float Value;
        public float Duration;
        public object Source; //attention cascade effectuée par un professionnel, à ne pas reproduire Yanis

        public bool UpdateDuration()
        {
            if (Duration > 0f)
            {
                Duration -= Time.deltaTime;
                return true;
            }

            return false;
        }
    }
}
