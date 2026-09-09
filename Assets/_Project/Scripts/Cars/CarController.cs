using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CarMotor))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputs;
        [SerializeField] private CarMotor _motor;
        [SerializeField] private StatusEffectController _statusController;

        private void Awake()
        {
            if (_inputs == null) _inputs = GetComponent<InputReader>();
            if (_motor == null) _motor = GetComponent<CarMotor>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            
            if (_statusController != null && _statusController.Has(StatusType.Stunned))
            {
                _motor.Drive(Vector2.zero, dt);
                return;
            }

            _motor.Drive(_inputs.Move, dt);
        }
    }
}
