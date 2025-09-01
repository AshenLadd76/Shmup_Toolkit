using UnityEngine;

namespace ToolBox.Performance.Fps
{
    public class FpsCalculator
    {
        private readonly float _smoothingFactor;
        private float _smoothedFps;
        private bool _initialized;

        public float SmoothedFps => _smoothedFps;

        public FpsCalculator(float smoothingFactor = 0.1f)
        {
            _smoothingFactor = smoothingFactor;
        }

        public void Update(float deltaTime)
        {
            float currentFps = 1f / deltaTime;

            if (!_initialized)
            {
                _smoothedFps = currentFps;
                _initialized = true;
            }
            else
            {
                _smoothedFps = Mathf.Lerp(_smoothedFps, currentFps, _smoothingFactor);
            }
        }
    }
}