using UnityEngine;

namespace Game
{
    public class BornePickUp : MonoBehaviour, IPickup
    {
        [SerializeField] private BorneCardDefinition _definition;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _consummed;

        public BorneCardDefinition Definition => _definition;

        private void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void SetDefinition(BorneCardDefinition definition)
        {
            _definition = definition;
        }

        public void OnPickedUp(Actor collector)
        {
            if (_consummed) return;

            if (_definition == null)
            {
                Debug.LogError("[BornePickUp] " + name + " n'a pas de BorneCardDefinition.", this);
                return;
            }

            _consummed = true;

            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.AddBornes(collector.Faction, _definition.BorneValue);
            }

            if (CardRevealDirector.Instance != null)
            {
                Sprite back = _spriteRenderer != null ? _spriteRenderer.sprite : null;

                CardRevealDirector.Instance.PlaySelf(back, _definition.Icon, EffectPolarity.Bonus,
                    collector, transform.position);
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor == null) return;

            OnPickedUp(actor);
        }
    }
}
