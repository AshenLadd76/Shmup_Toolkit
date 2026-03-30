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
            
            _activeAudioSources[(owner,key)] =  audioSource;
            
            audioSource.Play();
        }

        public void PlayAudioLoopAtPosition(Object owner, string key, IAudioDefinition audioDefinition, Vector3 position)
        {
            if( audioDefinition?.Clip == null ) return;  
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());
            
            Logger.Log($"AudioService playing audio loop at {position}");
            
            audioSource.transform.position = position;
            audioSource.spatialBlend = 1;
            audioSource.minDistance = MinDistance;
            audioSource.maxDistance = MaxDistance;
            
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
        }

        
        public void PlayMusic(Object owner, string id, IAudioDefinition audioDefinition)
        {
            
            if( owner == null || string.IsNullOrEmpty(id)) return;
            
            string key = id.Trim().ToLower();
            
            if (_activeAudioSources.TryGetValue((owner, key), out var audioSourceToStop))
            {
                Logger.LogError($"AudioSource with owner: {owner} and key: { key } is already playing. ");
                return;
            }
            
            _currentMusicTrack = (owner, key);
            
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
        }
        
        public void CrossFadeAudioTrack(Object owner, string fadeInId, IAudioDefinition audioDefinition)
        {
            if (_currentMusicTrack.owner == null || string.IsNullOrEmpty(_currentMusicTrack.id)) return;
           
           
            var keyToAdd = (owner, fadeInId);
            
            if (!_activeAudioSources.TryGetValue(_currentMusicTrack, out var fadeOutAudioSource))
            {
                Logger.LogError($"AudioService clip not found {_currentMusicTrack}");
                return;
            }
        
            var audioSource = _audioSourcePool.Get();
            var fadeInAudioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, audioSource);
            
            _activeAudioSources[keyToAdd] = fadeInAudioSource;
            
            _currentMusicTrack = keyToAdd;
            
            _audioCrossFader.CrossFade( fadeOutAudioSource, fadeInAudioSource, () => { _audioSourcePool.Release(fadeOutAudioSource);
            }, 4f );
        }
    }

    public static class AudioSourceConfigurator
    {
        public static AudioSource ConfigAudioSource(IAudioDefinition audioDefinition, AudioSource audioSource)
        {
            audioSource.transform.localPosition = Vector3.zero;
            
            Logger.Log($"AudioService playing audio clip { audioDefinition.SpatialBlend } {audioDefinition.MinDistance} to {audioDefinition.MaxDistance}");
                 
            audioSource.clip = audioDefinition.Clip;
            audioSource.playOnAwake = audioDefinition.PlayOnAwake;
            audioSource.loop = audioDefinition.Loop;
            audioSource.volume = audioDefinition.Volume;
            audioSource.mute = audioDefinition.Mute;
            audioSource.pitch =  audioDefinition.Pitch;
            audioSource.spatialBlend = audioDefinition.SpatialBlend;
            audioSource.minDistance = audioDefinition.MinDistance;
            audioSource.maxDistance = audioDefinition.MaxDistance;
            audioSource.rolloffMode = audioDefinition.RolloffMode;
            audioSource.bypassEffects = audioDefinition.BypassEffects;
            audioSource.bypassReverbZones = audioDefinition.BypassReverbZones;
            audioSource.bypassListenerEffects = audioDefinition.BypassListenerEffects;
            
            return audioSource;
        }
    }
}