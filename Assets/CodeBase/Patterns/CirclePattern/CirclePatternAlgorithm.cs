using UnityEngine;

namespace CodeBase.Patterns.CirclePattern
{
    public  struct CirclePatternAlgorithm
    {
        public int Count;
        public float Radius;
        public float Phase;
        
        private const float TAU = 2.0f * Mathf.PI;

        public Vector2 Calculate(int index, float runtimePhase = 0f)
        {
            float angle = Phase + index * ( TAU / Count ) + runtimePhase ;
            
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) ) * Radius;
        }
    }
}
