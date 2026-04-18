using UnityEngine;

namespace CodeBase.Services
{
    public interface IGameObjectFactory
    {
        GameObject Instantiate(string path);
    }

    public class GameObjectFactory : MonoBehaviour, IGameObjectFactory
    {
        public GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }
    
  
    }
}