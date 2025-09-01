using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToolBox.Utils
{
    /// <summary>
    /// Utility class for  synchronously and asynchronously loading scenes
    /// Supports both additive and single scene loads with progress and completion callbacks.
    /// </summary>
    ///
    public static class SceneLoader
    {
        private const float SceneLoadProgressThreshold = 0.9f;
        private const float MaxProgressThreshold = 1f;
        private static bool _isAsyncLoaderBusy;
        private static bool _isAsyncUnLoaderBusy;

        /// <summary>
        /// Loads a scene synchronously
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="onComplete">Optional Callback invoked once the scene is fully loaded.</param>
        /// <param name="onFailure">Optional Callback invoked when failure ooccurs.</param>
        public static void LoadScene(string sceneName, Action onComplete = null, Action onFailure = null)
        {
            if (!CanSceneBeLoaded(sceneName)) return;

            try
            {
                SceneManager.LoadScene(sceneName);
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                Logger.LogError($"Error loading scene '{sceneName}': {e.Message}");
                onFailure?.Invoke();
            }
        }
        
        /// <summary>
        /// Loads a scene asynchronously with optional progress and completion callbacks.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="onProgress">Callback invoked every frame with the current load progress (0 to 1).</param>
        /// <param name="onComplete">Callback invoked once the scene is fully loaded.</param>
        /// <param name="onFailure">Optional Callback invoked when failure ooccurs.</param>
        /// <param name="mode">The load mode (Additive by default).</param>
        /// <returns>Coroutine IEnumerator for use with StartCoroutine.</returns>
        public static IEnumerator LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action onComplete = null, Action onFailure = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (_isAsyncLoaderBusy) yield break;  
            
            if (!CanSceneBeLoaded(sceneName)) yield break;

            if (IsSceneLoaded(sceneName))
            {
                onProgress?.Invoke(MaxProgressThreshold);
                onComplete?.Invoke();
                yield break;
            }

            _isAsyncLoaderBusy = true;

            var asyncOp = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            
            if (asyncOp == null)
            {
                Logger.LogError($"Failed to start async load for scene '{sceneName}'.");
                _isAsyncLoaderBusy = false;
                onFailure?.Invoke();
                yield break;
            }

            while (!asyncOp.isDone)
            {
                var progress = Mathf.Clamp01(asyncOp.progress / SceneLoadProgressThreshold);
                
                onProgress?.Invoke(progress);
                
                yield return null;
            }
            
            onProgress?.Invoke(MaxProgressThreshold);
            
            onComplete?.Invoke();

            _isAsyncLoaderBusy = false;
        }


        /// <summary>
        /// unLoads a scene asynchronously
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="onComplete">Optional Callback invoked once the scene is fully unloaded.</param>
        /// <param name="onFailure">Optional Callback invoked in the event of failure</param>
        /// <returns>Coroutine IEnumerator for use with StartCoroutine.</returns>
        public static IEnumerator UnloadScene(string sceneName, Action onComplete = null, Action onFailure = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Logger.LogError( $"String supplied is null or empty" );
                onFailure?.Invoke();
                yield break;
            }
            
            if (!IsSceneLoaded(sceneName)) yield break;

            if (_isAsyncUnLoaderBusy) yield break;  
            
            _isAsyncUnLoaderBusy = true;
            
            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            
            if (asyncOp == null)
            {
                _isAsyncUnLoaderBusy = false;
                onFailure?.Invoke();
                yield break;
            }
            
            while (!asyncOp.isDone)
                yield return null;

            onComplete?.Invoke();

            _isAsyncUnLoaderBusy = false;
        }

          
        /// <summary>
        /// Checks to see if the named scene can be loaded.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check.</param>
        /// <returns>bool</returns>
        private static bool CanSceneBeLoaded(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Logger.LogError( $"String supplied is null or empty" );
                return false;
            }
            
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Logger.LogError($"Scene '{sceneName}' cannot be loaded.");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Checks to see if a scene is loaded
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>bool</returns>
        private static bool IsSceneLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;
    }
}