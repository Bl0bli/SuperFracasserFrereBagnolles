using UnityEngine;

namespace Game
{
    public class Actor : MonoBehaviour
    {
        [Header("Gameplay Components")]
        [SerializeField] private FactionType _faction;
        //[SerializeField] private Health _health; TODO decommenter
        //[SerializeField] private StatBlock _stats;
        //[SerializeField] private StatusEffectController _statusController;
        
        [Header(" Unity Components")]
        [SerializeField] private Rigidbody2D _rb;

        private void Awake()
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
        }
    }
}
