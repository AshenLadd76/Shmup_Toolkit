using CodeBase.Projectile;
using Unity.Collections;
using UnityEngine.Jobs;

namespace CodeBase.Jobs
{
    public struct ProjectileMoverJob : IJobParallelForTransform
    {
        public float DeltaTime;

        public int ActiveProjectileCount;
        
        public NativeArray<ProjectileDataStruct> ProjectileDataArray;
        
        
        public void Execute(int index, TransformAccess transform)
        {
            if (index >= ActiveProjectileCount) return;
            
            ProjectileDataStruct projectileData = ProjectileDataArray[index];
            
            projectileData.Position += projectileData.Velocity * DeltaTime;
            
            projectileData.LifeSpan -= DeltaTime;
            
            ProjectileDataArray[index] = projectileData;
            
            // Write back to the transform
            transform.position = projectileData.Position;
         }
    }
}
