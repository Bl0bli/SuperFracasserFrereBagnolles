using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CardDefinition", menuName = "Scriptable Objects/CardDefinition")]
    public class CardDefinition : ScriptableObject
    {
       public string ID;
       public int SpawnWeight;
       [SerializeField] CardEffect _playerEffect;
       [SerializeField] CardEffect _entityEffect;
    
       public CardEffect GetEffect(FactionType f)
       {
            if (f == FactionType.Car)
            {
                return _playerEffect;
            }
            return _entityEffect;
        }
    
    }
}
