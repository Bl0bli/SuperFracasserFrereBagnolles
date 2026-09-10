using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public class StatusEffectController : MonoBehaviour
    {
        [SerializeField] private Actor _actor;

        private readonly Dictionary<StatusType, float> _activeStatus = new Dictionary<StatusType, float>();
        private readonly List<StatusType> _buffer = new List<StatusType>();

        public event Action<StatusType> OnStatusChanged;

        public string Label => _actor != null ? name + " [" + _actor.Faction + "]" : name;

        private void Awake()
        {
            if (_actor == null) _actor = GetComponent<Actor>();
        }

        private void Update()
        {
            if (_activeStatus.Count == 0) return;

            _buffer.Clear();
            _buffer.AddRange(_activeStatus.Keys);

            foreach (StatusType status in _buffer)
            {
                _activeStatus[status] -= Time.deltaTime;

                if (_activeStatus[status] <= 0f)
                {
                    RemoveInternal(status);
                }
            }
        }

        public void Apply(StatusType type, float duration)
        {
            _activeStatus[type] = duration;
            OnStatusChanged?.Invoke(type);
        }

        public bool Has(StatusType type)
        {
            return _activeStatus.ContainsKey(type);
        }

        public float Remaining(StatusType type)
        {
            return _activeStatus.TryGetValue(type, out float value) ? value : 0f;
        }

        public void Remove(StatusType type)
        {
            RemoveInternal(type);
        }

        private void RemoveInternal(StatusType type)
        {
            if (!Has(type)) return;

            _activeStatus.Remove(type);
            OnStatusChanged?.Invoke(type);
            
        }
    }
}
