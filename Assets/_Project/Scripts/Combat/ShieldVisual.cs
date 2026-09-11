using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class ShieldVisual : MonoBehaviour
    {
        [SerializeField] private ShieldController _shield;

        [Tooltip("Bulle affichee autour du vehicule. Laissez-la desactivee dans le prefab.")]
        [SerializeField] private Transform _bubble;

        [SerializeField] private SpriteRenderer _renderer;

        [Tooltip("Particules jouees a la casse du bouclier.")]
        [SerializeField] private ParticleSystem _breakFx;

        [Tooltip("La bulle garde son orientation malgre la rotation du vehicule.")]
        [SerializeField] private bool _keepUpright = true;

        [Header("Animation")]
        [SerializeField, Min(0.05f)] private float _appearDuration = 0.3f;
        [SerializeField, Min(0.05f)] private float _pulsePeriod = 1.1f;
        [SerializeField] private float _pulseScale = 1.08f;
        [SerializeField, Min(0.05f)] private float _fadeDuration = 0.25f;

        private Vector3 _baseScale = Vector3.one;
        private Color _baseColor = Color.white;

        private void Awake()
        {
            if (_shield == null) _shield = GetComponentInParent<ShieldController>();
            if (_bubble == null) _bubble = transform;
            if (_renderer == null) _renderer = _bubble.GetComponentInChildren<SpriteRenderer>(true);

            _baseScale = _bubble.localScale;
            if (_renderer != null) _baseColor = _renderer.color;

            _bubble.gameObject.SetActive(false);

            if (_shield == null)
            {
                Debug.LogError("[ShieldVisual] Aucun ShieldController trouve pour " + name + ".", this);
            }
        }

        private void OnEnable()
        {
            if (_shield == null) return;

            _shield.OnShieldUp += HandleUp;
            _shield.OnShieldExpired += HandleExpired;
        }

        private void OnDisable()
        {
            if (_shield != null)
            {
                _shield.OnShieldUp -= HandleUp;
                _shield.OnShieldExpired -= HandleExpired;
            }

            DOTween.Kill(_bubble);
        }

        private void LateUpdate()
        {
            if (_keepUpright) _bubble.rotation = Quaternion.identity;
        }

        private void HandleUp()
        {
            DOTween.Kill(_bubble);

            _bubble.gameObject.SetActive(true);
            _bubble.localScale = Vector3.zero;

            if (_renderer != null) _renderer.color = _baseColor;

            Sequence sequence = DOTween.Sequence().SetTarget(_bubble);
            sequence.Append(_bubble.DOScale(_baseScale, _appearDuration).SetEase(Ease.OutBack));
            sequence.OnComplete(() =>
                _bubble.DOScale(_baseScale * _pulseScale, _pulsePeriod)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetTarget(_bubble));
        }

        private void HandleExpired(bool survived)
        {
            DOTween.Kill(_bubble);

            Sequence sequence = DOTween.Sequence().SetTarget(_bubble);

            if (survived)
            {
                sequence.Append(_bubble.DOScale(_baseScale * 1.2f, _fadeDuration).SetEase(Ease.OutQuad));
            }
            else
            {
                if (_breakFx != null) _breakFx.Play();
                sequence.Append(_bubble.DOPunchScale(_baseScale * 0.5f, _fadeDuration, 8, 0.8f));
            }

            if (_renderer != null)
            {
                Color transparent = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0f);
                sequence.Join(DOTween.To(() => _renderer.color, c => _renderer.color = c,
                    transparent, _fadeDuration));
            }

            sequence.OnComplete(() =>
            {
                _bubble.gameObject.SetActive(false);
                _bubble.localScale = _baseScale;
                if (_renderer != null) _renderer.color = _baseColor;
            });
        }
    }
}
