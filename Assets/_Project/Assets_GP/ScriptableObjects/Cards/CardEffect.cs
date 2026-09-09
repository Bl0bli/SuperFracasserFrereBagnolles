using UnityEngine;

namespace Game
{
    
public abstract class CardEffect : ScriptableObject
{
    public abstract void Apply(Actor Target);
    private float power;
    private float duration;
    
}
}

