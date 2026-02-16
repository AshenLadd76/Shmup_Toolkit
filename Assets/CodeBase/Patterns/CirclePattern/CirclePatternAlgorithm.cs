using UnityEngine;

namespace CodeBase.Patterns.CirclePattern
{
    public  struct CirclePatternAlgorithm
    {
        public int Count;
        public float Radius;
        public float Phase;
        
        private const float TAU = 2.0f * Mathf.PI;

        public Vector2 CalculateCircle(int index, float runtimePhase = 0f)
        {
            float angle = Phase + index * ( TAU / Count ) + runtimePhase ;
            
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) ) * Radius;
        }

        public Vector2 CalculateArc(int index, int stepCount, float arcAngleDeg, float arcRotationDeg, float runtimePhase = 0f )
        {
            // Divide the arc evenly between bullets
            float angleStep = arcAngleDeg / Mathf.Max(stepCount, 1); 

            // Start the arc at arcRotationDeg (first bullet points exactly at rotation)
            float startAngle = arcRotationDeg + angleStep * 0.5f;

            // Compute final angle in radians
            float angleRad = (startAngle + index * angleStep) * Mathf.Deg2Rad + runtimePhase;

            // Return the vector
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * Radius;
        }
    }
}
