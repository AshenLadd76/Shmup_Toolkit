using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlash : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private float duration = 0.5f;
    
     [SerializeField] private Color originalColor;

    private LTDescr _ltDescr;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = _spriteRenderer.color; // cache the original color
    }

    /// <summary>
    /// Temporarily changes the sprite color and fades back.
    /// </summary>
    /// <param name="flashColor">Color to flash to</param>
    /// <param name="duration">Time it takes to fade back to original color</param>
    public void FlashColor()
    {
        if (_ltDescr != null)
        {
            LeanTween.cancel(_ltDescr.id);
            _spriteRenderer.color = originalColor;
        }

        if (!_spriteRenderer) return;
        
        
        // Generate a random color
        Color flashColor = new Color(Random.value, Random.value, Random.value);

        // Immediately set to flash color
        _spriteRenderer.color = flashColor;

        // Tween back to original color
        _ltDescr = LeanTween.value(gameObject, 0f, 1f, duration)
            .setOnUpdate((float t) =>
            {
                _spriteRenderer.color = Color.Lerp(flashColor, originalColor, t);
            });
    }
}

