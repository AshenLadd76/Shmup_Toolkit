using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public class MusicAudioService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
        
        private Dictionary<string, AudioSource> _activeAudioSources = new Dictionary<string, AudioSource>();
        
        private Coroutine _cleanupCoroutine;
        
        private readonly ICoroutineRunner _coroutineRunner;
        
        public MusicAudioService(ICoroutineRunner coroutineRunner, IPool<AudioSource> audioSourcePool)
        {
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }

        public void PlayAudioLoop(string key, IAudioDefinition audioDefinition)
        {
            if (audioDefinition?.Clip == null) return; 
            
            var audioSource = GetAndConfigAudioSource(audioDefinition);
            
            _activeAudioSources[key] =  audioSource;
            
            audioSource.Play();
        }

        public void PlayAudioLoopAtPosition(string key, IAudioDefinition audioDefinition, Vector3 position)
        {
            PlayAudioLoop(key, audioDefinition);
            
            Logger.Log($"AudioService playing audio loop at {position}");
            
            
        }

        public void StopAudioLoop(string key)
        {
           
        }
        
        private AudioSource GetAndConfigAudioSource(IAudioDefinition audioDefinition)
        {
            var audioSource = _audioSourcePool.Get();
            
            audioSource.transform.localPosition = Vector3.zero;
                 
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