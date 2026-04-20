using System.Collections.Generic;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public class LoopingAudioService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
        
        private readonly Dictionary<(Object owner,string id), AudioSource> _activeAudioSources = new();
   
        public LoopingAudioService(IPool<AudioSource> audioSourcePool)
        {
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }

        public void PlayAudioLoop(Object owner, string key, IAudioDefinition audioDefinition) => PlayAudioLoopAtPosition(owner,key,audioDefinition,Vector3.zero);
        
        public void PlayAudioLoopAtPosition(Object owner, string key, IAudioDefinition audioDefinition, Vector3 position)
        {
            if (owner == null || string.IsNullOrEmpty(key) || audioDefinition?.Clip == null)
            {
                Logger.LogError($"PlayAudioLoopAtPosition: owner={owner}, key='{key}', clip={audioDefinition?.Clip}");
                return;
            }

            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            if (_activeAudioSources.TryGetValue((owner, key), out var activeAudioSource))
            {
                activeAudioSource.Stop();
                _audioSourcePool.Release(activeAudioSource);
            }
            
            _activeAudioSources[(owner, key)] =  audioSource;
            
            audioSource.transform.position = position;
            audioSource.Play();
        }

        public void StopAudioLoop(Object owner, string key)
        {
            if( owner == null || string.IsNullOrEmpty(key) ) return;
            
            if (!_activeAudioSources.TryGetValue((owner, key), out var audioSourceToStop))
            {
                Logger.LogError($"No active AudioSource found for owner: {owner} key: {key}");
                return;
            }
            
            //TODO: add fade-out here
            audioSourceToStop.volume = 0f;
            audioSourceToStop.Stop();
            
            _activeAudioSources.Remove((owner, key));
            
            _audioSourcePool.Release(audioSourceToStop);
        }
    }
}