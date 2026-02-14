using UnityEngine;

namespace CodeBase.Modifiers
{
    /// <summary>
    /// A reusable phase accumulator that increments a phase value over time.
    /// Can be used for rotations, oscillations, or any cyclic value.
    /// </summary>
    
    
    // Pure, reusable phase struct
    public readonly struct PhaseAccumulator
    {
        private readonly float _accumulationSpeed;  // radians per second
     
   
        public  PhaseAccumulator(float accumulationSpeed)
        {
            _accumulationSpeed = accumulationSpeed;
        }

      
        public void Accumulate(ref float phase, float deltaTime)
        {
            phase += _accumulationSpeed * deltaTime;
           
            phase %= Mathf.PI * 2f;
            
            if (phase < 0f) phase += Mathf.PI * 2f;
        }
    }
}