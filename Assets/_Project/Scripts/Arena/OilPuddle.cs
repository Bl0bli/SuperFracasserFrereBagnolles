using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class OilPuddle : MonoBehaviour, IHazard
    {
        [Tooltip("Facteur applique a MoveSpeed tant qu'on est dedans. 0.5 = deux fois plus lent.")]
        [SerializeField, Range(0.05f, 1f)] private float _slowFactor = 0.5f;

        [Tooltip("Duree de vie de la flaque en secondes. 0 = permanente.")]
        [SerializeField, Min(0f)] private float _lifetime = 10f;

        [Tooltip("Decoche : l'entite traverse sans etre ralentie.")]
        [SerializeField] private bool _affectsEntity = false;

        [SerializeField] private bool _verboseLogs = true;

        private readonly HashSet<StatBlock> _inside = new HashSet<StatBlock>();
        private Actor _owner;

        public void Initialize(Actor owner)
        {
            _owner = owner;
        }

        private void Start()
        {
            if (_lifetime > 0f) Destroy(gameObject, _lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor == null || actor.Stats == null) return;
            if (!_affectsEntity && actor.Faction == FactionType.Cthulhu) return;
            if (!_inside.Add(actor.Stats)) return;

            actor.Stats.AddModifier(new StatModifier
            {
                Stats = StatType.MoveSpeed,
                Modifier = ModifierMode.Multiply,
                Value = _slowFactor,
                Duration = 0f,
                Source = this
            });

            if (_verboseLogs) Debug.Log("[Flaque] " + actor.name + " entre dans une flaque.", this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor == null || actor.Stats == null) return;
            if (!_inside.Remove(actor.Stats)) return;

            actor.Stats.RemoveModifier(this);

            if (_verboseLogs) Debug.Log("[Flaque] " + actor.name + " sort d'une flaque.", this);
        }

        private void OnDestroy()
        {
            foreach (StatBlock stats in _inside)
            {
                if (stats != null) stats.RemoveModifier(this);
            }

            _inside.Clear();
        }
    }
}
