using System.Collections.Generic;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase
{
    public abstract class BasePoolManager<T> : MonoBehaviour where T : Component, IPoolable<T>
    {
        protected readonly Dictionary<string, GenericPool<T>> Pools = new();
        
        protected virtual void Awake() => InitializePools();
        
        protected abstract void InitializePools(); // Implemented by the derived class
        
        public bool AddPool(string id, T prefab, int preload = 10, int maxSize = 100, Transform parent = null)
        {
            if (string.IsNullOrEmpty(id) || prefab == null)
            {
                Logger.LogError( $"Required id or prefab is null or empty" );
                return false;
            }

            if (Pools.ContainsKey(id))
            {
                Logger.LogWarning($"Pool with id {id} already exists.");
                return false;
            }

            var pool = new GenericPool<T>(prefab, parent, preload, maxSize);
            
            Pools.Add(id, pool);
            
            return true;
        }
        
        public virtual T Get(string id)
        {
            if (Pools.TryGetValue(id, out var pool))
                return pool.Get();
            
            Debug.LogError($"Pool with key '{id}' not found.");
            
            return null;
        }
        
        public void Release(string id, T obj)
        {
            if (obj == null) return;
            
            if (Pools.TryGetValue(id, out var pool))
                pool.Release(obj);
            else
                Debug.LogError($"Pool with key '{id}' not found for release.");
        }
        
        
        public void ClearAllPools()
        {
            foreach (var pool in Pools.Values)
                pool.Dispose();
            
            Pools.Clear();
        }
    }
}