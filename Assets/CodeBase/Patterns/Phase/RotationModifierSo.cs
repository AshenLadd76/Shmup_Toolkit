using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Patterns.Phase
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/Rotation Modifier")]
    public class RotationModifierSo : BaseModifierSo
    {
        [SerializeField] private float rotationSpeed = 3f; // radians per second
         // initial offset
        
       public override void Apply(ref PatternSample sample, float deltaTime = 0f)
       { 
           ConstantRotationPhase phaseStruct = new ConstantRotationPhase(rotationSpeed);
           
           //use the struct in a state less fashion here; 
           phaseStruct.GetPhase(ref sample.RuntimePhase, deltaTime);
       }
    }
}


