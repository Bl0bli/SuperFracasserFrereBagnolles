using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GrantWeaponEffect", menuName = "Scriptable Objects/GrantWeaponEffect")]


public class GrantWeaponEffect : CardEffect
{
    public override void Apply(Actor Target)
    {
        //Core Script pour l'obtention d'une arme au joueur 
    }
    //private WeaponDefinition weapon; TODO retirer les comm
}

}