using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ICollisionDetectionSystem
    {
        void CollisionCheck(Vector2 currentGridOrigin);
    }
}