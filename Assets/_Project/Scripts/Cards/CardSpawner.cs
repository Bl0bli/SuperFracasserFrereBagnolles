using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private List<CardDefinition> _cardDefinitions;
        [SerializeField] private CardPickup _cardPrefab;
        
        private float _spawnInterval = 10f;

        private void Start()
        {
            _spawnInterval = MatchManager.Instance.Settings.PowerUpInterval;
            MatchManager.Instance.OnMatchStarted += LaunchRoutine;
        }

        private void LaunchRoutine()
        {
            StartCoroutine(SpawnRoutine());
        }

        private void PickWeightedCard()
        {
            Debug.Log($"[CardSpawner] Picking card");
            int totalWeight = 0;
            foreach (CardDefinition card in _cardDefinitions)
            {
                totalWeight += card.SpawnWeight;
            }

            int randomValue = Random.Range(0, totalWeight);
            int cumulativeWeight = 0;

            foreach (CardDefinition card in _cardDefinitions)
            {
                cumulativeWeight += card.SpawnWeight;
                if (randomValue < cumulativeWeight)
                {
                    SpawnCard(card);
                    return;
                }
            }
        }

        private void SpawnCard(CardDefinition card)
        {
            Instantiate(_cardPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Count)].position, Quaternion.identity)
                .GetComponent<CardPickup>().SetCardDefinition(card);
            
            Debug.Log($"[CardSpawner] Spawned card {card.ID}");
        }

        private IEnumerator SpawnRoutine()
        {
            if (MatchManager.Instance.Settings.RandomBetweenValues)
            {
                Debug.Log($"[CardSpawner] Random interval between {MatchManager.Instance.Settings.PowerUpIntervalMin} and {MatchManager.Instance.Settings.PowerUpIntervalMax}");
                while(MatchManager.Instance.State == MatchState.Playing)
                {
                    PickWeightedCard();
                    yield return new WaitForSeconds(Random.Range(MatchManager.Instance.Settings.PowerUpIntervalMin, MatchManager.Instance.Settings.PowerUpIntervalMax));
                }
            }
            else
            {
                Debug.Log($"[CardSpawner] Fixed interval of {MatchManager.Instance.Settings.PowerUpInterval}");
                while(MatchManager.Instance.State == MatchState.Playing)
                {
                    PickWeightedCard();
                    yield return new WaitForSeconds(_spawnInterval);
                }
            }
        }
    }
}
