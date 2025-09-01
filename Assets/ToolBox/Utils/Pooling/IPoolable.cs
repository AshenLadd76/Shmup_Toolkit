using UnityEngine.Pool;

namespace ToolBox.Utils.Pooling
{
    public interface IPoolable<T> where T : class
    {
        public void SetParentPool(ObjectPool<T>  pool);
        public void Release();

        public void OnGetFromPool();

        public void OnReturnedToPool();
    }
}
