using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GrantWeaponEffect", menuName = "Scriptable Objects/CardEffect/GrantWeaponEffect")]
    public class GrantWeaponEffect : CardEffect
    {
        [Header("Arme")]
        [SerializeField] private WeaponDefinition _weapon;

        public WeaponDefinition Weapon => _weapon;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (_weapon == null)
            {
                Debug.LogError("[GrantWeaponEffect] " + name + " n'a pas de WeaponDefinition.", this);
                return;
            }
            
            // TODO armes : quand WeaponSlot sera un MonoBehaviour, remplacer par
            Debug.LogWarning("[GrantWeaponEffect] " + _weapon.name +
                             " non equipee : WeaponSlot pas encore un composant.", this);
        }
    }
}
