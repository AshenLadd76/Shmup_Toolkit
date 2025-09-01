using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Shmup
{
    [CreateAssetMenu(fileName = "ProjectileDatabase", menuName = "Pooling/Projectile Database")]

    public class ProjectileDataBaseSo : ScriptableObject
    {
        [SerializeField]
        private List<ProjectileData> projectileDataList = new List<ProjectileData>();

        public List<ProjectileData> ProjectilesList => projectileDataList;
    }
}
