using System.Collections;
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

        private string _currentMusicTrack;
        
        private AudioCrossFader _audioCrossFader;
        
        public MusicAudioService(ICoroutineRunner coroutineRunner, IPool<AudioSource> audioSourcePool)
        {
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
            
            _audioCrossFader = new AudioCrossFader(coroutineRunner, _audioSourcePool);
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

        
        public void PlayMusic(string key, IAudioDefinition audioDefinition)
        {
            _currentMusicTrack = key;
            PlayAudioLoop(key, audioDefinition);
        }
        
        public void CrossFadeAudioTrack(string fadeInId, IAudioDefinition audioDefinition)
        {
            if (string.IsNullOrEmpty(_currentMusicTrack)) return;
            
             
            if (!_activeAudioSources.TryGetValue(_currentMusicTrack, out var fadeOutAudioSource))
            {
                Logger.LogError($"AudioService clip not found {_currentMusicTrack}");
                return;
            }
        
            var fadeInAudioSource = GetAndConfigAudioSource(audioDefinition);
            
            _activeAudioSources[fadeInId] = fadeInAudioSource;
            
            _currentMusicTrack = fadeInId;
            
            _audioCrossFader.CrossFade( fadeOutAudioSource, fadeInAudioSource, 4f );
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

    public class AudioCrossFader
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPool<AudioSource> _audioSourcePool;

        public AudioCrossFader(ICoroutineRunner coroutineRunner, IPool<AudioSource> audioSourcePool)
        {
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _audioSourcePool = audioSourcePool ?? throw new System.ArgumentNullException(nameof(audioSourcePool));
        }

        
        private Coroutine _crossFadeCoroutine;

        public void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float duration)
        {
            if(_crossFadeCoroutine != null) return;
            
            _crossFadeCoroutine = _coroutineRunner.StartCoroutine(CrossFadeCr(fadeOutSource, fadeInSource, duration));
        }
        
        private IEnumerator CrossFadeCr(AudioSource fadeOutSource, AudioSource fadeInSource, float duration)
        {
            float elapsedTime = 0;

            fadeInSource.volume = 0;
            fadeOutSource.volume = 1;
            
            fadeInSource.Play();

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float t = Mathf.Clamp01(elapsedTime / duration);

                fadeInSource.volume = t;
                fadeOutSource.volume = 1 - t;
                
                yield return null;
            }
            
            fadeOutSource.Stop();
            
            _audioSourcePool.Release(fadeOutSource);
            
            _crossFadeCoroutine = null;
            
            Logger.Log($"AudioService crossfade finished {elapsedTime}");
        }
    }
}