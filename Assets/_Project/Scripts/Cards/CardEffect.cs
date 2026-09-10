using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public abstract class CardEffect : ScriptableObject
    {
        [Header("Presentation")]
        [Tooltip("Face visible de la carte une fois retournee.")]
        [SerializeField] private Sprite _icon;

        [Tooltip("Colore les particules sous la carte : lumineuses ou sombres.")]
        [SerializeField] private EffectPolarity _polarity = EffectPolarity.Bonus;

        [Tooltip("Couleur du flash joue sur la cible. Laissez l'alpha a 0 pour utiliser " +
                 "la couleur par defaut de la polarite.")]
        [SerializeField] private Color _feedbackColor = new Color(1f, 1f, 1f, 0f);

        [Header("Ciblage")]
        [Tooltip("Qui subit l'effet. Self = le ramasseur. Cars = les voitures. " +
                 "Cthulhu = l'entite. RandomCar = une voiture au hasard. Everyone = tout le monde.")]
        [SerializeField] protected EffectTarget _target = EffectTarget.Self;

        public EffectTarget Target => _target;
        public Sprite Icon => _icon;
        public EffectPolarity Polarity => _polarity;
        public Color FeedbackColor => _feedbackColor;

        public IReadOnlyList<Actor> Apply(Actor collector)
        {
            if (collector == null)
            {
                Debug.LogError("[CardEffect] " + name + " applique sans ramasseur.", this);
                return Array.Empty<Actor>();
            }

            List<Actor> targets = ResolveTargets(collector);

            if (targets.Count == 0)
            {
                Debug.LogWarning("[CardEffect] " + name + " n'a touche personne : cible " + _target +
                                 ", ramasseur " + Describe(collector) + ".", this);
                return targets;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] != null) ApplyTo(targets[i], collector);
            }

            return targets;
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
                    AddAlive(buffer, collector);
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

        protected static string Describe(Actor actor)
        {
            if (actor == null) return "null";
            return actor.name + " [" + actor.Faction + "]";
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

            if (candidates.Count > 1) candidates.Remove(collector);
            if (candidates.Count == 0) return;

            buffer.Add(candidates[UnityEngine.Random.Range(0, candidates.Count)]);
        }
    }
}
