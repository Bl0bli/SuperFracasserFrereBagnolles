using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class ActorJuice : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _visuals;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Health _health;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private StatusEffectController _status;
        [SerializeField] private RoleBinder _binder;

        [Header("Tremblement moteur")]
        [Tooltip("Amplitude du tremblement a l'arret, moteur au ralenti.")]
        [SerializeField, Range(0f, 0.1f)] private float _idleWobble = 0.012f;

        [Tooltip("Amplitude du tremblement a pleine vitesse.")]
        [SerializeField, Range(0f, 0.2f)] private float _driveWobble = 0.05f;

        [Tooltip("Frequence du tremblement a l'arret, en oscillations par seconde.")]
        [SerializeField, Min(0.1f)] private float _idleFrequency = 8f;

        [Tooltip("Frequence du tremblement a pleine vitesse.")]
        [SerializeField, Min(0.1f)] private float _driveFrequency = 22f;

        [Tooltip("Vitesse consideree comme pleine vitesse pour doser le tremblement.")]
        [SerializeField, Min(1f)] private float _referenceSpeed = 9f;

        [Tooltip("Vitesse de rattrapage de l'amplitude et de la frequence.")]
        [SerializeField, Min(0.1f)] private float _wobbleSmoothing = 6f;

        [Header("Impact")]
        [SerializeField, Min(0f)] private float _hitPunch = 0.45f;
        [SerializeField, Min(0.05f)] private float _hitPunchDuration = 0.25f;
        [SerializeField] private Color _hitColor = Color.white;

        [Header("Bonus et malus")]
        [SerializeField] private Color _bonusColor = new Color(0.45f, 1f, 0.6f);
        [SerializeField] private Color _malusColor = new Color(0.75f, 0.3f, 1f);
        [SerializeField, Min(0f)] private float _reactionPunch = 0.3f;
        [SerializeField, Min(0.05f)] private float _flashDuration = 0.14f;

        [Header("Toupie")]
        [Tooltip("Nombre de tours COMPLETS. Un entier garantit que le vehicule retombe droit.")]
        [SerializeField, Min(1)] private int _spinTurns = 1;

        [SerializeField, Min(0.1f)] private float _spinDuration = 0.5f;

        private Vector3 _baseScale = Vector3.one;
        private Color _baseColor = Color.white;
        private Tween _flash;
        private Tween _spin;
        private float _phase;
        private float _amplitude;
        private float _frequency;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_health == null) _health = GetComponent<Health>();
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_status == null) _status = GetComponent<StatusEffectController>();
            if (_binder == null) _binder = GetComponent<RoleBinder>();

            if (_visuals == null)
            {
                Transform found = transform.Find("Visuals");
                _visuals = found != null ? found : transform;
            }

            if (_visuals == transform)
            {
                Debug.LogWarning("[ActorJuice] Aucun enfant Visuals sur " + name +
                                 " : l'animation deformerait le collider, elle est desactivee.", this);
                enabled = false;
                return;
            }

            if (_renderer == null) _renderer = _visuals.GetComponentInChildren<SpriteRenderer>(true);

            _baseScale = _visuals.localScale;
            if (_renderer != null) _baseColor = _renderer.color;

            _amplitude = _idleWobble;
            _frequency = _idleFrequency;
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnTakeDamage += HandleDamaged;
            if (_stats != null)
            {
                _stats.OnModifierAdded += HandleModifierAdded;
                _stats.OnModifierRemoved += HandleModifierRemoved;
            }
            if (_status != null) _status.OnStatusChanged += HandleStatusChanged;
            if (_binder != null) _binder.OnRoleApplied += HandleRoleApplied;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnTakeDamage -= HandleDamaged;
            if (_stats != null)
            {
                _stats.OnModifierAdded -= HandleModifierAdded;
                _stats.OnModifierRemoved -= HandleModifierRemoved;
            }
            if (_status != null) _status.OnStatusChanged -= HandleStatusChanged;
            if (_binder != null) _binder.OnRoleApplied -= HandleRoleApplied;

            DOTween.Kill(_visuals);
            _flash = null;
            _spin = null;
        }

        private void LateUpdate()
        {
            if (_visuals == null || _rb == null) return;

            // Un punch ou une toupie en cours a la priorite sur le tremblement.
            if (DOTween.IsTweening(_visuals)) return;

            float speed01 = Mathf.Clamp01(_rb.linearVelocity.magnitude / _referenceSpeed);

            float targetAmplitude = Mathf.Lerp(_idleWobble, _driveWobble, speed01);
            float targetFrequency = Mathf.Lerp(_idleFrequency, _driveFrequency, speed01);

            _amplitude = Mathf.MoveTowards(_amplitude, targetAmplitude, _wobbleSmoothing * Time.deltaTime);
            _frequency = Mathf.MoveTowards(_frequency, targetFrequency,
                _wobbleSmoothing * 8f * Time.deltaTime);

            _phase += _frequency * Time.deltaTime;

            // X et Y en opposition : le sprite vibre sans jamais s'allonger.
            float wobble = Mathf.Sin(_phase * Mathf.PI * 2f) * _amplitude;

            _visuals.localScale = new Vector3(
                _baseScale.x * (1f + wobble),
                _baseScale.y * (1f - wobble),
                _baseScale.z);
        }

        private void HandleDamaged(DamageInfos infos)
        {
            Punch(_hitPunch, _hitPunchDuration);
            Flash(_hitColor);
        }

        private void HandleRoleApplied(FactionType faction)
        {
            if (_renderer != null) _baseColor = _renderer.color;
        }

        private void HandleStatusChanged(StatusType type)
        {
            if (_status == null || !_status.Has(type)) return;

            if (type == StatusType.Shielded)
            {
                React(_bonusColor);
                return;
            }

            React(_malusColor);

            if (type == StatusType.Stunned || type == StatusType.Inverted) Spin();
        }

        private void HandleModifierAdded(StatModifier modifier)
        {
            bool penalty = IsPenalty(modifier);

            React(penalty ? _malusColor : _bonusColor);

            if (penalty && modifier.Stats == StatType.MoveSpeed) Spin();
        }

        private void HandleModifierRemoved(StatModifier modifier)
        {
            if (IsPenalty(modifier)) return;

            Flash(_bonusColor);
        }

        private static bool IsPenalty(StatModifier modifier)
        {
            return modifier.Modifier == ModifierMode.Multiply
                ? modifier.Value < 1f
                : modifier.Value < 0f;
        }

        private void React(Color color)
        {
            Punch(_reactionPunch, _hitPunchDuration);
            Flash(color);
        }

        private void Punch(float strength, float duration)
        {
            if (_visuals == null || strength <= 0f) return;

            _visuals.localScale = _baseScale;
            _visuals.DOPunchScale(_baseScale * strength, duration, 8, 0.7f)
                .SetTarget(_visuals)
                .OnComplete(() => _visuals.localScale = _baseScale);
        }

        private void Flash(Color color)
        {
            if (_renderer == null) return;

            _flash?.Kill();
            _renderer.color = color;

            _flash = DOTween.To(() => _renderer.color, c => _renderer.color = c, _baseColor, _flashDuration)
                .SetTarget(_visuals);
        }

        private void Spin()
        {
            if (_visuals == null) return;

            _spin?.Kill();
            _visuals.localEulerAngles = Vector3.zero;

            // Tours entiers : l'angle final est un multiple de 360, donc le vehicule
            // termine exactement droit, sans recalage visible.
            float angle = -360f * _spinTurns;

            _spin = _visuals.DOLocalRotate(new Vector3(0f, 0f, angle), _spinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetTarget(_visuals)
                .OnComplete(() => _visuals.localEulerAngles = Vector3.zero);
        }
    }
}
