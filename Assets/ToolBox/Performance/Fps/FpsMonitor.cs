using UnityEngine;
using UnityEngine.Events;

namespace ToolBox.Performance.Fps
{
    public class FpsMonitor: MonoBehaviour
    {
        [Header("Broadcast Settings")]
        [SerializeField, Tooltip("Interval in seconds between FPS broadcasts.")]
         private float broadcastInterval = 1f;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<float> onFpsUpdated;
        [SerializeField] private UnityEvent<string> onFpsStringUpdated;

        private FpsCalculator _fpsCalculator;
        private float _timer = 0f;
        
        private void Awake()
        {
            _fpsCalculator = new FpsCalculator( .1f);
        }

        private void Update()
        {
            _fpsCalculator.Update(Time.unscaledDeltaTime);
            _timer += Time.unscaledDeltaTime;
            
            if (!(_timer >= broadcastInterval)) return;
            
            onFpsUpdated?.Invoke(_fpsCalculator.SmoothedFps);
            onFpsStringUpdated?.Invoke($"{_fpsCalculator.SmoothedFps:F1} FPS");
            
            _timer = 0;
        }
    }
}
