using System.Collections;
using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Utils.Pooling;
using UnityEngine;

namespace CodeBase.Audio
{
    public class SfxAudioService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
        
        private readonly List<AudioSource> _activeSfxAudioSources = new();
        
        private Coroutine _cleanupCoroutine;
        
        private readonly ICoroutineRunner _coroutineRunner;

        private const int MinDistance = 1;
        private const int MaxDistance = 100;
        
        public SfxAudioService( ICoroutineRunner coroutineRunner, IPool<AudioSource> audioSourcePool)
        { 
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }
        
        public void PlayOneShot(IAudioDefinition audioDefinition)
        {
            if (audioDefinition?.Clip == null) return; 
            
            var audioSource = GetAndConfigAudioSource(audioDefinition);
            
            _activeSfxAudioSources.Add(audioSource);
            
            audioSource.Play();
            
            StartCleanUpCoroutine();
        }

        public void PlayOneShotAtPosition(IAudioDefinition audioDefinition, Vector3 position)
        {
            if (audioDefinition.Clip == null) return; 
            
            audioDefinition.SpatialBlend = 1f;
            audioDefinition.MinDistance = MinDistance;
            audioDefinition.MaxDistance = MaxDistance;
            
            var audioSource = GetAndConfigAudioSource(audioDefinition);
            
            audioSource.transform.position = position;
            
            _activeSfxAudioSources.Add(audioSource);
            
            audioSource.Play();
            
            StartCleanUpCoroutine();
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
       
        
        
        private void StartCleanUpCoroutine()
        {
            if (_cleanupCoroutine != null) return;

            _cleanupCoroutine = _coroutineRunner.StartCoroutine(ReleaseOneShotAfterPlay());
        }
        
        private IEnumerator ReleaseOneShotAfterPlay()
        {
            const float maxDelay = 0.5f;
            const float minDelay = 0.05f;
            
            while (_activeSfxAudioSources.Count > 0)
            {
                for (int x = _activeSfxAudioSources.Count - 1; x >= 0; x--)
                {
                    if (!_activeSfxAudioSources[x].clip || _activeSfxAudioSources[x].isPlaying) continue;
                    
                    var audioSource = _activeSfxAudioSources[x];
                    
                    _audioSourcePool.Release(audioSource);
                    _activeSfxAudioSources.RemoveAt(x);
                }
                
                float delay = Mathf.Clamp(maxDelay / Mathf.Max(1, _activeSfxAudioSources.Count), minDelay, maxDelay);
                
                yield return new WaitForSeconds(delay);
            }
            
            _cleanupCoroutine = null;
        }
    }
}