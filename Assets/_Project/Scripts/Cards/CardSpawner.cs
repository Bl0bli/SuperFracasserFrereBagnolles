using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> _spawnPoints;

        [Header("Cartes pouvoir")]
        [SerializeField] private List<CardDefinition> _cardDefinitions;
        [SerializeField] private CardPickup _cardPrefab;

        [Header("Cartes bornes")]
        [SerializeField] private List<BorneCardDefinition> _borneDefinitions;
        [SerializeField] private BornePickUp _bornePrefab;

        private float _spawnInterval = 10f;

        private void Start()
        {
            _spawnInterval = MatchManager.Instance.Settings.PowerUpInterval;
            MatchManager.Instance.OnMatchStarted += LaunchRoutine;
        }

        private void OnDestroy()
        {
            if (MatchManager.Instance != null) MatchManager.Instance.OnMatchStarted -= LaunchRoutine;
        }

        private void LaunchRoutine()
        {
            StartCoroutine(SpawnRoutine());
        }
        
        private void PickWeightedCard()
        {
            int totalWeight = 0;

            if (_cardDefinitions != null)
            {
                foreach (CardDefinition card in _cardDefinitions)
                {
                    if (card != null) totalWeight += card.SpawnWeight;
                }
            }

            if (_borneDefinitions != null)
            {
                foreach (BorneCardDefinition borne in _borneDefinitions)
                {
                    if (borne != null) totalWeight += borne.SpawnWeight;
                }
            }

            if (totalWeight <= 0)
            {
                Debug.LogWarning("[CardSpawner] Aucune carte a tirer : les deux listes sont vides " +
                                 "ou tous les poids valent 0.", this);
                return;
            }

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            if (_cardDefinitions != null)
            {
                foreach (CardDefinition card in _cardDefinitions)
                {
                    if (card == null) continue;

                    cumulative += card.SpawnWeight;
                    if (roll < cumulative)
                    {
                        SpawnCard(card);
                        return;
                    }
                }
            }

            if (_borneDefinitions != null)
            {
                foreach (BorneCardDefinition borne in _borneDefinitions)
                {
                    if (borne == null) continue;

                    cumulative += borne.SpawnWeight;
                    if (roll < cumulative)
                    {
                        SpawnBorne(borne);
                        return;
                    }
                }
            }
        }

        private Vector3 PickSpawnPoint()
        {
            if (_spawnPoints == null || _spawnPoints.Count == 0)
            {
                Debug.LogError("[CardSpawner] Aucun point de spawn assigne.", this);
                return transform.position;
            }

            Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            return point != null ? point.position : transform.position;
        }

        private void SpawnCard(CardDefinition card)
        {
            if (_cardPrefab == null)
            {
                Debug.LogError("[CardSpawner] Aucun prefab de carte pouvoir.", this);
                return;
            }

            Instantiate(_cardPrefab, PickSpawnPoint(), Quaternion.identity).SetCardDefinition(card);
        }

        private void SpawnBorne(BorneCardDefinition borne)
        {
            if (_bornePrefab == null)
            {
                Debug.LogError("[CardSpawner] Aucun prefab de carte borne.", this);
                return;
            }

            Instantiate(_bornePrefab, PickSpawnPoint(), Quaternion.identity).SetDefinition(borne);
        }

        private IEnumerator SpawnRoutine()
        {
            MatchSettings settings = MatchManager.Instance.Settings;

            while (MatchManager.Instance != null && MatchManager.Instance.State == MatchState.Playing)
            {
                PickWeightedCard();

                float wait = settings != null && settings.RandomBetweenValues
                    ? Random.Range(settings.PowerUpIntervalMin, settings.PowerUpIntervalMax)
                    : _spawnInterval;

                yield return new WaitForSeconds(wait);
            }
        }
    }
}
