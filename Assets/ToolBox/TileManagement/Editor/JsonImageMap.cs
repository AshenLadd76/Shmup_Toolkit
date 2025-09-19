using System;
using System.Collections.Generic;

namespace ToolBox.TileManagement.Editor
{
    [Serializable]
    public class JsonImageMap
    {
        public int TileWidth;
        public int TileHeight;
        public int Columns;
        public int Rows;
        public int[,] Map; // map[row, col] -> unique tile index
        public List<TileMetaData> Tiles; // metadata for each unique tile
    }
}