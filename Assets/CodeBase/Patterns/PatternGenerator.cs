using System.Collections;
using System.Collections.Generic;
using CodeBase.Projectile;
using ToolBox.Utils.Validation;
using UnityEngine;

namespace CodeBase.Patterns
{
    public class PatternGenerator : MonoBehaviour
    {
        [Validate, SerializeField, Tooltip("Manager responsible for pooling and retrieving bullets.")]
        private PoolManager poolManager;

        [Validate, SerializeField] private PatternSo patternSo;
        [Validate, SerializeField] private RotationSo rotationSo;

        [Validate] private IReadOnlyList<ProjectileInfo> _precomputedPatternList;
        // [Validate] private IReadOnlyList<Quaternion> _precomputedRotationList;
        
        private WaitForSeconds _fireDelay;
        private Coroutine _generatorCoroutine;

        private Transform _transform;
        private float _speed;
        private float _bulletLifeSpan;
        private float _spawnRadius;
        private Vector3 _originPosition;
        private Color _colour;
        private Quaternion _muzzleRotation;
        
        [Validate, SerializeField] private Transform gunMuzzleTransform;


        private PatternConfig _patternConfig;
        private int _rotationCount;
    
        
        private Color _randomColour;
        

        private bool _isFiring;

        private void OnEnable() => StartGeneratorCoroutine();
        
        private void OnDisable() => StopGeneratorCoroutine();
        
        private void Awake()
        {
            ObjectValidator.Validate(patternSo);
            
            _transform = transform;

            _fireDelay = new WaitForSeconds(patternSo.PatternConfig.FireRate);

            _precomputedPatternList = patternSo.PrecomputedPatternPositions;
            //_precomputedRotationList = rotationSo.PrecomputedPatternRotations;
            
           // _rotationCount = _precomputedRotationList.Count;
            
            _patternConfig = patternSo.PatternConfig;
            
            CacheProperties();
            
            Debug.Log( $"Speed {_speed}" );
        }

        private void Update()
        {
            _isFiring = Input.GetKey(KeyCode.Space);
        }
        
       
        
        private void StartGeneratorCoroutine()
        {
            if (_generatorCoroutine != null) return;

            _generatorCoroutine = StartCoroutine(GeneratePatternCoroutine());
        }

        private void StopGeneratorCoroutine()
        {
            if (_generatorCoroutine == null) return;

            StopCoroutine(_generatorCoroutine);

            _generatorCoroutine = null;
        }
        
        // private IEnumerator GeneratePatternCoroutine()
        // {
        //     float timer = 0f; // counts time between shots
        //     float fireRate = _patternConfig.FireRate; // seconds per wave
        //
        //     while (true)
        //     {
        //         if (_isFiring)
        //         {
        //             timer += Time.deltaTime;
        //
        //             // Fire immediately if timer >= fireRate OR first frame of firing
        //             if (timer >= fireRate || timer == 0f)
        //             {
        //                 var wave = patternSo.GetWave();
        //                 _randomColour = Random.ColorHSV();
        //
        //                 for (int x = 0; x < wave.Length; x++)
        //                 {
        //                     var localDirection = wave[x].Direction;
        //                     var localRotation = wave[x].Rotation;
        //
        //                     InitializeProjectile(
        //                         localDirection,
        //                         localRotation,
        //                         _transform.position,
        //                         _speed,
        //                         _bulletLifeSpan,
        //                         _randomColour
        //                     );
        //                 }
        //
        //                 timer = 0f; // reset timer after firing
        //             }
        //         }
        //         else
        //         {
        //             timer = 0f; // reset timer when not firing
        //         }
        //
        //         yield return null; // wait until next frame
        //     }
        // }


        private IEnumerator GeneratePatternCoroutine()
        {
            int waveCount = _patternConfig.WaveCount;
            int patternCount = _precomputedPatternList.Count;
            
            while (true)
            {
                if (_isFiring)
                {
                    var wave = patternSo.GetWave();
                    
                    _randomColour = Random.ColorHSV();
                    
                    for (int x = 0; x < wave.Length; x++)
                    {
                        var localDirection = wave[x].Direction;
                        var localRotation = wave[x].Rotation;
                        
                        InitializeProjectile(localDirection, localRotation, _transform.position, _speed, _bulletLifeSpan, _randomColour);
                    }
                    
                    yield return _fireDelay;
                }
                else
                {
                    yield return null;
                }
               
            }
        }

        private Projectile.Projectile GetProjectileFromPool() => poolManager.Get(ShmupStrings.RegularProjectile);
        

        private void InitializeProjectile(Vector3 direction, Quaternion rotation,  Vector3 position, float speed, float lifeSpan, Color color)
        {
            IProjectile projectile = GetProjectileFromPool();
            
            projectile.LifeSpan = lifeSpan;
            projectile.Radius = 0.1f;
            projectile.Speed = speed;
            projectile.SetDirection(direction.normalized);
            projectile.SetPosition(position);
            projectile.SetRotation(rotation);
            projectile.SetColour(color);
            projectile.IsActive = true;
        }
        
        private void CacheProperties()
        {
            _originPosition = _transform.position;               // Cache position once
            _spawnRadius = _patternConfig.SpawnRadius;            // Cache spawn radius once
            _muzzleRotation = gunMuzzleTransform.rotation;  // Cache rotation once
            _bulletLifeSpan = _patternConfig.ProjectileLifeSpan;
            _speed = _patternConfig.ProjectileSpeed;
        }
    }
}


