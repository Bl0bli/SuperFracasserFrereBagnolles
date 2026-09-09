using UnityEngine;

namespace Game
{
    public class Actor : MonoBehaviour
    {
        [Header("Gameplay Components")]
        [SerializeField] private FactionType _faction;
        [SerializeField] private Health _health;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private StatusEffectController _statusController;

        [Header(" Unity Components")]
        [SerializeField] private Rigidbody2D _rb;

        public FactionType Faction => _faction;
        public Health Health => _health;
        public StatBlock Stats => _stats;
        public StatusEffectController StatusController => _statusController;
        public Rigidbody2D Rb => _rb;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_health == null) _health = GetComponent<Health>();
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
        }
        
        public void SetFaction(FactionType faction)
        {
            _faction = faction;
        }
    }
}
