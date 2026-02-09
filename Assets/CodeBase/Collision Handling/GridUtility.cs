using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public static class GridUtility
    {
        public static Vector2Int GetCellCountWorldUnits(Camera camera, float cellSizeWorldUnits )
        {
            if (camera == null) return Vector2Int.zero;
            
            //Camera orthographicSize gives half the vertical world units visible.
            //Multiply by 2 → total vertical span in world units
            float worldHeight = camera.orthographicSize * 2f;
            
            // horizontal world units visible
            float worldWidth = worldHeight * camera.aspect;
            
            int cellsX = Mathf.CeilToInt( worldWidth / cellSizeWorldUnits );
            int cellsY = Mathf.CeilToInt( worldHeight / cellSizeWorldUnits );
            
            return new Vector2Int( cellsX, cellsY );
        }
        
        public static Vector2Int GetCellFromWorldPosition(Vector3 worldPosition, Vector3 gridOrigin, float inverseCellSize)
        {
            
            Vector3 relativePosition = worldPosition - gridOrigin;
            
            int cellX = Mathf.FloorToInt(relativePosition.x * inverseCellSize);
            int cellY = Mathf.FloorToInt(relativePosition.y * inverseCellSize);
            
            return new Vector2Int(cellX, cellY);
        }
        
        public static int GetCellIndex(float worldCoord, float gridOriginCoord, float inverseCellSize)
        {
            return Mathf.FloorToInt((worldCoord - gridOriginCoord) * inverseCellSize);
        }

        
        public static (Vector2 min, Vector2 max) GetWorldBounds(Vector2 position, Vector2 halfSize)
        {
            Vector2 min = position - halfSize;
            Vector2 max = position + halfSize;
            return (min, max);
        }
    }
}