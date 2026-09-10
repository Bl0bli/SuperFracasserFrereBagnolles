using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public enum WeaponType
    {
        Melee,
        Distance
    }

    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Scriptable Objects/WeaponDefinition")]
    public class WeaponDefinition : ScriptableObject
    {
        public WeaponType weaponType;
        public GameObject weponPrefab;
        [ShowIf("weaponType", WeaponType.Distance)]
        public int cadence;
        [ShowIf("weaponType", WeaponType.Distance)]
        public float portee;
        [ShowIf("weaponType", WeaponType.Distance)]
        public int munition;
        public int degats;
    }
}
