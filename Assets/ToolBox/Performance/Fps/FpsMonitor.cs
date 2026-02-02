using TMPro;
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
        
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        private FpsCalculator _fpsCalculator;
        private float _timer = 0f;
        
        private float _lastFps = -1f;
        
        private void Awake()
        {
            _fpsCalculator = new FpsCalculator( .1f);
        }

        private void Update()
        {
            _fpsCalculator.Update(Time.unscaledDeltaTime);
            _timer += Time.unscaledDeltaTime;
            
            if (!(_timer >= broadcastInterval)) return;
            
          //  onFpsUpdated?.Invoke(_fpsCalculator.SmoothedFps);
          //  onFpsStringUpdated?.Invoke($"{_fpsCalculator.SmoothedFps:F1} FPS");
          
          float fps = _fpsCalculator.SmoothedFps;
          if (Mathf.Abs(fps - _lastFps) > 0.01f)  // only update if changed
          {
              textMeshProUGUI.text = fps.ToString("0.00");
              _lastFps = fps;
          }
            
            _timer = 0;
        }
    }
}
