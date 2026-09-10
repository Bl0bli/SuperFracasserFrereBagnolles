using UnityEngine;

namespace Game
{
    public class DisplayEntityBornes : SliderDisplay
    {
        protected override bool TryGetValues(out float current, out float max)
        {
            current = 0f;
            max = 1f;

            MatchManager match = MatchManager.Instance;
            if (match == null || match.BorneObjective <= 0) return false;

            if (match.State == MatchState.Warmup) return false;

            current = match.EntityBornes;
            max = match.BorneObjective;
            return true;
        }
    }
}
