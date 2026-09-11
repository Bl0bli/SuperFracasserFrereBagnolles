using UnityEngine;

namespace Game
{
    public class DisplayHealth : SliderDisplay
    {
        private Health _health;

        protected override bool TryGetValues(out float current, out float max)
        {
            if (_health == null) _health = GetComponentInParent<Health>();

            if (_health == null)
            {
                current = 0f;
                max = 1f;
                return false;
            }

            current = _health.getCurrentHealth();
            max = Mathf.Max(1f, _health.Max);
            return true;
        }
    }
}
