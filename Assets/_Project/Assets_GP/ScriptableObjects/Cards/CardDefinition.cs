using UnityEngine;

namespace Game
{
    

public class CardDefinition
{
   public string id;
   public int spawnWeight;
   private CardEffect playerEffect;
   private CardEffect entityEffect;

   public CardEffect GetEffect(FactionType f)
   {
    if (f == FactionType.Car)
    {
        return playerEffect;
    }
    return entityEffect;
}

}
}
