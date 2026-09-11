using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Point unique du son. Tous les clips se reglent ici, et c'est lui qui s'abonne
    /// aux evenements du jeu : aucun script de gameplay ne connait le son.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        [Serializable]
        public class MusicEntry
        {
            public MatchState State;
            public AudioClip Clip;
            [Range(0f, 1f)] public float Volume = 0.7f;
        }

        [Serializable]
        public class SfxEntry
        {
            public SoundId Id;

            [Tooltip("Plusieurs clips : un est tire au hasard a chaque lecture.")]
            public AudioClip[] Clips;

            [Range(0f, 1f)] public float Volume = 1f;

            [Tooltip("Pitch tire entre ces deux bornes, pour eviter la repetition mecanique.")]
            public Vector2 Pitch = new Vector2(0.95f, 1.05f);

            [Tooltip("Delai minimum entre deux lectures de ce son, en secondes.")]
            [Min(0f)] public float MinInterval = 0.04f;
        }

        public static SoundManager Instance { get; private set; }

        [Header("Musique")]
        [SerializeField] private List<MusicEntry> _music = new List<MusicEntry>();
        [SerializeField, Min(0f)] private float _crossfade = 1f;

        [Header("Effets")]
        [SerializeField] private List<SfxEntry> _sfx = new List<SfxEntry>();

        [Tooltip("Nombre de sons simultanes possibles.")]
        [SerializeField, Min(2)] private int _voices = 12;

        [Header("Moteur des vehicules")]
        [SerializeField] private AudioClip _engineLoop;
        [SerializeField, Range(0f, 1f)] private float _engineVolume = 0.35f;

        [Tooltip("Part du volume deja presente a l'arret, moteur au ralenti.")]
        [SerializeField, Range(0f, 1f)] private float _engineIdleRatio = 0.3f;

        [SerializeField] private Vector2 _enginePitch = new Vector2(0.8f, 1.7f);
        [SerializeField, Min(1f)] private float _engineReferenceSpeed = 9f;

        private readonly Dictionary<SoundId, SfxEntry> _byId = new Dictionary<SoundId, SfxEntry>();
        private readonly Dictionary<SoundId, float> _lastPlayed = new Dictionary<SoundId, float>();
        private readonly Dictionary<Actor, AudioSource> _engines = new Dictionary<Actor, AudioSource>();
        private readonly List<Actor> _hooked = new List<Actor>();

        private AudioSource[] _pool;
        private int _next;
        private AudioSource _musicA;
        private AudioSource _musicB;
        private bool _usingA = true;
        private Coroutine _fade;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[SoundManager] Un second SoundManager a ete ignore.", this);
                return;
            }

            Instance = this;

            foreach (SfxEntry entry in _sfx)
            {
                if (entry != null) _byId[entry.Id] = entry;
            }

            _pool = new AudioSource[_voices];
            for (int i = 0; i < _voices; i++)
            {
                _pool[i] = CreateSource("Sfx " + i, false);
            }

            _musicA = CreateSource("Music A", true);
            _musicB = CreateSource("Music B", true);
        }

        private void OnEnable()
        {
            StartCoroutine(HookMatchWhenReady());
        }

        private void OnDisable()
        {
            UnhookMatch();
            UnhookActors();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (_engineLoop == null || _engines.Count == 0) return;

            foreach (KeyValuePair<Actor, AudioSource> pair in _engines)
            {
                Actor actor = pair.Key;
                AudioSource source = pair.Value;

                if (actor == null || source == null) continue;

                float speed = actor.Rb != null ? actor.Rb.linearVelocity.magnitude : 0f;
                float speed01 = Mathf.Clamp01(speed / _engineReferenceSpeed);

                source.volume = _engineVolume * Mathf.Lerp(_engineIdleRatio, 1f, speed01);
                source.pitch = Mathf.Lerp(_enginePitch.x, _enginePitch.y, speed01);
            }
        }

        // ------------------------------------------------------------ API publique

        public void Play(SoundId id)
        {
            if (!_byId.TryGetValue(id, out SfxEntry entry) || entry.Clips == null || entry.Clips.Length == 0)
            {
                return;
            }

            if (_lastPlayed.TryGetValue(id, out float last) && Time.unscaledTime - last < entry.MinInterval)
            {
                return;
            }

            _lastPlayed[id] = Time.unscaledTime;

            AudioClip clip = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];
            if (clip == null) return;

            AudioSource source = _pool[_next];
            _next = (_next + 1) % _pool.Length;

            source.pitch = UnityEngine.Random.Range(entry.Pitch.x, entry.Pitch.y);
            source.PlayOneShot(clip, entry.Volume);
        }

        public void PlayMusic(MatchState state)
        {
            MusicEntry entry = _music.Find(m => m != null && m.State == state);

            AudioSource from = _usingA ? _musicA : _musicB;
            AudioSource to = _usingA ? _musicB : _musicA;

            if (entry == null || entry.Clip == null)
            {
                if (_fade != null) StopCoroutine(_fade);
                _fade = StartCoroutine(Crossfade(from, to, null, 0f));
                _usingA = !_usingA;
                return;
            }

            if (from.clip == entry.Clip && from.isPlaying) return;

            if (_fade != null) StopCoroutine(_fade);
            _fade = StartCoroutine(Crossfade(from, to, entry.Clip, entry.Volume));
            _usingA = !_usingA;
        }

        // ------------------------------------------------------------ abonnements

        private IEnumerator HookMatchWhenReady()
        {
            while (MatchManager.Instance == null) yield return null;

            MatchManager match = MatchManager.Instance;

            match.OnStateChanged += HandleStateChanged;
            match.OnCountdownTick += HandleCountdown;
            match.OnActorDied += HandleActorDied;
            match.OnBornesChanged += HandleBornes;
            match.OnMatchEnded += HandleMatchEnded;
            match.OnActorRegistered += HookActor;
            match.OnLobbyReset += HandleLobbyReset;

            foreach (Actor actor in match.Players) HookActor(actor);

            PlayMusic(match.State);
        }

        private void UnhookMatch()
        {
            MatchManager match = MatchManager.Instance;
            if (match == null) return;

            match.OnStateChanged -= HandleStateChanged;
            match.OnCountdownTick -= HandleCountdown;
            match.OnActorDied -= HandleActorDied;
            match.OnBornesChanged -= HandleBornes;
            match.OnMatchEnded -= HandleMatchEnded;
            match.OnActorRegistered -= HookActor;
            match.OnLobbyReset -= HandleLobbyReset;
        }

        private void HookActor(Actor actor)
        {
            if (actor == null || _hooked.Contains(actor)) return;

            _hooked.Add(actor);

            if (actor.Health != null) actor.Health.OnTakeDamage += HandleDamage;
            if (actor.Stats != null) actor.Stats.OnModifierAdded += HandleModifier;
            if (actor.StatusController != null) actor.StatusController.OnStatusChanged += HandleStatus;

            ShieldController shield = actor.GetComponent<ShieldController>();
            if (shield != null)
            {
                shield.OnShieldUp += HandleShieldUp;
                shield.OnShieldExpired += HandleShieldExpired;
            }

            ThrowAbility throwAbility = actor.GetComponent<ThrowAbility>();
            if (throwAbility != null) throwAbility.OnThrown += HandleThrow;

            TentacleAttack tentacle = actor.GetComponent<TentacleAttack>();
            if (tentacle != null) tentacle.OnAttack += HandleTentacle;

            StartEngine(actor);
        }

        private void UnhookActors()
        {
            foreach (Actor actor in _hooked)
            {
                if (actor == null) continue;

                if (actor.Health != null) actor.Health.OnTakeDamage -= HandleDamage;
                if (actor.Stats != null) actor.Stats.OnModifierAdded -= HandleModifier;
                if (actor.StatusController != null) actor.StatusController.OnStatusChanged -= HandleStatus;

                ShieldController shield = actor.GetComponent<ShieldController>();
                if (shield != null)
                {
                    shield.OnShieldUp -= HandleShieldUp;
                    shield.OnShieldExpired -= HandleShieldExpired;
                }

                ThrowAbility throwAbility = actor.GetComponent<ThrowAbility>();
                if (throwAbility != null) throwAbility.OnThrown -= HandleThrow;

                TentacleAttack tentacle = actor.GetComponent<TentacleAttack>();
                if (tentacle != null) tentacle.OnAttack -= HandleTentacle;
            }

            _hooked.Clear();
            _engines.Clear();
        }

        // ------------------------------------------------------------ reactions

        private void HandleStateChanged(MatchState state)
        {
            PlayMusic(state);
        }

        private void HandleCountdown(int value)
        {
            Play(value > 0 ? SoundId.Countdown : SoundId.Go);
        }

        private void HandleActorDied(Actor actor)
        {
            Play(SoundId.Death);
            StopEngine(actor);
        }

        private void HandleBornes(FactionType faction, int total)
        {
            Play(SoundId.Borne);
        }

        private void HandleMatchEnded(FactionType winner)
        {
            Play(SoundId.Victory);

            foreach (KeyValuePair<Actor, AudioSource> pair in _engines)
            {
                if (pair.Value != null) pair.Value.Stop();
            }
        }

        private void HandleLobbyReset()
        {
            UnhookActors();
            PlayMusic(MatchState.Warmup);
        }

        private void HandleDamage(DamageInfos infos)
        {
            Play(infos.Type == DamageType.Ram ? SoundId.Collision : SoundId.Hit);
        }

        private void HandleModifier(StatModifier modifier)
        {
            bool penalty = modifier.Modifier == ModifierMode.Multiply
                ? modifier.Value < 1f
                : modifier.Value < 0f;

            Play(penalty ? SoundId.PowerUpMalus : SoundId.PowerUpBonus);
        }

        private void HandleStatus(StatusType type)
        {
            if (type == StatusType.Shielded) return;

            Play(SoundId.PowerUpMalus);
        }

        private void HandleShieldUp()
        {
            Play(SoundId.ShieldUp);
        }

        private void HandleShieldExpired(bool survived)
        {
            if (!survived) Play(SoundId.ShieldBreak);
        }

        private void HandleThrow()
        {
            Play(SoundId.Throw);
        }

        private void HandleTentacle()
        {
            Play(SoundId.Tentacle);
        }

        // ------------------------------------------------------------ moteur

        private void StartEngine(Actor actor)
        {
            if (_engineLoop == null || actor == null || _engines.ContainsKey(actor)) return;

            GameObject holder = new GameObject("EngineLoop");
            holder.transform.SetParent(actor.transform, false);

            AudioSource source = holder.AddComponent<AudioSource>();
            source.clip = _engineLoop;
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 0f;
            source.Play();

            _engines[actor] = source;
        }

        private void StopEngine(Actor actor)
        {
            if (actor == null || !_engines.TryGetValue(actor, out AudioSource source)) return;

            if (source != null) source.Stop();
            _engines.Remove(actor);
        }

        // ------------------------------------------------------------ outils

        private AudioSource CreateSource(string label, bool loop)
        {
            GameObject holder = new GameObject(label);
            holder.transform.SetParent(transform, false);

            AudioSource source = holder.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;

            return source;
        }

        private IEnumerator Crossfade(AudioSource from, AudioSource to, AudioClip clip, float volume)
        {
            if (clip != null)
            {
                to.clip = clip;
                to.volume = 0f;
                to.Play();
            }

            float startFrom = from != null ? from.volume : 0f;
            float elapsed = 0f;

            while (elapsed < _crossfade)
            {
                elapsed += Time.unscaledDeltaTime;
                float k = _crossfade <= 0f ? 1f : elapsed / _crossfade;

                if (from != null) from.volume = Mathf.Lerp(startFrom, 0f, k);
                if (clip != null) to.volume = Mathf.Lerp(0f, volume, k);

                yield return null;
            }

            if (from != null)
            {
                from.Stop();
                from.clip = null;
            }

            _fade = null;
        }
    }
}
