using System;
using UnityEngine;

namespace CodeBase.Patterns
{
    [CreateAssetMenu(fileName =  "CircleAlgorithmSO", menuName = "Shmup/Algorithm")]
    public class CircleAlgorithmSo : AlgorithmBaseSo
    {
        private float _angleStep = 0f;
        private float _halfTotal = 0f;

        private bool _isPrepared;
        
        private float CalculateAngleStep(PatternConfig config)
        {
            var projectileCount = config.ProjectileCount;
            var spreadAngle = config.SpreadAngle;

            float fullCircle = 360f;

            var angleStep = Mathf.Approximately(spreadAngle, fullCircle)
                ? spreadAngle / projectileCount
                : spreadAngle / (projectileCount - 1);

            return angleStep;
        }
        
        public  override ProjectileInfo CalculatePatternStep(PatternConfig pattern, int projectileIndex)
        {
            if(!_isPrepared) PrepareAlgorithm(pattern);
            
            var angleOffset = pattern.AngleOffset;
            
            var angle = angleOffset + (_angleStep * (projectileIndex - _halfTotal));
                
            // Convert angle to radians for Mathf trig functions
            var angleRad = angle * Mathf.Deg2Rad;
            
            var direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);
            var rotation = Quaternion.Euler(0f, 0f, angle);

            return new ProjectileInfo(direction, rotation);

        }

        private float CalculateCentreOfSpread(int projectileCount) => (projectileCount - 1) / 2f; 
        
        public override void PrepareAlgorithm(PatternConfig patternConfig)
        {
            _angleStep = CalculateAngleStep(patternConfig);
            _halfTotal = CalculateCentreOfSpread(patternConfig.ProjectileCount);
            
            _isPrepared = true;
        }
    }
    
    
    [Serializable]
    public struct ProjectileInfo
    {
        public ProjectileInfo(Vector3 direction, Quaternion rotation)
        {
            this.direction = direction;
            this.rotation = rotation;
        }
        
        
        [SerializeField] private Vector3 direction;
        public Vector3 Direction
        {
            get => direction;
            set => direction = value;
        }

        [SerializeField] private Quaternion rotation;
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }
    }
}