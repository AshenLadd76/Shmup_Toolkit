using System;
using System.Collections.Generic;
using System.IO;
using ToolBox.Editor;
using ToolBox.Extensions;
using ToolBox.TileManagement.TileExtraction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.TileManagement.Editor
{
    public class TileSetExtractor : EditorWindow
    {
        private const int PreviewWidth = 512;
        private const int PreviewHeight = 512;
        private const int ButtonWidth = 300;
        private const int ButtonHeight = 40;
        private const int ButtonFontSize = 18;
        private const int ButtonMarginBottom = 10;
        private const int PreviewMarginTop = 20;
        private const int MinTileSize = 8;
        private const int MaxTileSize = 1024;

        private int _tileMapChunkSizeX = 128;
        private int _tileMapChunkSizeY = 128;
        
        private int _tileWidth = 16;
        private int _tileHeight = 16;
        private int _tolerance;
        
        private Texture2D _loadedTexture;
        private Image _texturePreview;
        
        private string _currentTileMapName;
        
        private const string DefaultSavePath = "Assets/Sprites/TileMaps";

        private const int DefaultTextFieldWidth = 800;
        private const int DefaultTextFieldHeight = 20;
        
        private TextField _savePathTextField;
        
        private bool _addCollidersToTileMap = false;
        
        [MenuItem("Tools/Tile Set Extractor")]
        public static void ShowWindow() => GetWindow<TileSetExtractor>( "TileSet Extractor" );
        
        
        private void SetupRootStyle(VisualElement root)
        {
            // Make root a flex column and center everything
            root.style.flexDirection = FlexDirection.Column;
            root.style.justifyContent = Justify.FlexStart;   // vertical centering
            root.style.alignItems = Align.Center;         // horizontal centering
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            
            SetupRootStyle(root);
            
            AddEditorFields(root);
            
            AddProgressBar(root);
            
            CreatePreviewTexture(root); 
            
            AddChunkDropdown(root, val => { _tileMapChunkSizeX = val;  });
            AddChunkDropdown( root, val => { _tileMapChunkSizeY = val;  } );
            
            CreateAddCollidersCheckbox(root);
            
            var spacer = new VisualElement
            {
                style =
                {
                    flexGrow = 1 // takes all remaining vertical space
                }
            };

            root.Add(spacer);
            
            CreateOutPutTextField(root);
            
            root.Add(spacer);
            
            HandleButtonPositioning(root);
            
            
        }
        
        private void CreateOutPutTextField(VisualElement root)
        {
            _savePathTextField = new TextField("Save Path")
            {
                value = DefaultSavePath,
                isReadOnly = false, // prevents editing, still selectable
                style =
                {
                    flexGrow = 1, // optional: makes it expand to fill row
                    minWidth = DefaultTextFieldWidth,  // pixels
                    minHeight = DefaultTextFieldHeight,
                    height = 5,
                }
            };

            root.Add(_savePathTextField);
        }
        
        private void CreatePreviewTexture(VisualElement root)
        {
            _texturePreview = new Image
            {
                image = TextureLoader.CreateTexture( PreviewWidth, PreviewHeight, _tileWidth, _tileHeight ),
                scaleMode = ScaleMode.ScaleToFit,
                
                style =
                {
                    width = PreviewWidth,
                    height = PreviewHeight,
                    marginTop = PreviewMarginTop,
                    marginBottom = PreviewMarginTop,
                }
            };
            
            CreateDragDropOverlay(_texturePreview);
            
            root.Add(_texturePreview);
        }
        
        private void AddEditorFields(VisualElement root)
        {
            root.Add( CreateIntegerField( "Tile Width" , _tileWidth, MinTileSize, MaxTileSize,  value => { _tileWidth = value; }));
            root.Add( CreateIntegerField( "Tile Height" , _tileHeight, MinTileSize, MaxTileSize, value => { _tileHeight = value; }));
            root.Add( CreateIntegerField( "Tolerance" , _tolerance, MinTileSize, MaxTileSize, value => { _tolerance = value; }));
        }

        private void AddChunkDropdown(VisualElement root, Action<int> action)
        {
            var options = new List<int> { 128, 256, 512, 1024 };
            int defaultOption = 128;

            var popup = new PopUpBuilder<int>()
                .Label("Select Tile Map Chunk Size")
                .Choices(options)
                .DefaultValue(defaultOption)
                .Size(400,20)
                .OnValueChanged( action )
                .Build();
            
                root.Add(popup);
        }

       


        private void HandleButtonPositioning(VisualElement root)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row, // horizontal layout
                    flexWrap = Wrap.Wrap, // allow wrapping when space runs out
                    justifyContent = Justify.FlexStart
                }
            };

            row.Add( CreateButton( "Select Image", SelectImageAction) );
            row.Add(CreateButton( "Extract Tiles", RunTileExtractionPipeline) );
            row.Add( CreateButton( "Set OutPut Path" , SetOutPutPath ) );
           
            root.Add(row);
        }

        private void CreateAddCollidersCheckbox(VisualElement root)
        {
            var collisionToggle = new CheckBoxBuilder( "Add Colliders", false )
                .OnValueChanged( SetTileMapCollisions )
                .Build();
            
            root.Add( collisionToggle );
        }

        private void SetTileMapCollisions(bool b)
        {
            Logger.Log( $"SetTileMapCollisions {b}" );
            _addCollidersToTileMap = b;
        }

        private Button CreateButton( string buttonText, Action clickEvent )
        {
            return new ButtonBuilder(buttonText, clickEvent)
                .Width(ButtonWidth).Height(ButtonHeight).FontSize(ButtonFontSize).MarginBottom(ButtonMarginBottom).Build();
        }
        
        private IntegerField CreateIntegerField(string label, int initialValue, int min, int max, Action<int> onChanged)
        {
            if (string.IsNullOrEmpty(label)) return null;
            
            var integerField = new IntegerField(label) { value = initialValue };
            
            integerField.RegisterValueChangedCallback(evt => { onChanged(Mathf.Clamp(evt.newValue, min, max)); });
            
            return integerField;
        }
        
        private void DragAndDropImageAction(string path)
        {
            LoadTexture(path);
        }

        
        private void AddProgressBar(VisualElement root)
        {
            var progressBarBuilder = new ProgressBarBuilder("Building...")
                .Width(300)
                .Height(20)
                .Range(0, 1)
                .ProgressColor(Color.green);
            
            var progressBar = progressBarBuilder.Build();
            
            root.Add( progressBar );
        }
        
        
        
        private void SelectImageAction(  )
        {
            var path = EditorUtility.OpenFilePanel("Choose a png image ", "", "png");

            if (string.IsNullOrEmpty(path)) return;
                
            LoadTexture( path );
        }

        private void LoadTexture(string path)
        {
            _loadedTexture = TextureLoader.LoadTextureFromFile(path);
            _texturePreview.image = _loadedTexture;
            
            _currentTileMapName = Path.GetFileNameWithoutExtension(path);
        }
        
        private void RunTileExtractionPipeline()
        {
            if (_loadedTexture == null)
            {
                Debug.LogWarning("No texture loaded!");
                return;
            }
            
            var filename = $"{_currentTileMapName}_{ DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
            
            var uniqueTilesList = GetUniqueTilesList(filename);
            
            //Build the tile sheet from the extracted tiles
            var tileSet = BuildTileSet(uniqueTilesList);
            
            _texturePreview.image = tileSet;
            
            SaveTileSet(tileSet, filename);

            SliceTileSet(filename, _tileWidth, _tileHeight);
            
            BuildTileMap(filename);
        }

        private void BuildTileMap(string filename)
        {
            var jsonPath = $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.json";
            var atlasPath = $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png";
            
            Vector2Int chunkSize = new Vector2Int(_tileMapChunkSizeX, _tileMapChunkSizeY);

            ITileMapBuilder tileMapBuilder = new TileMapBuilder(jsonPath, atlasPath, filename, chunkSize, _addCollidersToTileMap, CompositeCollider2D.GeometryType.Outlines);
            
            tileMapBuilder.Build();
         }

        private void SliceTileSet(string filename, int tileWidth, int tileHeight)
        {
            SpriteAssetSlicer.Slice( $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png", tileWidth,tileHeight);
        }

        private Texture2D BuildTileSet(List<Color32[]> uniqueTilesList)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(uniqueTilesList.Count)); // square-ish
            int rows = Mathf.CeilToInt((float)uniqueTilesList.Count / columns);

            var tileSet = new TileSetBuilder(uniqueTilesList, _tileWidth, _tileHeight).SetColumnCount(columns).SetRowCount(rows).Build();
            
            return tileSet;
        }

        private List<Color32[]> GetUniqueTilesList(string filename)
        {
            ITileExtractor tileExtractor = new TileExtractor( _loadedTexture, _tileWidth, _tileHeight, filename );
            
            var tiles = tileExtractor.ExtractTiles();

            if (tiles.IsNullOrEmpty())
                Debug.LogError("No unique tiles found! Probably due to the image size. Aborting extraction.");
    
            return tiles ?? new List<Color32[]>();
        }

        private void SaveTileSet(Texture2D texture, string filename)
        {
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes( $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png", pngData);
            
            AssetDatabase.Refresh();
        }

        
        private void SetOutPutPath()
        {
            var fileDialogHelper = new FileDialogHelper(_savePathTextField);
            
            fileDialogHelper.SetSavePathInProject();
        }
        
        private void CreateDragDropOverlay(VisualElement visualElement)
        {
            var overlay = new VisualElement
            {
                pickingMode = PickingMode.Position,
                style =
                {
                    width = visualElement.style.width,
                    height = visualElement.style.height,
                    position = visualElement.style.position,
                    backgroundColor = new Color(0, 0, 0, 0f)
                    
                }
            };

            DragDropHandler.RegisterDropArea(overlay, obj => { Logger.Log($"Dropped {obj.Length}");

                if (obj.IsNullOrEmpty())
                {
                    Logger.Log($"No objects found!");
                    return;
                }

                DragAndDropImageAction( AssetDatabase.GetAssetPath( obj[0] ) );
            }, DragAndDropImageAction);
            visualElement.Add(overlay);
        }
    }
}

