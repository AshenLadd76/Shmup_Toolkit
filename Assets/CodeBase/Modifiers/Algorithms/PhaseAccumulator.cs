using UnityEngine;

namespace CodeBase.Modifiers.Algorithms
{
    /// <summary>
    /// A reusable phase accumulator that increments a phase value over time.
    /// Can be used for rotations, oscillations, or any cyclic value.
    /// </summary>
    
    
    // Pure, reusable phase struct
    public readonly struct PhaseAccumulator
    {
        private const float TAU = Mathf.PI * 2f;
        
        public void Accumulate(ref float phase, float rotationSpeed, float rotationMultiplier,  float deltaTime)
        {
            var accumulationSpeed = rotationSpeed * rotationMultiplier;

            phase += accumulationSpeed * deltaTime;
           
            phase %= Mathf.PI * 2f;
            
            if (phase < 0f) phase += TAU;
        }
    }
}