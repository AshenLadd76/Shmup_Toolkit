using UnityEngine;
using UnityEngine.Tilemaps;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileMapFactory
    {
        Tilemap BuildTileMap(string tileMapName, TileImageMap tileImageMap,  Tile[] uniqueTiles, bool b, Vector2Int chunkSize = default);
        
    }
}