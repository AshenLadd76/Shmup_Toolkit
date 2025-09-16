using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileExtractor
    {
        void ExtractTiles();
    }

    public class TileExtractor : ITileExtractor
    {
        private Texture2D _textureToTile;
        private int _tileWidth;
        private int _tileHeight;
        private int _tolerance;
        
        public TileExtractor(Texture2D textureToTile, int tileWidth, int tileHeight, int tolerance)
        {
            Debug.Log( $"Extracting Tiles from { textureToTile.name } { tileWidth } { tileHeight } tolerance { tolerance }" );
            _textureToTile = textureToTile;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _tolerance = tolerance;
        }

        public void ExtractTiles()
        {
            Debug.Log( $"Extracting Tiles from { _textureToTile.name }" );
        }
    }
}