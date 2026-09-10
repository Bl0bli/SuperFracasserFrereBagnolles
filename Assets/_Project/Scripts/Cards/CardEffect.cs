using UnityEngine;

namespace Game
{
    
public abstract class CardEffect : ScriptableObject
{
    public abstract void Apply(Actor target);
    [SerializeField] protected float _power;
    [SerializeField] protected float _duration;
    
}
}

