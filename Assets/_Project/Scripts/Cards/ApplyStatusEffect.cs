using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ApplyStatusEffect", menuName = "Scriptable Objects/CardEffect/ApplyStatusEffect")]
    public class ApplyStatusEffect : CardEffect
    {
        [Header("Statut")]
        [SerializeField] private StatusType _status = StatusType.Stunned;

        [Tooltip("Duree du statut en secondes.")]
        [SerializeField] private float _duration = 5f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.StatusController == null)
            {
                Debug.LogWarning("[ApplyStatusEffect] " + target.name + " n'a pas de StatusEffectController.", target);
                return;
            }
            
            target.StatusController.Apply(_status, _duration);
        }

        public override float RemainingOn(Actor target)
        {
            if (target == null || target.StatusController == null) return 0f;
            return target.StatusController.Has(_status) ? target.StatusController.Remaining(_status) : 0f;
        }
    }
}
