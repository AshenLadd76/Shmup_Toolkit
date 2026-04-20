using System.Collections;
using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public class SfxAudioService
    {
        private readonly IPool<AudioSource> _audioSourcePool;
   
        private readonly List<AudioSource> _activeSfxAudioSources = new();
        
        private Coroutine _cleanupCoroutine;
        
        private readonly ICoroutineRunner _coroutineRunner;
        
        public SfxAudioService( ICoroutineRunner coroutineRunner, IPool<AudioSource> audioSourcePool)
        { 
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }
        
        public void PlayOneShot(IAudioDefinition audioDefinition) => PlayOneShotAtPosition( audioDefinition, Vector3.zero);
        
        
        public void PlayOneShotAtPosition(IAudioDefinition audioDefinition, Vector3 position)
        {
            if (audioDefinition?.Clip == null)
            {
                Logger.LogError($"Audio definition or Audio clip is null");
                return;
            }
            
            var audioSource = AudioSourceConfigurator.ConfigAudioSource(audioDefinition, _audioSourcePool.Get());

            if (audioSource == null)
            {
                Logger.LogError($"Audio source is null");
                return;
            }
            
            audioSource.transform.position = position;
            
            PlayAudioSource(audioSource);
        }
        
        private void PlayAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) return;
            
            _activeSfxAudioSources.Add(audioSource);
            
            audioSource.Play();
            StartCleanUpCoroutine();
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
                for (var x = _activeSfxAudioSources.Count - 1; x >= 0; x--)
                {
                    var audioSource = _activeSfxAudioSources[x];

                    if (audioSource != null && audioSource.isPlaying) continue;
                    
                    AudioSourceConfigurator.ResetAudioSource(audioSource);
                    
                    _audioSourcePool.Release(audioSource);
                    _activeSfxAudioSources.RemoveAt(x);
                }
                
                // Check more frequently with many active sources, less frequently with few
                var delay = Mathf.Clamp(maxDelay / Mathf.Max(1, _activeSfxAudioSources.Count), minDelay, maxDelay);
                
                yield return new WaitForSeconds(delay);
            }
            
            _cleanupCoroutine = null;
        }
    }
}