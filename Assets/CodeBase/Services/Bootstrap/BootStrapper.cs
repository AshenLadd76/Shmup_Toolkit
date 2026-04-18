using UnityEngine;

namespace CodeBase.Services.Bootstrap
{
    /// <summary>
    /// responsible for initializing and persisting systems or objects
    /// in the game at the start of the application before the scene loads.
    /// </summary>
    /// 
    public static class Bootstrapper
    {
        /// <summary>
        /// The name of the resource to load and instantiate for bootstrapping.
        /// </summary>
        private const string ResourceToLoad = "Services";
        
        /// <summary>
        /// Method that executes before the scene loads to initialize and persist certain systems or objects.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            var systemPrefab = Resources.Load<GameObject>(ResourceToLoad);
            
            if (systemPrefab == null)
            {
                Debug.LogError($"Failed to find the \"{ResourceToLoad}\" prefab resource. Make sure it exists in the Resources folder.");
                return;
            }

            var instantiatedSystem = Object.Instantiate(systemPrefab);
            
            if (instantiatedSystem == null)
            {
                Debug.LogError($"Failed to instantiate the \"{ResourceToLoad}\" prefab.");
                return;
            }

            // Mark the instantiated system as persistent across scenes.
            Object.DontDestroyOnLoad(instantiatedSystem);
        }
    }
}

