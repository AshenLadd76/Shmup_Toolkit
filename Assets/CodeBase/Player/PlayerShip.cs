using CodeBase.Weapons;
using ToolBox.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace CodeBase.Player
{
    [RequireComponent(typeof(IWeapon))]
    public class PlayerShip : MonoBehaviour
    {
        private int _currentAnimationHash;
        
        private Transform _transform;

        private Animator _animator;
        
        [SerializeField] private PlayerMovementData playerMovementData;

        [SerializeField] private Vector3 direction;
        
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;

        [SerializeField] private UnityEvent<float> onSpecialAttack;
        
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
            
            _camera = Camera.main;

            _bounds = _camera.GetBounds(0.2f);
            
            _animator = GetComponent<Animator>();

            _shmupAnimator = new ShmupShipAnimator(_animator, playerMovementData);

            _weaponSystem = GetComponent<IWeapon>();
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
            _shmupAnimator.UpdateAnimator(direction);
        }


        private void Move()
        {
            var delta = Vector3.ClampMagnitude(direction, 1) * (moveSpeed * Time.deltaTime);
            
            var positionToClamp = _transform.position + delta;
            
            _transform.position = ClampPositionToScreenBounds(positionToClamp);
        }

        
        private Vector3 ClampPositionToScreenBounds(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _bounds.min.x, _bounds.max.x);
            position.y = Mathf.Clamp(position.y, _bounds.min.y, _bounds.max.y);
            
            return position; 
        }
    }
}
