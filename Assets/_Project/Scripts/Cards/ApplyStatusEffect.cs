using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ApllyStatusEffect", menuName = "Scriptable Objects/CardEffect/ApllyStatusEffect")]

    public class ApplyStatusEffect : CardEffect
    {
        [SerializeField] private StatusType _status;
        public override void Apply(Actor target)
        {
            target.StatusController.Apply(_status, _duration);   
        }
    }
}
