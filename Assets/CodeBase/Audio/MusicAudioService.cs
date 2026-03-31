using System.Collections.Generic;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public class MusicAudioService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
        private readonly ICrossFader _audioCrossFader;
        
        private readonly Dictionary<(Object owner,string id), AudioSource> _activeAudioSources = new();
        
        private Coroutine _cleanupCoroutine;
        
        private (Object owner,string id) _currentMusicTrack;
        
        private const int MinDistance = 1;
        private const int MaxDistance = 100;
        
        public MusicAudioService(IPool<AudioSource> audioSourcePool, ICrossFader audioCrossFader)
        {
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
            _audioCrossFader = audioCrossFader ?? throw new System.ArgumentNullException(nameof(audioCrossFader));
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
            
            audioSource.Play();
        }

        public void PlayAudioLoopAtPosition(Object owner, string key, IAudioDefinition audioDefinition, Vector3 position)
        {
            if( audioDefinition?.Clip == null ) return;  
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            Logger.Log($"AudioService playing audio loop at {position}");
            
            if (_activeAudioSources.TryGetValue((owner, key), out var existing))
            {
                existing.Stop();
                _audioSourcePool.Release(existing);
            }
            
            _activeAudioSources[(owner, key)] =  audioSource;
            
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

        
        public void PlayMusic(Object owner, string id, IAudioDefinition audioDefinition)
        {
            if( owner == null || string.IsNullOrEmpty(id)) return;
            
            if (_activeAudioSources.TryGetValue((owner, id), out var audioSourceToStop))
            {
                Logger.LogError($"AudioSource with owner: {owner} and key: {id } is already playing. ");
                return;
            }
            
            _currentMusicTrack = (owner, id);
            
            Logger.Log($"AudioService playing music {_currentMusicTrack}");
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            _activeAudioSources[_currentMusicTrack] =  audioSource;
            
            audioSource.Play();
        }

        public void StopMusic(Object owner, string key)
        {
            if (!_activeAudioSources.TryGetValue((owner, key), out var audioSourceToStop))
            {
                Logger.LogError($"AudioService clip not found {_currentMusicTrack}");
                return;
            }
            
            audioSourceToStop.volume = 0f;
            audioSourceToStop.Stop();
            
            _activeAudioSources.Remove((owner, key));
            
            _audioSourcePool.Release(audioSourceToStop);
            
            if (_currentMusicTrack == (owner, key))
                _currentMusicTrack = (null, null);
        }
        
        public void CrossFadeAudioTrack(Object owner, string fadeInId, IAudioDefinition audioDefinition, float fadeDuration = 3f)
        {
            if (_currentMusicTrack.owner == null || string.IsNullOrEmpty(_currentMusicTrack.id)) return;
            
            var keyToAdd = (owner, fadeInId);
            var keyToRemove = _currentMusicTrack;
            
            if (!_activeAudioSources.TryGetValue(_currentMusicTrack, out var fadeOutAudioSource))
            {
                Logger.LogError($"AudioService clip not found {_currentMusicTrack}");
                return;
            }
            
            var fadeInAudioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            _activeAudioSources[keyToAdd] = fadeInAudioSource;
            
            _currentMusicTrack = keyToAdd;
            
            _audioCrossFader.CrossFade( fadeOutAudioSource, fadeInAudioSource, () =>
            {
                _activeAudioSources.Remove(keyToRemove);
                _audioSourcePool.Release(fadeOutAudioSource);
            }, fadeDuration );
        }
    }
}