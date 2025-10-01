using CodeBase.Weapons;
using ToolBox.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBase.Player
{
    [RequireComponent(typeof(IWeapon))]
    public class PlayerShip : MonoBehaviour
    {
        private int _currentAnimationHash;
        
        private Transform _transform;
        
        private Transform _parentTransform;

        private Animator _animator;
        
        private IScreenClamper _screenClamper;
        
        [SerializeField] private PlayerMovementData playerMovementData;
        
        [SerializeField] private Vector3 direction;
        
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;

        [SerializeField] private UnityEvent<float> onSpecialAttack;

        private const float CameraBoundsPadding = 0.2f;
        
        private int _lastDirectionX;

        private Vector2Int _lastDirection;
        private Vector2Int _newDirection;
        
        private float _tiltTime;
        
        private (Vector3 min, Vector3 max) _bounds;
        
        private Camera _camera;
        
        private ShmupShipAnimator _shmupAnimator;

        private IWeapon _weaponSystem;
        
        private void Awake()
        {
            _transform = transform;
            
            _parentTransform = transform.parent;
            
            _camera = Camera.main;

            _bounds = _camera.GetBounds(CameraBoundsPadding);
            
            _animator = GetComponent<Animator>();

            _shmupAnimator = new ShmupShipAnimator(_animator, playerMovementData);

            _weaponSystem = GetComponent<IWeapon>();

            _screenClamper = new ScreenClamper(_parentTransform, _transform);
        }

        private void Update()
        {
            if( Input.GetKeyDown( KeyCode.Space) ) onSpecialAttack?.Invoke( _transform.position.x );
            
            if( Input.GetKey( KeyCode.LeftControl ) ) _weaponSystem.Fire();
            
            if( Input.GetKeyUp( KeyCode.LeftControl) ) _weaponSystem.StopFire();
            
           vertical = Input.GetAxisRaw("Vertical");
           horizontal = Input.GetAxisRaw("Horizontal");
           
           direction = new Vector2(horizontal, vertical);
           
           Move();
        }

        private void LateUpdate()
        {
            _bounds = _camera.GetBounds(CameraBoundsPadding);
            
            _shmupAnimator.UpdateAnimator(direction);
        }
        
        private void Move()
        {
            var delta = Vector3.ClampMagnitude(direction, 1) * (moveSpeed * Time.deltaTime);

            var worldPosition = _screenClamper.CalculateWorldPosition(delta);
            
            worldPosition = _screenClamper.ClampPositionToScreenBounds(worldPosition, _bounds);

            _transform.localPosition = _screenClamper.CalculateLocalPosition(worldPosition);
        }
    }
}
