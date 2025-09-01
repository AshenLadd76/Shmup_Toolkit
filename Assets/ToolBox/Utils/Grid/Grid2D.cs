using System;
using UnityEngine;

namespace CodeBase.Tools.Grids
{
    public class Grid2D<T>
    {
        private T[,] _gridArray;
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector2 _offset;
        private bool _showDebug;

        /// <summary>
        /// Constructor for the 2D grid.
        /// </summary>
        /// <param name="width">Width of the grid (number of columns).</param>
        /// <param name="height">Height of the grid (number of rows).</param>
        /// <param name="cellSize">Size of each cell.</param>
        /// <param name="offset">Offset for the grid origin.</param>
        /// <param name="initializeFunc">Function to initialize the cells.</param>
        public Grid2D(int width, int height, float cellSize, Vector2 offset, bool enableDebug,Func<Grid2D<T>, int, int, T> CreateGridObject)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _offset = offset;
            _showDebug = enableDebug;

            _gridArray = new T[width, height];

            // Initialize the grid with the provided function
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    var worldPos = GetWorldPosition(x, y);
                    
                    _gridArray[x, y] = CreateGridObject(this, x + (int)_offset.x, y + (int)_offset.y);
                }
            }
            
            if(_showDebug) DisplayDebug();
            
        }

        /// <summary>
        /// Gets the value of a cell at the specified grid position.
        /// </summary>
        public T GetValue(int x, int y)
        {
            if (IsWithinBounds(x, y))
            {
                return _gridArray[x, y];
            }
            else
            {
                Debug.LogWarning($"Attempted to get value outside grid boundaries at ({x}, {y}).");
                return default;
            }
        }
        
        public T GetValue(Vector2Int gridPosition)
        {
            if (IsWithinBounds(gridPosition.x, gridPosition.y))
            {
                return _gridArray[gridPosition.x, gridPosition.y];
            }
            else
            {
                Debug.LogWarning($"Attempted to get value outside grid boundaries at ({gridPosition.x}, {gridPosition.y}).");
                return default;
            }
        }
        
        

        /// <summary>
        /// Sets the value of a cell at the specified grid position.
        /// </summary>
        public void SetValue(int x, int y, T value)
        {
            if (IsWithinBounds(x, y))
            {
                _gridArray[x, y] = value;
            }
            else
            {
                Debug.LogWarning($"Attempted to set value outside grid boundaries at ({x}, {y}).");
            }
        }

        /// <summary>
        /// Checks if the specified grid coordinates are within the grid boundaries.
        /// </summary>
        /// <param name="x">X-coordinate in the grid.</param>
        /// <param name="y">Y-coordinate in the grid.</param>
        /// <returns>True if the coordinates are within the grid, otherwise false.</returns>
        private bool IsWithinBounds(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    

        /// <summary>
        /// Converts world position to grid coordinates.
        /// </summary>
        public Vector2Int WorldToGridPosition(Vector2 worldPosition)
        {
            var x = Mathf.FloorToInt((worldPosition.x - _offset.x) / _cellSize);
            var y = Mathf.FloorToInt((worldPosition.y - _offset.y) / _cellSize);
            
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Converts grid coordinates to world position.
        /// </summary>
        public Vector2 GetWorldPosition(int x, int y)
        {
            if (IsWithinBounds(x, y))
            {
                return new Vector2(x * _cellSize + _offset.x, y * _cellSize + _offset.y);
            }
            else
            {
                //Debug.LogWarning($"Attempted to get world position for out-of-bounds grid coordinates ({x}, {y}).");
                return Vector2.zero;
            }
        }

        /// <summary>
        /// Prints the grid to the console for debugging.
        /// </summary>
        public void DebugPrint()
        {
            for (int y = _height - 1; y >= 0; y--)
            {
                string row = "";
                for (int x = 0; x < _width; x++)
                {
                    row += $"{_gridArray[x, y]} ";
                }
                Debug.Log(row);
            }
        }
        
        private void DisplayDebug()
        {
            var color = Color.blue;
            
            if( _showDebug )
            {
               // _debugTextArray = new TextMesh[ _width,_height ]; 
        
                for( var x = 0; x < _gridArray.GetLength(0); x++ )
                {
                    for( var y = 0; y < _gridArray.GetLength(1); y++ )
                    {
                        Debug.DrawLine( GetWorldPosition( x, y ), GetWorldPosition( x, y + 1 ), color, 100f );
                        Debug.DrawLine( GetWorldPosition( x, y ), GetWorldPosition( x + 1, y ), color, 100f );
                    }
                }
        
                Debug.DrawLine( GetWorldPosition( 0, _height ), GetWorldPosition( _width, _height ), color, 100f );
                Debug.DrawLine( GetWorldPosition( _width, 0 ), GetWorldPosition( _width, _height ), color, 100f );
            }
        }
        
        public bool IsWithinBounds( Vector2Int position )
        {
            return (position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height);
        }
    }
}
