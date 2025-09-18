using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public class TileSetBuilder
    {
        private List<Color32[]> _tileList;
        private int _rowCount;
        private int _columnCount;
        
        private int _tileWidth;
        private int _tileHeight;

        private int _atlasWidth;
        private int _atlasHeight;
        
        
        public TileSetBuilder(List<Color32[]> tilesList, int tileWidth, int tileHeight)
        {
            _tileList = tilesList;
            
            _tileWidth =  tileWidth;
            _tileHeight = tileHeight;
        }

        public TileSetBuilder SetRowCount(int rowCount)
        {
            _rowCount = rowCount;
            return this;
        }

        public TileSetBuilder SetColumnCount(int columnCount)
        {
            _columnCount = columnCount;
            return this;
        }

        public Texture2D Build()
        {
           _atlasWidth = _columnCount * _tileWidth;
           _atlasHeight = _rowCount * _tileHeight;
           
           var atlas = new Texture2D(_atlasWidth, _atlasHeight, TextureFormat.RGBA32, false);
           
           DefaultToTransparent(atlas, _atlasWidth, _atlasHeight);

           int index = 0;
           
           foreach (var tile in _tileList)
           { 
               int row = index / _columnCount;
               int col = index % _columnCount;

               int startX = col * _tileWidth;
               int startY = (_rowCount - 1 - row) * _tileHeight; // flip Y
               
               // Paint pixels from tile into atlas
               atlas.SetPixels32(startX, startY, _tileWidth, _tileHeight, tile);

               index++;
           }
           
           atlas.filterMode = FilterMode.Point;  // sharp, no blur
           atlas.wrapMode = TextureWrapMode.Clamp; // avoid bleeding from opposite side
           atlas.alphaIsTransparency = true;
           atlas.Apply();
           
           return atlas;
        }

        private void DefaultToTransparent( Texture2D texture, int textureWidth, int textureHeight )
        {
            Color32[] emptyPixels = new Color32[textureWidth * textureHeight];
            
            for (int i = 0; i < emptyPixels.Length; i++)
            {
                emptyPixels[i] = new Color32(0, 0, 0, 0); // fully transparent
            }

            texture.SetPixels32(emptyPixels);
            
            texture.Apply();
        }

        
        //
        // (y)
        //   3 ┌───┬───┬───┬───┐  <- top row (y=3)
        //     │   │   │   │   │
        //   2 ├───┼───┼───┼───┤
        //     │   │   │   │   │
        //   1 ├───┼───┼───┼───┤
        //     │   │   │   │   │
        //   0 └───┴───┴───┴───┘  <- bottom row (y=0)
        //     0   1   2   3   (x)
        //
        //
        
        private Texture2D TrimTransparentRows(Texture2D texture, int textureWidth, int textureHeight)
        {
            var pixels = texture.GetPixels32();

            int minY = textureHeight;
            
            int maxY = -1;

            for (int y = 0; y < textureHeight; y++)   // outer loop → row by row
            {
                bool rowEmpty = true;
                
                for (int x = 0; x < textureWidth; x++) // inner loop → left to right in that row
                {
                    if (CheckForTransparentPixel(pixels, x, y))
                    {
                        rowEmpty = false;
                        break;
                    }
                }

                if (!rowEmpty)
                {
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
            
            // If texture is fully transparent, return an empty texture
            if (maxY < minY)
                return new Texture2D(textureWidth, 1);
            
            int newHeight = maxY - minY + 1;
           
            Color32[] newPixels = new Color32[textureWidth * newHeight];

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < textureWidth ; x++)
                {
                    newPixels[y * textureWidth  + x] = pixels[(minY + y) * textureWidth  + x];
                }
            }

            Texture2D trimmedTexture = new Texture2D(textureWidth , newHeight);
            trimmedTexture.SetPixels32(newPixels);
            trimmedTexture.Apply();

            return trimmedTexture;
        }

        private bool CheckForTransparentPixel(Color32[] pixels, int x, int y) => (pixels[y * _atlasWidth + x].a > 0);
        
    }
}