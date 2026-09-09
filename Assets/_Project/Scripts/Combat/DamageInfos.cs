using UnityEngine;

namespace Game
{
    public struct DamageInfos
    {
        public int Amount;
        public Actor Source;
        public Vector2 Direction;
        public float Knockback; //optionnel
    }
}
