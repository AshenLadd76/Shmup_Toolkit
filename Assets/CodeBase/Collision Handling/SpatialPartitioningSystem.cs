using System.Collections.Generic;
using ToolBox.Extensions;
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
        
        public void UpdateCheck(ISpatialObject[] spatialObjects, Vector3 gridOrigin , float cellSize)
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

       
        
        public void AddToSpatialPartitionGrid(ISpatialObject spatialObject)
        {
            if(TryGetValidCell(spatialObject.LastCellPosition, out var cellSet))
                cellSet.Add(spatialObject);
        }
        
        public void RemoveFromSpatialPartitionGrid(ISpatialObject spatialObject, Vector2Int cellPosition)
        {
            if(TryGetValidCell(cellPosition, out var cellSet))
                cellSet.Remove(spatialObject); // O(1)
        }
        
        private void UpdateActiveObjectsPosition(ISpatialObject spatialObject, Vector2Int newCellPosition)
        {
            RemoveFromSpatialPartitionGrid(spatialObject, spatialObject.LastCellPosition);
                
            spatialObject.LastCellPosition = newCellPosition;
                
            AddToSpatialPartitionGrid(spatialObject);
        }
        
        private bool RemoveInActiveSpatialObjects(ISpatialObject projectile, Vector3 gridOrigin, float cellSize)
        {
            if (projectile.IsActive) return true;
            
            Vector2Int cellPos = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), gridOrigin, cellSize);
       
            RemoveFromSpatialPartitionGrid(projectile, cellPos);

            return false;
        }
        
        private bool IsValidCell(Vector2Int cell) => _spatialPartitioningDictionary.ContainsKey(cell);
        
        public bool TryGetValidCell(Vector2Int cell, out HashSet<ISpatialObject> cellSet)
        {
            if (_spatialPartitioningDictionary.TryGetValue(cell, out cellSet))
                return true;

            //cellSet = null;
            return false;
        }
    }
}