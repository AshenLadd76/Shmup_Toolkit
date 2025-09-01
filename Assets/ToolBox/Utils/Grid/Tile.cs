using UnityEngine;
using TMPro;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Tools.Grids
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textBox;
        [SerializeField] private Color walkableColour;
        [SerializeField] private Color unWalkableColour;
        
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetText(string text)
        {
            if (textBox == null || string.IsNullOrEmpty(text))
            {
                Logger.LogWarning($"[Tile] The textBox is null or the text is null/empty on GameObject: {gameObject.name}");

                return;
            }
            

            textBox.text = text;
        }

        public void SetTileColour(Color color) => _spriteRenderer.color = color;

        public void SetTileAsWalkable() => SetTileColour(walkableColour);

        public void SetTileAsUnWalkable() => SetTileColour(unWalkableColour);
    }
}
