using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    //gestion des manettes et profils
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerHub : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject _playerPrefab;

        [Header("Scene")]
        [SerializeField] private SpawnPointSet _spawns;

        private PlayerInputManager _playerInputManager;
        private int _joinCount;

        private void Awake()
        {
            _playerInputManager = GetComponent<PlayerInputManager>();

            if (_playerPrefab == null)
            {
                Debug.LogError("[PlayerHub] Aucun prefab joueur : aucun join possible.", this);
                return;
            }

            _playerInputManager.playerPrefab = _playerPrefab;
            _playerInputManager.onPlayerJoined += OnPlayerJoined;

            if (MatchManager.Instance != null) MatchManager.Instance.OnLobbyReset += HandleLobbyReset;
        }

        private void OnDestroy()
        {
            if (_playerInputManager != null)
            {
                _playerInputManager.onPlayerJoined -= OnPlayerJoined;
            }

            if (MatchManager.Instance != null) MatchManager.Instance.OnLobbyReset -= HandleLobbyReset;
        }

        private void HandleLobbyReset()
        {
            _joinCount = 0;
        }

        private void OnPlayerJoined(PlayerInput player)
        {
            if (_spawns != null)
            {
                Transform spawn = _spawns.GetSpawnPoint(FactionType.Car, _joinCount);
                player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            }
            else
            {
                Debug.LogWarning("[PlayerHub] Aucun SpawnPointSet assigne : spawn a l'origine.", this);
            }

            _joinCount++;

            Actor actor = player.GetComponent<Actor>();
            if (actor == null)
            {
                Debug.LogError("[PlayerHub] Le prefab " + player.name + " n'a pas d'Actor sur sa racine.", player);
                return;
            }
            
            PlayerVisual visual = player.GetComponent<PlayerVisual>();
            if (visual != null) visual.SetPlayerIndex(player.playerIndex);

            InputReader reader = player.GetComponent<InputReader>();
            if (reader != null) reader.StartPressed += HandleStartPressed;

            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.Register(actor);
            }
            else
            {
                Debug.LogWarning("[PlayerHub] Aucun MatchManager dans la scene : acteur non enregistre.", this);
            }
        }

        private void HandleStartPressed()
        {
            if (MatchManager.Instance != null) MatchManager.Instance.RequestStart();
        }
    }
}
