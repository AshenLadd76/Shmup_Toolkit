using System.Collections.Generic;
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
        
        
        private Dictionary<byte[], Color32[]> _uniqueTileDictionary;
        
        public TileExtractor(Texture2D textureToTile, int tileWidth, int tileHeight)
        {
            Debug.Log( $"Extracting Tiles from { textureToTile.name } { tileWidth } { tileHeight } " );
            _textureToTile = textureToTile;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            
            _uniqueTileDictionary = new Dictionary<byte[], Color32[]>(new ByteArrayComparer());
        }

        public void ExtractTiles()
        {
            Debug.Log( $"Extracting Tiles from { _textureToTile.name }" );
            
            var numHorizontalTiles = _textureToTile.width / _tileWidth;
            var numVerticalTiles = _textureToTile.height / _tileHeight;

            Color32[] texturePixels = _textureToTile.GetPixels32();
            
            //Outer Loop
            for (int numHorizontal = 0; numHorizontal < numHorizontalTiles; numHorizontal++)
            {
                for (int numVertical = 0; numVertical < numVerticalTiles; numVertical++)
                {
                    var tile = GetTile(texturePixels, numHorizontal, numVertical);

                    CheckAndAddUniqueTile(tile);
                }
            }
        }

        private Color32[] GetTile(Color32[] texturePixels, int outerIndexX , int outerIndexY)
        {
            Color32[] tile = new Color32[_tileWidth * _tileHeight];
            
            for (int x = 0; x < _tileWidth; x++)
            {
                for (int y = 0; y < _tileHeight; y++)
                {
                    int outerX = (outerIndexX * _tileWidth) + x;
                    int outerY = (outerIndexY * _tileHeight) + y;
                    
                    int pixelIndex = outerY * _textureToTile.width + outerX;
                    
                    tile[y * _tileWidth + x] = texturePixels[pixelIndex];
                }
            }
            
            return tile;
        }

        private void CheckAndAddUniqueTile(Color32[] tile )
        {
            var tileKey = TileToKey(tile);

            if (_uniqueTileDictionary.TryGetValue(tileKey, out var existingTile))
            {
                Debug.Log( $"tile { tileKey } already exists { existingTile }" );
                return;
            }

            Debug.Log( $"Adding new tile { tileKey }" );

            _uniqueTileDictionary.Add(tileKey, tile);
        }

        
        private byte[] TileToKey(Color32[] tile)
        {
            byte[] key = new byte[tile.Length * 4];
            int i = 0;
            foreach (var color in tile)
            {
                key[i++] = color.r;
                key[i++] = color.g;
                key[i++] = color.b;
                key[i++] = color.a;
            }
            return key;
        }
    }
}