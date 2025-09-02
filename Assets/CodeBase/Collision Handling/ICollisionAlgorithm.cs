using CodeBase.Collision_Handling;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ICollisionAlgorithm
    {
        bool CheckCollision(Vector3 objectA, float radiusA, Vector3 objectB, float radiusB);
    }


    public class CircleCollisionAlgorithm : ICollisionAlgorithm
    {
        public bool CheckCollision(Vector3 objectA, float radiusA, Vector3 objectB, float radiusB)
        {
            Vector2 difference = objectA - objectB;
            float sqrDistance = difference.sqrMagnitude;
            float sqrRadii = (radiusA + radiusB) * (radiusA + radiusB);
            return sqrDistance <= sqrRadii;
        }
    }
}
