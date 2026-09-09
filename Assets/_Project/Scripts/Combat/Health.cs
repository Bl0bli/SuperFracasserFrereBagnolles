using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(StatBlock))]
    public class Health : MonoBehaviour
    {
        [SerializeField] private StatBlock _stats;
        [SerializeField] private Actor _actor;
        [SerializeField] private Knockback _knockback;

        [Tooltip("Duree pendant laquelle l'acteur ne peut plus etre touche apres un coup. " +
                 "Empeche un contact prolonge de vider les PV en une demi-seconde.")]
        [SerializeField] private float _invulnDuration = 0.35f;

        private float _current;
        private float _max;
        private float _invulnUntil;

        public event Action<DamageInfos> OnTakeDamage;
        public event Action<Actor> OnDied;

        public float Current => _current;
        public float Max => _max;
        public float Ratio => _max > 0f ? _current / _max : 0f;
        public bool IsAlive => _current > 0f;
        public bool IsInvulnerable => Time.time < _invulnUntil;

        private void Awake()
        {
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_actor == null) _actor = GetComponent<Actor>();
            if (_knockback == null) _knockback = GetComponent<Knockback>();
            ResetToMax();
        }
        
        public void ResetToMax()
        {
            _max = _stats.Get(StatType.MaxHealth);
            _current = _max;
            _invulnUntil = 0f;

            if (_max <= 0f)
            {
                Debug.LogError("[Health] MaxHealth vaut 0 sur " + name + " : CharacterStats non assigne ?", this);
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            _current = Mathf.Min(_current + amount, _max);
        }

        public void TakeDamage(DamageInfos infos)
        {
            if (!IsAlive || IsInvulnerable) return;

            _current -= infos.Amount;
            _invulnUntil = Time.time + _invulnDuration;
            
            if (_knockback != null) _knockback.Apply(infos.Direction, infos.Knockback);

            OnTakeDamage?.Invoke(infos);

            if (_current <= 0f)
            {
                _current = 0f;
                OnDied?.Invoke(_actor);
            }
        }
    }
}
