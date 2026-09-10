using UnityEngine;
using UnityEngine.UI;

namespace Game
{
        public class DisplayBorne : MonoBehaviour
    {
        private MatchManager matchManager;
        public Slider compteurBorne;

        void Start()
        {
            matchManager = FindObjectOfType<MatchManager>();
        }

        void Update()
        {
            if (matchManager != null)
            {
                UpdateSlider(matchManager.getBornes());
            }
        }

        public void UpdateSlider(float value) 
        {
            compteurBorne.value = value;
        }
    }
}
