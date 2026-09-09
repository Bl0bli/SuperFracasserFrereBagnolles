using UnityEngine;

namespace Game
{
    //gestion de l'apparence du joueur en fonction de sa faction (fonction faction haha c'est drole, il est minuit tuer moi)
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private PlayerSkinSet _skins;
        [SerializeField] private RoleBinder _roleBinder;
        [SerializeField] private Actor _actor;

        private int _playerIndex;

        private void Awake()
        {
            if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_roleBinder == null) _roleBinder = GetComponent<RoleBinder>();
            if (_actor == null) _actor = GetComponent<Actor>();
        }

        private void OnEnable()
        {
            if (_roleBinder != null) _roleBinder.OnRoleApplied += Refresh;
            if (_actor != null) Refresh(_actor.Faction);
        }

        private void OnDisable()
        {
            if (_roleBinder != null) _roleBinder.OnRoleApplied -= Refresh;
        }

        public void SetPlayerIndex(int index)
        {
            _playerIndex = index;
            if (_actor != null) Refresh(_actor.Faction);
        }

        private void Refresh(FactionType faction)
        {
            if (_renderer == null || _skins == null) return;

            Sprite sprite = _skins.GetSprite(faction, _playerIndex);
            if (sprite != null) _renderer.sprite = sprite;

            _renderer.color = _skins.GetTint(faction, _playerIndex);
        }
    }
}
