using UnityEngine;

namespace Game
{
    public class LogoScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _logo;

        private void Awake()
        {
            if (_logo == null) _logo = gameObject;
        }

        private void Update()
        {
            MatchManager match = MatchManager.Instance;

            bool waiting = match != null
                           && match.State == MatchState.Warmup
                           && match.Players.Count == 0;

            if (_logo.activeSelf != waiting) _logo.SetActive(waiting);
        }
    }
}
