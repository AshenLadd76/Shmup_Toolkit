using System;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    [Serializable]
    public class TileMetaData 
    {
        public int tileWidth;
        public int tileHeight;
        public int columns;
        public int rows;

        public bool isFlippedHorizontally;
        public bool isFlippedVertically;

        public Vector2Int positionInImage; // row/col in original image

        public int uniqueIndex; // index in unique tile atlas
    }
}
