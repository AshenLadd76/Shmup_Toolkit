using ToolBox.Interfaces;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Performance.Fps
{
    
    /// <summary>
    /// Manages the application's target frame rate dynamically using an event-driven approach.
    /// Allows adaptive frame rate adjustments to optimize performance and smoothness,
    /// such as boosting the frame rate during animation-heavy operations or UI transitions,
    /// then reverting to a default or low frame rate to conserve resources.
    /// </summary>
    
    public class TargetFrameRateController : MonoBehaviour, IFrameRateController
    {
        [SerializeField] private int defaultTargetFrameRate = 30;
        [SerializeField] private int boostFrameRate = 60;
        [SerializeField] private int lowFpsThreshold = 25;

        private int _currentTargetFrameRate;
        private bool _isBoosted = false;

        private IEventHandler _targetFrameRateEventHandler;
        
        private ITimedFrameRateBooster _timedFrameRateBooster;

        private void OnEnable() => _targetFrameRateEventHandler?.Subscribe();
        
        private void OnDisable() => _targetFrameRateEventHandler?.UnSubscribe();
        

        private void Awake()
        {
            _targetFrameRateEventHandler = new TargetFrameRateControllerEventHandler(this);

            _timedFrameRateBooster = new TimedFrameRateBooster(this, SetFrameRate);
        }
        
        private RefreshRate GetRefreshRate() => Screen.currentResolution.refreshRateRatio;
        
        public void SetMaxFrameRate() => SetFrameRate( boostFrameRate );
        public void SetMidFrameRate() => SetFrameRate( defaultTargetFrameRate );
        public void SetMinFrameRate() => SetFrameRate( lowFpsThreshold );
        public void SetCustomFrameRate(int fps) => SetFrameRate(fps);

        public void TriggerTimedFpsBoost(float duration) =>_timedFrameRateBooster.TempBoostFps(boostFrameRate, defaultTargetFrameRate, duration);
        

        
        
        /// <summary>
        /// Sets the frame rate to the predefined fps value for animation-heavy operations.
        /// </summary>
        private void SetFrameRate(int desiredFrameRate)
        {
            if (_currentTargetFrameRate == desiredFrameRate) return;
            
            desiredFrameRate = Mathf.Min(desiredFrameRate, Mathf.FloorToInt(GetMaxPossibleFrameRate()));
            
            QualitySettings.vSyncCount = 0; // Disable VSync to allow manual FPS control
            Application.targetFrameRate = desiredFrameRate;
            
            _currentTargetFrameRate = desiredFrameRate;

            Logger.Log($"[TargetFrameRateController] Set frame rate to {desiredFrameRate}");
        }
        
        private float GetMaxPossibleFrameRate()
        {
            if (QualitySettings.vSyncCount > 0)
                return (float)GetRefreshRate().value;
            
            if (Application.targetFrameRate > 0)
                return Application.targetFrameRate;

            // -1 means uncapped, so you might want to estimate or return int.MaxValue
            return int.MaxValue;
        }
    }

    public interface IFrameRateController
    {
        void SetCustomFrameRate(int desiredFrameRate);
    }
}
