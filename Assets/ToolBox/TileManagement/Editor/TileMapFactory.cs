using ToolBox.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.TileManagement.Editor
{
    public class TileMapFactory : ITileMapFactory
    {
        private readonly Matrix4x4 HorizontalFlip = Matrix4x4.Scale(new Vector3(-1, 1, 1));
        private readonly Matrix4x4 VerticalFlip = Matrix4x4.Scale(new Vector3(1, -1, 1));
        private readonly Matrix4x4 BothFlip = Matrix4x4.Scale(new Vector3(-1, -1, 1));
        
        private readonly Vector3 _defaultCellSize;
        private readonly bool _useCompositeColliders;
        private readonly CompositeCollider2D.GeometryType _colliderGeometryType;
        
        public TileMapFactory( Vector3 defaultCellSize, CompositeCollider2D.GeometryType colliderGeometryType = CompositeCollider2D.GeometryType.Outlines)
        {
            _defaultCellSize = defaultCellSize;
            _colliderGeometryType = colliderGeometryType;
        }
        
        public Tilemap CreateTileMap(string tileMapName, TileImageMap tileImageMap, Tile[] uniqueTiles, bool isCollsionMap)
        {
            var parentGo = CreateGrid(tileMapName);
            
            var parent = parentGo.transform;
            
            // Create Tilemap GameObject
            GameObject tilemapGo = new GameObject(tileMapName);
            
            tilemapGo.transform.SetParent(parent, false);
    
            // Add Tilemap and Renderer
            Tilemap tilemap = tilemapGo.AddComponent<Tilemap>();
            
            TilemapRenderer tilemapRenderer = tilemapGo.AddComponent<TilemapRenderer>();
            
            if(isCollsionMap)
                SetupColliders( tilemapGo );
            
            tilemapRenderer.sortingOrder = 0; 
            
            PopulateTileMap(tileImageMap, tilemap, uniqueTiles);
            
            return tilemap;
        }
        
        private  GameObject CreateGrid(string fileName)
        {
            GameObject grid = new GameObject();
            grid.name = $"{fileName}_Grid";
            grid.AddComponent<Grid>();
             
            var gridComponent = grid.GetComponent<Grid>();
            gridComponent.cellSize = new Vector3(1, 1, 0);
             
            return grid;
        }
        
        private void SetupColliders(GameObject tilemapGo)
        {
            TilemapCollider2D tilemapCollider2D = tilemapGo.AddComponent<TilemapCollider2D>();
            
            Rigidbody2D rigidbody2D = tilemapGo.AddComponent<Rigidbody2D>();
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            
            CompositeCollider2D compositeCollider2D = tilemapGo.AddComponent<CompositeCollider2D>();
            
            compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Outlines;
            compositeCollider2D.generationType = CompositeCollider2D.GenerationType.Synchronous;

            tilemapCollider2D.compositeOperation = Collider2D.CompositeOperation.Merge;
        }
        
        public void PopulateTileMap(TileImageMap tileImageMap, Tilemap tilemap, Tile[] uniqueTiles)
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
         
        private void CheckForFlip(Tilemap tilemap, TileMapCell cell, Vector3Int position)
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
    }
}