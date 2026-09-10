using UnityEngine;

namespace Game
{
[CreateAssetMenu(fileName = "ApllyStatusEffect", menuName = "Scriptable Objects/CardEffect/ApllyStatusEffect")]

public class ApplyStatusEffect : CardEffect
{
    public override void Apply(Actor Target)
    {
        //core script pour le changement de statue
        
    }
    [SerializeField]
    private StatusType status;
}
}
