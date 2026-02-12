using UnityEngine;

namespace CodeBase.Patterns.Phase
{
    // Pure, reusable phase struct
    public readonly struct ConstantRotationPhase
    {
        private readonly float _rotationSpeed;  // radians per second
       
     
        // Initialize with optional start phase
        public ConstantRotationPhase(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }

        // Call every frame to get the current phase
        public void GetPhase(ref float phase, float deltaTime)
        {
            phase += _rotationSpeed * deltaTime;
            // Optional: wrap around 0 -> 2π to keep value manageable
            if (phase > Mathf.PI * 2f)
                phase -= Mathf.PI * 2f;
            
        }
    }
}