using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Game
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance { get; private set; }

        [Header("Reglages")]
        [SerializeField] private MatchSettings _settings;

        [Header("Evenements inspecteur")]
        [Tooltip("Pour brancher du son ou un VFX depuis la scene, sans ecrire de code.")]
        [SerializeField] private UnityEvent _onMatchStarted;
        [SerializeField] private UnityEvent _onMatchEnded;

        private readonly List<Actor> _players = new List<Actor>();

        // Tous les acteurs instancies, morts compris : sert au nettoyage entre deux parties.
        private readonly List<Actor> _spawned = new List<Actor>();
        private readonly List<Actor> _cars = new List<Actor>();
        private Actor _cthulhu;
        private MatchState _state = MatchState.Warmup;
        private float _remainingTime;
        private int _entityBornes;
        private int _carBornes;
        private int _lastTickedSecond = -1;

        #region Events

        public event Action OnMatchStarted;
        public event Action<float> OnTimerTick;
        public event Action<Actor> OnActorDied;
        public event Action<FactionType, int> OnBornesChanged;
        public event Action<FactionType> OnMatchEnded;
        public event Action<int> OnCountdownTick;
        public event Action<MatchState> OnStateChanged;
        
        public event Action OnLobbyReset;

        public event Action<Actor> OnActorRegistered;

        #endregion
        
        #region Properties
        public IReadOnlyList<Actor> Players => _players;
        public IReadOnlyList<Actor> Cars => _cars;
        public Actor Cthulhu => _cthulhu;
        public MatchState State => _state;
        public float RemainingTime => _remainingTime;
        public int EntityBornes => _entityBornes;
        public int CarBornes => _carBornes;
        public int BorneObjective => BorneGoal;
        public bool CanStart => _state == MatchState.Warmup && _players.Count >= MinPlayers;
        
        public MatchSettings Settings => _settings;
        
        #endregion

        [Header("UI")]
        [SerializeField] private TMP_Text _countdownText;

        private int MinPlayers => _settings != null ? _settings.MinPlayers : 2;
        private int CountdownSeconds => _settings != null ? _settings.CountdownSeconds : 3;
        private float MatchDuration => _settings != null ? _settings.MatchDuration : 180f;
        private int BorneGoal => _settings != null ? _settings.BorneGoal : 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[MatchManager] Un second MatchManager a ete detruit.", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (_settings == null)
            {
                Debug.LogError("[MatchManager] Aucun MatchSettings assigne : valeurs par defaut utilisees.", this);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (_state != MatchState.Playing) return;

            _remainingTime -= Time.deltaTime;

            // Un tick par seconde entiere, pas un par frame : le HUD n'a rien a filtrer.
            int second = Mathf.Max(0, Mathf.CeilToInt(_remainingTime));
            if (second != _lastTickedSecond)
            {
                _lastTickedSecond = second;
                OnTimerTick?.Invoke(_remainingTime);
            }

            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                EvaluateVictory();
            }
        }

        public void Register(Actor actor)
        {
            if (actor == null || _players.Contains(actor)) return;

            _players.Add(actor);
            if (!_spawned.Contains(actor)) _spawned.Add(actor);
            OnActorRegistered?.Invoke(actor);

            if (actor.Health != null)
            {
                actor.Health.OnDied += HandleActorDied;
            }
            else
            {
                Debug.LogWarning("[MatchManager] " + actor.name + " n'a pas de Health : mort non suivie.", actor);
            }

        }

        public void RequestStart()
        {
            if (_state != MatchState.Warmup)
            {
                return;
            }

            if (_players.Count < MinPlayers)
            {
                return;
            }

            StartCoroutine(StartSequence());
        }

        public void AddBornes(int amount)
        {
            AddBornes(FactionType.Cthulhu, amount);
        }

        public void AddBornes(FactionType faction, int amount)
        {
            if (_state != MatchState.Playing || amount <= 0) return;

            if (faction == FactionType.Cthulhu) _entityBornes += amount;
            else _carBornes += amount;

            OnBornesChanged?.Invoke(faction, faction == FactionType.Cthulhu ? _entityBornes : _carBornes);

            EvaluateVictory();
        }
        public void ResetToLobby()
        {
            StopAllCoroutines();

            ClearArena();
            ClearPlayers();

            _entityBornes = 0;
            _carBornes = 0;
            _remainingTime = 0f;
            _lastTickedSecond = -1;

            SetState(MatchState.Warmup);
            OnLobbyReset?.Invoke();
        }

        private void ClearPlayers()
        {
            foreach (Actor actor in _spawned)
            {
                if (actor == null) continue;

                if (actor.Health != null) actor.Health.OnDied -= HandleActorDied;
                Destroy(actor.gameObject);
            }

            _spawned.Clear();
            _players.Clear();
            _cars.Clear();
            _cthulhu = null;
        }

        private static void ClearArena()
        {
            DestroyAll(FindObjectsByType<CardPickup>(FindObjectsSortMode.None));
            DestroyAll(FindObjectsByType<BornePickUp>(FindObjectsSortMode.None));
            DestroyAll(FindObjectsByType<Projectile>(FindObjectsSortMode.None));
            DestroyAll(FindObjectsByType<OilPuddle>(FindObjectsSortMode.None));
            DestroyAll(FindObjectsByType<CardVisual>(FindObjectsSortMode.None));
        }

        private static void DestroyAll<T>(T[] items) where T : Component
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null) Destroy(items[i].gameObject);
            }
        }

        private IEnumerator StartSequence()
        {
            SetState(MatchState.Starting);

            Actor chosen = _players[UnityEngine.Random.Range(0, _players.Count)];
            _cars.Clear();
            _cthulhu = null;

            foreach (Actor actor in _players)
            {
                bool isEntity = actor == chosen;
                FactionType faction = isEntity ? FactionType.Cthulhu : FactionType.Car;
                RoleBinder binder = actor.GetComponent<RoleBinder>();
                if (binder != null)
                {
                    binder.Apply(faction);
                }
                else
                {
                    Debug.LogWarning("[MatchManager] " + actor.name + " n'a pas de RoleBinder.", actor);
                }
                if (isEntity) _cthulhu = actor;
                else _cars.Add(actor);
                Freeze(actor, CountdownSeconds + 0.1f);
            }

            for (int i = CountdownSeconds; i > 0; i--)
            {
                if (_countdownText != null)
                {
                    _countdownText.gameObject.SetActive(true);
                    _countdownText.text = i.ToString();
                }
                OnCountdownTick?.Invoke(i);
                yield return new WaitForSeconds(1f);
            }
            OnCountdownTick?.Invoke(0);
            _remainingTime = MatchDuration;
            _lastTickedSecond = -1;
            _entityBornes = 0;
            _carBornes = 0;
            SetState(MatchState.Playing);
            OnMatchStarted?.Invoke();
            _onMatchStarted?.Invoke();
            if (_countdownText != null) _countdownText.gameObject.SetActive(false);
        }

        private void HandleActorDied(Actor actor)
        {
            if (actor == null) return;

            if (actor.Health != null) actor.Health.OnDied -= HandleActorDied;

            _players.Remove(actor);

            if (actor.Faction == FactionType.Cthulhu) _cthulhu = null;
            else _cars.Remove(actor);

            OnActorDied?.Invoke(actor);
            EvaluateVictory();
        }

        private void EvaluateVictory()
        {
            if (_state != MatchState.Playing) return;
 
            //serie de IF effectué par un professionnel, flemme d'expliquer mais à éviter Yanis
            if (_cthulhu == null)
            {
                EndMatch(FactionType.Car, "l'entite est eliminee");
                return;
            }
            if (_cars.Count == 0)
            {
                EndMatch(FactionType.Cthulhu, "toutes les voitures sont eliminees");
                return;
            }
            if (BorneGoal > 0 && _carBornes >= BorneGoal)
            {
                EndMatch(FactionType.Car, "les voitures atteignent " + BorneGoal + " bornes");
                return;
            }
            if (BorneGoal > 0 && _entityBornes >= BorneGoal)
            {
                EndMatch(FactionType.Cthulhu, "l'entite atteint " + BorneGoal + " bornes");
                return;
            }
            if (_remainingTime <= 0f)
            {
                DecideOnBornes();
            }
        }

        private void DecideOnBornes()
        {
            if (_carBornes > _entityBornes)
            {
                EndMatch(FactionType.Car, "chrono ecoule, les voitures menent " +
                                          _carBornes + " a " + _entityBornes);
                return;
            }

            if (_entityBornes > _carBornes)
            {
                EndMatch(FactionType.Cthulhu, "chrono ecoule, l'entite mene " +
                                              _entityBornes + " a " + _carBornes);
                return;
            }

            bool entityWins = _settings == null || _settings.EntityWinsTies;

            EndMatch(entityWins ? FactionType.Cthulhu : FactionType.Car,
                "chrono ecoule, egalite a " + _carBornes + " bornes");
        }

        private void EndMatch(FactionType winner, string reason)
        {
            SetState(MatchState.Ended);

            foreach (Actor actor in _players)
            {
                Freeze(actor, 3600f);
            }

            OnMatchEnded?.Invoke(winner);
            _onMatchEnded?.Invoke();
            Debug.Log("[MatchManager] " + reason);
        }
        
        private static void Freeze(Actor actor, float duration) //freeze un actor (ex pendant le decompte)
        {
            if (actor != null && actor.StatusController != null)
            {
                actor.StatusController.Apply(StatusType.Stunned, duration);
            }
        }

        private void SetState(MatchState next)
        {
            if (_state == next) return;

            MatchState previous = _state;
            _state = next;

            OnStateChanged?.Invoke(next);
        }
        public float getCurrentTime() //retourne le temps restant du match pour DisplayTimer
        {
            return _remainingTime;
        }

        public int getBornes() //compteur de l'entite, conserve pour compatibilite
        {
            return _entityBornes;
        }
        
    }
}
