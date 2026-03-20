using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/Oscillating Rotation Modifier")]
    public class OscillationRotationModifierSo : BaseModifierSo
    {
        [SerializeField] private float amplitude = 0.1f;
        [SerializeField] private float frequency = 0.5f;
        [SerializeField] private float baseRotationDegrees = 0f;
        
        private OscillatingRotationModifier _oscillatingRotationModifier;

        private void OnEnable() => InitAlgorithm();
        
        private void OnValidate() => InitAlgorithm();
        
        private void InitAlgorithm() =>_oscillatingRotationModifier = new OscillatingRotationModifier(amplitude, frequency);
        

        public override void Apply(ref PatternSample patternSample, ref float speed, float deltaTime = 0)
        {
            var oscillationOffset = _oscillatingRotationModifier.Evaluate(ref patternSample.OscillationPhase, deltaTime);

            // Apply to the projectile's rotation quaternion
            patternSample.Rotation = Quaternion.Euler(0f, 0f, baseRotationDegrees + oscillationOffset * Mathf.Rad2Deg);
        }
    }
}
