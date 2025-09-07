using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Player
{
    public class PlayerShip : MonoBehaviour
    {
        private static readonly int DirectionX = Animator.StringToHash("DirectionX");
        private static readonly int TiltTime = Animator.StringToHash("TiltTime");
        private Transform _transform;

        private Animator _animator;

        [SerializeField] private Vector3 direction;
        
        [SerializeField] private float moveSpeed = 20f;
        
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;
 
        private int _currentDirectionX;
        private int _lastDirectionX;
        
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
           vertical = Input.GetAxisRaw("Vertical");
           horizontal = Input.GetAxisRaw("Horizontal");
           
           direction = new Vector2(horizontal, vertical);
           
           _currentDirectionX = Mathf.RoundToInt( direction.x );
           
           TiltCheck();
           
           Move();
           
        }

        private void LateUpdate()
        {
            _animator.SetInteger( DirectionX, _currentDirectionX );
            _animator.SetFloat( TiltTime , _tiltTime );
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

        private void TiltCheck()
        {
            _tiltTime = (_currentDirectionX == 0 || _currentDirectionX != _lastDirectionX) ? 0f : _tiltTime + Time.deltaTime;

            _lastDirectionX = _currentDirectionX;
        }
    }
}
