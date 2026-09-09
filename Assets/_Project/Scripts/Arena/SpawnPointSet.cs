using UnityEngine;

namespace Game
{
    public class SpawnPointSet : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPointsCar;
        [SerializeField] private Transform[] _spawnPointsCthulhu;

        public Transform GetSpawnPoint(FactionType faction, int indexInFaction)
        {
            Transform[] points = faction == FactionType.Cthulhu ? _spawnPointsCthulhu : _spawnPointsCar;

            if (points == null || points.Length == 0)
            {
                Debug.LogError("[SpawnPointSet] Aucun point de spawn pour la faction " + faction +
                               " : le joueur apparaitra sur le SpawnPointSet.", this);
                return transform;
            }

            int index = ((indexInFaction % points.Length) + points.Length) % points.Length;

            if (points[index] == null)
            {
                Debug.LogError("[SpawnPointSet] Le point " + index + " de la faction " + faction +
                               " n'est pas assigne.", this);
                return transform;
            }

            return points[index];
        }
    }
}
