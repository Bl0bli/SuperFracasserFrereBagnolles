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
        [SerializeField] private Actor _actor;

        [Tooltip("Poussee appliquee quand MustKeepMoving est actif et que le joueur lache le stick.")]
        [SerializeField, Range(0f, 1f)] private float _forcedThrottle = 0.6f;

        [Tooltip("Trace les entrees et sorties de stun, d'inversion et d'obligation d'avancer.")]
        [SerializeField] private bool _verboseLogs = true;

        private bool _wasStunned;
        private bool _wasInverted;
        private bool _wasForced;

        private void Awake()
        {
            if (_inputs == null) _inputs = GetComponent<InputReader>();
            if (_motor == null) _motor = GetComponent<CarMotor>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
            if (_throwAbility == null) _throwAbility = GetComponent<ThrowAbility>();
            if (_actor == null) _actor = GetComponent<Actor>();
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

            _wasStunned = false;
            _wasInverted = false;
            _wasForced = false;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            Vector2 command = _inputs.Move;

            if (_statusController != null)
            {
                bool inverted = _statusController.Has(StatusType.Inverted);
                _inputs.SetInverted(inverted);
                LogChange("Inverted", inverted, ref _wasInverted);

                bool stunned = _statusController.Has(StatusType.Stunned);
                LogChange("Stunned", stunned, ref _wasStunned);

                if (stunned)
                {
                    _motor.Drive(Vector2.zero, dt);
                    return;
                }

                bool forced = _statusController.Has(StatusType.MustKeepMoving);
                LogChange("MustKeepMoving", forced, ref _wasForced);

                if (forced && command.sqrMagnitude < 0.01f)
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

        private void LogChange(string status, bool active, ref bool previous)
        {
            if (active == previous) return;

            previous = active;

            if (_verboseLogs)
            {
                string who = _actor != null ? name + " [" + _actor.Faction + "]" : name;
                Debug.Log("[CarController] " + who + " : " + status + (active ? " ACTIF" : " termine"), this);
            }
        }
    }
}
