using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GrantThrowEffect", menuName = "Scriptable Objects/CardEffect/GrantThrowEffect")]
    public class GrantThrowEffect : CardEffect
    {
        [Header("Lancer")]
        [Tooltip("Projectile accorde par cette carte. Vide : celui du prefab de la cible.")]
        [SerializeField] private Projectile _projectile;

        [Tooltip("Duree pendant laquelle la cible peut lancer.")]
        [SerializeField, Min(0f)] private float _duration = 8f;

        [Tooltip("Delai minimum entre deux tirs.")]
        [SerializeField, Min(0.05f)] private float _cooldown = 0.5f;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            ThrowAbility ability = target.GetComponent<ThrowAbility>();

            if (ability == null)
            {
                Debug.LogWarning("[GrantThrowEffect] " + target.name + " n'a pas de ThrowAbility.", target);
                return;
            }

            ability.Grant(_projectile, _duration, _cooldown);
        }
    }
}
