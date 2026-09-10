using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SpawnHazardEffect", menuName = "Scriptable Objects/CardEffect/SpawnHazardEffect")]
    public class SpawnHazardEffect : CardEffect
    {
        [Header("Hazard")]
        [SerializeField] private GameObject _hazardPrefab;

        [Tooltip("Nombre d'objets semes a chaque ramassage.")]
        [SerializeField, Min(1)] private int _count = 3;

        [Tooltip("Rayon de dispersion autour du ramasseur si aucun ArenaBounds n'est present " +
                 "dans la scene.")]
        [SerializeField, Min(0f)] private float _fallbackRadius = 6f;

        protected override List<Actor> ResolveTargets(Actor collector)
        {
            return new List<Actor> { collector };
        }

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (_hazardPrefab == null)
            {
                Debug.LogError("[SpawnHazardEffect] " + name + " n'a pas de prefab.", this);
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                Instantiate(_hazardPrefab, PickPosition(collector), Quaternion.identity);
            }
        }

        private Vector2 PickPosition(Actor collector)
        {
            if (ArenaBounds.Instance != null) return ArenaBounds.Instance.RandomPoint();

            return (Vector2)collector.transform.position + Random.insideUnitCircle * _fallbackRadius;
        }
    }
}
