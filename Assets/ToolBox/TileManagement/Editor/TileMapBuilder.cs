using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ToolBox.Extensions;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.TileManagement.Editor
{
    public static class TileMapBuilder
    {
        public static void BuildTileMap(string jsonPath, string atlasPath, string tileMapName, Transform parent = null)
        {
            //Load json
            TileImageMap tileImageMap = LoadTileImageMapFromJson(jsonPath);
            
            if (!TileImageMapValidator.ValidateTileImageMap(tileImageMap, tileMapName))
            {
                Logger.LogError("Tile Image Map Validation Error");
                return;
            }
            
            // Load all sprites from the atlas
            Sprite[] sprites = LoadSpriteAssets(atlasPath);
            
            if (sprites.IsNullOrEmpty() )
            {
                Logger.LogError("No sprites found in atlas!..could be null or empty");
                return;
            }
            
            // Create Grid if not provided
            var tilemap = CreateTileMap(tileMapName, parent);
            
            Tile[] uniqueTiles = CreateUniqueTiles(sprites);

            if (uniqueTiles.IsNullOrEmpty())
            {
                Logger.Log("No tiles found in atlas!");
                return;
            }
            
            PopulateTileMap( tileImageMap, tilemap, uniqueTiles );
            
            Logger.Log("Tilemap successfully built from JSON and atlas!");
        }

        private static Sprite[] LoadSpriteAssets(string atlasPath)
        {
            var emptyArray = Array.Empty<Sprite>();
            
            if (string.IsNullOrEmpty(atlasPath))
            {
                Logger.Log( $"Path to atlas {atlasPath} is null or empty!" );
                return emptyArray;
            }
            
            var assetArray = AssetDatabase.LoadAllAssetsAtPath(atlasPath).OfType<Sprite>().ToArray();

            if (!assetArray.IsNullOrEmpty()) return assetArray;

            Logger.Log($"Failed to load assets from atlas {atlasPath}!");

            return emptyArray;
        }

        private static readonly Matrix4x4 HorizontalFlip = Matrix4x4.Scale(new Vector3(-1, 1, 1));
        private static readonly Matrix4x4 VerticalFlip = Matrix4x4.Scale(new Vector3(1, -1, 1));
        private static readonly Matrix4x4 BothFlip = Matrix4x4.Scale(new Vector3(-1, -1, 1));
        
        private static void CheckForFlip(Tilemap tilemap, TileMapCell cell, Vector3Int position)
        {
            if (cell.flipType == FlipType.None) return;

            Matrix4x4 matrix = cell.flipType switch
            {
                FlipType.Horizontal => HorizontalFlip,
                FlipType.Vertical => VerticalFlip,
                FlipType.Both => BothFlip,
                _ => Matrix4x4.identity
            };
            tilemap.SetTransformMatrix(position, matrix);
        }

        private static Tile[] CreateUniqueTiles(Sprite[] sprites)
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

        private static Tilemap CreateTileMap(string tileMapName, Transform parent)
        {
            GameObject parentGo;
            
            if(parent == null)
            {
                parentGo = CreateGrid(tileMapName);
                parent  = parentGo.transform;
            }

            
            // Create Tilemap GameObject
            GameObject tilemapGo = new GameObject(tileMapName);
            
            tilemapGo.transform.SetParent(parent, false);
    
            // Add Tilemap and Renderer
            Tilemap tilemap = tilemapGo.AddComponent<Tilemap>();
            
            TilemapRenderer tilemapRenderer = tilemapGo.AddComponent<TilemapRenderer>();
            
            tilemapRenderer.sortingOrder = 0; 
            
            return tilemap;
        }

        private static void PopulateTileMap(TileImageMap tileImageMap, Tilemap tilemap, Tile[] uniqueTiles)
        {
            int rows = tileImageMap.Rows;
            int columns = tileImageMap.Columns;
            var cells = tileImageMap.Cells;
            int cellCount = cells.Count;
            
            
            for (int y = 0; y < tileImageMap.Rows; y++)
            {
                int yPos = rows - 1 - y;
                
                for (int x = 0; x < tileImageMap.Columns; x++)
                {
                    int index = y * tileImageMap.Columns + x; // converts 2D coordinates to 1D
                    
                    if (index >= tileImageMap.Cells.Count)
                    {
                        Logger.LogWarning($"Cell index {index} out of range for tilemap {tilemap.name}");
                        continue;
                    }
                    
                    TileMapCell cell = tileImageMap.Cells[index];

                    int tileIndex = cell.uniqueIndex;
                        
                    if(!uniqueTiles.IsIndexValid( tileIndex )) continue;

                    Tile tile = uniqueTiles[tileIndex];
                    
                    // Position in Tilemap (Tilemap uses Vector3Int coordinates)
                    Vector3Int position = new Vector3Int(x, yPos, 0);
                    
                    tilemap.SetTile(position, tile);
                    
                    
                    CheckForFlip(tilemap, cell, position);
                }
            }
            
            tilemap.RefreshAllTiles();
        }

        private static GameObject CreateGrid(string fileName)
        {
             GameObject grid = new GameObject();
             grid.name = $"{fileName}_Grid";
             grid.AddComponent<Grid>();
             
             return grid;
        }

        private static TileImageMap LoadTileImageMapFromJson(string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath))
            {
                Logger.Log($"Path to json {jsonPath} is null or empty!");
                return null;
            }
            
            string json = File.ReadAllText(jsonPath);
            
            Logger.Log(jsonPath);
            
            if( string.IsNullOrEmpty(json) ) return null;
            
            Logger.Log(json);
            
            TileImageMap tileImageMap;
            
            try
            {
                tileImageMap = JsonConvert.DeserializeObject<TileImageMap>(json);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to parse JSON at {jsonPath}: {ex.Message}");
                return null;
            }
            
            return tileImageMap;
        }
    }
}
