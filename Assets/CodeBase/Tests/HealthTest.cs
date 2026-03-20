using UnityEngine;
using UnityEngine.Events;

namespace CodeBase.Tests
{
    [RequireComponent( typeof( SpriteRenderer ) )]
    public class HealthTest : MonoBehaviour
    {
        [SerializeField] private float health = 200;
        [SerializeField] private Color originalColor = Color.green;
        [SerializeField] private Color targetColor = Color.red;
        [SerializeField, Range(0f, 1f)] private float increment = 0.2f; // How far to move towards target each hit
        [SerializeField] private float fadeSpeed = 2f; // How fast to return to original color

        [SerializeField] private UnityEvent onDeath;
        
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = originalColor;
        }

        private void Update()
        {
            // Smoothly return towards original color
            if (_spriteRenderer.color != _originalColor)
                _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, originalColor, Time.deltaTime * fadeSpeed);
            
        }

        /// <summary>
        /// Call this every time the object is hit
        /// </summary>
        public void OnHit()
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, targetColor, increment);
            Damage(1);

            if (health <= 0)
            {
                onDeath?.Invoke();
            }
            
            
        }

        public void Damage(float damage)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, targetColor, increment);
            
            health -= damage;
            
            if (health <= 0)
            {
                onDeath?.Invoke();
            }
        }
    }
}
