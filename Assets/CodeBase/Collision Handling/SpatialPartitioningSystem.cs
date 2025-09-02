using System.Collections.Generic;
using CodeBase.Projectile;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class SpatialPartitioningSystem : ISpatialPartitioningSystem
    {
        private Dictionary<Vector2Int, HashSet<ISpatialObject>> _spatialPartitioningDictionary;
        
        public SpatialPartitioningSystem(Vector2Int gridSize)
        {
            InitDictionary(gridSize);
        }
        
        private void InitDictionary(Vector2Int gridSize)
        {
            _spatialPartitioningDictionary = new Dictionary<Vector2Int, HashSet<ISpatialObject>>(gridSize.x * gridSize.y);
            
            for(int x = 0; x < gridSize.x; x++)
                for(int y = 0; y < gridSize.y; y++)
                    _spatialPartitioningDictionary.Add(new Vector2Int(x, y), new HashSet<ISpatialObject>());
        }
        
        public void RemoveFromSpatialPartitionGrid(ISpatialObject spatialObject, Vector2Int cellPosition)
        {
            if (_spatialPartitioningDictionary.TryGetValue(cellPosition, out var cellSet))
                cellSet.Remove(spatialObject); // O(1)
        }

        
        public void AddToSpatialPartitionGrid(ISpatialObject spatialObject)
        {
            if (_spatialPartitioningDictionary.TryGetValue(spatialObject.LastCellPosition, out var cellSet))
                cellSet.Add(spatialObject); // HashSet automatically ignores duplicates
        }

        public HashSet<ISpatialObject> GetCellSet(Vector2Int key)
        {
            return _spatialPartitioningDictionary.GetValueOrDefault(key);
        }
    }
}