using System.Collections.Generic;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class SpatialPartitioningSystem : ISpatialPartitioningSystem
    {
        private readonly HashSet<ICollisionObject>[] _grid1D;
        private readonly int _cellsX, _cellsY;
        
        
        public SpatialPartitioningSystem(Vector2Int gridSize)
        {
            _cellsX = gridSize.x;
            _cellsY = gridSize.y;
            
            _grid1D = new HashSet<ICollisionObject>[_cellsX * _cellsY];
            
            for( int x = 0; x < _grid1D.Length; x++ )
                _grid1D[x] = new HashSet<ICollisionObject>();
        }
        
        
        private bool IsValidCell( int cellX, int cellY ) => cellX >= 0 && cellX < _cellsX && cellY >= 0 && cellY < _cellsY;
        
        private HashSet<ICollisionObject> GetCell(int cellX, int cellY)
        {
            if( !IsValidCell(cellX, cellY) ) return null;
            
            var index = cellX + cellY * _cellsX;
            
            return _grid1D[index];
        }

        public void UpdateCheck(ICollisionObject[] spatialObjects, Vector3 gridOrigin , float cellSize)
        {
            if (spatialObjects.IsNullOrEmpty() ) return;
            
            for (var i = 0; i < spatialObjects.Length; i++)
            {
                var spatialObject = spatialObjects[i];
                
                if(spatialObject == null) continue;

                if(!RemoveInActiveSpatialObjects(spatialObject, gridOrigin, cellSize)) continue;
                
                var newCellPosition = GridUtility.GetCellFromWorldPosition(spatialObject.GetPosition(), gridOrigin, cellSize);

                if (!IsValidCell(newCellPosition.x ,newCellPosition.y)) continue;
                
                if( newCellPosition == spatialObject.LastCellPosition ) continue;
                
                UpdateActiveObjectsPosition( spatialObject, newCellPosition );
            }
        }
        
        
        public void AddToSpatialPartitionGrid(ICollisionObject spatialObject)
        {
            var cellSet = GetCell(spatialObject.LastCellPosition.x, spatialObject.LastCellPosition.y);
            cellSet?.Add(spatialObject);
        }
        
        
        public void RemoveFromSpatialPartitionGrid(ICollisionObject spatialObject, Vector2Int cellPosition)
        {
            var cellSet = GetCell(cellPosition.x, cellPosition.y);
            cellSet?.Remove(spatialObject);
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
            
            RemoveFromSpatialPartitionGrid(projectile, projectile.LastCellPosition);

            return false;
        }
        
        
        public bool TryGetValidCell(int cellX, int cellY, out HashSet<ICollisionObject> cellSet)
        {
            if (!IsValidCell(cellX, cellY))
            {
                cellSet = null;
                return false;
            }
            
            cellSet = GetCell( cellX, cellY );
            
            return true;
        }
    }
}