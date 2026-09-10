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

        [Header("Debug")]
        [Tooltip("Trace dans la console chaque changement d'etat et chaque evenement du lobby. " +
                 "A decocher avant le build.")]
        [SerializeField] private bool _verboseLogs = true;

        private readonly List<Actor> _players = new List<Actor>();
        private readonly List<Actor> _cars = new List<Actor>();
        private Actor _cthulhu;
        private MatchState _state = MatchState.Warmup;
        private float _remainingTime;
        private int _bornes;
        private int _lastTickedSecond = -1;

        #region Events

        public event Action OnMatchStarted;
        public event Action<float> OnTimerTick;
        public event Action<Actor> OnActorDied;
        public event Action<int> OnBornesChanged;
        public event Action<FactionType> OnMatchEnded;
        public event Action<int> OnCountdownTick;
        public event Action<MatchState> OnStateChanged;

        #endregion
        
        #region Properties
        public IReadOnlyList<Actor> Players => _players;
        public IReadOnlyList<Actor> Cars => _cars;
        public Actor Cthulhu => _cthulhu;
        public MatchState State => _state;
        public float RemainingTime => _remainingTime;
        public int Bornes => _bornes;
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
            Log("Etat initial : " + _state + ". En attente de joueurs (minimum " + MinPlayers + ").");
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
                Log("Chrono ecoule.");
                EvaluateVictory();
            }
        }

        public void Register(Actor actor)
        {
            if (actor == null || _players.Contains(actor)) return;

            _players.Add(actor);

            if (actor.Health != null)
            {
                actor.Health.OnDied += HandleActorDied;
            }
            else
            {
                Debug.LogWarning("[MatchManager] " + actor.name + " n'a pas de Health : mort non suivie.", actor);
            }

            Log("Joueur enregistre : " + actor.name + " (" + _players.Count + "/" + MinPlayers +
                (CanStart ? ") - Start disponible." : ") - il en manque."));
        }

        public void RequestStart()
        {
            if (_state != MatchState.Warmup)
            {
                Log("Start ignore : la partie est deja en etat " + _state + ".");
                return;
            }

            if (_players.Count < MinPlayers)
            {
                Log("Start refuse : " + _players.Count + " joueur(s), il en faut " + MinPlayers + ".");
                return;
            }

            StartCoroutine(StartSequence());
        }

        public void AddBornes(int amount) //si on veut faire le systeme de borne pour le mechant
        {
            if (_state != MatchState.Playing || amount <= 0) return;

            _bornes += amount;
            OnBornesChanged?.Invoke(_bornes);
            Log("Bornes : " + _bornes + (BorneGoal > 0 ? " / " + BorneGoal : " (objectif desactive)"));

            EvaluateVictory();
        }

        private IEnumerator StartSequence()
        {
            SetState(MatchState.Starting);

            Actor chosen = _players[UnityEngine.Random.Range(0, _players.Count)];
            Log("Entite tiree au sort : " + chosen.name + " parmi " + _players.Count + " joueurs.");
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

            Log("Roles distribues : " + _cars.Count + " voiture(s) contre 1 entite. Decompte...");
            for (int i = CountdownSeconds; i > 0; i--)
            {
                _countdownText.text = i.ToString();
                Log("Decompte : " + i);
                OnCountdownTick?.Invoke(i);
                yield return new WaitForSeconds(1f);
            }
            Log("GO");
            OnCountdownTick?.Invoke(0);
            _remainingTime = MatchDuration;
            _lastTickedSecond = -1;
            _bornes = 0;
            SetState(MatchState.Playing);
            OnMatchStarted?.Invoke();
            _onMatchStarted?.Invoke();
            _countdownText.gameObject.SetActive(false);
        }

        private void HandleActorDied(Actor actor)
        {
            if (actor == null) return;

            if (actor.Health != null) actor.Health.OnDied -= HandleActorDied;

            _players.Remove(actor);

            if (actor.Faction == FactionType.Cthulhu) _cthulhu = null;
            else _cars.Remove(actor);

            Log("Mort de " + actor.name + " (" + actor.Faction + "). Restant : " +
                _cars.Count + " voiture(s), entite " + (_cthulhu == null ? "eliminee" : "vivante") + ".");

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
            if (BorneGoal > 0 && _bornes >= BorneGoal)
            {
                EndMatch(FactionType.Cthulhu, "les " + BorneGoal + " bornes sont atteintes");
                return;
            }
            if (_remainingTime <= 0f)
            {
                EndMatch(FactionType.Cthulhu, "le chrono est ecoule");
            }
        }

        private void EndMatch(FactionType winner, string reason)
        {
            SetState(MatchState.Ended);
            Log("Victoire " + (winner == FactionType.Car ? "des voitures" : "de l'entite") + " : " + reason + ".");

            foreach (Actor actor in _players)
            {
                Freeze(actor, 3600f);
            }

            OnMatchEnded?.Invoke(winner);
            _onMatchEnded?.Invoke();
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

            Log("Etat : " + previous + " -> " + next);
            OnStateChanged?.Invoke(next);
        }

        private void Log(string message)
        {
            if (_verboseLogs) Debug.Log("[MatchManager] " + message, this);
        }
        public float getCurrentTime() //retourne le temps restant du match pour DisplayTimer
        {
            return _remainingTime;
        }

        public int getBornes() //retourne le nombre de bornes pour DisplayBorne
        {
            return _bornes;
        }
        
    }
}
