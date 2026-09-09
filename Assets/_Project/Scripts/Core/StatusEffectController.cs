using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusEffectController
    {
        private Dictionary<StatusType, float> _activeStatus;
        
        public event Action<StatusType> OnStatusChanged;

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
            if(Has(type))
            {
                _activeStatus.Remove(type);
                OnStatusChanged?.Invoke(type);
            }
        }
    }
}
