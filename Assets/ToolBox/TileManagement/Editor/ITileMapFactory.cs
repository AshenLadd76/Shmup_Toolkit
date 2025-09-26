using UnityEngine.Tilemaps;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileMapFactory
    {
        Tilemap CreateTileMap(string tileMapName, TileImageMap tileImageMap,  Tile[] uniqueTiles, bool b);
        
    }
}