using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/RotationModifier")]
    public class RotationModifierSo : BaseModifierSo
    {
        [SerializeField] private float rotationSpeed = 3f; // radians per second
         // initial offset
        
       public override void Apply(ref PatternSample sample, float deltaTime = 0f)
       {
           if (!isEnabled) return;
           
           PhaseAccumulator phaseAccumulator = new PhaseAccumulator(rotationSpeed * sample.RotationMultiplier);
           
           //use the struct in a state less fashion here; 
           phaseAccumulator.Accumulate(ref sample.RotationPhase, deltaTime);
       }
    }
}


