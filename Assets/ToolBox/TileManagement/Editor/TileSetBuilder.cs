using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public class TileSetBuilder
    {
        private Dictionary<byte[], Color32[]> _dictionary;
        private int _rowCount;
        private int _columnCount;
        
        
        public TileSetBuilder(Dictionary<byte[], Color32[]> tilesDictionary)
        {
            _dictionary = tilesDictionary;
            
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
            //TODO  do all the magic here...
            return null;
        }
    }
}