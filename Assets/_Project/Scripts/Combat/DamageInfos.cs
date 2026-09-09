using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct DamageInfos
    {
        public int Amount;
        public DamageType Type;
        public Actor Source;
        public Vector2 Direction;
        public float Knockback; //optionnel
    }
}
