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
        
        private  Vector2 _gridOrigin;
        
        private readonly ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private readonly HashSet<ICollisionObject> _deadSpatialObjects = new HashSet<ICollisionObject>();
        
        private readonly List<ICollisionObject> _collisionObjects;
        
        private readonly ICollisionAlgorithm _collisionAlgorithm;

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
        
        public void CollisionCheck(Vector2 gridOrigin)
        {
            _gridOrigin = gridOrigin;
            
            if (_collisionObjects.IsNullOrEmpty())
                return;
            
            _collisionObjects.RemoveAll(o => o == null);
            
            DeleteDeadSpatialObjects();

            for (int x = 0; x < _collisionObjects.Count; x++)
            {
                var collisionObject = _collisionObjects[x];
                var position = collisionObject.Position;
                var size = collisionObject.Size;
                
                //Get the bounds of the current object...Fix this bounds should be supplied by the collision object
                var bounds = GridUtility.GetWorldBounds( position,  size);
                
                CheckForMultiCellCollisions(collisionObject, bounds);
            }
        }
        
        
        //Potential for optimisation
        private void CheckForMultiCellCollisions(ICollisionObject collisionObject,  (Vector2 min, Vector2 max) bounds)
        {
            int minX = GridUtility.GetCellIndex(bounds.min.x, _gridOrigin.x, _cellSize);
            int minY = GridUtility.GetCellIndex(bounds.min.y, _gridOrigin.y, _cellSize);
            int maxX = GridUtility.GetCellIndex(bounds.max.x, _gridOrigin.x, _cellSize);
            int maxY = GridUtility.GetCellIndex(bounds.max.y, _gridOrigin.y, _cellSize);
            
            if (minX == maxX && minY == maxY)
            {
                if (_spatialPartitioningSystem.TryGetValidCell(minX, minY, out var cellSet))
                    CheckForCollisions(cellSet, collisionObject);

                return;
            }
            
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            
            int totalCells = width * height;

            for (int i = 0; i < totalCells; i++)
            {
                int x = minX + (i % width);
                int y = minY + (i / width);

                if (!_spatialPartitioningSystem.TryGetValidCell(x, y, out var cellSet)) continue;
                
                CheckForCollisions(cellSet, collisionObject);
            }
        }
        
        private void CheckForCollisions(HashSet<ICollisionObject> cellSet, ICollisionObject collisionObject)
        {
            float bulletRadius = 0.1f;
            
            if (cellSet.IsNullOrEmpty()) return;
            
            foreach (var spatialObject in cellSet)
            {
                if( spatialObject == null ) continue;
                
                if (!spatialObject.IsActive) continue;

                var spatialPosition = spatialObject.GetPosition();
                
                if (_collisionAlgorithm.CheckCollision(collisionObject, spatialObject))
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