using System;
using UnityEngine;


namespace CodeBase.Patterns.CirclePattern
{
    [CreateAssetMenu(menuName = "Shmup Patterns/Circle Pattern")]
    public class CirclePatternSo : BasePatternSo
    {
        
        [SerializeField] private float phase;
        [SerializeField] private float rotationSpeed;
        
        [SerializeField] private Vector2[] precomputedOffsetArray;
        
        [SerializeField] private bool precomputePatterns;
        

        public Vector2[] PrecomputedOffsetArray
        {
            get => precomputedOffsetArray;
            set => precomputedOffsetArray = value;
        }

        private CirclePatternAlgorithm _circlePatternAlgorithm;
        
        private void OnEnable()
        {
            _circlePatternAlgorithm = new CirclePatternAlgorithm
            {
                Count = projectileCount,
                Radius = radius,
                Phase = phase
            };
            
            PrecomputeCirclePattern();
        }

        private void OnValidate()
        {
            _circlePatternAlgorithm.Count = projectileCount;
            _circlePatternAlgorithm.Radius = radius;
            _circlePatternAlgorithm.Phase = phase;
            
            PrecomputeCirclePattern();
        }
        
        private void PrecomputeCirclePattern()
        {
            if (!precomputePatterns) return;
            
            precomputedOffsetArray = new Vector2[projectileCount];

            for (int x = 0; x < projectileCount; x++)
                precomputedOffsetArray[x] = _circlePatternAlgorithm.CalculateArc(x, projectileCount, 90f, 45f);
            // precomputedOffsetArray[x] = _circlePatternAlgorithm.CalculateCircle(x);
        }

    
        public override void Execute(int index, ref PatternSample patternSample, Action<PatternSample> callBack,float deltaTime = 0)
        {
            Vector2 offset = precomputePatterns ? PrecomputedOffsetArray[index] : _circlePatternAlgorithm.CalculateArc(index, projectileCount, 180f, 45f, patternSample.RotationPhase);//_circlePatternAlgorithm.CalculateCircle(index, patternSample.RotationPhase);
            
            Vector3 spawnPosition = patternSample.Origin + new Vector3(offset.x, offset.y, 0);
                        
            var direction = offset.normalized;
            
            patternSample.SpawnPosition = spawnPosition;
            patternSample.Direction = direction;
            patternSample.Radius = radius;
            patternSample.MovementSpeed = speed;
            
            AddModifiers(ref patternSample);
            
            //set bullet facing direction
            if (applyRotation)
                patternSample.Rotation = CalculateProjectileRotation(direction);
            
            callBack?.Invoke(patternSample);
        }
        
      
        private Quaternion CalculateProjectileRotation(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        
            return Quaternion.Euler(0f, 0f, angle);
        }
    }
    
    
    

    public struct PatternSample
    {
        public float RotationPhase;
        public float ColourPhase;
        public float OscillationPhase;
        public float WavePhase;
        public Color ProjectileColor;
        public Vector3 Origin;
        public Vector3 SpawnPosition;
        public Vector2 Direction;
        public Quaternion Rotation;
        public float RotationMultiplier;
        public float MovementSpeed;
        public float Radius;
    }
}
