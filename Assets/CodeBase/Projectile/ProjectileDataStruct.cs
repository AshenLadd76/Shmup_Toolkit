using Unity.Mathematics;
using UnityEngine;

namespace CodeBase.Projectile
{
    public struct ProjectileDataStruct
    {
        public float3 Position;
        public float3 Velocity;
        public float LifeSpan;
        public float Radius;
        public Vector2Int LastGridPosition;
    }
}