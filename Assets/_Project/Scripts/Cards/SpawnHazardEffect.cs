using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //spawn des trucs au hasard sur la scene
    [CreateAssetMenu(fileName = "SpawnHazardEffect", menuName = "Scriptable Objects/CardEffect/SpawnHazardEffect")]
    public class SpawnHazardEffect : CardEffect
    {
        [Header("Hazard")]
        [SerializeField] private GameObject _hazardPrefab;
        [SerializeField, Min(1)] private int _count = 3;

        [Tooltip("Rayon de dispersion autour du ramasseur si aucun ArenaBounds n'est present.")]
        [SerializeField] private float _fallbackRadius = 6f;
        
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
