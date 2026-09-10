using UnityEngine;

namespace Game
{
    public class DisplayEntityBornes : SliderDisplay
    {
        [SerializeField] private Actor _actor;

        protected override void Awake()
        {
            base.Awake();
            if (_actor == null) _actor = GetComponentInParent<Actor>();
        }

        protected override bool TryGetValues(out float current, out float max)
        {
            current = 0f;
            max = 1f;

            MatchManager match = MatchManager.Instance;
            if (match == null || match.BorneObjective <= 0) return false;
            if (_actor == null || _actor.Faction != FactionType.Cthulhu) return false;

            current = match.EntityBornes;
            max = match.BorneObjective;
            return true;
        }
    }
}
