using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    [Serializable]
    public class ImageMapBuilder
    {
        private List<Color32[]> _uniqueTiles;
        private List<Color32[]> _allImageTiles;
        private int _tileWidth;
        private int _tileHeight;
        private int _columns;
        private int _rows;
        
        public ImageMapBuilder SetUniqueTiles(List<Color32[]> uniqueTiles)
        {
            _uniqueTiles = uniqueTiles;
            return this;
        }

        public ImageMapBuilder SetAllImageTiles(List<Color32[]> allImageTiles)
        {
            _allImageTiles = allImageTiles;
            return this;
            
        }

        public ImageMapBuilder SetTileWidth(int tileWidth)
        {
            _tileWidth = tileWidth;
            return this;
        }

        public ImageMapBuilder SetTileHeight(int tileHeight)
        {
            _tileHeight = tileHeight;
            return this;
        }

        public ImageMapBuilder SetColumns(int columns)
        {
            _columns = columns;
            return this;
        }

        public ImageMapBuilder SetRows(int rows)
        {
            _rows = rows;
            return this;
        }

        public JsonImageMap Build()
        {
            //create unique tile look up
            var uniqueTileLookup = BuildUniqueTileLookup();
            
            //Fill map and meta data
            var jsonImageMap = FillTileMap( uniqueTileLookup );
            
            return jsonImageMap;
        }

        private JsonImageMap BuildImageMap()
        {
            return new JsonImageMap
            {
                TileWidth = _tileWidth,
                TileHeight = _tileHeight,
                Columns = _columns,
                Rows = _rows,
                Map = new int[_rows, _columns],
                Tiles = new List<TileMetaData>()
            };
        }

        private Dictionary<string, int> BuildUniqueTileLookup()
        {
            Dictionary<string, int> uniqueLookup = new Dictionary<string, int>();
            for (int i = 0; i < _uniqueTiles.Count; i++)
            {
                uniqueLookup[TileToKey(_uniqueTiles[i])] = i;
            }

            return uniqueLookup;
        }


        private JsonImageMap FillTileMap( Dictionary<string, int> uniqueTileLookup)
        {
            //build tile map 
            var jsonImageMap = BuildImageMap();
            
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    int index = row * _columns + column;
                    
                    Color32[] tile = _allImageTiles[index];
                    string key = TileToKey(tile);
                    
       

                    if (uniqueTileLookup.TryGetValue(key, out int uniqueIndex))
                    {
                      Debug.Log($"Match found at index: {uniqueIndex}");

                        jsonImageMap.Map[row, column] = uniqueIndex;

                        // Optionally add metadata for each tile
                        //     var meta = new TileMetaData
                        //     {
                        //         tileWidth = _tileWidth,
                        //         tileHeight = _tileHeight,
                        //         columns = _columns,
                        //         rows = _rows,
                        //         positionInImage = new Vector2Int(row, column),
                        //         uniqueIndex = uniqueIndex
                        //     };
                        //     jsonTileMap.Tiles.Add(meta);
                        
                    }
                }
            }
            
            Debug.Log(jsonImageMap.Map.Length);

            return jsonImageMap;
        }
        
        private string TileToKey(Color32[] tile)
        {
            byte[] key = new byte[tile.Length * 4];
            int i = 0;
            foreach (var c in tile)
            {
                key[i++] = c.r;
                key[i++] = c.g;
                key[i++] = c.b;
                key[i++] = c.a;
            }
            return Convert.ToBase64String(key);
        }
    }
}