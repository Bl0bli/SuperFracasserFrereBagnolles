using System;
using UnityEngine;

namespace Game
{
    public class BornePickUp : MonoBehaviour, IPickup
    {
        [SerializeField] private int _borneValue;

        private bool _consummed = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor
                actor = other.GetComponentInParent<Actor>(); //attention à ne pas reproduire, ceci a été effectué par un professionnel
            if (actor != null && actor.Faction == FactionType.Cthulhu)
            {
                MatchManager.Instance.AddBornes(_borneValue);
                _consummed = true;
            }

            Destroy(gameObject);
        }

        public void OnPickedUp(Actor collector)
        {
            throw new NotImplementedException();
        }
    }
}
