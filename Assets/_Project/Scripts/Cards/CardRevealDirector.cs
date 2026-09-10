using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class CardRevealDirector : MonoBehaviour
    {
        public static CardRevealDirector Instance { get; private set; }

        [SerializeField] private CardVisual _cardVisualPrefab;

        [Tooltip("Temps pendant lequel la carte reste au-dessus de la tete avant de disparaitre. " +
                 "0 = elle reste jusqu'a la fin de la partie.")]
        [SerializeField, Min(0f)] private float _holdDuration = 3f;

        [Tooltip("Decalage entre deux cartes distribuees, pour que la volee s'etale.")]
        [SerializeField, Min(0f)] private float _distributionStagger = 0.08f;

        [SerializeField, Min(0f)] private float _distributedHold = 1.2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[CardReveal] Un second CardRevealDirector a ete ignore.", this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Play(Sprite back, CardEffect effect, Actor collector, IReadOnlyList<Actor> targets, Vector3 origin)
        {
            if (_cardVisualPrefab == null)
            {
                Debug.LogError("[CardReveal] Aucun prefab de CardVisual assigne.", this);
                return;
            }

            if (collector == null || effect == null) return;

            if (effect.Icon == null)
            {
                Debug.LogWarning("[CardReveal] L'effet " + effect.name + " n'a pas d'Icon : la carte " +
                                 "se retourne mais garde son dos, le retournement est invisible.", effect);
            }

            CardVisual card = Instantiate(_cardVisualPrefab, origin, Quaternion.identity);
            card.SetSprite(back);

            Sequence reveal = card.PlayReveal(collector, effect.Icon, effect.Polarity);

            bool selfOnly = targets == null || targets.Count == 0 ||
                            (targets.Count == 1 && targets[0] == collector);

            if (selfOnly)
            {
                reveal.OnComplete(() =>
                {
                    Feedback(collector, effect.FeedbackColor, effect.Polarity);
                    card.PlayHold();
                    if (_holdDuration > 0f) card.Dismiss(_holdDuration);
                });

                return;
            }

            reveal.OnComplete(() => Distribute(card, effect, collector, targets));
        }

        public void PlaySelf(Sprite back, Sprite front, EffectPolarity polarity,
            Actor collector, Vector3 origin)
        {
            PlaySelf(back, front, polarity, default, collector, origin);
        }

        public void PlaySelf(Sprite back, Sprite front, EffectPolarity polarity, Color feedbackColor,
            Actor collector, Vector3 origin)
        {
            if (_cardVisualPrefab == null)
            {
                Debug.LogError("[CardReveal] Aucun prefab de CardVisual assigne.", this);
                return;
            }

            if (collector == null) return;

            CardVisual card = Instantiate(_cardVisualPrefab, origin, Quaternion.identity);
            card.SetSprite(back);

            Sequence reveal = card.PlayReveal(collector, front, polarity);

            reveal.OnComplete(() =>
            {
                Feedback(collector, feedbackColor, polarity);
                card.PlayHold();
                if (_holdDuration > 0f) card.Dismiss(_holdDuration);
            });
        }

        private static void Feedback(Actor actor, Color color, EffectPolarity polarity)
        {
            if (actor == null) return;

            ActorJuice juice = actor.GetComponent<ActorJuice>();
            if (juice != null) juice.PlayFeedback(color, polarity);
        }

        private static bool Contains(IReadOnlyList<Actor> list, Actor actor)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == actor) return true;
            }

            return false;
        }

        private void Distribute(CardVisual origin, CardEffect effect, Actor collector, IReadOnlyList<Actor> targets)
        {
            float delay = 0f;

            for (int i = 0; i < targets.Count; i++)
            {
                Actor target = targets[i];
                if (target == null) continue;

                if (target == collector)
                {
                    Feedback(target, effect.FeedbackColor, effect.Polarity);
                    origin.PlayHold();
                    if (_holdDuration > 0f) origin.Dismiss(_holdDuration);
                    continue;
                }

                CardVisual copy = Instantiate(_cardVisualPrefab, origin.transform.position, Quaternion.identity);
                copy.SetSprite(effect.Icon);

                CardVisual captured = copy;

                DOVirtual.DelayedCall(delay, () =>
                {
                    if (captured == null || target == null) return;

                    captured.PlayTravel(target, effect.Polarity, () =>
                    {
                        Feedback(target, effect.FeedbackColor, effect.Polarity);
                        captured.Dismiss(_distributedHold);
                    });
                });

                delay += _distributionStagger;
            }

            if (!Contains(targets, collector)) origin.Dismiss(delay);
        }
    }
}
