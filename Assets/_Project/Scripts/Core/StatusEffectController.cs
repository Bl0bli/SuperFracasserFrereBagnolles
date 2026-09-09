using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusEffectController : MonoBehaviour
    {
        private readonly Dictionary<StatusType, float> _activeStatus = new Dictionary<StatusType, float>();

        // on ne modifie pas le dictionnaire pendant qu'on l'enumere.
        private readonly List<StatusType> _buffer = new List<StatusType>();

        public event Action<StatusType> OnStatusChanged;

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
                    Remove(status);
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

        public void Remove(StatusType type)
        {
            if (Has(type))
            {
                _activeStatus.Remove(type);
                OnStatusChanged?.Invoke(type);
            }
        }
    }
}
