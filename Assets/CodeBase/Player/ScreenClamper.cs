using UnityEngine;

namespace CodeBase.Player
{
    public class ScreenClamper : IScreenClamper
    {
        private Transform _parentTransform;
        private Transform _transform;

        public ScreenClamper(Transform parentTransform, Transform transform)
        {
            _parentTransform = parentTransform;
            _transform = transform;
            
        }
        
        
        public Vector3 ClampPositionToScreenBounds(Vector3 position, (Vector3 min, Vector3 max) bounds)
        {
            if (_parentTransform)
            {
                Vector3 parentPos = _parentTransform.position;
                position.x = Mathf.Clamp(position.x, bounds.min.x + parentPos.x, bounds.max.x + parentPos.x);
                position.y = Mathf.Clamp(position.y, bounds.min.y + parentPos.y, bounds.max.y + parentPos.y);
            }
            else
            {
                position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
                position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
            }

            return position; 
        }
        
        
        public Vector3 CalculateWorldPosition( Vector3 delta )
        {
            return (_parentTransform) ? _parentTransform.position + _transform.localPosition + delta : _transform.position + delta;
        }

        public Vector3 CalculateLocalPosition(Vector3 worldPosition)
        {
            // Convert back to local space if necessary
            return (_parentTransform) ? worldPosition - _parentTransform.position : worldPosition;
        }
        
    }
}