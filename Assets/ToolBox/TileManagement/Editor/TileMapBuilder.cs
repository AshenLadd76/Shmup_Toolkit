using UnityEngine;
using UnityEngine.Tilemaps;
using ToolBox.Extensions;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileMapBuilder
    {
        public void Build();
    }
    
    public class TileMapBuilder : ITileMapBuilder
    {
        private readonly string _jsonFilePath;
        private readonly string _atlasFilePath;
        private readonly string _tileMapName;
   
        
        private bool _isCollisionMap = false;
        private Transform _parent = null;
        private Vector3 _cellSize = Vector3.one;
        private CompositeCollider2D.GeometryType _geometryType;
        private Vector2Int _chunkSize;

        
        public TileMapBuilder(string jsonFilePath, string atlasFilePath, string tileMapName, Vector2Int chunkSize, 
            bool isCollisionMap = false, CompositeCollider2D.GeometryType geometryType = CompositeCollider2D.GeometryType.Outlines,  Transform parent = null )
        {
            _jsonFilePath = jsonFilePath;
            _atlasFilePath = atlasFilePath;
            _tileMapName = tileMapName;
            _isCollisionMap = isCollisionMap;
            _parent = parent;
            _geometryType = geometryType;
            _chunkSize = chunkSize;
            
            Logger.Log( $"Add collision map: {isCollisionMap}" );
           
        }
        
        public void Build()
        {
            TileImageMap tileImageMap = new TileImageMapLoader().Load(_jsonFilePath);

            if (tileImageMap == null)
            {
                Logger.LogError($"Failed to load tile map: {_jsonFilePath}");
                return;
            }
            
            if (!TileImageMapValidator.ValidateTileImageMap(tileImageMap, _tileMapName))
            {
                Logger.LogError("Tile Image Map Validation Error");
                return;
            }
            
            // Load all sprites from the atlas
            Sprite[] sprites = SpriteAtlasLoader.LoadSpriteAssets(_atlasFilePath);
            
            if (sprites.IsNullOrEmpty() )
            {
                Logger.LogError("No sprites found in atlas!..could be null or empty");
                return;
            }
            
            Tile[] uniqueTiles = CreateUniqueTiles(sprites);

            if (uniqueTiles.IsNullOrEmpty())
            {
                Logger.Log("No tiles found in atlas!");
                return;
            }

            ITileMapFactory tilemapFactory = new TileMapFactory( new Vector3(1,1,1),_geometryType);
            
            tilemapFactory.BuildTileMap(_tileMapName, tileImageMap, uniqueTiles, _isCollisionMap, _chunkSize);
        }
        
        private  Tile[] CreateUniqueTiles(Sprite[] sprites)
        {
            if (sprites.IsNullOrEmpty())
            {
                Logger.Log($"No sprites found in atlas!");
                return null;
            }
            
            // Create a Tile for each unique sprite
            Tile[] tiles = new Tile[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                tiles[i] = ScriptableObject.CreateInstance<Tile>();
                tiles[i].sprite = sprites[i];
            }
            
            return tiles;
        }
    }
}
