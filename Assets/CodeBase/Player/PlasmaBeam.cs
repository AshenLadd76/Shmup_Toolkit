using UnityEngine;

namespace CodeBase.Player
{
    public class PlasmaBeam : MonoBehaviour
    {
        [SerializeField] private Vector2 anchor;

        [SerializeField] private Vector2 targetPosition;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float beamHeight;
        
        private Transform _transform;
        
        private void Awake()
        {
            _transform = transform;
            
            spriteRenderer.enabled = false;
        }

        private void Start()
        {
            SetBeamHeight( new Vector2( 0, beamHeight ) );
        }

        private void Update()
        {
            if( Input.GetKeyDown( KeyCode.Space ) ) 
                FireBeam();
            
            if( Input.GetKeyUp(KeyCode.Space ) ) 
                HideBeam();
        }

        private void SetBeamHeight(Vector2 position)
        {
            var yHeight = position.y - anchor.y;

            var tempSize = spriteRenderer.size;
            
            tempSize.y = yHeight; 
            
            spriteRenderer.size = tempSize;
        }

        private void FireBeam() => spriteRenderer.enabled = true;
        
        private void HideBeam() => spriteRenderer.enabled = false;
    }
}
