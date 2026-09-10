using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SpawnHazardEffect", menuName = "Scriptable Objects/CardEffect/SpawnHazardEffect")]


    public class SpawnHazardEffect : CardEffect
    {
        [SerializeField] private GameObject _hazardPrefab ;
        [SerializeField] private int _count ;
        public override void Apply(Actor target)
        {
            for (int i = 0; i < _count; i++)
            {
                GameObject hazard = Instantiate(_hazardPrefab, target.transform.position, Quaternion.identity);
                //TODO spawner hazard aleatoirement dans les bornes de la map
            }
        }
    }

}
