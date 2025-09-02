using System.Collections.Generic;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class CollisionDetectionSystem : ICollisionDetectionSystem
    {
        private readonly float _cellSize;
        
        private readonly Vector2 _gridOrigin;
        
        private readonly ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private readonly HashSet<ISpatialObject> _deadSpatialObjects = new HashSet<ISpatialObject>();
        
        private readonly List<GameObject> _collisionObjects;
        
        private ICollisionAlgorithm _collisionAlgorithm;
        
        public CollisionDetectionSystem(ISpatialPartitioningSystem spatialPartitioningSystem, List<GameObject> collisionObjects, Vector2 gridOrigin, float cellSize, ICollisionAlgorithm collisionAlgorithm)
        {
            _spatialPartitioningSystem = spatialPartitioningSystem ?? throw new System.ArgumentNullException(nameof(spatialPartitioningSystem), "Spatial partitioning system cannot be null.");
            _collisionObjects = collisionObjects ?? throw new System.ArgumentNullException(nameof(collisionObjects), "Collision objects list cannot be null.");
            _collisionAlgorithm = collisionAlgorithm ?? throw new System.ArgumentNullException(nameof(collisionAlgorithm));
            
            _gridOrigin = gridOrigin;
            _cellSize = cellSize;
        }
        
        public void CollisionCheck()
        {
            if (_collisionObjects.IsNullOrEmpty())
                return;
            
            DeleteDeadSpatialObjects();

            for (int x = 0; x < _collisionObjects.Count; x++)
            {
                //Fix this....
                var collisionObjectPosition = _collisionObjects[x].transform.position;
                
                //Get the bounds of the current object
                var bounds = GridUtility.GetWorldBounds( collisionObjectPosition,  Vector2.one * 0.25f );
                
                CheckForMultiCellCollisions(bounds, collisionObjectPosition);
            }
        }
        
        private void CheckForMultiCellCollisions((Vector2 min, Vector2 max) bounds, Vector3 collisionObjectPosition)
        {
            Vector2Int currentCellKey = new Vector2Int();
            
            Vector2Int minCell = GridUtility.GetCellFromWorldPosition(bounds.min, _gridOrigin, _cellSize);
            Vector2Int maxCell = GridUtility.GetCellFromWorldPosition(bounds.max, _gridOrigin, _cellSize);
            
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    currentCellKey.x = x;
                    currentCellKey.y = y;
                    
                    var cellSet = _spatialPartitioningSystem.GetCellSet(currentCellKey);

                    if (cellSet == null) continue; 
                    
                    CheckForCollisions(cellSet, collisionObjectPosition);
                }
            }
        }
        
        private void CheckForCollisions(HashSet<ISpatialObject> cellSet, Vector3 objectPosition)
        {
            if (cellSet.IsNullOrEmpty()) return;
            
            foreach (var spatialObject in cellSet)
            {
                if( spatialObject == null ) continue;
                
                if (!spatialObject.IsActive) continue;

                var spatialPosition = spatialObject.GetPosition();
                
                if (_collisionAlgorithm.CheckCollision(objectPosition, 0.25f, spatialPosition, .1f))
                {
                    spatialObject.LifeSpan = 0;
                    _deadSpatialObjects.Add(spatialObject);
                }
                
                spatialObject.LastPosition = spatialPosition;
            }
        }
        
        private void DeleteDeadSpatialObjects()
        {
            foreach (var spatialObject in _deadSpatialObjects)
                spatialObject.LastCellPosition = new Vector2Int(int.MinValue, int.MinValue);
            
            _deadSpatialObjects.Clear();
        }
    }
}