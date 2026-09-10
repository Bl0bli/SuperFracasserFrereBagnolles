using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatBlock : MonoBehaviour
    {
        [SerializeField] private CharacterStats _baseStats;

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        public IReadOnlyList<StatModifier> Modifiers => _modifiers;

        private void Awake()
        {
        }

        public void SetBaseStats(CharacterStats stats)
        {
            if (stats == null)
            {
                Debug.LogError("[StatBlock] SetBaseStats appele avec null sur " + name + ".", this);
                return;
            }

            _baseStats = stats;
        }

        public float Get(StatType stat)
        {
            float additive = 0f;
            float factor = 1f;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                StatModifier m = _modifiers[i];
                if (m.Stats != stat) continue;

                if (m.Modifier == ModifierMode.Additive)
                {
                    additive += m.Value;
                }
                else
                {
                    factor *= m.Value;
                }
            }

            return (GetBaseStat(stat) + additive) * factor;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(object source)
        {
            if (source == null) return;

            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(_modifiers[i].Source, source)) continue;

                StatType stat = _modifiers[i].Stats;

                _modifiers.RemoveAt(i);
            }
        }

        public bool HasModifierFrom(object source)
        {
            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (ReferenceEquals(_modifiers[i].Source, source)) return true;
            }

            return false;
        }

        private void Update()
        {
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                StatModifier m = _modifiers[i];

                if (m.IsPermanent) continue;

                m.Duration -= Time.deltaTime;

                if (m.Duration <= 0f)
                {
                    _modifiers.RemoveAt(i);
                }
                else
                {
                    _modifiers[i] = m;
                }
            }
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
                case StatType.Mass:
                    return _baseStats.Mass;
                case StatType.LinearDamping:
                    return _baseStats.LinearDamping;
                case StatType.RamSpeedThreshold:
                    return _baseStats.RamSpeedThreshold;
                case StatType.RamDamage:
                    return _baseStats.RamDamage;
                case StatType.KnockbackForce:
                    return _baseStats.KnockbackForce;
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
    }
}
