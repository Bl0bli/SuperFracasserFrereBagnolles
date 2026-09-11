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

        [Tooltip("Marqueur optionnel : seule sa distance au vehicule est utilisee, sa rotation est ignoree.")]
        [SerializeField] private Transform _cardAnchor;

        [Tooltip("Hauteur en unites monde si aucun marqueur n'est assigne.")]
        [SerializeField, Min(0f)] private float _cardAnchorHeight = 2f;

        public FactionType Faction => _faction;
        public Health Health => _health;
        public StatBlock Stats => _stats;
        public StatusEffectController StatusController => _statusController;
        public Rigidbody2D Rb => _rb;
        public Vector3 CardAnchorPosition => transform.position + Vector3.up * CardAnchorHeight;

        private float CardAnchorHeight =>
            _cardAnchor != null ? _cardAnchor.localPosition.magnitude : _cardAnchorHeight;

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
