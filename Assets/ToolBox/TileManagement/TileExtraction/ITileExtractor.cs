using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.TileManagement.TileExtraction
{
    public interface ITileExtractor
    {
        List<Color32[]> ExtractTiles();
        
        public List<Color32[]> GetAllImageTiles();
        
    }
}