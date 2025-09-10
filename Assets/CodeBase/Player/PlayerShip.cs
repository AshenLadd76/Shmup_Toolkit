using ToolBox.Extensions;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Player
{
    public class PlayerShip : MonoBehaviour
    {


        private int _currentAnimationHash;
        
        private  readonly int _directionX = Animator.StringToHash("DirectionX");
        private readonly int _tiltTimeField = Animator.StringToHash("TiltTime");
        private readonly int _directionY = Animator.StringToHash("DirectionY");
        private readonly int _superBombTrigger = Animator.StringToHash("SuperBomb");
        
        private Transform _transform;

        private Animator _animator;
        
        [SerializeField] private PlayerMovementData playerMovementData;

        [SerializeField] private Vector3 direction;
        
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;
 
        private int _currentDirectionX;
        private int _currentDirectionY;
        private int _lastDirectionX;

        private Vector2Int _lastDirection;
        private Vector2Int _newDirection;
        
        private float _tiltTime;
        
        private (Vector3 min, Vector3 max) _bounds;
        
        private Camera _camera;
        
        private void Awake()
        {
            _transform = transform;
            
            _camera = Camera.main;

            _bounds = _camera.GetBounds(0.2f);
            
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if( Input.GetKeyDown( KeyCode.Space) ) _animator.SetTrigger( _superBombTrigger ); 
            
           vertical = Input.GetAxisRaw("Vertical");
           horizontal = Input.GetAxisRaw("Horizontal");
           
           direction = new Vector2(horizontal, vertical);
           
           _currentDirectionX = Mathf.RoundToInt( direction.x );
           _currentDirectionY = Mathf.RoundToInt( direction.y );
           
           TiltCheck();
           
           Move();
           
        }

        private void LateUpdate()
        {
            _animator.SetFloat(_tiltTimeField, _tiltTime);
            
            HandleAnimation();
        }


        private void Move()
        {
            var delta = Vector3.ClampMagnitude(direction, 1) * (moveSpeed * Time.deltaTime);
            
            var positionToClamp = _transform.position + delta;
            
            _transform.position = ClampPositionToScreenBounds(positionToClamp);
        }

        private void HandleAnimation()
        {
            _newDirection = Vector2Int.RoundToInt( direction );

            if (_lastDirection == _newDirection) return;
            
            _lastDirection = _newDirection;
                
            _currentAnimationHash = playerMovementData.GetAnimationHash( _newDirection );
            
            _animator.CrossFade( _currentAnimationHash, 0.1f);
        }

        private Vector3 ClampPositionToScreenBounds(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _bounds.min.x, _bounds.max.x);
            position.y = Mathf.Clamp(position.y, _bounds.min.y, _bounds.max.y);
            
            return position; 
        }

        private void TiltCheck()
        {
            _tiltTime = (_currentDirectionX == 0 || _currentDirectionX != _lastDirectionX) ? 0f : _tiltTime + Time.deltaTime;

            _lastDirectionX = _currentDirectionX;
        }
    }
}
