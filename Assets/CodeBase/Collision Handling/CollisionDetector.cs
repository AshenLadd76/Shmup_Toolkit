using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private bool disableCollisionDetection = true;
        
        [SerializeField] private bool showGrid;
        
        [SerializeField] private float cellSize = 1f;

        [SerializeField] private Vector2 gridOffset;
        
        [SerializeField] private Vector2 _gridOrigin;

        [SerializeField] private List<MonoBehaviour> collisionObjects;
        
       [ShowInInspector] private List<ICollisionObject> _iCollisionObjectsList;
        
        private ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private ICollisionDetectionSystem _collisionDetectionSystem;
        
        private Camera _mainCamera;
        
        private Vector2Int _gridSize;

        private void Awake()
        {
            LoadICollisionObjectsList();
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;

            _gridOrigin = new Vector2(0, 0) + gridOffset;

            _gridSize = GridUtility.GetCellCountWorldUnits(_mainCamera,  cellSize);

            _spatialPartitioningSystem = new SpatialPartitioningSystem(_gridSize);
            
            _collisionDetectionSystem = new CollisionDetectionSystem(_spatialPartitioningSystem, _iCollisionObjectsList, _gridOrigin, cellSize, new CircleCollisionAlgorithm() );
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
        
        private void LoadICollisionObjectsList()
        {
            _iCollisionObjectsList = new List<ICollisionObject>();

            foreach (var collisionObject in collisionObjects   )
            {
                ICollisionObject iCollisionObject  = collisionObject as ICollisionObject;

                if (iCollisionObject == null) continue;
                
                _iCollisionObjectsList.Add(iCollisionObject);
            }
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
