using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Collision_Handling
{
    [CreateAssetMenu(fileName = "CircleCollision", menuName = "Collision/Algorithms/Circle")]
    public class CircleCollisionAlgorithmSo : BaseCollisionAlgorithmSo
    {
        
        [SerializeField] private bool useRadiusFromObject  = true;
        [SerializeField] private float fixedRadius = 0.5f;
        
        public override bool CheckCollision(ICollisionObject collisionObjectA, ISpatialObject collisionObjectB)
        {
            float radiusA = useRadiusFromObject ? collisionObjectA.RadiusX : fixedRadius;
            float radiusB = useRadiusFromObject ? collisionObjectB.RadiusX : fixedRadius;


            Logger.Log( $"Checking circle collisions {radiusA} and {radiusB}" );

            Vector2 diff = (Vector2)(collisionObjectA.Position - collisionObjectB.Position);
            float sqrDistance = diff.sqrMagnitude;
            float sqrRadii = (radiusA + radiusB) * (radiusA + radiusB);

            return sqrDistance <= sqrRadii;
        }

        public override void DrawDebug(Vector3 position, float radiusX, float radiusY)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, radiusX);
        }
    }
}
