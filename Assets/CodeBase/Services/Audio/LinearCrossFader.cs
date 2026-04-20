using System;
using System.Collections;
using ToolBox.Helpers;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public interface ICrossFader
    {
        void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource,  Action onCrossFadeComplete, float duration);
    }

    public class LinearCrossFader : ICrossFader
    {
        private readonly ICoroutineRunner _coroutineRunner;
        
        public LinearCrossFader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
        }

        
        private Coroutine _crossFadeCoroutine;

        public void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource,  Action onCrossFadeComplete, float duration)
        {
            if (fadeOutSource == null || fadeInSource == null)
            {
                Logger.LogError("Required parameters are missing. fadeoutSource or fadeInSource is null.");
                return;
            }

            if(_crossFadeCoroutine != null) return;
            
            _crossFadeCoroutine = _coroutineRunner.StartCoroutine(CrossFadeCr(fadeOutSource, fadeInSource,onCrossFadeComplete, duration ));
        }
        
        private IEnumerator CrossFadeCr(AudioSource fadeOutSource, AudioSource fadeInSource,  Action onCrossFadeComplete, float duration = 0.1f)
        {
            float elapsedTime = 0;

            fadeInSource.volume = 0;
            fadeOutSource.volume = 1;
            
            fadeInSource.Play();

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                // True equal-power crossfade (constant perceived volume)
                fadeInSource.volume = Mathf.Sin(t * Mathf.PI * 0.5f);     // Smooth sine curve
                fadeOutSource.volume  = Mathf.Cos(t * Mathf.PI * 0.5f);     // Complementary cosine
                
                yield return null;
            }
            
            fadeInSource.volume = 1f;
            fadeOutSource.volume = 0f;
            
            fadeOutSource.Stop();
            
            onCrossFadeComplete?.Invoke();
            
            _crossFadeCoroutine = null;
            
            Logger.Log($"AudioService crossfade finished {elapsedTime}");
        }
    }
}