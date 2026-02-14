using UnityEngine;

namespace CodeBase.Modifiers
{
    
    /// <summary>
    /// Stateless modifier that applies an oscillating position offset to a projectile.
    /// Phase is stored per projectile in PatternSample.
    /// </summary>
    public readonly struct PositionOscillationModifier 
    {
        private readonly float _amplitude;     // Max offset in world units
        private readonly float _frequency;     // Cycles per second
        private readonly Vector2 _direction;   // Direction of oscillation (perpendicular to forward)
        
        public PositionOscillationModifier(float amplitude, float frequency, Vector2 direction)
        {
            _amplitude = amplitude;
            _frequency = frequency;
            _direction = direction.normalized;
        }
        
        /// <summary>
        /// Advances the phase and returns the position offset.
        /// </summary>
        /// <param name="phase">Projectile's per-modifier phase</param>
        /// <param name="deltaTime">Time since last frame</param>
        public Vector2 Evaluate(ref float phase, float deltaTime)
        {
            // Advance phase
            phase += _frequency * deltaTime;
            phase %= 1f;
            if (phase < 0f) phase += 1f;

            // Calculate oscillation along the given direction
            return _direction * (Mathf.Sin(phase * Mathf.PI * 2f) * _amplitude);
        }
    }
}
