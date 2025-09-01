using System;
using System.Collections.Generic;
using CodeBase.Projectile;
using CodeBase.Shmup;
using ToolBox.Utils.Pooling;
using ToolBox.Utils.Validation;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase
{
    [RequireComponent(typeof(ActiveProjectileManager))]
    public class PoolManager : MonoBehaviour
    {
        [Validate, SerializeField] private ProjectileDataBaseSo projectileDatabaseSo;
        
        [Validate] private readonly Dictionary<string, GenericPool<Projectile.Projectile>> _pools = new();
        
        private ActiveProjectileManager _activeProjectileManager;
        
        private void Awake()
        {
            _activeProjectileManager = GetComponent<ActiveProjectileManager>();
            
            ObjectValidator.Validate(this, this, true);
            
            InitializePools();
        }
        
        private void InitializePools()
        {
            foreach (var projectileData in projectileDatabaseSo.ProjectilesList)
                AddPool( projectileData.id, projectileData.projectile, projectileData.minPoolSize, projectileData.maxPoolSize );
        }
        
        private void AddPool(string key, Projectile.Projectile prefab, int preload = 10, int maxSize = 100) 
        {
            Logger.Log( $"Initializing pool: {key} with minPoolSize: {preload} maxPoolSize: {maxSize}" );
            
            if (string.IsNullOrEmpty(key))
            {
                Logger.LogError( $"Required key is null or empty" );
                return;
            }
            
            if (_pools.ContainsKey(key))
            {
                Logger.LogWarning($"Pool with key {key} already exists.");
                return;
            }

            var pool = new GenericPool<Projectile.Projectile>(prefab, transform, preload, maxSize);
            
            _pools.Add(key, pool);
        }

        public void RemovePool(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.LogError($"required key is null or empty");
                return;
            }
            
            if (_pools.Remove(key, out var poolObj) && poolObj is IDisposable disposablePool)
                    disposablePool.Dispose();
            
        }
        
        
        public Projectile.Projectile Get(string key) 
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                var projectile = pool.Get();
                
                projectile.SetActiveProjectileManager( _activeProjectileManager);

                return projectile;
            }

            Logger.LogError($"Pool with key {key} not found or type mismatch.");
            return null;
        }
        
        public void Release(string key, Projectile.Projectile obj)
        {
            if (obj == null)
            {
                Logger.LogWarning($"Tried to release null object to pool: {key}");
                return;
            }
            
            if (_pools.TryGetValue(key, out var pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogError($"Pool with key {key} not found or type mismatch.");
            }
        }
        
        public void ClearAllPools()
        {
            foreach (var pool in _pools   )
            {
                if (pool.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _pools.Clear();
        }
    }
}
