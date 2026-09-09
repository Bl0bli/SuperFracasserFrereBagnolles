using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Knockback : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        }

        public void Apply(Vector2 direction, float force)
        {
            if (_rb == null || force <= 0f) return;
            if (direction.sqrMagnitude < 0.0001f) return;

            _rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}
