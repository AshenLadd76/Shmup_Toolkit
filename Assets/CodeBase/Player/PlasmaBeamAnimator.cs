using UnityEngine;

namespace CodeBase.Player
{
    public class BeamAnimator : MonoBehaviour
    {
        private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");

        [SerializeField] private Material scrollingMaterial;
        [SerializeField] private Vector3 scrollSpeed;

        public SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer.material = scrollingMaterial;
            
            scrollingMaterial.SetVector(ScrollSpeed, scrollSpeed);
        }
     

        
    }
}

