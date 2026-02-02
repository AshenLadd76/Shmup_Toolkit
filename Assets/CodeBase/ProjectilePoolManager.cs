using CodeBase.Projectile;
using CodeBase.Shmup;
using UnityEngine;

namespace CodeBase
{
    [RequireComponent(typeof(ActiveProjectileManager))]
    public class ProjectilePoolManager : BasePoolManager<Projectile.NeoProjectile>
    {
        [SerializeField] private ProjectileDataBaseSo projectileDatabaseSo;
        
        private ActiveProjectileManager _activeProjectileManager;
        
        protected override void Awake()
        {
            _activeProjectileManager = GetComponent<ActiveProjectileManager>();
            
            InitializePools();
     
        }

        private void Start()
        {
          
        }
        
        protected override void InitializePools()
        {
            foreach (var projectileData in projectileDatabaseSo.ProjectilesList)
            {
                //Func
                NeoProjectile CreateProjectile()
                {
                    var go = projectileData.ProjectilePrefab;
                    
                    var projectileInstance = Instantiate(go, transform, true);

                    var neoProjectile = new NeoProjectile(projectileInstance.transform);

                    return neoProjectile;
                }

                // Optional Actions
                void OnGet(NeoProjectile p) => p.OnGetFromPool();
                void OnRelease(NeoProjectile p) => p.OnReturnedToPool();
                void OnDestroy(NeoProjectile p) => Destroy(p.Transform.gameObject);

                AddPool(projectileData.id, CreateProjectile, OnGet,OnRelease, OnDestroy,projectileData.minPoolSize, projectileData.maxPoolSize);
            }
        }
        
        public override NeoProjectile Get(string key) 
        {
            var projectile = base.Get(key);

            projectile?.SetActiveProjectileManager(_activeProjectileManager);
            projectile?.SetParentPool(Pools[key].Pool);
            projectile?.OnGetFromPool();
            
            
            return projectile;
        }
    }
}
