using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatBlock : MonoBehaviour
    {
        [SerializeField] private CharacterStats _baseStats;
        
        private List<StatModifier> _modifiers = new List<StatModifier>();

        public float Get(StatType stat)
        {
            float additive = 0f;
            float multiplier = 0f;

            foreach (StatModifier m in _modifiers)
            {
                if (m.Stats != stat) continue;

                if (m.Modifier == ModifierMode.Additive)
                {
                    additive += m.Value;
                }
                else
                {
                    multiplier += m.Value;
                }
            }
            
            return (GetBaseStat(stat) + additive) * (1f + multiplier);
        }

        private float GetBaseStat(StatType stat)
        {
            if (_baseStats == null)
            {
                Debug.LogError("[StatBlock] Aucun CharacterStats assigne sur " + name + ".", this);
                return 0f;
            }

            switch (stat)
            {
                case StatType.NULL:
                    return 0f;
                case StatType.MoveSpeed:
                    return _baseStats.MoveSpeed;
                case StatType.Acceleration:
                    return _baseStats.Acceleration;
                case StatType.Deceleration:
                    return _baseStats.Deceleration;
                case StatType.TurnRate:
                    return _baseStats.TurnRate;
                case StatType.Grip:
                    return _baseStats.Grip;
                case StatType.MaxHealth:
                    return _baseStats.MaxHealth;
                case StatType.Damage:
                    return _baseStats.Damage;
                case StatType.AttackCooldown:
                    return _baseStats.AttackCooldown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            //TODO implementer (couche cartes, jour 3)
        }

        public void RemoveModifier(StatModifier modifier)
        {
            //TODO implementer (couche cartes, jour 3)
        }
    }
}
