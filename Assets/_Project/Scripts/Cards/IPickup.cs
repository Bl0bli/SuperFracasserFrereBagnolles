using UnityEngine;

namespace Game
{
    public interface IPickup
    {
        public void OnPickedUp(Actor collector);
    }
}
