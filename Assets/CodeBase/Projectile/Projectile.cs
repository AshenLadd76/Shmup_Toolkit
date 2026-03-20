using System;
using CodeBase.Collision_Handling;
using ToolBox.Extensions;
using ToolBox.Utils.Pooling;
using UnityEngine;
using UnityEngine.Pool;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Projectile
{
    [Serializable]
    public class NeoProjectile : IProjectile, IPoolable<NeoProjectile>, ICollisionObject
    {
        private Sprite[] _animationSprites;
        
        private int _animationIndex;
        
        private ObjectPool<NeoProjectile> _pool; 
              
        private ActiveProjectileManager _activeProjectileManager;
        
        
        private readonly Transform _transform;

        public Transform Transform => _transform;

        private readonly SpriteRenderer _spriteRenderer;

        private float _radiusX = 0.2f;
        private float _radiusY = 0.2f;
        
        private Vector3 _cachedPosition;
        
        private Vector3 _velocity;

        private bool _isReleased;
        
        public float Speed { get; set; }
        
        public float LifeSpan { get; set; }
        
        public bool IsActive { get; set; }
        
        public Vector3 Velocity { get; set; }
        
        public float Radius { get; set; }
        
        public Vector3 LastPosition { get; set; }
        
        public Vector2Int LastCellPosition { get; set; }
        
        
        public NeoProjectile( Transform projectileTransform, Sprite[] animationSprites)
        {
            _transform = projectileTransform ?? throw new ArgumentNullException(nameof(projectileTransform));

            _spriteRenderer = _transform.GetComponent<SpriteRenderer>() ?? throw new MissingComponentException($"NeoProjectile requires a SpriteRenderer on '{_transform.name}'.");
            
            _animationSprites = !animationSprites.IsNullOrEmpty() ? animationSprites : Array.Empty<Sprite>();
        }
        
        public void SetActiveProjectileManager(ActiveProjectileManager activeProjectileManager)
        {
            if (!activeProjectileManager)
            {
                Logger.LogError("Active Projectile Manager is null!");
                return;
            }

            activeProjectileManager.AddActiveProjectile(this);
        }
        
        public void SetParentPool(ObjectPool<NeoProjectile> pool)
        {
            if (pool == null)
            {
                Logger.LogError( $"Required pool is null or empty " );
                return;
            }

            _pool = pool;
            
        }
        
        public void Release()
        {
            if (!IsActive) return;
            
            if (_isReleased)
            {
                Logger.LogError($"Attempted double release");
                return;
            }
            
            if (_pool == null)
            {
                Logger.LogError($"Missing pool reference");
                return;
            }
            
            _isReleased = true;
            
            _pool.Release(this);
        }
        
        private void SetActiveState(bool active)
        {
            _isReleased = !active;

            if (_spriteRenderer && _spriteRenderer.enabled != active)
                _spriteRenderer.enabled = active;

            IsActive = active;
        }
        
        public void OnGetFromPool() => SetActiveState(true);
        
        public void OnReturnedToPool() => SetActiveState( false );
        
        public Vector3 GetPosition() => _cachedPosition;
        
        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
            _cachedPosition = position;
            Position = position;
        }
        
        public void SetDirection(Vector3 direction) => Velocity = direction * Speed;
        
        public void SetRotation(Quaternion rotation) => _transform.rotation = rotation;
        
        public void SetColour(Color color) => _spriteRenderer.color = color;
        
        
        public void Animate()
        {
            if (_animationSprites.IsNullOrEmpty()) return;
            
            //_spriteRenderer.sprite = _animationSprites[_animationIndex];
           
            //_animationIndex = (_animationIndex + 1) % _animationSprites.Length;
        }
        
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public void OnCollision()
        {
            
        }

        public void Damage(float damage)
        {
           
        }
    }
}
