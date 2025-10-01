using CodeBase.Collision_Handling;
using Sirenix.OdinInspector;
using ToolBox.Extensions;
using ToolBox.Utils.Pooling;
using UnityEngine;
using UnityEngine.Pool;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Projectile
{
    public class Projectile : MonoBehaviour, IProjectile, IPoolable<Projectile>, ICollisionObject
    {
        private Transform _transform;
        
        private Collider2D _collider;
        
        [ShowInInspector] private bool _isReleased;
        
        private ObjectPool<Projectile> _pool;
        
        private SpriteRenderer _spriteRenderer;
        
        private ActiveProjectileManager _activeProjectileManager;
        
        private Vector3 _cachedPosition;
        
        private Vector3 _velocity;
        
        public float Speed { get; set; }

        [ShowInInspector] public float LifeSpan { get; set; }
        
        [ShowInInspector] public bool IsActive { get; set; }
        
        [ShowInInspector] public Vector3 Velocity { get; set; }
        
        public float Radius { get; set; }
        
        public Vector3 LastPosition { get; set; }
        
        public Vector2Int LastCellPosition { get; set; }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _transform = transform;

            Size = new Vector2(Radius, Radius);
            RadiusX = .2f;
            RadiusY = .2f;
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
        
        public void SetParentPool(ObjectPool<Projectile> pool)
        {
            if (pool == null)
            {
                Logger.LogError( $"Required pool is null or empty " );
                return;
            }

            _pool = pool;
           
            _isReleased = false;
        }

        public void Release()
        {
            if (!IsActive) return;
            
            if (_isReleased)
            {
                Logger.LogError($"Attempted double release on {name}");
                return;
            }
            
            if (_pool == null)
            {
                Logger.LogError($"Missing pool reference on {name}");
                return;
            }
            
            _isReleased = true;
            
            _pool.Release(this);
        }

        public void OnGetFromPool() => SetActiveState(true);
        
        public void OnReturnedToPool() => SetActiveState( false );
       
        private void SetActiveState(bool active)
        {
            _isReleased = !active;
            
            if (_collider && _collider.enabled != active)
                _collider.enabled = active;
            
            if (_spriteRenderer && _spriteRenderer.enabled != active)
                _spriteRenderer.enabled = active;

            IsActive = active;
            
            //LastCellPosition = new Vector2Int(int.MinValue, int.MinValue);
        }
        
        
        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
            _cachedPosition = position;
            Position = position;
        }
        
        public Vector3 GetPosition() => _cachedPosition;
        
        public void SetDirection(Vector3 direction) => Velocity = direction * Speed;
        
        public void SetRotation(Quaternion rotation) => _transform.rotation = rotation;
        
        public void SetColour(Color color) => _spriteRenderer.color = color;

        public Transform GetTransform() => _transform;


        private int _animationIndex = 0;
        [SerializeField] private Sprite[] animationSprites;
        public void Animate()
        {
            if (animationSprites.IsNullOrEmpty()) return;
            
            _spriteRenderer.sprite = animationSprites[_animationIndex];
           
            _animationIndex = (_animationIndex + 1) % animationSprites.Length;
        }

        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public void OnCollision()
        {
            
        }
    }
}
