using CodeBase.Collision_Handling;
using UnityEngine;

public abstract class BaseCollisionAlgorithmSo : ScriptableObject, ICollisionAlgorithm
{
    public abstract bool CheckCollision(ICollisionObject objectA, ISpatialObject objectB);
    
    // Draw debug visualization at a position with optional size/radius
    public abstract void DrawDebug(Vector3 position, float radiusX, float radiusY);

}
