using System;
using System.Collections.Generic;
using UnityEngine.Pool;


namespace ToolBox.Utils.Pooling
{
    public class GenericPool<T> : IDisposable where T : class
    {
        private bool _isDisposed;

        private int _count;
        
        private ObjectPool<T> _pool;

        public ObjectPool<T> Pool => _pool;

        public GenericPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, int preLoadCount = 10, int maxSize = 100)
        {
            InitializePool(createFunc,  onGet,onRelease, onDestroy,preLoadCount, maxSize);
            
            Preload(preLoadCount,  maxSize);
        }

        private void InitializePool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, int preload = 10,  int maxSize = 100)
        {
            _pool = new ObjectPool<T>(
                createFunc: createFunc,
                actionOnGet: onGet,
                actionOnRelease: onRelease,
                actionOnDestroy: onDestroy,
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
            
            Logger.Log( $"Releasing object : { obj } back to pool");
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
