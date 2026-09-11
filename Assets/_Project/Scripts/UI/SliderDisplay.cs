using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public abstract class SliderDisplay : MonoBehaviour
    {
        [FormerlySerializedAs("healthSlider")]
        [FormerlySerializedAs("compteurBorne")]
        [SerializeField] protected Slider _slider;

        [Tooltip("Masque la jauge quand sa source n'est pas disponible. Decocher pour la laisser " +
                 "toujours visible.")]
        [SerializeField] private bool _hideWhenUnavailable = true;

        [Tooltip("Vitesse de rattrapage de la jauge. 0 = suit la valeur instantanement.")]
        [SerializeField, Min(0f)] private float _smoothing = 8f;

        private CanvasGroup _group;

        protected virtual void Awake()
        {
            if (_slider == null) _slider = GetComponent<Slider>();
            if (_slider == null) return;
            
            _group = _slider.GetComponent<CanvasGroup>();
            if (_group == null) _group = _slider.gameObject.AddComponent<CanvasGroup>();
        }

        protected virtual void Update()
        {
            if (_slider == null) return;

            if (!TryGetValues(out float current, out float max))
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);

            if (!Mathf.Approximately(_slider.maxValue, max)) _slider.maxValue = max;

            _slider.value = _smoothing <= 0f
                ? current
                : Mathf.MoveTowards(_slider.value, current, Mathf.Max(1f, max) * _smoothing * Time.deltaTime);
        }

        protected abstract bool TryGetValues(out float current, out float max);

        protected virtual void SetVisible(bool visible)
        {
            if (_group == null) return;

            float alpha = visible || !_hideWhenUnavailable ? 1f : 0f;
            if (!Mathf.Approximately(_group.alpha, alpha)) _group.alpha = alpha;
        }
    }
}
