using UnityEngine;

namespace Game
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputs;
        [SerializeField] private CarMotor _motor;
        [SerializeField] private StatusEffectController _statusController;

        private void FixedUpdate()
        {
            
        }
    }
}
