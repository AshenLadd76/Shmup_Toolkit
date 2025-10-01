using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace ToolBox.Utils.Pooling
{
    public class GenericPool<T> : IDisposable where T : Component, IPoolable<T>
    {
        private bool _isDisposed;

        private int _count;
        
        private ObjectPool<T> _pool;
        public GenericPool(T prefab, Transform parent, int preLoadCount = 10, int maxSize = 100 )
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab), "Prefab cannot be null.");
            
            InitializePool(prefab, preLoadCount, maxSize, parent);
            
            Preload(preLoadCount,  maxSize);
        }

        private void InitializePool(T prefab, int preload, int maxSize,  Transform parent)
        {
            
            _pool = new ObjectPool<T>(
                createFunc: () =>
                {
                    var instance = Object.Instantiate(prefab, parent, true);

                    // Set pool reference in IPoolable object
                    if (instance is IPoolable<T> poolable)
                    {
                        poolable.SetParentPool(_pool);
                    }
                    
                    return instance;
                },
                actionOnGet: obj =>
                {
                    obj.OnGetFromPool();
                },
                actionOnRelease: obj => obj.OnReturnedToPool(),
                actionOnDestroy: obj =>
                {
                   
                    Object.Destroy(obj.gameObject);
                },
                collectionCheck: true,
                defaultCapacity: preload,
                maxSize: maxSize
            );
        }

        private void Preload(int preloadCount, int maxSize)
        {
            if (_pool == null)
            {
                Logger.Log( $"Pool is null so abort" );
                return;
            }
            
            var list = new List<T>();
            
            for (int i = 0; i < preloadCount; i++)
            {
                var obj = _pool.Get();
                
                obj.name = $"{obj.name} {i + 1}";
                
                list.Add(obj);
            }
            
            foreach (var obj in list)
                _pool.Release(obj);
            
        }
        

        public T Get()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(GenericPool<T>));

            return _pool.Get();
        }

        public void Release(T obj)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(GenericPool<T>));
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Cannot release null object to pool.");
            
            Logger.Log( $"Releasing object : { obj.name } back to pool");
            _pool.Release(obj);
        }

        public void Dispose()
        {
            Logger.Log( $"Disposing was called....");
            if (_isDisposed) return;
            
            _pool.Dispose();

            _isDisposed = true;
        }
    }
}
