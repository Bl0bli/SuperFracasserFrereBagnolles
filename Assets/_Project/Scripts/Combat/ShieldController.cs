using System;
using UnityEngine;

namespace Game
{
    public class ShieldController : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private StatusEffectController _statusController;

        private float _expiresAt;
        private bool _broken;
        private bool _frontalOnly;
        private int _healOnSurvive;

        public event Action OnShieldUp;
        public event Action<DamageInfos> OnShieldBroken;
        public event Action<bool> OnShieldExpired;

        public bool IsActive => !_broken && Time.time < _expiresAt;
        public float Remaining => Mathf.Max(0f, _expiresAt - Time.time);

        private void Awake()
        {
            if (_health == null) _health = GetComponent<Health>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
        }

        public void Activate(float duration, bool frontalOnly, int healOnSurvive)
        {
            _expiresAt = Time.time + duration;
            _broken = false;
            _frontalOnly = frontalOnly;
            _healOnSurvive = healOnSurvive;

            if (_statusController != null) _statusController.Apply(StatusType.Shielded, duration);

            OnShieldUp?.Invoke();

        }

        public bool TryAbsorb(DamageInfos infos)
        {
            if (!IsActive) return false;

            if (_frontalOnly && Vector2.Dot(infos.Direction, transform.up) >= 0f)
            {
                return false;
            }

            _broken = true;
            if (_statusController != null) _statusController.Remove(StatusType.Shielded);

            OnShieldBroken?.Invoke(infos);
            OnShieldExpired?.Invoke(false);

            return true;
        }

        private void Update()
        {
            if (_broken || _expiresAt <= 0f || Time.time < _expiresAt) return;

            _expiresAt = 0f;

            if (_statusController != null) _statusController.Remove(StatusType.Shielded);

            if (_healOnSurvive > 0 && _health != null) _health.Heal(_healOnSurvive);

            OnShieldExpired?.Invoke(true);

        }
    }
}
