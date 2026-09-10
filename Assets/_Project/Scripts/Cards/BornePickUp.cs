using UnityEngine;

namespace Game
{
    public class BornePickUp : MonoBehaviour, IPickup
    {
        [SerializeField, Min(1)] private int _borneValue = 25;

        private bool _consummed;

        public void OnPickedUp(Actor collector)
        {
            if (_consummed) return;
            _consummed = true;

            if (collector.Faction == FactionType.Cthulhu)
            {
                if (MatchManager.Instance != null) MatchManager.Instance.AddBornes(_borneValue);
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
