using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ISpatialPartitioningSystem
    {
        public void UpdateCheck(ISpatialObject[] projectiles, Vector3 gridOrigin, float cellSize);
        void RemoveFromSpatialPartitionGrid(ISpatialObject projectile, Vector2Int cellPosition);
        void AddToSpatialPartitionGrid(ISpatialObject projectile);

        public bool TryGetValidCell(Vector2Int cell, out HashSet<ISpatialObject> cellSet);
    }

    public interface ISpatialObject
    {
        public Vector3 GetPosition();
        
        public Vector3 LastPosition { get; set; }
        public Vector2Int LastCellPosition { get; set; }
        
        public float LifeSpan { get; set; }
        
        public bool IsActive { get; set; }
        
        
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }
        
        public void OnCollision();
    }
}