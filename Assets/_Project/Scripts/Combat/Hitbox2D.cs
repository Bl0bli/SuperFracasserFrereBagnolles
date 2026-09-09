using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Hitbox2D : MonoBehaviour
    {
        [Header("Proprietaire")]
        [SerializeField] private Actor _owner;

        [Header("Degats")]
        [Tooltip("Coche : les degats viennent de StatType.Damage du proprietaire, " +
                 "donc les cartes qui modifient cette stat s'appliquent aussi a cette attaque.")]
        [SerializeField] private bool _useOwnerDamageStat = true;
        [SerializeField] private int _fixedAmount = 10;
        [SerializeField] private DamageType _type = DamageType.Melee;
        [SerializeField] private float _knockback = 5f;

        [Header("Activation")]
        [Tooltip("Etat du collider au demarrage. Decoche pour une attaque declenchee.")]
        [SerializeField] private bool _startActive = false;

        [Tooltip("Une meme cible ne peut etre touchee qu'une fois par activation. " +
                 "Decoche pour une zone qui frappe a chaque entree.")]
        [SerializeField] private bool _oneHitPerActivation = true;

        private Collider2D _collider;
        private readonly HashSet<Health> _alreadyHit = new HashSet<Health>();

        /// <summary>Emis apres que les degats ont ete infliges : pour les VFX et le son.</summary>
        public event Action<Health, DamageInfos> OnHit;

        public bool IsActive => _collider != null && _collider.enabled;
        public Actor Owner => _owner;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            if (_collider == null)
            {
                Debug.LogError("[Hitbox2D] Aucun Collider2D sur " + name + ".", this);
                enabled = false;
                return;
            }

            if (!_collider.isTrigger)
            {
                Debug.LogWarning("[Hitbox2D] Le collider de " + name + " n'etait pas en trigger.", this);
                _collider.isTrigger = true;
            }

            if (_owner == null) _owner = GetComponentInParent<Actor>();
            if (_owner == null)
            {
                Debug.LogError("[Hitbox2D] Aucun Actor trouve sur les parents de " + name + ".", this);
                enabled = false;
                return;
            }

            _collider.enabled = _startActive;
        }
        
        public void Enable()
        {
            CancelInvoke(nameof(Disable));
            _alreadyHit.Clear();
            if (_collider != null) _collider.enabled = true;
        }

        public void Enable(float duration)
        {
            Enable();
            Invoke(nameof(Disable), duration);
        }

        public void Disable()
        {
            if (_collider != null) _collider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor target = other.GetComponentInParent<Actor>();
            if (target == null || target == _owner) return;
            if (target.Faction == _owner.Faction) return; //on tape pas les collegues (pour le moment)

            Health health = target.Health;
            if (health == null || !health.IsAlive) return;

            if (_oneHitPerActivation && !_alreadyHit.Add(health)) return;

            DamageInfos infos = BuildDamage(target);
            health.TakeDamage(infos);
            OnHit?.Invoke(health, infos);
        }

        private DamageInfos BuildDamage(Actor target)
        {
            int amount = _fixedAmount;
            if (_useOwnerDamageStat && _owner.Stats != null)
            {
                amount = Mathf.RoundToInt(_owner.Stats.Get(StatType.Damage));
            }

            Vector2 delta = target.transform.position - transform.position;
            Vector2 direction = delta.sqrMagnitude > 0.0001f ? delta.normalized : (Vector2)transform.up;

            return new DamageInfos
            {
                Amount = amount,
                Type = _type,
                Source = _owner,
                Direction = direction,
                Knockback = _knockback
            };
        }
    }
}
