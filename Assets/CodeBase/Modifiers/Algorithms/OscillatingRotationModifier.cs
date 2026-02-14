using UnityEngine;

namespace CodeBase.Modifiers
{
    /// <summary>
    /// Stateless modifier that applies an oscillating rotation to a circle pattern.
    /// </summary>
    /// 
    public struct OscillatingRotationModifier 
    {
        private readonly float _amplitude; // max angle in radians
        private readonly float _frequency; // oscillations per second

        public OscillatingRotationModifier (float amplitude, float frequency)
        {
            _amplitude = amplitude;
            _frequency = frequency;
        }
        
        /// <summary>
        /// Evaluates the rotation offset at the current phase.
        /// phase ∈ [0,1) and loops continuously.
        /// </summary>
        public float Evaluate(ref float phase, float deltaTime)
        {
            // Advance phase
            phase += _frequency * deltaTime;
            phase %= 1f;
            if (phase < 0f) phase += 1f;

            // Return oscillation offset
            return Mathf.Sin(phase * Mathf.PI * 2f) * _amplitude;
            
        }
    }
}
