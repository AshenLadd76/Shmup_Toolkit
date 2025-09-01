using System;
using System.Collections;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Performance.Fps
{
    public class TimedFrameRateBooster : ITimedFrameRateBooster
    {
        private Coroutine _boostCoroutine;
        private readonly MonoBehaviour _monoBehaviour;
        private readonly Action<int> _onSetFrameRate;
        
        public TimedFrameRateBooster(MonoBehaviour monoBehaviour, Action<int> onSetFrameRate )
        {
            _monoBehaviour = monoBehaviour ?? throw new ArgumentNullException(nameof(monoBehaviour));
            _onSetFrameRate = onSetFrameRate ?? throw new ArgumentNullException(nameof(onSetFrameRate));
        }
        
        public void TempBoostFps( int frameRate, int defaultFrameRate, float duration)
        {
            if (IsBoosting) return;
            
            _boostCoroutine = _monoBehaviour.StartCoroutine(BoostCoroutine(frameRate, defaultFrameRate, duration));
        }

        private IEnumerator BoostCoroutine(int boostFrameRate, int defaultFrameRate, float duration)
        {
                Logger.Log($"[TimedFrameRateBooster] Boosting frame rate to {boostFrameRate} for {duration} seconds.");
                
                _onSetFrameRate?.Invoke(boostFrameRate);

                yield return new WaitForSecondsRealtime(duration);
                
                _onSetFrameRate?.Invoke(defaultFrameRate);
                
                Logger.Log("[TimedFrameRateBooster] Boost duration ended, reverted frame rate.");

                _boostCoroutine = null;
        }
        
        private bool IsBoosting => _boostCoroutine != null;

        public void Cancel()
        {
            if (!IsBoosting) return;
            
            _monoBehaviour.StopCoroutine(_boostCoroutine);
            
            _boostCoroutine = null;
            
            Logger.Log("[TimedFrameRateBooster] Boost cancelled.");
        }
    }
}
