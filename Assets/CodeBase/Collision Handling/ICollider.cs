using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ICollider 
    {
        Vector2 Position { get; set; }
        
        float Radius { get; set; }
        
        List<Vector2Int> OccupiedCells { get; set; }
        
        // Calculates and returns world-space bounding box
        (Vector2 min, Vector2 max) GetWorldBounds();
    }
}
