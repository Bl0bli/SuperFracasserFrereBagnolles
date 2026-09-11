using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CarMotor))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputs;
        [SerializeField] private CarMotor _motor;
        [SerializeField] private StatusEffectController _statusController;
        [SerializeField] private ThrowAbility _throwAbility;

        [Tooltip("Poussee appliquee quand MustKeepMoving est actif et que le joueur lache le stick.")]
        [SerializeField, Range(0f, 1f)] private float _forcedThrottle = 0.6f;

        private void Awake()
        {
            if (_inputs == null) _inputs = GetComponent<InputReader>();
            if (_motor == null) _motor = GetComponent<CarMotor>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
            if (_throwAbility == null) _throwAbility = GetComponent<ThrowAbility>();
        }

        private void OnEnable()
        {
            if (_inputs != null) _inputs.ActionPressed += HandleActionPressed;
        }

        private void OnDisable()
        {
            if (_inputs != null)
            {
                _inputs.ActionPressed -= HandleActionPressed;
                _inputs.SetInverted(false);
            }
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            Vector2 command = _inputs.Move;

            if (_statusController != null)
            {
                _inputs.SetInverted(_statusController.Has(StatusType.Inverted));

                if (_statusController.Has(StatusType.Stunned))
                {
                    _motor.Drive(Vector2.zero, dt);
                    return;
                }

                if (_statusController.Has(StatusType.MustKeepMoving) && command.sqrMagnitude < 0.01f)
                {
                    command = (Vector2)transform.up * _forcedThrottle;
                }
            }

            _motor.Drive(command, dt);
        }

        private void HandleActionPressed()
        {
            if (_statusController != null && _statusController.Has(StatusType.Stunned)) return;
            if (_throwAbility != null) _throwAbility.TryThrow();
        }
    }
}
