using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/StatModifierEffect")]


public class StatModifierEffect : CardEffect
{
    public override void Apply(Actor Target)
    {
       //core script pour le changement de satistique 
    }
    //private StateType stat; TODO retirer les comm
    //private ModifierMode mode;
}
}
