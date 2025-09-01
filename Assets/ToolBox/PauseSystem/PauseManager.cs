using ToolBox.Messenger;
using ToolBox.PauseSystem;
using ToolBox.Utils.Tween;
using UnityEngine;
using UnityEngine.Events;

namespace Toolbox.PauseSystem
{
    public class PauseManager : MonoBehaviour, IPausable
    {
        private bool _isPaused;

        public bool IsPaused => _isPaused;
        
        private float _baseTimeScale = 1f;
        private float _slowMotionScale = 1f;
        
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 1f;

        private Coroutine _timeScaleRoutine;
        
        [SerializeField] private UnityEvent onPauseEvent;
        [SerializeField] private UnityEvent onResumeEvent;

        private void OnEnable()
        {
            MessageBus.Instance.AddListener(  PauseSystemMessages.OnPause, OnPause );
            MessageBus.Instance.AddListener(  PauseSystemMessages.OnResume, OnResume );
            MessageBus.Instance.AddListener<float>(  PauseSystemMessages.OnSlow, SetSlowMotion );
        }

        private void OnDisable()
        {
            MessageBus.Instance.RemoveListener(  PauseSystemMessages.OnPause, OnPause );
            MessageBus.Instance.RemoveListener(  PauseSystemMessages.OnResume, OnResume );
            MessageBus.Instance.RemoveListener<float>(  PauseSystemMessages.OnSlow, SetSlowMotion );
        }

        private void Awake()
        {
            _baseTimeScale = Time.timeScale;
        }
        
        public void TogglePause()
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
        
        private void Pause()
        {
            if (_isPaused) return;  // Use _isPaused flag for clarity

            _isPaused = true;
            ApplyTimeScale(MinTimeScale);  // Set timeScale to 0
            onPauseEvent?.Invoke();
        }

        private void Resume()
        {
            if (!_isPaused) return;

            _isPaused = false;
            ApplyTimeScale(_baseTimeScale * _slowMotionScale); // Restore normal or slow-mo speed
            onResumeEvent?.Invoke();
        }


        public void OnPause() => Pause();
        
        public void OnResume() => Resume();

        public void OnSmoothPause(float duration = 0.5f)
        {
            if (_isPaused) return;

            _isPaused = true;

            if (_timeScaleRoutine != null)
                StopCoroutine(_timeScaleRoutine);

            _timeScaleRoutine = StartCoroutine( TweenUtils.LerpFloat(Time.timeScale, MinTimeScale, duration, val => Time.timeScale = val));
            
            onPauseEvent?.Invoke();
        }

        public void OnSmoothResume(float duration = 0.5f)
        {
            if (!_isPaused) return;

            _isPaused = false;

            if (_timeScaleRoutine != null)
                StopCoroutine(_timeScaleRoutine);

            _timeScaleRoutine = StartCoroutine(TweenUtils.LerpFloat(Time.timeScale, _baseTimeScale * _slowMotionScale, duration, val => Time.timeScale = val));
            onResumeEvent?.Invoke();
        }
        
        
        
        private void ApplyTimeScale(float t) => Time.timeScale = t;
        
        public void SetBaseTimeScale(float scale)
        {
            _baseTimeScale = Mathf.Max(MinTimeScale, scale);
            ApplyTimeScale(_baseTimeScale);
        }
        
        private void SetSlowMotion(float scale)
        {
            _slowMotionScale = Mathf.Clamp(scale, MinTimeScale, MaxTimeScale);

            // Only apply if NOT paused; timeScale should remain 0 if paused
            if (!_isPaused)
            {
                ApplyTimeScale(_baseTimeScale * _slowMotionScale);
            }
        }
    }
}

