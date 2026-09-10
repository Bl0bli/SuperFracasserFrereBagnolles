using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BorneCardDefinition", menuName = "Scriptable Objects/BorneCardDefinition")]
    public class BorneCardDefinition : ScriptableObject
    {
        public string ID;

        [Tooltip("Poids de tirage, partage avec les cartes bonus.")]
        [Min(0)] public int SpawnWeight = 1;

        [Tooltip("Bornes creditees au camp qui ramasse la carte.")]
        [Min(1)] public int BorneValue = 25;

        [Tooltip("Face revelee apres le retournement, avec le nombre de bornes dessine dessus.")]
        public Sprite Icon;
    }
}
