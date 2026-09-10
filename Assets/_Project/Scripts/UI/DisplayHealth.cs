using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class DisplayHealth : MonoBehaviour
    {
        private Health health;
        public Slider healthSlider;

        void Update()
        {
            if (health == null)
            {
                health = GetComponentInParent<Health>();
                if (health == null) return;
            }
            
            if (!Mathf.Approximately(healthSlider.maxValue, health.Max))
            {
                healthSlider.maxValue = health.Max;
            }

            healthSlider.value = health.getCurrentHealth();
        }

        public void UpdateSlider(float value) //mettre la valeur temps réel des vies dans le slider
        {
            healthSlider.value = value;
        }
    }
}
