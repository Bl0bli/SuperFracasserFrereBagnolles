using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SpawnHazardEffect", menuName = "Scriptable Objects/CardEffect/SpawnHazardEffect")]


public class SpawnHazardEffect : CardEffect
{
    public override void Apply(Actor Target)
    {
        //core script pour les power up instantiable 
    }
    [SerializeField] 
    private GameObject hazardPrefab ;
    private int count ;
}

}
