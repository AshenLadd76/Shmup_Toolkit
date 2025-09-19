using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public interface ITileExtractor
    {
        List<Color32[]> ExtractTiles();
        
        public List<Color32[]> GetAllImageTiles();
        
    }

    public class TileExtractor : ITileExtractor
    {
        private readonly Texture2D _textureToTile;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        
        private readonly List<Color32[]> _uniqueTiles;

        private readonly List<Color32[]> _allImageTiles;

        private readonly List<TileMapCell> _tileMapCellList;
        
        private TileImageMap _tileImageMap;
        
        public TileExtractor(Texture2D textureToTile, int tileWidth, int tileHeight)
        {
            Debug.Log( $"Extracting Tiles from { textureToTile.name } { tileWidth } { tileHeight } " );
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
            Debug.Log( $"Extracting Tiles from { _textureToTile.name }" );
            
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
                    var tile = GetTile(texturePixels, numHorizontal, numVertical);
                    _allImageTiles.Add(tile);
                    CheckAndAddUniqueTile(tile);
                }
            }

            _tileImageMap.Columns = numHorizontalTiles;
            _tileImageMap.Rows = numVerticalTiles;
            _tileImageMap.Cells = _tileMapCellList;

            
             Debug.Log( $"Extracted { _count } unique Tiles from { _textureToTile.name }" );
            //
            // var jsonImageMap = new ImageMapBuilder()
            //     .SetUniqueTiles( _uniqueTiles )
            //     .SetAllImageTiles( _allImageTiles )
            //     .SetTileWidth(_tileWidth)
            //     .SetTileHeight(_tileHeight )
            //     .SetRows(numVerticalTiles)
            //     .SetColumns(numHorizontalTiles)
            //     .Build();
            //
             var jsonText =  JsonConvert.SerializeObject(_tileImageMap);
             Debug.Log(jsonText);
            
            JsonSaver.SaveJson(jsonText, $"{EditorPrefs.GetString("TileExtractor_SavePath")}/jsonTileMap.json");
            
            _count = 0;

            return _uniqueTiles;
        }

        private Color32[] GetTile(Color32[] texturePixels, int outerIndexX , int outerIndexY)
        {
            Color32[] tile = new Color32[_tileWidth * _tileHeight];
            
            for (int x = 0; x < _tileWidth; x++)
            {
                for (int y = 0; y < _tileHeight; y++)
                {
                    int outerX = (outerIndexX * _tileWidth) + x;
                    int outerY = (_textureToTile.height - (outerIndexY + 1) * _tileHeight) + y;
                    
                    int pixelIndex = outerY * _textureToTile.width + outerX;
                    
                    tile[y * _tileWidth + x] = texturePixels[pixelIndex];
                }
            }
            
            return tile;
        }
        
        private int _count;
        private void CheckAndAddUniqueTile(Color32[] tile )
        {
           // var tileKey = TileToKey(tile);
            
           for( int i = 0; i < _uniqueTiles.Count; i++ )
           {
               var equivalentResult = IsTileEquivalent( _uniqueTiles[i], tile, 0 );
               
               if (equivalentResult.isEquivalent)
               {
                   var tileMapCell = new TileMapCell()
                   {
                       uniqueIndex = i,
                       flipType = equivalentResult.flipType,
                   };
                   
                   _tileMapCellList.Add( tileMapCell );

                   return;
               }
           }
           
           _count++;
           
           _uniqueTiles.Add(tile);
           
           var newTileMapCell = new TileMapCell()
           {
               uniqueIndex = _uniqueTiles.Count - 1, // Index of newly added tile
               flipType = FlipType.None,             // No flip for new tile
           };
          
           _tileMapCellList.Add(newTileMapCell);
        }

        public List<Color32[]> GetAllImageTiles()
        {
            return _allImageTiles;
        }
        
        
        //Tile utility
        (bool isEquivalent, FlipType flipType) IsTileEquivalent(Color32[] newTile, Color32[] existingTile, byte tolerance = 0)
        {
            if (TilesAreEqual(newTile, existingTile, tolerance))
                return (true, FlipType.None);

            if (TilesAreEqual(TileFlipper.FlipHorizontal(newTile, _tileWidth, _tileHeight), existingTile, tolerance))
                return (true, FlipType.Horizontal);

            if (TilesAreEqual(TileFlipper.FlipVertical(newTile, _tileWidth, _tileHeight), existingTile, tolerance))
                return (true, FlipType.Vertical);

            if (TilesAreEqual(TileFlipper.FlipBoth(newTile, _tileWidth, _tileHeight), existingTile, tolerance))
                return (true, FlipType.Both);

            return (false, FlipType.None);
        }
        
        
        // bool IsTileEquivalent(Color32[] newTile, Color32[] existingTile, byte tolerance = 0)
        // {
        //     return TilesAreEqual(newTile, existingTile, tolerance)
        //     || TilesAreEqual(TileFlipper.FlipHorizontal(newTile, _tileWidth, _tileHeight), existingTile, tolerance) 
        //     || TilesAreEqual(TileFlipper.FlipVertical(newTile, _tileWidth, _tileHeight), existingTile, tolerance) 
        //     || TilesAreEqual(TileFlipper.FlipBoth(newTile, _tileWidth, _tileHeight),existingTile, tolerance);
        // }
        
        //Tile Utility
        private bool TilesAreEqual(Color32[] tileA, Color32[] tileB, byte tolerance)
        {
            for (int i = 0; i < tileA.Length; i++)
            {
                // Ignore fully transparent pixels
                if (tileA[i].a == 0 && tileB[i].a == 0)
                   continue;

                if (Mathf.Abs(tileA[i].r - tileB[i].r) > tolerance ||
                    Mathf.Abs(tileA[i].g - tileB[i].g) > tolerance ||
                    Mathf.Abs(tileA[i].b - tileB[i].b) > tolerance ||
                    Mathf.Abs(tileA[i].a - tileB[i].a) > tolerance)
                    return false;
            }
            return true;
        }
    }
    
    [Serializable]
    public class TileMapCell
    {
        public int uniqueIndex;
        public FlipType flipType;
    }

    public class TileImageMap
    {
        public int Columns;
        public int Rows;
        public List<TileMapCell> Cells;
    }
    
    public enum FlipType
    {
        None,
        Horizontal,
        Vertical,
        Both
    }
}