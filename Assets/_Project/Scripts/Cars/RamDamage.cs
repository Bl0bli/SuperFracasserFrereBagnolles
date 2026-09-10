using UnityEngine;

namespace Game
{
    //degat au contact entre vehicule
    [RequireComponent(typeof(Rigidbody2D))]
    public class RamDamage : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private Actor _actor;
        
        private Vector2 _velocityBeforeImpact;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_actor == null) _actor = GetComponent<Actor>();
        }

        private void FixedUpdate()
        {
            _velocityBeforeImpact = _rb.linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Actor target = collision.collider.GetComponentInParent<Actor>(); //attention bullshit effectuer par un cascadeur professionnel, a ne pas reproduire Yanis
            if (target == null || target == _actor) return;
            if (target.Faction == _actor.Faction) return;

            Health health = target.Health;
            if (health == null || !health.IsAlive) return;

            float speed = _velocityBeforeImpact.magnitude;
            float threshold = _stats.Get(StatType.MoveSpeed) * _stats.Get(StatType.RamSpeedThreshold);

            Vector2 direction = target.transform.position - transform.position;
            if (direction.sqrMagnitude < 0.0001f) direction = _velocityBeforeImpact;

            // Il faut aller VERS la cible : un vehicule a l'arret percute ne rend pas les coups.
            bool movingIntoTarget = Vector2.Dot(_velocityBeforeImpact, direction) > 0f;

            if (speed < threshold || !movingIntoTarget) return;

            health.TakeDamage(new DamageInfos
            {
                Amount = Mathf.RoundToInt(_stats.Get(StatType.RamDamage)),
                Type = DamageType.Ram,
                Source = _actor,
                Direction = direction.normalized,
                Knockback = _stats.Get(StatType.KnockbackForce)
            });
        }
    }
}
