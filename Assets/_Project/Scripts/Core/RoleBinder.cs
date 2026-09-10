using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    //setup complet de chaque joueur au tirage de la faction
    public class RoleBinder : MonoBehaviour
    {
        [SerializeField] private Actor _actor;
        [SerializeField] private StatBlock _stats;
        [SerializeField] private Rigidbody2D _rb;

        [Header("Role voiture")]
        [SerializeField] private CharacterStats _carStats;
        [SerializeField] private GameObject _carVisual;
        [Tooltip("Composants actifs uniquement en voiture : CarController, CarMotor, RamDamage, " +
                 "un collider propre a la voiture...")]
        [SerializeField] private Behaviour[] _carBehaviours;

        [Header("Role entite")]
        [SerializeField] private CharacterStats _entityStats;
        [SerializeField] private GameObject _entityVisual;
        [Tooltip("Composants actifs uniquement en entite : EntityController, EntityMotor, " +
                 "TentacleAttack, un collider plus large...")]
        [SerializeField] private Behaviour[] _entityBehaviours;

        [Header("Transformation")]
        [Tooltip("Grossissement bref au changement de role, en attendant la vraie animation.")]
        [SerializeField] private bool _punchOnApply = true;
        [SerializeField] private float _punchScale = 1.6f;
        [SerializeField] private float _punchDuration = 0.45f;

        [Header("Debug")]
        [Tooltip("Trace la masse et la force de poussee effectivement appliquees a chaque role.")]
        [SerializeField] private bool _logBodySettings = true;

        //Point d'accroche pour les VFX, le son et l'Animator de transformation
        public event Action<FactionType> OnRoleApplied;

        private void Awake()
        {
            if (_actor == null) _actor = GetComponent<Actor>();
            if (_stats == null) _stats = GetComponent<StatBlock>();
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            
            Apply(FactionType.Car, false);
        }

        public void Apply(FactionType faction)
        {
            Apply(faction, _punchOnApply);
        }

        private void Apply(FactionType faction, bool animate)
        {
            bool isEntity = faction == FactionType.Cthulhu;
            _actor.SetFaction(faction);
            CharacterStats stats = isEntity ? _entityStats : _carStats;
            if (stats == null)
            {
                Debug.LogError("[RoleBinder] Aucun CharacterStats pour le role " + faction +
                               " sur " + name + " : masse, damping et PV inchanges.", this);
            }
            else
            {
                _stats.SetBaseStats(stats);
                if (_actor.Health != null) _actor.Health.ResetToMax();
                ApplyBodySettings(faction);
            }

            if (_carVisual != null) _carVisual.SetActive(!isEntity);
            if (_entityVisual != null) _entityVisual.SetActive(isEntity);
            SetEnabled(_carBehaviours, !isEntity);
            SetEnabled(_entityBehaviours, isEntity);
            if (animate && isActiveAndEnabled)
            {
                Transform visualsRoot = isEntity && _entityVisual != null
                    ? _entityVisual.transform
                    : transform;

                StopAllCoroutines();
                StartCoroutine(Punch(visualsRoot));
            }
            OnRoleApplied?.Invoke(faction);
        }

        private void ApplyBodySettings(FactionType faction)
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();

            if (_rb == null)
            {
                Debug.LogError("[RoleBinder] Aucun Rigidbody2D sur " + name +
                               " : la masse du role n'est pas appliquee.", this);
                return;
            }

            if (_rb.useAutoMass)
            {
                //on mange pas de ce pain là (ça nique notre physique custom)
                Debug.LogError("[RoleBinder] Auto Mass est coche sur le Rigidbody2D de " + name +
                               " : Unity ignore la masse du role. Decochez-le.", this);
                return;
            }

            _rb.mass = _stats.Get(StatType.Mass);
            _rb.linearDamping = _stats.Get(StatType.LinearDamping);

            if (_logBodySettings)
            {
                /*Debug.Log("[RoleBinder] " + name + " -> role " + faction +
                          " | masse " + _rb.mass +
                          " | damping " + _rb.linearDamping +
                          " | force de poussee " + (_stats.Get(StatType.Acceleration) * _rb.mass), this);*/
            }
        }

        private static void SetEnabled(Behaviour[] behaviours, bool value)
        {
            if (behaviours == null) return;
            foreach (Behaviour b in behaviours)
            {
                if (b != null) b.enabled = value;
            }
        }

        private IEnumerator Punch(Transform target)
        {
            Vector3 baseScale = Vector3.one;
            float elapsed = 0f;
            while (elapsed < _punchDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _punchDuration;
                float curve = Mathf.Sin(t * Mathf.PI);
                target.localScale = baseScale * (1f + (_punchScale - 1f) * curve);
                yield return null;
            }
            target.localScale = baseScale;
        }
    }
}
