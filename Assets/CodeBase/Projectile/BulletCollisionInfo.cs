using System;
using UnityEngine;

namespace CodeBase.Projectile
{
    [Serializable]
    public struct BulletCollisionInfo
    {
        public BulletCollisionInfo(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Vector3 Position;
        public float Radius;
    }
}