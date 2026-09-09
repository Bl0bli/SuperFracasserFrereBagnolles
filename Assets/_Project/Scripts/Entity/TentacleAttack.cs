using System;
using UnityEngine;

namespace Game
{
    public class TentacleAttack : MonoBehaviour
    {
        [SerializeField] private Hitbox2D _hitbox;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private StatusEffectController _status;

        [Tooltip("Duree pendant laquelle la zone reste ouverte, en secondes.")]
        [SerializeField, Range(0.05f, 2f)] private float _swingDuration = 0.25f;

        private float _nextAttackTime;

        //Point d'accroche pour l'animation, le son et les VFX
        public event Action OnAttack;

        public float CooldownRemaining => Mathf.Max(0f, _nextAttackTime - Time.time);
        public bool IsSilenced => _status != null && _status.Has(StatusType.Silenced);
        public bool CanAttack => Time.time >= _nextAttackTime && !IsSilenced;

        private void Awake()
        {
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_status == null) _status = GetComponent<StatusEffectController>();
            if (_hitbox == null) _hitbox = GetComponentInChildren<Hitbox2D>(true);

            if (_hitbox == null)
            {
                Debug.LogError("[TentacleAttack] Aucune Hitbox2D dans les enfants de " + name + ".", this);
                enabled = false;
            }
        }

        public bool TryAttack()
        {
            if (!CanAttack) return false;

            _nextAttackTime = Time.time + _stats.Get(StatType.AttackCooldown);
            _hitbox.Enable(_swingDuration);
            OnAttack?.Invoke();
            return true;
        }
    }
}
