using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/CardEffect/StatModifierEffect")]
    public class StatModifierEffect : CardEffect
    {
        [Header("Modificateur")]
        [Tooltip("Statistique touchee.")]
        [FormerlySerializedAs("Stats")]
        [SerializeField] private StatType _stat = StatType.MoveSpeed;

        [Tooltip("Additive : la valeur s'ajoute. Multiply : la valeur est un pourcentage, " +
                 "0.5 signifie +50 %, -0.5 signifie -50 %.")]
        [FormerlySerializedAs("Mode")]
        [SerializeField] private ModifierMode _mode = ModifierMode.Multiply;

        [FormerlySerializedAs("_power")]
        [SerializeField] private float _value = 0.5f;

        [Tooltip("Duree en secondes. Zero ou moins = permanent jusqu'a la fin de la partie.")]
        [SerializeField] private float _duration = 5f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            if (target.Stats == null)
            {
                Debug.LogWarning("[StatModifierEffect] " + target.name + " n'a pas de StatBlock.", target);
                return;
            }

            target.Stats.AddModifier(new StatModifier
            {
                Stats = _stat,
                Modifier = _mode,
                Value = _value,
                Duration = _duration,
                Source = this
            });
        }
    }
}
