using Unity.VisualScripting;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public interface ICollisionObject
    {
        Vector3 Position { get; }
        
        Vector3 Size { get; } 
     
        
        void OnCollision();
        
    }
}
