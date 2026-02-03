using System;
using System.Collections.Generic;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase
{
    public abstract class BasePoolManager<T> : MonoBehaviour where T : class
    {
        protected readonly Dictionary<string, ToolBox.Utils.Pooling.GenericPool<T>> Pools = new();
        
        protected virtual void Awake() => InitializePools();
        
        protected abstract void InitializePools(); // Implemented by the derived class
        
        protected virtual GenericPool<T> AddPool(string id, Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null,  int preload = 10, int maxSize = 100)
        {
            if (string.IsNullOrEmpty(id) || createFunc == null)
            {
                Logger.LogError( $"Required id or prefab is null or empty" );
                return null;
            }

            if (Pools.ContainsKey(id))
            {
                Logger.LogWarning($"Pool with id {id} already exists.");
                return null;
            }

            var pool = new GenericPool<T>(createFunc,  onGet, onRelease, onDestroy,preload,maxSize);
            
            Logger.Log($"Pool with id {id} has been added.");
            
            Pools.Add(id, pool);

            return pool;
        }
        
        public virtual T Get(string id)
        {
            if (Pools.TryGetValue(id, out var pool))
                return pool.Get();
            
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