using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "MatchSettings", menuName = "Scriptable Objects/MatchSettings")]
    public class MatchSettings : ScriptableObject
    {
        [Header("Lobby")]
        [Tooltip("Nombre minimum de joueurs pour pouvoir lancer. 2 pour tester, 4 en partie reelle.")]
        [Range(2, 8)] public int MinPlayers = 2;

        [Tooltip("Duree du decompte avant GO, en secondes.")]
        [Range(1, 10)] public int CountdownSeconds = 3;

        [Header("Partie")]
        [Tooltip("Duree de la partie en secondes. Chrono ecoule = victoire de l'entite.")]
        [Range(30, 600)] public float MatchDuration = 180f;

        [Header("Bornes")]
        [Tooltip("Total de bornes que l'entite doit atteindre pour gagner. " +
                 "0 desactive cette condition de victoire (phase cartes non implementee).")]
        [Range(0, 1000)] public int BorneGoal = 0;

        [Header("Power Ups")] 
        public bool RandomBetweenValues = false;
        [Tooltip("Interval de temps régulier entre les spawns de carte")]
        [Range(1, 20), ShowIf("RandomBetweenValues", false)] public float PowerUpInterval = 10f;
        [Tooltip("Interval de temps min entre les spawns de carte")]
        [Range(1, 20), ShowIf("RandomBetweenValues", true)] public float PowerUpIntervalMin = 5f;
        [Tooltip("Interval de temps max entre les spawns de carte")]
        [Range(1, 20), ShowIf("RandomBetweenValues", true)] public float PowerUpIntervalMax = 15f;
        
     }
}
