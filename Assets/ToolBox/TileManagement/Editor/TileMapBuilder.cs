using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace ToolBox.TileManagement.Editor
{
    public static class TileMapBuilder
    {
        public static void BuildTileMap(string jsonPath, string atlasPath, Transform parent = null)
        {
            
            //Load json
            string json = File.ReadAllText(jsonPath);
            
            Debug.Log(jsonPath);
            
            if( string.IsNullOrEmpty(json) ) return;
            
            Debug.Log(json);
            
            TileImageMap tileImageMap = JsonConvert.DeserializeObject<TileImageMap>(json);


            if (tileImageMap == null)
            {
                Debug.Log($"Failed to load tile image map from json: {jsonPath}");
                return;
            }
            

            // Load all sprites from the atlas
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(atlasPath).OfType<Sprite>().ToArray();
            
            if (sprites.Length == 0)
            {
                Debug.LogError("No sprites found in atlas!");
                return;
            }
            
            // Create GameObject and Tilemap
            GameObject go = new GameObject("Tilemap");
            if (parent != null) go.transform.parent = parent;
            
            Tilemap tilemap = go.AddComponent<Tilemap>();
            TilemapRenderer renderer = go.AddComponent<TilemapRenderer>();
            
            // Create a Tile for each unique sprite
            Tile[] tiles = new Tile[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                tiles[i] = ScriptableObject.CreateInstance<Tile>();
                tiles[i].sprite = sprites[i];
            }

            for (int y = 0; y < tileImageMap.Rows; y++)
            {
                for (int x = 0; x < tileImageMap.Columns; x++)
                {
                    int index = y * tileImageMap.Columns + x; // converts 2D coordinates to 1D
                    
                    TileMapCell cell = tileImageMap.Cells[index];

                    Tile tile = tiles[cell.uniqueIndex];
           
                    
                    // Position in Tilemap (Tilemap uses Vector3Int coordinates)
                    Vector3Int position = new Vector3Int(x, tileImageMap.Rows - 1 - y, 0);
                    
                    tilemap.SetTile(position, tile);
                    
                    if (cell.flipType != FlipType.None)
                    {
                        Matrix4x4 matrix = Matrix4x4.identity;
                        switch (cell.flipType)
                        {
                            case FlipType.Horizontal:
                                matrix  = Matrix4x4.Scale(new Vector3(-1, 1, 1));
                                break;
                            case FlipType.Vertical:
                                matrix = Matrix4x4.Scale(new Vector3(1, -1, 1));
                                break;
                            case FlipType.Both:
                                matrix = Matrix4x4.Scale(new Vector3(-1, -1, 1));
                                break;
                        }
                        tilemap.SetTransformMatrix(position, matrix);
                    }
                }
            }
            
            Debug.Log("Tilemap successfully built from JSON and atlas!");
        }
    }
}
