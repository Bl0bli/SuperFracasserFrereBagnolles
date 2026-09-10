using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CardPickup : MonoBehaviour, IPickup
    {
        [SerializeField] private CardDefinition _cardDef;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider;

        private bool _consummed;

        public CardDefinition Definition => _cardDef;

        private void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_collider == null) _collider = GetComponent<Collider2D>();
        }

        public void SetCardDefinition(CardDefinition card)
        {
            _cardDef = card;
        }

        public void OnPickedUp(Actor collector)
        {
            if (_consummed) return;

            if (_cardDef == null)
            {
                Debug.LogError("[CardPickup] " + name + " n'a pas de CardDefinition.", this);
                return;
            }

            CardEffect effect = _cardDef.GetEffect(collector.Faction);

            if (effect == null)
            {
                Debug.LogWarning("[CardPickup] " + _cardDef.ID + " n'a pas d'effet pour la faction " +
                                 collector.Faction + ".", this);
                return;
            }

            _consummed = true;

            IReadOnlyList<Actor> targets = effect.Apply(collector);

            if (CardRevealDirector.Instance != null)
            {
                Sprite back = _spriteRenderer != null ? _spriteRenderer.sprite : null;
                CardRevealDirector.Instance.Play(back, effect, collector, targets, transform.position);
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
