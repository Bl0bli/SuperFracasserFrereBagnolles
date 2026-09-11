using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class CardVisual : MonoBehaviour
    {
        [Tooltip("Enfant qui porte le sprite et subit la rotation de retournement.")]
        [SerializeField] private Transform _flipper;
        [SerializeField] private SpriteRenderer _renderer;

        [Header("FX")]
        [SerializeField] private GameObject _bonusFx;
        [SerializeField] private GameObject _malusFx;

        [Header("Durees")]
        [SerializeField, Min(0.05f)] private float _slideDuration = 0.35f;
        [SerializeField, Min(0.05f)] private float _flipDuration = 0.55f;

        [Tooltip("Temps d'arret quand la carte est sur la tranche, avant de reveler la face.")]
        [SerializeField, Min(0f)] private float _flipHold = 0.08f;

        [SerializeField, Min(0.05f)] private float _travelDuration = 0.4f;
        [SerializeField, Min(0.05f)] private float _dismissDuration = 0.25f;

        [Header("Ampleurs")]
        [SerializeField] private float _holdScale = 1.15f;
        [SerializeField, Min(0.1f)] private float _holdPulse = 0.9f;
        [SerializeField] private float _travelArc = 1.2f;

        private Actor _follow;
        private Vector3 _baseScale;

        private void Awake()
        {
            if (_flipper == null) _flipper = transform.childCount > 0 ? transform.GetChild(0) : transform;
            if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();

            _baseScale = _flipper.localScale;
            SetFx(null);
        }

        private void LateUpdate()
        {
            if (_follow != null) transform.position = _follow.CardAnchorPosition;

            transform.rotation = Quaternion.identity;
        }

        public void SetSprite(Sprite sprite)
        {
            if (_renderer == null)
            {
                Debug.LogError("[CardVisual] Aucun SpriteRenderer sur " + name + ".", this);
                return;
            }

            if (sprite == null) return;

            _renderer.sprite = sprite;
        }

        public void Follow(Actor actor)
        {
            _follow = actor;
        }

        public Sequence PlayReveal(Actor holder, Sprite front, EffectPolarity polarity)
        {
            Sequence sequence = DOTween.Sequence().SetTarget(transform);

            sequence.Append(transform.DOMove(holder.CardAnchorPosition, _slideDuration).SetEase(Ease.OutCubic));
            sequence.Join(_flipper.DOScale(_baseScale * 1.1f, _slideDuration).SetEase(Ease.OutBack));
            sequence.AppendCallback(() => Follow(holder));

            sequence.Append(_flipper.DOLocalRotate(new Vector3(0f, 90f, 0f), _flipDuration * 0.5f)
                .SetEase(Ease.InQuad));

            sequence.AppendCallback(() =>
            {
                SetSprite(front);
                _flipper.localEulerAngles = new Vector3(0f, -90f, 0f);
                SetFx(polarity);
            });

            if (_flipHold > 0f) sequence.AppendInterval(_flipHold);

            sequence.Append(_flipper.DOLocalRotate(Vector3.zero, _flipDuration * 0.5f).SetEase(Ease.OutBack));

            return sequence;
        }

        public void PlayHold()
        {
            _flipper.DOScale(_baseScale * _holdScale, _holdPulse)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetTarget(transform);
        }

        public Sequence PlayTravel(Actor target, EffectPolarity polarity, Action onArrive)
        {
            Follow(null);
            SetFx(polarity);

            Vector3 destination = target.CardAnchorPosition;
            Vector3 mid = Vector3.Lerp(transform.position, destination, 0.5f) + Vector3.up * _travelArc;

            Sequence sequence = DOTween.Sequence().SetTarget(transform);
            sequence.Append(transform.DOPath(new[] { mid, destination }, _travelDuration, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad));
            sequence.Join(_flipper.DORotate(new Vector3(0f, 360f, 0f), _travelDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad));
            sequence.AppendCallback(() =>
            {
                Follow(target);
                onArrive?.Invoke();
            });
            sequence.Append(_flipper.DOPunchScale(_baseScale * 0.4f, 0.25f, 6, 0.6f));

            return sequence;
        }

        public void Dismiss(float delay)
        {
            Sequence sequence = DOTween.Sequence().SetTarget(transform);
            sequence.AppendInterval(delay);
            sequence.Append(_flipper.DOScale(Vector3.zero, _dismissDuration).SetEase(Ease.InBack));

            if (_renderer != null)
            {
                Color from = _renderer.color;
                Color to = new Color(from.r, from.g, from.b, 0f);

                sequence.Join(DOTween.To(() => _renderer.color, c => _renderer.color = c,
                    to, _dismissDuration));
            }

            sequence.OnComplete(() => Destroy(gameObject));
        }

        private void SetFx(EffectPolarity? polarity)
        {
            if (_bonusFx != null) _bonusFx.SetActive(polarity == EffectPolarity.Bonus);
            if (_malusFx != null) _malusFx.SetActive(polarity == EffectPolarity.Malus);
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);
        }
    }
}
