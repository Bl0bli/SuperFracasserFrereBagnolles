using System;
using UnityEngine;

namespace Game
{
    public class ThrowAbility : MonoBehaviour
    {
        [Tooltip("Projectile utilise quand la carte n'en impose pas un.")]
        [SerializeField] private Projectile _defaultProjectile;

        [SerializeField] private Actor _actor;
        [SerializeField] private StatusEffectController _statusController;

        [Tooltip("Point de depart du projectile. Vide : devant le vehicule.")]
        [SerializeField] private Transform _muzzle;

        [Tooltip("Distance devant le vehicule quand aucun muzzle n'est assigne.")]
        [SerializeField, Min(0f)] private float _muzzleOffset = 0.9f;

        [SerializeField] private bool _verboseLogs = true;

        private Projectile _activeProjectile;
        private float _activeUntil;
        private float _nextThrowTime;
        private float _cooldown = 0.5f;
        private bool _wasActive;

        public event Action<float> OnGranted;
        public event Action OnThrown;
        public event Action OnExpired;

        public bool IsActive => Time.time < _activeUntil;
        public float Remaining => Mathf.Max(0f, _activeUntil - Time.time);
        public bool CanThrow => IsActive && Time.time >= _nextThrowTime && !IsSilenced;
        public Projectile CurrentProjectile => _activeProjectile != null ? _activeProjectile : _defaultProjectile;

        private bool IsSilenced => _statusController != null && _statusController.Has(StatusType.Silenced);
        private string Label => _actor != null ? name + " [" + _actor.Faction + "]" : name;

        private void Awake()
        {
            if (_actor == null) _actor = GetComponent<Actor>();
            if (_statusController == null) _statusController = GetComponent<StatusEffectController>();
        }

        public void Grant(float duration, float cooldown)
        {
            Grant(null, duration, cooldown);
        }

        public void Grant(Projectile projectile, float duration, float cooldown)
        {
            _activeProjectile = projectile != null ? projectile : _defaultProjectile;
            _activeUntil = Time.time + duration;
            _cooldown = Mathf.Max(0.05f, cooldown);
            _nextThrowTime = 0f;
            _wasActive = true;

            OnGranted?.Invoke(duration);

            if (_verboseLogs)
            {
                string ammo = _activeProjectile != null ? _activeProjectile.name : "AUCUN PROJECTILE";
                Debug.Log("[Throw] " + Label + " : " + ammo + " pour " + duration.ToString("F1") +
                          "s, cooldown " + _cooldown.ToString("F2") + "s.", this);
            }
        }

        public bool TryThrow()
        {
            if (!CanThrow) return false;

            Projectile prefab = CurrentProjectile;

            if (prefab == null)
            {
                Debug.LogError("[ThrowAbility] Aucun projectile a lancer sur " + name +
                               " : ni la carte ni le prefab par defaut n'en fournissent.", this);
                return false;
            }

            _nextThrowTime = Time.time + _cooldown;

            Vector2 direction = transform.up;
            Vector3 origin = _muzzle != null ? _muzzle.position : transform.position + (Vector3)direction * _muzzleOffset;

            Instantiate(prefab, origin, transform.rotation).Launch(_actor, direction);

            OnThrown?.Invoke();

            if (_verboseLogs) Debug.Log("[Throw] " + Label + " lance " + prefab.name + ".", this);

            return true;
        }

        private void Update()
        {
            if (!_wasActive || IsActive) return;

            _wasActive = false;
            _activeProjectile = null;
            OnExpired?.Invoke();

            if (_verboseLogs) Debug.Log("[Throw] " + Label + " : lancer termine.", this);
        }
    }
}
