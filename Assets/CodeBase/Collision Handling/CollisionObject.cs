using CodeBase.Tests;
using ToolBox.Messaging;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Collision_Handling
{
    [ExecuteAlways]
    public class CollisionObject : MonoBehaviour, ICollisionObject
    {
        [SerializeField] private bool showDebug;
        
        private HealthTest _healthTest;
        
        private Transform _transform;

        public Vector3 GetPosition()
        {
            return Position;
        }

        public Vector3 LastPosition { get; set; }
        public Vector2Int LastCellPosition { get; set; }
        public float LifeSpan { get; set; }
        public bool IsActive { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        private const float Half = 0.5f; 

        private void Awake()
        {
            _transform = transform;
            
            _healthTest = GetComponent<HealthTest>();
            
            Position = _transform.position;
            Size = _transform.localScale;

            RadiusX = transform.localScale.x * Half;  
            RadiusY = transform.localScale.y * Half;
        }

        public void OnCollision()
        {
            _healthTest.OnHit();
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

        public void Death()
        {
            MessageBus.Broadcast( nameof(CollisionDetectorMessages.RemoveCollisionObject), this );
            
            gameObject.SetActive( false );
        }
        
    }
}
