using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(EntityMotor))]
    public class EntityController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputs;
        [SerializeField] private EntityMotor _motor;
        [SerializeField] private TentacleAttack _attack;
        [SerializeField] private StatusEffectController _statusController;
        [SerializeField] private Actor _actor;

        [Tooltip("Trace les entrees et sorties de stun et d'inversion.")]
        [SerializeField] private bool _verboseLogs = true;

        private bool _wasStunned;
        private bool _wasInverted;

        private void Awake()
        {
            if (_inputs == null) _inputs = GetComponent<InputReader>();
            if (_motor == null) _motor = GetComponent<EntityMotor>();
            if (_attack == null) _attack = GetComponent<TentacleAttack>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
            if (_actor == null) _actor = GetComponent<Actor>();
        }

        private void OnEnable()
        {
            if (_inputs != null) _inputs.ActionPressed += HandleActionPressed;
        }

        private void OnDisable()
        {
            if (_inputs != null) _inputs.ActionPressed -= HandleActionPressed;

            if (_inputs != null) _inputs.SetInverted(false);
            _wasStunned = false;
            _wasInverted = false;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            bool stunned = false;

            if (_statusController != null)
            {
                bool inverted = _statusController.Has(StatusType.Inverted);
                _inputs.SetInverted(inverted);
                LogChange("Inverted", inverted, ref _wasInverted);

                stunned = _statusController.Has(StatusType.Stunned);
                LogChange("Stunned", stunned, ref _wasStunned);
            }

            _motor.Drive(stunned ? Vector2.zero : _inputs.Move, dt);
        }

        private void HandleActionPressed()
        {
            if (_statusController != null && _statusController.Has(StatusType.Stunned))
            {
                if (_verboseLogs) Debug.Log("[EntityController] attaque bloquee : Stunned actif.", this);
                return;
            }

            if (_attack != null) _attack.TryAttack();
        }

        private void LogChange(string status, bool active, ref bool previous)
        {
            if (active == previous) return;

            previous = active;

            if (_verboseLogs)
            {
                string who = _actor != null ? name + " [" + _actor.Faction + "]" : name;
                Debug.Log("[EntityController] " + who + " : " + status + (active ? " ACTIF" : " termine"), this);
            }
        }
    }
}
