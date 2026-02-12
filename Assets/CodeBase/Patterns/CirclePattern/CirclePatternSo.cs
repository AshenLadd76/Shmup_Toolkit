using CodeBase.Patterns.Phase;
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
                Count = count,
                Radius = radius,
                Phase = phase
            };
            
            PrecomputeCirclePattern();
        }

        private void OnValidate()
        {
            _circlePatternAlgorithm.Count = count;
            _circlePatternAlgorithm.Radius = radius;
            _circlePatternAlgorithm.Phase = phase;
            
            PrecomputeCirclePattern();
        }
        
        private void PrecomputeCirclePattern()
        {
            if (!precomputePatterns) return;
            
            precomputedOffsetArray = new Vector2[count];
            
            for (int x = 0; x < count; x++)
                precomputedOffsetArray[x] = _circlePatternAlgorithm.Calculate(x);
        }

    
        public override void Execute(int index, ref PatternSample patternSample, float deltaTime = 0)
        {
            Vector2 offset = precomputePatterns ? PrecomputedOffsetArray[index] : _circlePatternAlgorithm.Calculate(index, patternSample.RuntimePhase);
            
            Vector3 spawnPosition = patternSample.SpawnPosition + new Vector3(offset.x, offset.y, 0);
                        
            var direction = offset.normalized;
            
            patternSample.SpawnPosition = spawnPosition;
            patternSample.Direction = direction;

            if (applyRotation)
            {
                patternSample.Rotation = CalculateProjectileRotation(direction);
            }
        }
        
        private Quaternion CalculateProjectileRotation(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        
            return Quaternion.Euler(0f, 0f, angle);
        }
    }
    
    
    

    public struct PatternSample
    {
        public float RuntimePhase;
        public Vector3 SpawnPosition;
        public Vector2 Direction;
        public Quaternion Rotation; // optional
    }
    
    public abstract class BasePatternSo : ScriptableObject
    {
        [SerializeField] protected int count = 8;
        [SerializeField] protected float radius = 1f;
        [SerializeField] protected float speed = 1f;
        [SerializeField] protected float lifeSpan = 5f;
        [SerializeField] protected float fireRate = 0.1f;
        [SerializeField] protected bool applyRotation = true;

        public int Count => count;
        public float Radius => radius;
        public float Speed => speed;
        public float LifeSpan => lifeSpan;
        public float FireRate => fireRate;
        public bool ApplyRotation => applyRotation;
        

        // Core execute method — every pattern implements this
        public abstract void Execute(int index, ref PatternSample patternSample, float deltaTime = 0f);
    }
}
