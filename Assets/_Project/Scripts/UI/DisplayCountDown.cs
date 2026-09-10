using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game
{
    public class DisplayCountdown : MonoBehaviour
    {
        private MatchManager countDown;
        public TMP_Text countdownText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            countDown = FindObjectOfType<MatchManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (countDown != null)
            {
                //UpdateCountdown(countDown.getCountdown());
            }
        }

        public void UpdateCountdown(int value)
        {
            countdownText.text = value.ToString("F2");
        }
    }
}
