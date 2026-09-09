using UnityEngine;

namespace Game
{
    public class CarMotor : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private StatBlock _stats;

        public void Drive(Vector2 command, float dt)
        {
            
        }

        private void ApplyLateralFriction()
        {
            
        }
    }
}
