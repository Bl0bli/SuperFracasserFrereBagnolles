using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game
{
    public class DisplayTimer : MonoBehaviour
    {
        private MatchManager timer;
        public TMP_Text timerText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            timer = FindObjectOfType<MatchManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (timer != null)
            {
                UpdateTimer(timer.getCurrentTime());
            }
        }
        public void UpdateTimer(float value)
        {
            int minutes = Mathf.FloorToInt(timer.getCurrentTime() / 60f);
            int seconds = Mathf.FloorToInt(timer.getCurrentTime() % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }
}
