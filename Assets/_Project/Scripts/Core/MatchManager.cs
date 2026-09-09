using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class MatchManager : MonoBehaviour
    {
        [Header("Unity Events")]
        [SerializeField] private UnityEvent _onMatchStarted;
        [SerializeField] private UnityEvent _onMatchEnded;
        
        [Header("Params")]
        //[SerializeField] private MatchSettings _settings; TODO décommenter
        private List<Actor> _cars = new List<Actor>();
        private Actor _cthulhu;
        private MatchState _state = MatchState.Warmup;
        private float _remainingTime = 0f;
        
        public event Action OnMatchStarted;
        public event Action OnTimerTick;
        public event Action OnActorDied;
        public event Action OnMatchEnded;

        private void Register(Actor actor)
        {
            
        }

        private void EvaluateVictory()
        {
            
        }
    }
}
