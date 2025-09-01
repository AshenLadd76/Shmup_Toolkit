using System;
using System.Collections.Generic;
using ToolBox.Interfaces;
using ToolBox.Utils.Validation;
using UnityEngine;

namespace ToolBox.Utils.Pooling
{
    
    public class MegaPool : MonoBehaviour
    {
        private readonly Dictionary<string, object> _pools = new();

        [Validate] private IEventHandler _eventHandler;

       

        public void AddPool<T>(string key, T prefab, int preload = 10, int maxSize = 100) where T : Component, IPoolable<T>
        {
            if (_pools.ContainsKey(key))
            {
                Logger.LogWarning($"Pool with key {key} already exists.");
                return;
            }

            var pool = new GenericPool<T>(prefab, transform, preload, maxSize);
            _pools.Add(key, pool);
        }

        public void RemovePool(string key)
        {
            if (_pools.TryGetValue(key, out var poolObj))
            {
                if (poolObj is IDisposable disposablePool)
                {
                    disposablePool.Dispose();
                }
            }
        }
        
        public T Get<T>(string key) where T : Component, IPoolable<T>
        {
            if (_pools.TryGetValue(key, out var poolObj) && poolObj is GenericPool<T> pool)
            {
                return pool.Get();
            }

            Logger.LogError($"Pool with key {key} not found or type mismatch.");
            return null;
        }
        
        public void Release<T>(string key, T obj) where T : Component, IPoolable<T>
        {
            if (_pools.TryGetValue(key, out var poolObj) && poolObj is GenericPool<T> pool)
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogError($"Pool with key {key} not found or type mismatch.");
            }
        }
    }
}
