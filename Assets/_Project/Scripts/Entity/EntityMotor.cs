using UnityEngine;

namespace Game
{
    //physique du méchant
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityMotor : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private StatBlock _stats;

        [Tooltip("Decalage d'orientation du sprite. -90 si l'entite est dessinee pointant vers " +
                 "le haut (+Y), 0 si elle pointe vers la droite (+X).")]
        [SerializeField] private float _spriteAngleOffset = -90f;

        private const float InputThreshold = 0.0001f;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_stats == null) _stats = GetComponent<StatBlock>();
        }

        public void Drive(Vector2 command, float dt)
        {
            if (command.sqrMagnitude <= InputThreshold)
            {
                Decelerate(dt);
                return;
            }

            Vector2 direction = command.normalized;
            float throttle = Mathf.Clamp01(command.magnitude);

            Push(direction, throttle);
            AlignVelocity(direction);
            FaceDirection(direction, dt);
        }
        
        private void Push(Vector2 direction, float throttle)
        {
            float max = _stats.Get(StatType.MoveSpeed) * throttle;
            float along = Vector2.Dot(_rb.linearVelocity, direction);
            if (along >= max) return;
            float force = _stats.Get(StatType.Acceleration) * _rb.mass * throttle;
            _rb.AddForce(direction * force);
        }

        //bullshit maths pour réduire les forces perpendiculaire
        private void AlignVelocity(Vector2 direction)
        {
            Vector2 velocity = _rb.linearVelocity;
            Vector2 lateral = velocity - direction * Vector2.Dot(velocity, direction);
            _rb.linearVelocity = velocity - lateral * _stats.Get(StatType.Grip);
        }

        private void Decelerate(float dt)
        {
            _rb.linearVelocity = Vector2.MoveTowards(
                _rb.linearVelocity, Vector2.zero, _stats.Get(StatType.Deceleration) * dt);
        }

        private void FaceDirection(Vector2 direction, float dt)
        {
            float target = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _spriteAngleOffset;
            _rb.MoveRotation(Mathf.MoveTowardsAngle(_rb.rotation, target, _stats.Get(StatType.TurnRate) * dt));
        }
    }
}
