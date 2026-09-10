using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GrantWeaponEffect", menuName = "Scriptable Objects/CardEffect/GrantWeaponEffect")]


public class GrantWeaponEffect : CardEffect
{
    WeaponSlot weaponSlot = new WeaponSlot(); 
    public override void Apply(Actor target)
    {
        //Core Script pour l'obtention d'une arme au joueur
        weaponSlot.weaponEquipped = weapon;
    }
    private WeaponDefinition weapon;
}

}