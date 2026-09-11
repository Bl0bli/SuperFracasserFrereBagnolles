using UnityEngine;

namespace Game
{
    public class DisplayBorne : SliderDisplay
    {
        protected override bool TryGetValues(out float current, out float max)
        {
            current = 0f;
            max = 1f;

            MatchManager match = MatchManager.Instance;
            if (match == null || match.BorneObjective <= 0) return false;

            current = match.CarBornes;
            max = match.BorneObjective;
            return true;
        }
    }
}
