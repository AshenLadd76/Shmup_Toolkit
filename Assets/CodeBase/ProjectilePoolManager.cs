using CodeBase.Projectile;
using CodeBase.Shmup;
using ToolBox.Utils.Validation;
using UnityEngine;

namespace CodeBase
{
    [RequireComponent(typeof(ActiveProjectileManager))]
    public class ProjectilePoolManager : BasePoolManager<Projectile.Projectile>
    {
        [Validate, SerializeField] private ProjectileDataBaseSo projectileDatabaseSo;
        
        private ActiveProjectileManager _activeProjectileManager;
        
        protected override void Awake()
        {
            _activeProjectileManager = GetComponent<ActiveProjectileManager>();
            
            ObjectValidator.Validate(this, this, true);
            
            InitializePools();
        }
        
        protected override void InitializePools()
        {
            foreach (var projectileData in projectileDatabaseSo.ProjectilesList)
                AddPool( projectileData.id, projectileData.projectile, projectileData.minPoolSize, projectileData.maxPoolSize, transform );
        }
        
        
        public override Projectile.Projectile Get(string key) 
        {
            var projectile = base.Get(key);

            projectile?.SetActiveProjectileManager(_activeProjectileManager);
            
            return projectile;
        }
    }
}
