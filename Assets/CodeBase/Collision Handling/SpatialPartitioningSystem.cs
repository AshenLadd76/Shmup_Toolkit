using System.Collections.Generic;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class SpatialPartitioningSystem : ISpatialPartitioningSystem
    {
        private Dictionary<Vector2Int, HashSet<ICollisionObject>> _spatialPartitioningDictionary;
        
        public SpatialPartitioningSystem(Vector2Int gridSize)
        {
            InitDictionary(gridSize);
        }
        
        private void InitDictionary(Vector2Int gridSize)
        {
            _spatialPartitioningDictionary = new Dictionary<Vector2Int, HashSet<ICollisionObject>>(gridSize.x * gridSize.y);
            
            for(int x = 0; x < gridSize.x; x++)
                for(int y = 0; y < gridSize.y; y++)
                    _spatialPartitioningDictionary.Add(new Vector2Int(x, y), new HashSet<ICollisionObject>());
        }
        
        public void UpdateCheck(ICollisionObject[] spatialObjects, Vector3 gridOrigin , float cellSize)
        {
            var newCellPosition = new Vector2Int();
            
            if (spatialObjects.IsNullOrEmpty() ) return;
            
            for (var i = 0; i < spatialObjects.Length; i++)
            {
                var spatialObject = spatialObjects[i];
                
                if(spatialObject == null) continue;

                if(!RemoveInActiveSpatialObjects(spatialObject, gridOrigin, cellSize)) continue;
                
                newCellPosition = GridUtility.GetCellFromWorldPosition(spatialObject.GetPosition(), gridOrigin, cellSize);

                if (!IsValidCell(newCellPosition)) continue;
                
                if( newCellPosition == spatialObject.LastCellPosition ) continue;
                
                UpdateActiveObjectsPosition( spatialObject, newCellPosition );
            }
        }

       
        
        public void AddToSpatialPartitionGrid(ICollisionObject spatialObject)
        {
            if(TryGetValidCell(spatialObject.LastCellPosition, out var cellSet))
                cellSet.Add(spatialObject);
        }
        
        public void RemoveFromSpatialPartitionGrid(ICollisionObject spatialObject, Vector2Int cellPosition)
        {
            if(TryGetValidCell(cellPosition, out var cellSet))
                cellSet.Remove(spatialObject); // O(1)
        }
        
        private void UpdateActiveObjectsPosition(ICollisionObject spatialObject, Vector2Int newCellPosition)
        {
            RemoveFromSpatialPartitionGrid(spatialObject, spatialObject.LastCellPosition);
                
            spatialObject.LastCellPosition = newCellPosition;
                
            AddToSpatialPartitionGrid(spatialObject);
        }
        
        private bool RemoveInActiveSpatialObjects(ICollisionObject projectile, Vector3 gridOrigin, float cellSize)
        {
            if (projectile.IsActive) return true;
            
            Vector2Int cellPos = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), gridOrigin, cellSize);
       
            RemoveFromSpatialPartitionGrid(projectile, cellPos);

            return false;
        }
        
        private bool IsValidCell(Vector2Int cell) => _spatialPartitioningDictionary.ContainsKey(cell);
        
        public bool TryGetValidCell(Vector2Int cell, out HashSet<ICollisionObject> cellSet)
        {
            if (_spatialPartitioningDictionary.TryGetValue(cell, out cellSet))
                return true;

            //cellSet = null;
            return false;
        }
    }
}