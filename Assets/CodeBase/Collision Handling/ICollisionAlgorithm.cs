using CodeBase.Collision_Handling;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ICollisionAlgorithm
    {
        bool CheckCollision(ICollisionObject objectA, ICollisionObject objectB);
    }


    // public class CircleCollisionAlgorithm : ICollisionAlgorithm
    // {
    //     public bool CheckCollision(Vector3 objectA, float radiusA, Vector3 objectB, float radiusB)
    //     {
    //         Vector2 difference = objectA - objectB;
    //         float sqrDistance = difference.sqrMagnitude;
    //         float sqrRadii = (radiusA + radiusB) * (radiusA + radiusB);
    //         return sqrDistance <= sqrRadii;
    //     }
    // }
    //
    // public class AabbCollisionAlgorithm : ICollisionAlgorithm
    // {
    //     public bool CheckCollision(Vector3 objectA, float radiusA, Vector3 objectB, float radiusB)
    //     {
    //         // Here radiusA and radiusB represent half-extents (x or y)
    //         return Mathf.Abs(objectA.x - objectB.x) <= (radiusA + radiusB) &&
    //                Mathf.Abs(objectA.y - objectB.y) <= (radiusA + radiusB);
    //     }
    // }

}
