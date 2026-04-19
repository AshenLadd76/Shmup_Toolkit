using System.Collections.Generic;
using CodeBase.Audio;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public class MusicService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
        private readonly ICrossFader _audioCrossFader;
        
        private readonly Dictionary<(Object owner,string id), AudioSource> _activeAudioSources = new();
        
        private (Object owner,string id) _currentMusicTrack;
        
        public MusicService(IPool<AudioSource> audioSourcePool, ICrossFader audioCrossFader)
        {
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
            _audioCrossFader = audioCrossFader ?? throw new System.ArgumentNullException(nameof(audioCrossFader));
        }
        
        public void PlayMusic(Object owner, string id, IAudioDefinition audioDefinition)
        {
            if( owner == null || string.IsNullOrEmpty(id)) return;
            
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            if (_activeAudioSources.TryGetValue((owner, id), out var activeAudioSource))
            {
                activeAudioSource.Stop();
                _audioSourcePool.Release(activeAudioSource);
            }
            
            _currentMusicTrack = (owner, id);
            
            _activeAudioSources[_currentMusicTrack] =  audioSource;
            
            audioSource.Play();
        }
        
        public void PlayMusicAtPosition(Object owner, string key, IAudioDefinition audioDefinition, Vector3 position)
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
            if (_currentMusicTrack.owner == null || string.IsNullOrEmpty(_currentMusicTrack.id))
            {
                Logger.LogError( $"No valid audio track to cross fade out from" );
                return;
            }

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