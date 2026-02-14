using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers.Algorithms
{
    /// <summary>
    /// Cycles a projectile color over time between two or more colors.
    /// Very lightweight for fast updates in shmup scenarios.
    /// </summary>
    
    public struct ColourCycle 
    {
        private readonly Color[] _colors;
        private readonly float _cycleSpeed;
        
        public ColourCycle(Color[] colors, float cycleSpeed)
        {
            if (colors == null || colors.Length < 2)
                throw new System.ArgumentException("At least two colors are required");

            _colors = colors;
            _cycleSpeed = cycleSpeed;
        }
        
        /// <summary>
        /// Updates phase and sets an interpolated color.
        /// Phase is expected in [0,1) and will loop.
        /// </summary>
        public void  GetColor(ref PatternSample patternSample, float deltaTime)
        {
            var phase = patternSample.ColourPhase;
            
            // Advance phase
            phase += _cycleSpeed * deltaTime;
            phase %= 1f;
            if (phase < 0f) phase += 1f;

            // Interpolate between nearest two colors
            int segmentCount = _colors.Length - 1;
            float scaledPhase = phase * segmentCount;
            int index = Mathf.FloorToInt(scaledPhase);
            int nextIndex = (index + 1) % _colors.Length;
            float t = scaledPhase - index;

            patternSample.ColourPhase = phase;
            patternSample.ProjectileColor =  Color.Lerp(_colors[index], _colors[nextIndex], t);
        }
    }
}
