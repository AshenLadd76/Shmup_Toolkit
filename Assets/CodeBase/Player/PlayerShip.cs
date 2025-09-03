using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Player
{
    public class PlayerShip : MonoBehaviour
    {
        private Transform _transform;

        [SerializeField] private Vector2 direction;
        
        [SerializeField] private float moveSpeed = 20f;
        
        [SerializeField] private float shipBoundsPadding = 0.25f;
        
        private (Vector3 min, Vector3 max) _bounds;
        
        private Camera _camera;
        
        private void Awake()
        {
            _transform = transform;
            
            _camera = Camera.main;

            _bounds = _camera.GetBounds(shipBoundsPadding);
        }

        private void Update()
        {
           GetDirection();
           
           Move();
        }
        
        private void GetDirection()
        {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");
            
            direction = new Vector2(horizontal, vertical);
        }


        private void Move()
        {
            var delta = Vector3.ClampMagnitude(direction, 1f) * (moveSpeed * Time.deltaTime);
            
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
