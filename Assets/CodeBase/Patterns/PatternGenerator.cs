using System.Collections;
using System.Collections.Generic;
using CodeBase.Modifiers;
using CodeBase.Patterns.CirclePattern;
using CodeBase.Patterns.CompositePatterns;
using CodeBase.Patterns.CompositePatterns.Flower;
using CodeBase.Projectile;

using ToolBox.Utils.Validation;
using UnityEngine;


namespace CodeBase.Patterns
{
    public class PatternGenerator : MonoBehaviour
    {
        [Validate, SerializeField, Tooltip("Manager responsible for pooling and retrieving bullets.")]
        private ProjectilePoolManager poolManager;

        //[Validate, SerializeField] private BaseModifierSo rotationModifierSo;
        [Validate, SerializeField] private BasePatternSo patternSo;
        [Validate] private IReadOnlyList<ProjectileInfo> _precomputedPatternList;
  
        [SerializeField] private float rotationMultiplier = 1f;
        
        [SerializeField] private  FlowerPatternSo flowerPatternSo;
        
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
        
        
        
        private void Awake()
        {
            //ObjectValidator.Validate(patternSo);
            
            _transform = transform;

            _fireDelay = new WaitForSeconds(patternSo.FireRate);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartGeneratorCoroutine();
            

            if (Input.GetKeyUp(KeyCode.Space))
                StopGeneratorCoroutine();
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
        
        private IEnumerator GeneratePatternCoroutine()
        {
            PatternSample[] patternSample = new PatternSample[1];
            
            for (int x = 0; x < 1; x++)
                patternSample[x] = CreatePatternSample();
            
            
            while (true)
            {
                for (int x = 0; x < patternSo.ProjectileCount; x++)
                    patternSo.Execute(x, ref patternSample[0], InitializeProjectile, Time.deltaTime);
                
                
                yield return _fireDelay;
            }
        }


        private PatternSample CreatePatternSample()
        {
            return new PatternSample
            {
                RotationPhase = 0f,
                Origin = _transform.position,
                Direction = Vector3.zero,
                Rotation = Quaternion.identity,
                RotationMultiplier = rotationMultiplier,
            };
        }
        
        
        private NeoProjectile GetProjectileFromPool() => poolManager.Get(ShmupStrings.RegularProjectile);
        
        
        private void InitializeProjectile(PatternSample patternSample)
        {
            IProjectile projectile = GetProjectileFromPool();
            
            projectile.LifeSpan = patternSo.LifeSpan;
            projectile.Radius = patternSample.Radius;
            projectile.Speed = patternSample.MovementSpeed;
            projectile.SetDirection(patternSample.Direction.normalized);
            projectile.SetPosition(patternSample.SpawnPosition);
            projectile.SetRotation(patternSample.Rotation);
            projectile.SetColour(patternSample.ProjectileColor);
            projectile.IsActive = true;
        }
        
        private void InitializeProjectile(Vector3 direction, Quaternion rotation,  Vector3 position, float speed, float lifeSpan,Color color)
        {
            IProjectile projectile = GetProjectileFromPool();
            
            projectile.LifeSpan = lifeSpan;
            projectile.Radius = 0.25f;
            projectile.Speed = speed;
            projectile.SetDirection(direction.normalized);
            projectile.SetPosition(position);
            projectile.SetRotation(rotation);
            projectile.SetColour(color);
            projectile.IsActive = true;
        }
    }
}


