using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class EndScreen : MonoBehaviour
    {
        [Tooltip("Visuel affiche quand les voitures gagnent.")]
        [SerializeField] private GameObject _carsWin;

        [Tooltip("Visuel affiche quand l'entite gagne.")]
        [SerializeField] private GameObject _entityWin;

        [Tooltip("Duree d'affichage avant le retour au lobby, en secondes.")]
        [SerializeField, Min(0.5f)] private float _displayDuration = 5f;

        [SerializeField, Min(0f)] private float _popDuration = 0.4f;

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            MatchManager.Instance.OnMatchEnded += HandleMatchEnded;
        }

        private void OnDisable()
        {
            if (MatchManager.Instance != null) MatchManager.Instance.OnMatchEnded -= HandleMatchEnded;
        }

        private void HandleMatchEnded(FactionType winner)
        {
            StopAllCoroutines();
            StartCoroutine(ShowThenReset(winner));
        }

        private IEnumerator ShowThenReset(FactionType winner)
        {
            GameObject visual = winner == FactionType.Car ? _carsWin : _entityWin;

            if (visual == null)
            {
                Debug.LogError("[EndScreen] Aucun visuel assigne pour la victoire de " + winner + ".", this);
            }
            else
            {
                visual.SetActive(true);
                Pop(visual.transform);
            }

            // Realtime : un eventuel ralenti de fin ne doit pas allonger l'attente.
            yield return new WaitForSecondsRealtime(_displayDuration);

            Hide();

            if (MatchManager.Instance != null) MatchManager.Instance.ResetToLobby();
        }

        private void Pop(Transform target)
        {
            if (_popDuration <= 0f) return;

            DOTween.Kill(target);
            target.localScale = Vector3.zero;
            target.DOScale(Vector3.one, _popDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .SetTarget(target);
        }

        private void Hide()
        {
            if (_carsWin != null)
            {
                DOTween.Kill(_carsWin.transform);
                _carsWin.transform.localScale = Vector3.one;
                _carsWin.SetActive(false);
            }

            if (_entityWin != null)
            {
                DOTween.Kill(_entityWin.transform);
                _entityWin.transform.localScale = Vector3.one;
                _entityWin.SetActive(false);
            }
        }
    }
}
