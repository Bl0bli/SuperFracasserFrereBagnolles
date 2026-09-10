using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class CardEffect : ScriptableObject
    {
        [Header("Valeurs")]
        [Tooltip("Intensite de l'effet. Son sens depend de la sous-classe : degats, " +
                 "multiplicateur de stat, nombre de flaques...")]
        [SerializeField] protected float _power;

        [Tooltip("Duree en secondes. Zero ou moins = permanent pour un modificateur de stat.")]
        [SerializeField] protected float _duration;

        [Header("Ciblage")]
        [Tooltip("Qui subit l'effet. Self = le ramasseur. Cars = les trois voitures. " +
                 "Cthulhu = l'entite. RandomCar = une voiture au hasard.")]
        [SerializeField] protected EffectTarget _target = EffectTarget.Self;

        public EffectTarget Target => _target;

        public void Apply(Actor collector)
        {
            if (collector == null)
            {
                Debug.LogError("[CardEffect] " + name + " applique sans ramasseur.", this);
                return;
            }

            List<Actor> targets = ResolveTargets(collector);

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] != null) ApplyTo(targets[i], collector);
            }
        }

        protected abstract void ApplyTo(Actor target, Actor collector);
        
        protected virtual List<Actor> ResolveTargets(Actor collector)
        {
            List<Actor> buffer = new List<Actor>();
            MatchManager match = MatchManager.Instance;

            if (match == null)
            {
                Debug.LogWarning("[CardEffect] Aucun MatchManager : " + name + " s'applique au ramasseur.", this);
                buffer.Add(collector);
                return buffer;
            }

            switch (_target)
            {
                case EffectTarget.Self:
                    buffer.Add(collector);
                    break;

                case EffectTarget.Cars:
                    AddAlive(buffer, match.Cars);
                    break;

                case EffectTarget.Cthulhu:
                    AddAlive(buffer, match.Cthulhu);
                    break;

                case EffectTarget.RandomCar:
                    AddRandomCar(buffer, match, collector);
                    break;

                case EffectTarget.Everyone:
                    AddAlive(buffer, match.Players);
                    break;
            }

            return buffer;
        }

        private static void AddAlive(List<Actor> buffer, IReadOnlyList<Actor> source)
        {
            if (source == null) return;

            for (int i = 0; i < source.Count; i++)
            {
                AddAlive(buffer, source[i]);
            }
        }

        private static void AddAlive(List<Actor> buffer, Actor actor)
        {
            if (actor == null) return;
            if (actor.Health != null && !actor.Health.IsAlive) return;

            buffer.Add(actor);
        }

        private static void AddRandomCar(List<Actor> buffer, MatchManager match, Actor collector)
        {
            List<Actor> candidates = new List<Actor>();
            AddAlive(candidates, match.Cars);

            // On evite le ramasseur, sauf s'il est la seule voiture en vie.
            if (candidates.Count > 1) candidates.Remove(collector);
            if (candidates.Count == 0) return;

            buffer.Add(candidates[Random.Range(0, candidates.Count)]);
        }
    }
}
