using System;
using UnityEngine;

namespace Game
{
    public class ShieldController : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private StatusEffectController _statusController;
        [SerializeField] private Actor _actor;

        [Tooltip("Trace l'activation, l'absorption et l'expiration du bouclier.")]
        [SerializeField] private bool _verboseLogs = true;

        private float _expiresAt;
        private bool _broken;
        private bool _frontalOnly;
        private int _healOnSurvive;

        public event Action OnShieldUp;
        public event Action<DamageInfos> OnShieldBroken;
        public event Action<bool> OnShieldExpired;

        public bool IsActive => !_broken && Time.time < _expiresAt;
        public float Remaining => Mathf.Max(0f, _expiresAt - Time.time);

        private string Label => _actor != null ? name + " [" + _actor.Faction + "]" : name;

        private void Awake()
        {
            if (_health == null) _health = GetComponent<Health>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
            if (_actor == null) _actor = GetComponent<Actor>();
        }

        public void Activate(float duration, bool frontalOnly, int healOnSurvive)
        {
            _expiresAt = Time.time + duration;
            _broken = false;
            _frontalOnly = frontalOnly;
            _healOnSurvive = healOnSurvive;

            if (_statusController != null) _statusController.Apply(StatusType.Shielded, duration);

            OnShieldUp?.Invoke();

            if (_verboseLogs)
            {
                Debug.Log("[Shield] " + Label + " : bouclier actif " + duration.ToString("F1") + "s" +
                          (frontalOnly ? " (frontal)" : "") + ", rend " + healOnSurvive + " PV s'il tient.", this);
            }
        }

        public bool TryAbsorb(DamageInfos infos)
        {
            if (!IsActive) return false;

            if (_frontalOnly && Vector2.Dot(infos.Direction, transform.up) >= 0f)
            {
                if (_verboseLogs) Debug.Log("[Shield] " + Label + " : coup dans le dos, non bloque.", this);
                return false;
            }

            _broken = true;
            if (_statusController != null) _statusController.Remove(StatusType.Shielded);

            OnShieldBroken?.Invoke(infos);
            OnShieldExpired?.Invoke(false);

            if (_verboseLogs) Debug.Log("[Shield] " + Label + " : bouclier casse, " + infos.Amount + " degats absorbes.", this);

            return true;
        }

        private void Update()
        {
            if (_broken || _expiresAt <= 0f || Time.time < _expiresAt) return;

            _expiresAt = 0f;

            if (_statusController != null) _statusController.Remove(StatusType.Shielded);

            if (_healOnSurvive > 0 && _health != null) _health.Heal(_healOnSurvive);

            OnShieldExpired?.Invoke(true);

            if (_verboseLogs)
            {
                Debug.Log("[Shield] " + Label + " : bouclier tenu, " + _healOnSurvive + " PV rendus | PV " +
                          (_health != null ? _health.Current.ToString("F0") + "/" + _health.Max.ToString("F0") : "?"), this);
            }
        }
    }
}
