using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileExtractor
    {
        List<Color32[]> ExtractTiles();
    }

    public class TileExtractor : ITileExtractor
    {
        private Texture2D _textureToTile;
        private int _tileWidth;
        private int _tileHeight;
        
        
        private List<Color32[]> _uniqueTiles;
        
        public TileExtractor(Texture2D textureToTile, int tileWidth, int tileHeight)
        {
            Debug.Log( $"Extracting Tiles from { textureToTile.name } { tileWidth } { tileHeight } " );
            _textureToTile = textureToTile;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            
           _uniqueTiles = new List<Color32[]>();
        }

        public List<Color32[]> ExtractTiles()
        {
            Debug.Log( $"Extracting Tiles from { _textureToTile.name }" );
            
            var textureWidth = _textureToTile.width;
            var textureHeight = _textureToTile.height;

            if (textureWidth % _tileWidth != 0 || textureHeight % _tileHeight != 0)
            {
                Debug.LogError($"Invalid Texture Size { _textureToTile.name } { _tileWidth } { _tileHeight } ");
                return null;
            }
            
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
            
            Debug.Log( $"Extracted { _count } unique Tiles from { _textureToTile.name }" );

            _count = 0;

            return _uniqueTiles;
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


        private int _count;
        private void CheckAndAddUniqueTile(Color32[] tile )
        {
           // var tileKey = TileToKey(tile);
            
           foreach (var existingTile in _uniqueTiles)
           {
               if (IsTileEquivalent(existingTile, tile, 0))
               {
                   // Already exists
                   return;
               }
           }
           
           _count++;
           
           _uniqueTiles.Add(tile);
        }

        
        bool IsTileEquivalent(Color32[] newTile, Color32[] existingTile, byte tolerance = 0)
        {
            return TilesAreEqual(newTile, existingTile, tolerance)
                   || TilesAreEqual(TileFlipper.FlipHorizontal(newTile, _tileWidth, _tileHeight), existingTile, tolerance)
                   || TilesAreEqual(TileFlipper.FlipVertical(newTile, _tileWidth, _tileHeight), existingTile, tolerance) 
                   || TilesAreEqual(TileFlipper.FlipBoth(newTile, _tileWidth, _tileHeight),existingTile, tolerance);
        }
        
        private bool TilesAreEqual(Color32[] tileA, Color32[] tileB, byte tolerance)
        {
            for (int i = 0; i < tileA.Length; i++)
            {
                // Ignore fully transparent pixels
                if (tileA[i].a == 0 && tileB[i].a == 0)
                    continue;

                if (Mathf.Abs(tileA[i].r - tileB[i].r) > tolerance ||
                    Mathf.Abs(tileA[i].g - tileB[i].g) > tolerance ||
                    Mathf.Abs(tileA[i].b - tileB[i].b) > tolerance ||
                    Mathf.Abs(tileA[i].a - tileB[i].a) > tolerance)
                    return false;
            }
            return true;
        }
    }
}