using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ShieldEffect", menuName = "Scriptable Objects/CardEffect/ShieldEffect")]
    public class ShieldEffect : CardEffect
    {
        [Header("Bouclier")]
        [SerializeField, Min(0f)] private float _duration = 6f;

        [Tooltip("Coche : ne bloque que les coups recus de face, il faut frapper dans le dos.")]
        [SerializeField] private bool _frontalOnly = false;

        [Tooltip("PV rendus si le bouclier expire sans avoir ete casse. Plafonne aux PV max.")]
        [SerializeField, Min(0)] private int _healOnSurvive = 1;

        protected override void ApplyTo(Actor target, Actor collector)
        {
            ShieldController shield = target.GetComponent<ShieldController>();

            if (shield == null)
            {
                Debug.LogWarning("[ShieldEffect] " + target.name + " n'a pas de ShieldController.", target);
                return;
            }

            shield.Activate(_duration, _frontalOnly, _healOnSurvive);
            AudioManager.Instance.PlayShieldSound();
        }

        public override float RemainingOn(Actor target)
        {
            if (target == null) return 0f;

            ShieldController shield = target.GetComponent<ShieldController>();
            return shield != null && shield.IsActive ? shield.Remaining : 0f;
        }
    }
}
