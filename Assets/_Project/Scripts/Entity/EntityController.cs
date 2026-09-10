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

        private void Awake()
        {
            if (_inputs == null) _inputs = GetComponent<InputReader>();
            if (_motor == null) _motor = GetComponent<EntityMotor>();
            if (_attack == null) _attack = GetComponent<TentacleAttack>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
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
            bool stunned = false;

            if (_statusController != null)
            {
                _inputs.SetInverted(_statusController.Has(StatusType.Inverted));
                stunned = _statusController.Has(StatusType.Stunned);
            }

            _motor.Drive(stunned ? Vector2.zero : _inputs.Move, dt);
        }

        private void HandleActionPressed()
        {
            if (_statusController != null && _statusController.Has(StatusType.Stunned)) return;
            if (_attack != null) _attack.TryAttack();
        }
    }
}
