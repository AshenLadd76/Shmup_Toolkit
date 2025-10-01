using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Collision_Handling
{
    [ExecuteAlways]
    public class CollisionObject : MonoBehaviour, ICollisionObject
    {
        [SerializeField] private bool showDebug;
        
        private SpriteFlash _spriteFlash;
        
        private Transform _transform;
        
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        private const float Half = 0.5f; 

        private void Awake()
        {
            _transform = transform;
            
            _spriteFlash = GetComponent<SpriteFlash>();
            
            Position = _transform.position;
            Size = _transform.localScale;

            RadiusX = transform.localScale.x * Half;  
            RadiusY = transform.localScale.y * Half;
        }

        public void OnCollision()
        {
            
        }
        

        private  void Update()
        {
            Position = _transform.position;
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
