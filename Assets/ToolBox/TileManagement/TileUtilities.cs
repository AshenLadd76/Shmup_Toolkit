using UnityEngine;

namespace ToolBox.TileManagement
{
    public static class TileUtilities
    {
        public static  int ComputeTileHash(Color32[] pixels)
        {
            unchecked
            {
                int hash = 17;
                foreach (var c in pixels)
                    hash = hash * 31 + c.GetHashCode();
                return hash;
            }
        }
        
        public static int ComputeTileHashFlipped(Color32[] tile, FlipType flip, int width, int height)
        {
            unchecked
            {
                int hash = 17;
                // int width = _tileWidth;
                // int height = _tileHeight;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Compute flipped coordinates
                        int fx = flip == FlipType.Horizontal || flip == FlipType.Both ? (width - 1 - x) : x;
                        int fy = flip == FlipType.Vertical   || flip == FlipType.Both ? (height - 1 - y) : y;

                        int index = fy * width + fx;
                        Color32 c = tile[index];

                        // Simple hash combining RGBA
                        hash = hash * 31 + c.r;
                        hash = hash * 31 + c.g;
                        hash = hash * 31 + c.b;
                        hash = hash * 31 + c.a;
                    }
                }

                return hash;
            }
        }
        
        private static bool TilesAreEqual(Color32[] tileA, Color32[] tileB, byte tolerance)
        {
            for (int i = 0; i < tileA.Length; i++)
            {
                // Ignore fully transparent pixels
                if (tileA[i].a == 0 && tileB[i].a == 0)
                    continue;

                if ((tileA[i].r - tileB[i].r) > tolerance || (tileB[i].r - tileA[i].r) > tolerance) return false;
                
            }
            return true;
        }
        
        public static (bool isEquivalent, FlipType flipType)  IsTileEquivalent(Color32[] newTile, Color32[] existingTile, byte tolerance = 0, int tileWidth = 0, int tileHeight = 0)
        {
         
            
            
            var hFlip = TileFlipper.FlipHorizontal(newTile, tileWidth, tileHeight);
            var vFlip = TileFlipper.FlipVertical(newTile, tileWidth, tileHeight);
            var bFlip = TileFlipper.FlipBoth(newTile, tileWidth, tileHeight);
            
            
            if (TilesAreEqual(newTile, existingTile, tolerance))
                return (true, FlipType.None);

            if (TilesAreEqual(hFlip, existingTile, tolerance))
                return (true, FlipType.Horizontal);

            if (TilesAreEqual(vFlip, existingTile, tolerance))
                return (true, FlipType.Vertical);

            if (TilesAreEqual(bFlip, existingTile, tolerance))
                return (true, FlipType.Both);

            return (false, FlipType.None);
        }
    }
}