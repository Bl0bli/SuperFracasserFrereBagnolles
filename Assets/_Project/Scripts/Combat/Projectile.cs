using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IHazard
    {
        [Header("Lancer")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField, Min(0f)] private float _speed = 14f;
        [SerializeField, Min(0f)] private float _lifetime = 6f;

        [Tooltip("Nombre de rebonds avant destruction. 0 = disparait au premier contact.")]
        [SerializeField, Min(0)] private int _maxBounces = 0;

        [Header("Degats")]
        [Tooltip("Coche : les degats viennent de StatType.Damage du lanceur.")]
        [SerializeField] private bool _useOwnerDamageStat = true;
        [SerializeField, Min(0)] private int _damage = 10;

        [Tooltip("Multiplie la stat Damage du lanceur. Un pneu frappe plus fort qu'un coup de tentacule.")]
        [SerializeField, Min(0f)] private float _damageMultiplier = 1f;
        [SerializeField, Min(0f)] private float _knockback = 8f;

        [Header("Proprietaire")]
        [Tooltip("Coche : le camp du lanceur detruit le projectile en le touchant, sans degats.")]
        [SerializeField] private bool _crushedByOwnerFaction = true;

        [Tooltip("Duree pendant laquelle la collision avec le lanceur est desactivee.")]
        [SerializeField, Min(0f)] private float _ownerGrace = 0.3f;

        private readonly List<Collider2D> _ownerColliders = new List<Collider2D>();
        private Collider2D[] _myColliders;
        private Actor _owner;
        private int _bouncesLeft;
        private float _deathTime;
        private float _graceUntil;
        private bool _restored;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            _myColliders = GetComponentsInChildren<Collider2D>(true);
            _bouncesLeft = _maxBounces;
            _deathTime = Time.time + _lifetime;
        }

        public void Initialize(Actor owner)
        {
            Launch(owner, Random.insideUnitCircle.normalized);
        }

        public void Launch(Actor owner, Vector2 direction)
        {
            _owner = owner;
            _bouncesLeft = _maxBounces;
            _deathTime = Time.time + _lifetime;
            _restored = false;

            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;

            _rb.linearVelocity = direction.normalized * _speed;

            if (_owner == null)
            {
                Debug.LogError("[Projectile] " + name + " lance SANS proprietaire : il ne fera aucun " +
                               "degat et poussera le tireur.", this);
                return;
            }

            CacheOwnerColliders();
            SetOwnerCollision(true);
            _graceUntil = Time.time + _ownerGrace;

        }

        private void CacheOwnerColliders()
        {
            _ownerColliders.Clear();

            foreach (Collider2D c in _owner.GetComponentsInChildren<Collider2D>(true))
            {
                if (c != null) _ownerColliders.Add(c);
            }
        }

        private void SetOwnerCollision(bool ignore)
        {
            if (_myColliders == null) return;

            foreach (Collider2D mine in _myColliders)
            {
                if (mine == null) continue;

                foreach (Collider2D theirs in _ownerColliders)
                {
                    if (theirs != null) Physics2D.IgnoreCollision(mine, theirs, ignore);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_owner == null || _restored) return;

            if (Time.time < _graceUntil)
            {
                SetOwnerCollision(true);
                return;
            }

            SetOwnerCollision(false);
            _restored = true;
        }

        private void Update()
        {
            if (Time.time >= _deathTime) Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Actor other = collision.collider.GetComponentInParent<Actor>();

            if (_owner == null)
            {
                ConsumeBounce();
                return;
            }

            if (other == null)
            {
                ConsumeBounce();
                return;
            }

            if (other.Faction == _owner.Faction)
            {
                if (_crushedByOwnerFaction) Destroy(gameObject);
                return;
            }

            Hit(other);
        }

        private void Hit(Actor target)
        {
            Health health = target.Health;

            if (health == null || !health.IsAlive)
            {
                ConsumeBounce();
                return;
            }

            Vector2 direction = target.transform.position - transform.position;
            if (direction.sqrMagnitude < 0.0001f) direction = _rb.linearVelocity;

            int amount = ResolveDamage();

            health.TakeDamage(new DamageInfos
            {
                Amount = amount,
                Type = DamageType.Projectile,
                Source = _owner,
                Direction = direction.normalized,
                Knockback = _knockback
            });

            ConsumeBounce();
        }

        private int ResolveDamage()
        {
            if (_useOwnerDamageStat && _owner != null && _owner.Stats != null)
            {
                return Mathf.RoundToInt(_owner.Stats.Get(StatType.Damage) * _damageMultiplier);
            }

            return _damage;
        }

        private void ConsumeBounce()
        {
            if (_bouncesLeft <= 0)
            {
                Destroy(gameObject);
                return;
            }

            _bouncesLeft--;
        }
    }
}
