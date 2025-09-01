using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Patterns
{
    [CreateAssetMenu(fileName = "NewPatternSO", menuName = "Shmup/Pattern")]
    public class PatternSo : ScriptableObject
    {
        [SerializeField] private PatternConfig patternConfig;
        public PatternConfig PatternConfig => patternConfig;
        
        [SerializeField] private AlgorithmBaseSo algorithmBaseSo;
        
        [SerializeField] private ProjectileInfo[] precomputedPatternInfo;
        
        private List<ProjectileInfo[]> _waves;

        [SerializeField] private int currentWaveCount;

        private float _spiralOffsetPerWave;

        private void OnEnable() => Init();


        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            Init();
        }


        private void Init()
        {
            _waveIndex = 0;
            
            CalculateSpiralOffsetPerWave();
            
            PrecomputePattern();
            
            MakeWaves();
        }
        
        
        public ProjectileInfo[] PrecomputedPatternPositions
        {
            get
            {
                if (precomputedPatternInfo == null || precomputedPatternInfo.Length != patternConfig.ProjectileCount)
                    PrecomputePattern();
                
                return precomputedPatternInfo;
            }
        }
        
        private void PrecomputePattern( )
        {
            algorithmBaseSo.PrepareAlgorithm( patternConfig );
            
            var loopCount = patternConfig.ProjectileCount;
            
            precomputedPatternInfo = new ProjectileInfo[loopCount];
           

            for (int x = 0; x < loopCount; x++)
            {
                precomputedPatternInfo[x] = algorithmBaseSo.CalculatePatternStep(patternConfig, x);
            }
            
        }

        private void MakeWaves()
        {
            var waveCount = patternConfig.WaveCount;
            
            currentWaveCount = waveCount;
            
            _waves = new List<ProjectileInfo[]>(waveCount);
            
            for (int i = 0; i < waveCount; i++)
            {
                var wave = new ProjectileInfo[precomputedPatternInfo.Length];
                
                for (int x = 0; x < precomputedPatternInfo.Length; x++)
                {
                    var precomputedPosition = precomputedPatternInfo[x];
                    
                    //convert the current direction to an angle
                    float angle = PolarCoordinateConversionFormula(precomputedPosition.Direction);
                    
                    //Add spiral offset
                    float rotatedAngle = angle + (i * _spiralOffsetPerWave);
                    
                    // Convert back to direction vector
                    Vector3 dir = ConvertAngleToDirection(rotatedAngle);
                    
                    // Only update the direction; don't touch rotation
                    precomputedPosition.Direction = dir;
                    
                    precomputedPosition.Rotation = Quaternion.Euler( 0, 0, rotatedAngle );
                    
                    //Add to current wave
                    wave[x] = precomputedPosition;
                }
                
                _waves.Add(wave);
            }
        }

        private float PolarCoordinateConversionFormula(Vector3 p) => Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;
        
        private Vector3 ConvertAngleToDirection(float angle) => new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        
        private int _waveIndex = 0;

        public ProjectileInfo[] GetWave()
        {
            // cycle to next wave
            
            if(_waves == null) return null;
            
           // if(_waveIndex >= _waves.Count) _waveIndex = 0;
            
           var wave = _waves[_waveIndex];
           
            // advance index immediately, wrap around
            _waveIndex = (_waveIndex + 1) % _waves.Count;
            
            return wave;
        }

        private void CalculateSpiralOffsetPerWave()
        {
            var numStepsPerWave = patternConfig.NumberStepsPerWave;
            
            if (numStepsPerWave <= 0)
            {
                Debug.LogWarning("NumberStepsPerWave not set! Defaulting to 1.");
                numStepsPerWave = 1;
            }

            _spiralOffsetPerWave = patternConfig.SpreadAngle / numStepsPerWave;
        }
    }
}