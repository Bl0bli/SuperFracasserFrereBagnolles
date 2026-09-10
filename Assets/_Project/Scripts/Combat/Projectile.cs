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
        [SerializeField, Min(0f)] private float _knockback = 8f;

        [Header("Proprietaire")]
        [Tooltip("Coche : le camp du lanceur detruit le projectile en le touchant, sans degats.")]
        [SerializeField] private bool _crushedByOwnerFaction = true;

        [Tooltip("Duree pendant laquelle la collision avec le lanceur est desactivee, " +
                 "pour qu'il ne se pousse pas lui-meme au tir.")]
        [SerializeField, Min(0f)] private float _ownerGrace = 0.25f;

        private readonly List<Collider2D> _ignored = new List<Collider2D>();
        private Actor _owner;
        private int _bouncesLeft;
        private float _deathTime;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
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

            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;

            _rb.linearVelocity = direction.normalized * _speed;

            if (_ownerGrace > 0f)
            {
                SetOwnerCollision(true);
                Invoke(nameof(RestoreOwnerCollision), _ownerGrace);
            }
        }

        private void SetOwnerCollision(bool ignore)
        {
            if (_owner == null) return;

            Collider2D[] mine = GetComponentsInChildren<Collider2D>();
            Collider2D[] theirs = _owner.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D a in mine)
            {
                if (a == null) continue;

                foreach (Collider2D b in theirs)
                {
                    if (b == null) continue;

                    Physics2D.IgnoreCollision(a, b, ignore);
                    if (ignore) _ignored.Add(b);
                }
            }
        }

        private void RestoreOwnerCollision()
        {
            Collider2D[] mine = GetComponentsInChildren<Collider2D>();

            foreach (Collider2D a in mine)
            {
                if (a == null) continue;

                foreach (Collider2D b in _ignored)
                {
                    if (b != null) Physics2D.IgnoreCollision(a, b, false);
                }
            }

            _ignored.Clear();
        }

        private void Update()
        {
            if (Time.time >= _deathTime) Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Actor other = collision.collider.GetComponentInParent<Actor>();

            if (other != null && _owner != null)
            {
                if (other.Faction == _owner.Faction)
                {
                    if (_crushedByOwnerFaction) Destroy(gameObject);
                    return;
                }

                Hit(other);
                return;
            }

            ConsumeBounce();
        }

        private void Hit(Actor target)
        {
            Health health = target.Health;

            if (health != null && health.IsAlive)
            {
                Vector2 direction = target.transform.position - transform.position;
                if (direction.sqrMagnitude < 0.0001f) direction = _rb.linearVelocity;

                health.TakeDamage(new DamageInfos
                {
                    Amount = ResolveDamage(),
                    Type = DamageType.Projectile,
                    Source = _owner,
                    Direction = direction.normalized,
                    Knockback = _knockback
                });
            }

            ConsumeBounce();
        }

        private int ResolveDamage()
        {
            if (_useOwnerDamageStat && _owner != null && _owner.Stats != null)
            {
                return Mathf.RoundToInt(_owner.Stats.Get(StatType.Damage));
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
