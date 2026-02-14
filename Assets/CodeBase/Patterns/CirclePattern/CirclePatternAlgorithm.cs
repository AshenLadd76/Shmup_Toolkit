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

        public Vector2 CalculateArc(int index, int count, float arcAngleDeg, float arcRotationDeg, float runtimePhase = 0f )
        {
            float angleStep = arcAngleDeg / (count - 1); //Divide arc evenly
            float startAngle = -arcAngleDeg / 2f + arcRotationDeg; //Center arc + rotation;
            
            float angleDeg = startAngle + index * angleStep + runtimePhase * Mathf.Rad2Deg;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * Radius;
        }
    }
}
