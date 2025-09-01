using System;
using UnityEngine;
using UnityEngine.Events;
using Logger = ToolBox.Utils.Logger;


namespace CodeBase.Tools.Grids
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private int width;
        
        [SerializeField] private int height;
        
        [SerializeField] private float cellSize;
        
        [SerializeField] private Vector2 offset;
        
        [SerializeField] private bool showDebug;
        
        [SerializeField] private Tile tilePrefab;

        private Grid2D<Tile> _grid;
        private Transform _transform;

       [SerializeField] private UnityEvent<Grid2D<Tile>> onGridCompletedEvent;

       // private Color _highlightColour;
        //private Color _normalColour;


        private void Awake()
        {
            _transform = transform;

            //_normalColour= new Color(0.1866f, 0.1866f, 0.1866f, 0.47f);
           // _highlightColour = new Color(1f, 1f, 1f, 0.47f);
        }

        private void Start()
        {
           // BuildGrid();
        }
        
        // private void Update()
        // {
        //     if (UnityEngine.Input.GetMouseButtonDown(0))
        //         GetValidMouseClickOnGrid();
        //
        //     HoverOnGrid();
        // }

        
        private void BuildGrid()
        {
            _grid = new Grid2D<Tile>( width,height, cellSize,  offset,true,
                (Grid2D<Tile> g,int x, int y) => CreateTile( x, y ));
            
            
            onGridCompletedEvent?.Invoke( _grid );
        }

        private Tile CreateTile(int x , int y)
        {
            const float cellOffset = 0.5f;
            
            var position = new Vector2(
                x * cellSize +  cellOffset,
                y * cellSize +  cellOffset);
            
            var cloneTile = Instantiate(tilePrefab, new Vector2( position.x, position.y ), Quaternion.identity);
            
            cloneTile.transform.SetParent( _transform );
            
            if( showDebug )
                cloneTile.SetText( $"{x-offset.x},{y-offset.y}" );

            return cloneTile;
        }
        
        // private void GetValidMouseClickOnGrid()
        // {
        //     var gridPosition = _grid.WorldToGridPosition( MouseUtility.GetMouseWorldPosition());
        //
        //     if (!_grid.IsWithinBounds(gridPosition)) return;
        //         
        //     Logger.Log($"{gridPosition}");
        // }
        
        
        // private Vector2Int _lastHoveredTile;
        // private Vector2Int _gridPosition;
        // private void HoverOnGrid()
        // {
        //     if (_grid == null)
        //     {
        //         Logger.LogError( $"Grid is null...");
        //         return;
        //     }
        //     
        //
        //     var worldPosition = MouseUtility.GetMouseWorldPosition();
        //
        //     _gridPosition = _grid.WorldToGridPosition(worldPosition);
        //     
        //     if (_grid.IsWithinBounds(_gridPosition))
        //     {
        //         if (_lastHoveredTile != _gridPosition)
        //         {
        //             RemoveHghLight( _lastHoveredTile );
        //             _lastHoveredTile = _gridPosition;
        //             
        //             HighLightTile(_gridPosition);
        //         }
        //     }
        //     else
        //     {
        //         RemoveHghLight(_lastHoveredTile);
        //
        //         _lastHoveredTile = _gridPosition;
        //     }
        // }

        // private void HighLightTile( Vector2Int position )
        // {
        //     var tile = GetTile(position);
        //
        //     if (!tile)
        //     {
        //         Logger.LogError( $"No tile..." );
        //         return;
        //     }
        //
        //     tile.SetTileColour(_highlightColour);
        // }

       
        //private Tile GetTile(Vector2Int position) => !_grid.IsWithinBounds(position) ? null : _grid.GetValue(position);

        private const int cellSizeInPixels = 100;
        
        public void HandleScreenSizeChange(Vector2 newSize)
        {
            Debug.Log($"Screen size changed to: {newSize.x}x{newSize.y}");
        }
        
        public int rows = 5; // Default row count
        public int columns = 5; // Default column count
        public float padding = 0.1f; // Padding between cells (world units)
        public Color gridColor = Color.white;

        
        void AdjustGridToScreen()
        {
            var mainCamera = Camera.main;
            
            // Calculate world dimensions visible through the camera
            float screenHeight = mainCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * mainCamera.aspect;

            // Adjust cell size based on screen dimensions
            float cellWidth = (screenWidth - padding * (columns - 1)) / columns;
            float cellHeight = (screenHeight - padding * (rows - 1)) / rows;

            // Use the smaller of the two to ensure a square grid
            float cellSize = Mathf.Min(cellWidth, cellHeight);

            Logger.Log( cellSize );
        }
        
    }
}
