using UnityEngine;

namespace Game
{

    [CreateAssetMenu(fileName = "PlayerSkinSet", menuName = "Scriptable Objects/PlayerSkinSet")]
    public class PlayerSkinSet : ScriptableObject
    {
        [Tooltip("Un sprite par joueur, dans l'ordre des places. Le joueur 0 prend le premier.")]
        [SerializeField] private Sprite[] _carSprites;

        [Tooltip("Sprite du joueur transforme en entite, quelle que soit sa place.")]
        [SerializeField] private Sprite _entitySprite;

        [Tooltip("Teinte par place, si les sprites sont identiques et differencies par la couleur. " +
                 "Laisser vide pour ne rien teinter.")]
        [SerializeField] private Color[] _carTints;

        public Sprite GetSprite(FactionType faction, int playerIndex)
        {
            if (faction == FactionType.Cthulhu) return _entitySprite;
            if (_carSprites == null || _carSprites.Length == 0) return null;

            return _carSprites[Mathf.Clamp(playerIndex, 0, _carSprites.Length - 1)];
        }

        public Color GetTint(FactionType faction, int playerIndex)
        {
            if (faction == FactionType.Cthulhu) return Color.white;
            if (_carTints == null || _carTints.Length == 0) return Color.white;

            return _carTints[Mathf.Clamp(playerIndex, 0, _carTints.Length - 1)];
        }
    }
}
