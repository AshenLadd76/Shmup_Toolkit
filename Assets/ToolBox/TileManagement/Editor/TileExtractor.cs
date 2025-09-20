using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ToolBox.TileManagement.Editor
{
    public class TileExtractor : ITileExtractor
    {
        private readonly Texture2D _textureToTile;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        
        private readonly List<Color32[]> _uniqueTiles;
        private readonly List<Color32[]> _allImageTiles;
        
        private readonly Dictionary<int, int> _tileHashLookup = new(); // hash -> index in _uniqueTiles

        private int _count;
        
        private readonly List<TileMapCell> _tileMapCellList;
        
        private readonly TileImageMap _tileImageMap;
        
        public TileExtractor(Texture2D textureToTile, int tileWidth, int tileHeight)
        {
            _textureToTile = textureToTile;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            
           _uniqueTiles = new List<Color32[]>();
           _allImageTiles = new List<Color32[]>();
           
           _tileMapCellList = new List<TileMapCell>();
           _tileImageMap = new TileImageMap();
        }

        public List<Color32[]> ExtractTiles()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            _uniqueTiles.Clear();
            _tileHashLookup.Clear();
            
            var textureWidth = _textureToTile.width;
            var textureHeight = _textureToTile.height;

            if (textureWidth % _tileWidth != 0 || textureHeight % _tileHeight != 0)
            {
                Debug.LogError($"Invalid image size  Width cannot by { _textureToTile.name } { _tileWidth } { _tileHeight } ");
                return null;
            }
            
            var numHorizontalTiles = _textureToTile.width / _tileWidth;
            var numVerticalTiles = _textureToTile.height / _tileHeight;
            
            Color32[] texturePixels = _textureToTile.GetPixels32();
            
            
            //Outer Loop
            for (int numVertical = 0; numVertical < numVerticalTiles; numVertical++)
            {
                for (int numHorizontal = 0; numHorizontal < numHorizontalTiles; numHorizontal++)
                {
                    var tile = GetTileFromTexture(texturePixels, numHorizontal, numVertical);
                    _allImageTiles.Add(tile);
                    CheckAndAddUniqueTile(tile);
                }
            }
            
            stopwatch.Stop();
             
            Debug.Log( $"Extracted { _count } unique Tiles in { stopwatch.ElapsedMilliseconds } ms" );
            
            _tileImageMap.Columns = numHorizontalTiles;
            _tileImageMap.Rows = numVerticalTiles;
            _tileImageMap.Cells = _tileMapCellList;
           
            var jsonText =  JsonConvert.SerializeObject(_tileImageMap);
             Debug.Log(jsonText);
            
            JsonSaver.SaveJson(jsonText, $"{EditorPrefs.GetString("TileExtractor_SavePath")}/jsonTileMap.json");
            
            _count = 0;

            return _uniqueTiles;
        }

        
        
        // Top row
        // [ 56 57 58 59 60 61 62 63 ]  ← outerY = 7
        // [ 48 49 50 51 52 53 54 55 ]
        // [ 40 41 42 43 44 45 46 47 ]
        // [ 32 33 34 35 36 37 38 39 ]
        // [ 24 25 26 27 28 29 30 31 ]
        // [ 16 17 18 19 20 21 22 23 ]
        // [  8  9 10 11 12 13 14 15 ]
        // Bottom row
        // [  0  1  2  3  4  5  6  7 ]  ← outerY = 0
        
        private Color32[] GetTileFromTexture(Color32[] texturePixels, int outerIndexX , int outerIndexY)
        {
            Color32[] tile = new Color32[_tileWidth * _tileHeight];
            
            int startX = outerIndexX * _tileWidth;
            int startY = (_textureToTile.height - (outerIndexY + 1) * _tileHeight);
            int textureWidth = _textureToTile.width;
            
            for (int y = 0; y < _tileHeight; y++)
            {
                int outerY = startY + y;
                int rowStart = outerY * textureWidth + startX; 
                int tileRowStart = y * _tileWidth;
                
                for (int x = 0; x < _tileWidth; x++)
                {
                    int outerX = startX + x;
                    
                    tile[tileRowStart + x] = texturePixels[ rowStart + x ];
                }
            }
            
            return tile;
        }
        
   
        private void CheckAndAddUniqueTile(Color32[] tile )
        {
            int hashOriginal = TileUtilities.ComputeTileHash(tile);
            int hashH = TileUtilities.ComputeTileHashFlipped(tile, FlipType.Horizontal, _tileWidth, _tileHeight);
            int hashV = TileUtilities.ComputeTileHashFlipped(tile, FlipType.Vertical, _tileWidth, _tileHeight);
            int hashB = TileUtilities.ComputeTileHashFlipped(tile, FlipType.Both, _tileWidth, _tileHeight);
            
            
            if (_tileHashLookup.TryGetValue(hashOriginal, out int existingIndex) ||
                _tileHashLookup.TryGetValue(hashH, out existingIndex) ||
                _tileHashLookup.TryGetValue(hashV, out existingIndex) ||
                _tileHashLookup.TryGetValue(hashB, out existingIndex))
            {
                var equivalentResult =  TileUtilities.IsTileEquivalent( _uniqueTiles[existingIndex], tile, 0, _tileWidth, _tileHeight );

                if (equivalentResult.isEquivalent)
                {
                  
                    var tileMapCell = new TileMapCell()
                    {
                            uniqueIndex = existingIndex,
                            flipType = equivalentResult.flipType,
                    };
                        
                    _tileMapCellList.Add( tileMapCell );
                    
                    return;
                }
            }
            
            _uniqueTiles.Add(tile);
            
            int newIndex = _uniqueTiles.Count - 1;
            
            _tileHashLookup[hashOriginal] = newIndex;
            _tileHashLookup[hashH] = newIndex;
            _tileHashLookup[hashV] = newIndex;
            _tileHashLookup[hashB] = newIndex;
            
            
           var newTileMapCell = new TileMapCell()
           {
               uniqueIndex = newIndex, // Index of newly added tile
               flipType = FlipType.None,             // No flip for new tile
           };
          
           _tileMapCellList.Add(newTileMapCell);
           
           _count++;
        }

        public List<Color32[]> GetAllImageTiles() => _allImageTiles;
        
    }
}