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
        
        private Coroutine _cleanupCoroutine;
        
        private (Object owner,string id) _currentMusicTrack;
       
        
        public LoopingAudioService(IPool<AudioSource> audioSourcePool)
        {
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }

        public void PlayAudioLoop(Object owner, string key, IAudioDefinition audioDefinition)
        {
            if (audioDefinition?.Clip == null) return; 
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            if (_activeAudioSources.TryGetValue((owner, key), out var existing))
            {
                existing.Stop();
                _audioSourcePool.Release(existing);
            }
            
            _activeAudioSources[(owner,key)] =  audioSource;
            
            if(audioDefinition.AudioType == AudioCommand.Music)
                _currentMusicTrack = (owner, key);
            
            audioSource.Play();
        }

        public void PlayAudioLoopAtPosition(Object owner, string key, IAudioDefinition audioDefinition, Vector3 position)
        {
            if( audioDefinition?.Clip == null ) return;  
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            if (_activeAudioSources.TryGetValue((owner, key), out var activeAudioSource))
            {
                activeAudioSource.Stop();
                _audioSourcePool.Release(activeAudioSource);
            }
            
            _activeAudioSources[(owner, key)] =  audioSource;
            
            if(audioDefinition.AudioType == AudioCommand.Music)
                _currentMusicTrack = (owner, key);
            
            audioSource.transform.position = position;
            audioSource.Play();
        }

        public void StopAudioLoop(Object owner, string key)
        {
            if( owner == null || string.IsNullOrEmpty(key) ) return;
            
            if (!_activeAudioSources.TryGetValue((owner, key), out var audioSourceToStop))
            {
                Logger.LogError($"AudioService clip not found {_currentMusicTrack}");
                return;
            }
            
            audioSourceToStop.volume = 0f;
            audioSourceToStop.Stop();
            
            _activeAudioSources.Remove((owner, key));
            
            _audioSourcePool.Release(audioSourceToStop);
        }
    }
}