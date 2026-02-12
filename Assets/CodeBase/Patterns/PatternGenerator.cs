using System.Collections;
using System.Collections.Generic;
using CodeBase.Patterns.CirclePattern;
using CodeBase.Patterns.Phase;
using CodeBase.Projectile;
using ToolBox.Utils.Validation;
using UnityEngine;


namespace CodeBase.Patterns
{
    public class PatternGenerator : MonoBehaviour
    {
        [Validate, SerializeField, Tooltip("Manager responsible for pooling and retrieving bullets.")]
        private ProjectilePoolManager poolManager;

        [Validate, SerializeField] private BaseModifierSo rotationModifierSo;
        [Validate, SerializeField] private BasePatternSo patternSo;
        [Validate] private IReadOnlyList<ProjectileInfo> _precomputedPatternList;
  
        
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
            
            PatternSample patternSample = new PatternSample
            {
                RuntimePhase = 0f,
                SpawnPosition = _transform.position,
                Direction = Vector3.zero,
                Rotation = Quaternion.identity
            };

            while (true)
            {
                    for (int x = 0; x < patternSo.Count; x++)
                    {
                        rotationModifierSo?.Apply(ref patternSample, Time.deltaTime);
                       
                        patternSo.Execute(x, ref patternSample, Time.deltaTime);
                        
                        InitializeProjectile(patternSample.Direction, patternSample.Rotation, patternSample.SpawnPosition, patternSo.Speed, patternSo.LifeSpan);
                    }
                    
                    yield return _fireDelay;
            }
        }

       
        private NeoProjectile GetProjectileFromPool() => poolManager.Get(ShmupStrings.RegularProjectile);
        
        
        private void InitializeProjectile(Vector3 direction, Quaternion rotation,  Vector3 position, float speed, float lifeSpan)
        {
            IProjectile projectile = GetProjectileFromPool();
            
            projectile.LifeSpan = lifeSpan;
            projectile.Radius = 0.1f;
            projectile.Speed = speed;
            projectile.SetDirection(direction.normalized);
            projectile.SetPosition(position);
            projectile.SetRotation(rotation);
            projectile.IsActive = true;
        }
    }
}


