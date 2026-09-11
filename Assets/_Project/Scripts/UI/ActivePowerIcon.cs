using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class ActivePowerIcon : MonoBehaviour
    {
        [SerializeField] private Actor _actor;
        [SerializeField] private RoleBinder _binder;

        [Tooltip("SpriteRenderer de l'icone. Enfant direct de la racine, jamais sous Visuals. " +
                 "Vide : cherche un enfant nomme PowerIcon.")]
        [SerializeField] private SpriteRenderer _renderer;

        [Tooltip("Decalage par rapport au point d'ancrage de la carte, en unites monde.")]
        [SerializeField] private Vector3 _offset = new Vector3(1.1f, 0f, 0f);

        [Header("Animation")]
        [SerializeField, Min(0.05f)] private float _popDuration = 0.25f;
        [SerializeField, Min(0.05f)] private float _hideDuration = 0.15f;

        [Tooltip("L'icone clignote quand il reste moins de ce temps, en secondes. 0 : jamais.")]
        [SerializeField, Min(0f)] private float _blinkBelow = 1.5f;

        [SerializeField, Min(0.5f)] private float _blinkFrequency = 6f;

        private readonly List<CardEffect> _active = new List<CardEffect>();
        private CardEffect _shown;
        private Transform _icon;
        private Vector3 _baseScale = Vector3.one;
        private Color _baseColor = Color.white;

        private void Awake()
        {
            if (_actor == null) _actor = GetComponent<Actor>();
            if (_binder == null) _binder = GetComponent<RoleBinder>();

            if (_renderer == null)
            {
                Transform found = transform.Find("PowerIcon");
                if (found != null) _renderer = found.GetComponent<SpriteRenderer>();
            }

            if (_renderer == null)
            {
                Debug.LogError("[ActivePowerIcon] Aucun SpriteRenderer d'icone sur " + name +
                               " : ajoutez un enfant PowerIcon.", this);
                enabled = false;
                return;
            }

            _icon = _renderer.transform;
            _baseScale = _icon.localScale;
            _baseColor = _renderer.color;
            _icon.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CardEffect.OnAppliedTo += HandleApplied;
            if (_binder != null) _binder.OnRoleApplied += HandleRoleApplied;
        }

        private void OnDisable()
        {
            CardEffect.OnAppliedTo -= HandleApplied;
            if (_binder != null) _binder.OnRoleApplied -= HandleRoleApplied;

            Clear();
        }

        private void HandleApplied(CardEffect effect, Actor target)
        {
            if (effect == null || target != _actor) return;
            if (effect.ActiveIcon == null || effect.RemainingOn(target) <= 0f) return;

            _active.Remove(effect);
            _active.Add(effect);
        }

        private void HandleRoleApplied(FactionType faction)
        {
            Clear();
        }

        private void Update()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i] == null || _active[i].RemainingOn(_actor) <= 0f) _active.RemoveAt(i);
            }

            CardEffect current = _active.Count > 0 ? _active[_active.Count - 1] : null;
            if (current != _shown) Display(current);

            if (_shown != null) Blink(_shown.RemainingOn(_actor));
        }

        private void LateUpdate()
        {
            if (_icon == null || _actor == null) return;

            _icon.position = _actor.CardAnchorPosition + _offset;
            _icon.rotation = Quaternion.identity;
        }

        private void Display(CardEffect effect)
        {
            _shown = effect;
            DOTween.Kill(_icon);

            if (effect == null)
            {
                _renderer.color = _baseColor;
                _icon.DOScale(Vector3.zero, _hideDuration)
                    .SetEase(Ease.InBack)
                    .SetTarget(_icon)
                    .OnComplete(() =>
                    {
                        _icon.gameObject.SetActive(false);
                        _icon.localScale = _baseScale;
                    });
                return;
            }

            _renderer.sprite = effect.ActiveIcon;
            _renderer.color = _baseColor;

            _icon.gameObject.SetActive(true);
            _icon.localScale = Vector3.zero;
            _icon.DOScale(_baseScale, _popDuration).SetEase(Ease.OutBack).SetTarget(_icon);
        }

        private void Blink(float remaining)
        {
            Color color = _baseColor;

            if (remaining < _blinkBelow)
            {
                float wave = 0.5f + 0.5f * Mathf.Cos(Time.time * _blinkFrequency * Mathf.PI * 2f);
                color.a = _baseColor.a * Mathf.Lerp(0.25f, 1f, wave);
            }

            _renderer.color = color;
        }

        private void Clear()
        {
            _active.Clear();
            _shown = null;

            if (_icon == null) return;

            DOTween.Kill(_icon);
            _icon.localScale = _baseScale;
            _renderer.color = _baseColor;
            _icon.gameObject.SetActive(false);
        }
    }
}
