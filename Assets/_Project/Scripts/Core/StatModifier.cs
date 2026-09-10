using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct StatModifier
    {
        public StatType Stats;
        public ModifierMode Modifier;
        public float Value;

        [Tooltip("Duree en secondes. Zero ou moins = permanent, a retirer par RemoveModifier.")]
        public float Duration;

        [Tooltip("Qui a pose ce modificateur : l'asset de carte, une flaque, un statut... " +
                 "Sert a le retirer sans avoir a le retrouver par egalite de valeurs.")]
        public object Source;//attention cascade effectuée par un professionnel, à ne pas reproduire Yanis

        public bool IsPermanent => Duration <= 0f;
    }
}
