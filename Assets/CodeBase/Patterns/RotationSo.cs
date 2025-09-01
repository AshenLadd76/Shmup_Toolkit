using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Patterns
{
    [CreateAssetMenu(fileName = "NewRotationSO", menuName = "Shmup/Rotation")]
    public class RotationSo : ScriptableObject
    {
        [Tooltip("Number of steps to precompute.")]
        [SerializeField] private int steps = 12;

        [Tooltip("Degrees to rotate each step.")]
        [SerializeField] private float stepDegrees = 30f;

        [Tooltip("Optional oscillation amplitude in degrees.")]
        [SerializeField] private float oscillationAmplitude = 0f;

        [Tooltip("Optional oscillation frequency.")]
        [SerializeField] private float oscillationFrequency = 1f;

        
        [SerializeField] private Quaternion[] precomputedPatternRotations;

        private void OnEnable() => PrecomputeRotations();

        public Quaternion[] PrecomputedPatternRotations
        {
            get
            {
                if (precomputedPatternRotations == null || precomputedPatternRotations.Length != steps)
                    PrecomputeRotations();
                
                return precomputedPatternRotations;
            }
        }

        
        private void PrecomputeRotations()
        {
            precomputedPatternRotations = new Quaternion[steps];

            for (int i = 0; i < steps; i++)
            {
                float baseRotation = i * stepDegrees;
                float oscillation = Mathf.Sin(i * oscillationFrequency) * oscillationAmplitude;
                precomputedPatternRotations[i] = Quaternion.Euler(0, 0, baseRotation + oscillation);
            }
        }
    }
}