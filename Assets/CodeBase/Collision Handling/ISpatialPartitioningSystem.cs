using System.Collections.Generic;
using CodeBase.Projectile;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ISpatialPartitioningSystem
    {
        void RemoveFromSpatialPartitionGrid(ISpatialObject projectile, Vector2Int cellPosition);
        void AddToSpatialPartitionGrid(ISpatialObject projectile);
        HashSet<ISpatialObject> GetCellSet(Vector2Int key);
    }

    public interface ISpatialObject
    {
        public Vector3 GetPosition();
        
        public Vector3 LastPosition { get; set; }
        public Vector2Int LastCellPosition { get; set; }
        
        public float LifeSpan { get; set; }
        
        public bool IsActive { get; set; }
        
    }
}