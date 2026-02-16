using System;
using CodeBase.Modifiers.Algorithms;
using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/RotationModifier")]
    public class RotationModifierSo : BaseModifierSo
    {
        [SerializeField] private float rotationSpeed = 3f; // radians per second
         // initial offset
         
        

         private PhaseAccumulator _phaseAccumulator;

         private void OnEnable()
         {
            _phaseAccumulator = new PhaseAccumulator();
         }


         public override void Apply(ref PatternSample sample, float deltaTime = 0f)
       {
           if (!isEnabled) return;
           
           //use the struct in a state less fashion here; 
           _phaseAccumulator.Accumulate(ref sample.RotationPhase,  rotationSpeed, sample.RotationMultiplier, deltaTime);
       }
    }
}


