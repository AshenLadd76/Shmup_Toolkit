using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Collision_Handling
{
    [ExecuteAlways]
    public class CollisionObject : MonoBehaviour, ICollisionObject
    {
        [SerializeField] private bool showDebug;
        
        
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        
        
        
        private void Awake()
        {
            Position = transform.position;
            Size = transform.localScale;
        }
        public void OnCollision()
        {
            Logger.Log("OnCollision Aaahh ! I was hit !!!");
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebug) return;
            
            // Set Gizmo color
            Gizmos.color = Color.red;

            // Draw a wireframe cube at the Position with the Size
            Gizmos.DrawWireCube(Position, Size);
        }
        
    }
}
