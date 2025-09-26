using ToolBox.Extensions;
using UnityEngine;

namespace ToolBox.TileManagement
{
    public static class TileFlipper 
    {
        public static Color32[] FlipHorizontal(Color32[] colors, int width, int height)
        {
            if (colors.IsNullOrEmpty()) return null;
            
            Color32[] flipped = new Color32[colors.Length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = y * width + x;
                    int destIndex = y * width + (width - 1 - x);
                    flipped[destIndex] = colors[srcIndex];
                }
            }

            return flipped;
        }
        
        public static  Color32[] FlipVertical(Color32[] colors, int width, int height)
        {
            Color32[] flipped = new Color32[colors.Length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = y * width + x;
                    int destIndex = (height - 1 - y) * width + x;
                    flipped[destIndex] = colors[srcIndex];
                }
            }
            
            return flipped;
        }
        
        public static Color32[] FlipBoth(Color32[] tile, int tileWidth, int tileHeight)
        {
            return FlipHorizontal(FlipVertical(tile, tileWidth, tileHeight), tileWidth, tileHeight);
        }
    }
}
