using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerHub : MonoBehaviour
    {
        [Header("Unity Components")]
        [SerializeField] private PlayerInputManager _playerInputManager;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _carPrefab;
        [SerializeField] private GameObject _cthulhuPrefab;

        [SerializeField] private SpawnPointSet _spawns;
        
        private void Awake()
        {
            _playerInputManager = GetComponent<PlayerInputManager>();
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
        }

        private void OnPlayerJoined(PlayerInput player)
        {
            
        }

        private FactionType AssignRole(int id)
        {
            Actor actor = Instantiate(
                _carPrefab, 
                _spawns.GetSpawnPoint(id).position, 
                Quaternion.identity
                ).GetComponent<Actor>();
            
            return FactionType.Car;
        }
    }
}
