using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public static class TileComparer
    {
        // Reusable buffer for flipped tiles
        private static Color32[] _flipBuffer = null;

        // Ensure the flip buffer is large enough
        private static void EnsureFlipBuffer(int size)
        {
            if (_flipBuffer == null || _flipBuffer.Length < size)
                _flipBuffer = new Color32[size];
        }
        
        private static bool TilesAreEqual(Color32[] tileA, Color32[] tileB, byte tolerance)
        {
            for (int i = 0; i < tileA.Length; i++)
            {
                // Ignore fully transparent pixels
                if (tileA[i].a == 0 && tileB[i].a == 0)
                    continue;

                if ((tileA[i].r - tileB[i].r) > tolerance || (tileB[i].r - tileA[i].r) > tolerance ||
                    (tileA[i].g - tileB[i].g) > tolerance || (tileB[i].g - tileA[i].g) > tolerance ||
                    (tileA[i].b - tileB[i].b) > tolerance || (tileB[i].b - tileA[i].b) > tolerance ||
                    (tileA[i].a - tileB[i].a) > tolerance || (tileB[i].a - tileA[i].a) > tolerance)
                    return false;
                
            }
            return true;
        }
        
        public static (bool isEquivalent, FlipType flipType) IsTileEquivalent(Color32[] newTile, Color32[] existingTile, byte tolerance = 0, int tileWidth = 0, int tileHeight = 0)
        {
            int tileSize = tileWidth * tileHeight;
            
            EnsureFlipBuffer(tileSize);
            
            var hFlip = TileFlipper.FlipHorizontal(newTile, tileWidth, tileHeight);
            var vFlip = TileFlipper.FlipVertical(newTile, tileWidth, tileHeight);
            var bFlip = TileFlipper.FlipBoth(newTile, tileWidth, tileHeight);
            
            if (TilesAreEqual(newTile, existingTile, tolerance))
                return (true, FlipType.None);

            if (TilesAreEqual(hFlip, _flipBuffer, tolerance))
                return (true, FlipType.Horizontal);

            if (TilesAreEqual(vFlip, _flipBuffer, tolerance))
                return (true, FlipType.Vertical);

            if (TilesAreEqual(bFlip, _flipBuffer, tolerance))
                return (true, FlipType.Both);

            return (false, FlipType.None);
        }
    }
}