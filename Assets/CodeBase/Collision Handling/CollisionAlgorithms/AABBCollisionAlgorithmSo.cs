using CodeBase.Collision_Handling;
using UnityEngine;


[CreateAssetMenu(fileName = "AABBCollision", menuName = "Collision/Algorithms/AABBCollision")]
public class AABBCollisionAlgorithmSo : BaseCollisionAlgorithmSo
{
    public override bool CheckCollision(ICollisionObject objectA, ICollisionObject objectB)
    {
        return Mathf.Abs(objectA.Position.x - objectB.Position.x) <= objectA.RadiusX + objectB.RadiusX &&
               Mathf.Abs(objectA.Position.y - objectB.Position.y) <= objectA.RadiusY + objectB.RadiusY;
        
    }


    public bool CheckCollision(Vector2 centreA, Vector2 halfSizeA, Vector2 centreB, Vector2 halfSizeB)
    {
        return Mathf.Abs(centreA.x - centreB.x) <= halfSizeA.x + halfSizeB.x &&
               Mathf.Abs(centreA.y - centreB.y) <= halfSizeA.y + halfSizeB.y;
    }
    
    

    public override void DrawDebug(Vector3 position, float radiusX, float radiusY)
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(radiusX * 2f, radiusY * 2f, 0.1f);
        Gizmos.DrawWireCube(position, size);
    }
}
