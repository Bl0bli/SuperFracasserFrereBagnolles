using UnityEngine;

namespace Game
{
    public class CardPickup : MonoBehaviour, IPickup
    { 
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private CardDefinition _cardDef;

        private bool _consummed;

        public CardDefinition Definition => _cardDef;

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
            effect.Apply(collector);
            Debug.Log("[CardPickup] " + "applied on " + effect.Target, this);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor actor = other.GetComponentInParent<Actor>(); //attention à ne pas reproduire, ceci a été effectué par un professionnel
            if (actor == null) return;

            OnPickedUp(actor);
        }
    }
}
