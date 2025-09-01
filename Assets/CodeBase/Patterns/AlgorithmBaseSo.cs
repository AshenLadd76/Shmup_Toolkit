using UnityEngine;

namespace CodeBase.Patterns
{
    public abstract class AlgorithmBaseSo : ScriptableObject
    {
        /// <summary>
        /// Calculates the direction/position of a single bullet in the pattern.
        /// Must be implemented by each concrete algorithm.
        /// </summary>
        public abstract ProjectileInfo CalculatePatternStep(PatternConfig config, int projectileIndex);

        public abstract void PrepareAlgorithm(PatternConfig config);
    }
}