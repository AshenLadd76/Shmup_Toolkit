using System.Collections;
using CodeBase.Patterns;
using CodeBase.Projectile;
using UnityEngine;

namespace CodeBase.Weapons
{
    public class WeaponSystem : MonoBehaviour, IWeapon
    {
        [SerializeField] private Transform muzzleTransform;
        
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileLifeSpan = 4f;

        [SerializeField] private Vector3 movementDirection;
        
        private Coroutine _fireCoroutine;
        
        private WaitForSeconds _fireWait;


        private void Awake()
        {
            _fireWait = new WaitForSeconds(fireRate);
        }
        
        public void Fire()
        {
            if (_fireCoroutine != null) return;
            
            _fireCoroutine = StartCoroutine(FireCoroutine());

        }
        
        private IEnumerator FireCoroutine()
        {
            while (true)
            {
                InitializeProjectile( movementDirection, Quaternion.identity, muzzleTransform.position,projectileSpeed, projectileLifeSpan  );
                
                yield return _fireWait;
            }
        }

        public void StopFire()
        {
            if (_fireCoroutine == null) return;
            
            StopCoroutine(_fireCoroutine);

            _fireCoroutine = null;
        }
        
        private Projectile.Projectile GetProjectileFromPool() => poolManager.Get(ShmupStrings.TypeAProjectile);
        
        private void InitializeProjectile(Vector3 direction, Quaternion rotation,  Vector3 position, float speed, float lifeSpan)
        {
            IProjectile projectile = GetProjectileFromPool();
            
            projectile.LifeSpan = lifeSpan;
            projectile.Radius = 0.1f;
            projectile.Speed = speed;
            projectile.SetDirection(direction.normalized);
            projectile.SetPosition(position);
            projectile.SetRotation(rotation);
            projectile.IsActive = true;
        }
    }
}
