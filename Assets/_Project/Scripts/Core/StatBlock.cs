using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatBlock : MonoBehaviour
    {
        [SerializeField] private List<StatModifier> _modifiers;
        
        public float Get(StatType stat)
        {
            StatModifier m = _modifiers.Find(modifier => modifier.Stats == stat);
            if(m.Stats != StatType.NULL) return m.Value;
            return 0;
        }

        public void AddModifier(StatModifier modifier)
        {
            //TODO implémenter
        }

        public void RemoveModifier(StatModifier modifier)
        {
            //TODO implémenter
        }
        
    }
}
