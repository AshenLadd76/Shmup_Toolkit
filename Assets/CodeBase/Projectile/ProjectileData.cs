using UnityEngine;

namespace CodeBase.Projectile
{
    [System.Serializable]
    public class ProjectileData
    {
        [Tooltip("unique id of the projectile")]
        public string id;

        [Tooltip("Prefab with Projectile component attached")]
        
        
        [SerializeField] private GameObject projectilePrefab;
        public GameObject ProjectilePrefab => projectilePrefab;


        [Tooltip("Animation frames for the projectile")]
        [SerializeField] private Sprite[] animationsFrames;

        public Sprite[] AnimationsFrames
        {
            get => animationsFrames;
            set => animationsFrames = value;
        }

        [Tooltip("Minimum number of instances to prewarm in the pool")]
        public int minPoolSize = 5;

        [Tooltip("Maximum number of instances the pool can grow to")]
        public int maxPoolSize = 20;
        
        
       
    }
}
