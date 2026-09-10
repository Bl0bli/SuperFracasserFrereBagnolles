using System;
using UnityEngine;

namespace Game
{
    public class CardPickup : MonoBehaviour, IPickup
    {
        private CardDefinition _cardDef;
        
        private bool _consummed = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor actor = other.GetComponentInParent<Actor>(); //attention à ne pas reproduire, ceci a été effectué par un professionnel
            if (actor != null)
            {
                _cardDef.GetEffect(actor.Faction).Apply(actor);
                _consummed = true;
            }
            Destroy(gameObject);
        }

        public void OnPickedUp(Actor collector)
        {
            throw new NotImplementedException();
        }

        public void SetCardDefinition(CardDefinition card)
        {
            _cardDef = card;
        }
    }
}
