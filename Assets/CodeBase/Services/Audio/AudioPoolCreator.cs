using ToolBox.Utils.Pooling;
using UnityEngine;

namespace CodeBase.Services.Audio
{
    public interface IAudioPoolCreator
    {
        GenericPool<AudioSource> CreateAudioPool(string poolRootName, int preloadCount, int maxPoolSize, Transform parent);
    }

    public class AudioPoolCreator : IAudioPoolCreator
    {
        private const string PoolObjectName = "AudioSource";
        
        public GenericPool<AudioSource> CreateAudioPool(string poolRootName, int preloadCount, int maxPoolSize, Transform parent)
        {
            var poolRoot = new GameObject($"{poolRootName}");
            poolRoot.transform.SetParent(parent);
            
            return new GenericPool<AudioSource>(createFunc: () => {
                    
                    var go = new GameObject(PoolObjectName);
                    
                    go.transform.SetParent(poolRoot.transform);
                    
                    var audioSource = go.AddComponent<AudioSource>();
                    
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    
                    return audioSource;
                },
                onGet: source => source.gameObject.SetActive(true),
                onRelease: source => {
                    source.Stop();
                    source.clip = null;
                    source.gameObject.SetActive(false);
                },
                preLoadCount: preloadCount,
                maxSize: maxPoolSize);
        }
    }
}