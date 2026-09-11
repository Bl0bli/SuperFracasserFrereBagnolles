using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(StatBlock))]
    public class Health : MonoBehaviour
    {
        [SerializeField] private Transform _offsetSlider, _offsetLifebar;
        [SerializeField] private GameObject _slider;
        [SerializeField] private GameObject _lifebar;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private Actor _actor;
        [SerializeField] private Knockback _knockback;
        [SerializeField] private ShieldController _shield;
        [SerializeField] private GameObject _fxDeath;

        [Tooltip("Duree pendant laquelle l'acteur ne peut plus etre touche apres un coup.")]
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
            if (_shield == null) _shield = GetComponent<ShieldController>();
            ResetToMax(FactionType.Car);
        }

        public void ResetToMax(FactionType faction)
        {
            _max = _stats.Get(StatType.MaxHealth);
            _current = _max;
            _invulnUntil = 0f;

            if (_max <= 0f)
            {
                Debug.LogError("[Health] MaxHealth vaut 0 sur " + name + " : CharacterStats non assigne ?", this);
            }

            if (faction == FactionType.Cthulhu)
            {
                if (_slider != null) _slider.transform.position = _offsetSlider.transform.position;
                if (_lifebar != null) _lifebar.transform.position = _offsetLifebar.transform.position;
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

            if (_shield != null && _shield.TryAbsorb(infos)) return;

            _current -= infos.Amount;
            _invulnUntil = Time.time + _invulnDuration;

            if (_knockback != null) _knockback.Apply(infos.Direction, infos.Knockback);

            OnTakeDamage?.Invoke(infos);

            if (_current <= 0f)
            {
                _current = 0f;
                OnDied?.Invoke(_actor);
                Instantiate(_fxDeath, transform.position, Quaternion.identity);
                AudioManager.Instance.PlayExplosionSound();
                gameObject.SetActive(false);
            }
        }

        public float getCurrentHealth() //retourne la vie actuelle de l'acteur pour DisplayHealth
        {
            return _current;
        }
    }
}
