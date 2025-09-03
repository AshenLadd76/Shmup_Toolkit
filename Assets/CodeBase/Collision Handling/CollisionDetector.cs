using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private bool disableCollisionDetection = true;
        
        [SerializeField] private bool showGrid;
        
        [SerializeField] private float cellSize = 1f;
        
        private Vector2Int _gridSize;
        
        private Vector2 _gridOrigin;

        [SerializeField] private List<GameObject> collisionObjects;
        
        private ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private ICollisionDetectionSystem _collisionDetectionSystem;
        
        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;

            _gridOrigin = new Vector2(-2.5f, -5f);

            _gridSize = GridUtility.GetCellCountWorldUnits(_mainCamera,  cellSize);

            _spatialPartitioningSystem = new SpatialPartitioningSystem(_gridSize);
            
            _collisionDetectionSystem = new CollisionDetectionSystem(_spatialPartitioningSystem, collisionObjects, _gridOrigin, cellSize, new CircleCollisionAlgorithm() );
        }


        public void UpdateCheck(ISpatialObject[] spatialObjects)
        {
            _spatialPartitioningSystem.UpdateCheck(spatialObjects, _gridOrigin, cellSize);
            
            _collisionDetectionSystem?.CollisionCheck();
        }

        
        public void AddToCollisionCheckGridCell(ISpatialObject projectile)
        {
            if(disableCollisionDetection ) return;
            
            var cellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
            
            projectile.LastCellPosition = cellPosition;

            _spatialPartitioningSystem?.AddToSpatialPartitionGrid(projectile);
        }
        
        public void RemoveFromCollisionCheck(ISpatialObject projectile, Vector2Int cellPosition)
        {
            if(disableCollisionDetection ) return;
            
            _spatialPartitioningSystem?.RemoveFromSpatialPartitionGrid(projectile, cellPosition);
        }


        
        private void OnDrawGizmos()
        {
            if (!showGrid) return;
            
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector2 pos = _gridOrigin + new Vector2(x * cellSize, y * cellSize);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(pos + Vector2.one * cellSize * 0.5f, Vector3.one * cellSize);
                }
            }
        }
    }
}
