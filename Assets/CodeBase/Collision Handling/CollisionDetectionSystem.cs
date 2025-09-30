using System;
using System.Collections.Generic;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    [ExecuteAlways]
    public class CollisionDetectionSystem : ICollisionDetectionSystem
    {
        private readonly float _cellSize;
        
        private readonly Vector2 _gridOrigin;
        
        private readonly ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private readonly HashSet<ISpatialObject> _deadSpatialObjects = new HashSet<ISpatialObject>();
        
        private readonly List<ICollisionObject> _collisionObjects;
        
        private ICollisionAlgorithm _collisionAlgorithm;

        private float _boundsPadding;
        
        public CollisionDetectionSystem(ISpatialPartitioningSystem spatialPartitioningSystem, List<ICollisionObject> collisionObjects, Vector2 gridOrigin, float cellSize, ICollisionAlgorithm collisionAlgorithm)
        {
            _spatialPartitioningSystem = spatialPartitioningSystem ?? throw new System.ArgumentNullException(nameof(spatialPartitioningSystem), "Spatial partitioning system cannot be null.");
            _collisionObjects = collisionObjects ?? throw new System.ArgumentNullException(nameof(collisionObjects), "Collision objects list cannot be null.");
            _collisionAlgorithm = collisionAlgorithm ?? throw new System.ArgumentNullException(nameof(collisionAlgorithm));
            
            if (cellSize <= 0) throw new ArgumentOutOfRangeException(nameof(cellSize), "Cell size must be positive.");
            
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
                var collisionObject = _collisionObjects[x];

                if ( collisionObject == null ) continue;
                
                var position = collisionObject.Position;
                var size = collisionObject.Size;
                
                //Get the bounds of the current object...Fix this bounds should be supplied by the collision object
                var bounds = GridUtility.GetWorldBounds( position,  size);
                
                CheckForMultiCellCollisions(collisionObject, bounds);
            }
        }
        
        private void CheckForMultiCellCollisions(ICollisionObject collisionObject,  (Vector2 min, Vector2 max) bounds)
        {
            var collisionObjectPosition = collisionObject.Position;
            var collisionObjectSize = collisionObject.Size;
            
            float radiusX = collisionObjectSize.x * 0.5f;
            
            Vector2Int currentCellKey = new Vector2Int();
            
            Vector2Int minCell = GridUtility.GetCellFromWorldPosition(bounds.min, _gridOrigin, _cellSize);
            Vector2Int maxCell = GridUtility.GetCellFromWorldPosition(bounds.max, _gridOrigin, _cellSize);
            
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    currentCellKey.x = x;
                    currentCellKey.y = y;
                    
                    //var cellSet = _spatialPartitioningSystem.GetCellSet(currentCellKey);
                    if (!_spatialPartitioningSystem.TryGetValidCell(currentCellKey, out var cellSet)) continue;
                    
                    CheckForCollisions(cellSet, collisionObjectPosition,  radiusX, collisionObject);
                }
            }
        }
        
        private void CheckForCollisions(HashSet<ISpatialObject> cellSet, Vector3 objectPosition, float objectRadius, ICollisionObject collisionObject)
        {
            float bulletRadius = 0.1f;
            
            if (cellSet.IsNullOrEmpty()) return;
            
            foreach (var spatialObject in cellSet)
            {
                if( spatialObject == null ) continue;
                
                if (!spatialObject.IsActive) continue;

                var spatialPosition = spatialObject.GetPosition();
                
                if (_collisionAlgorithm.CheckCollision(objectPosition, objectRadius, spatialPosition, bulletRadius))
                {
                    collisionObject.OnCollision();
                    
                    spatialObject.OnCollision();
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