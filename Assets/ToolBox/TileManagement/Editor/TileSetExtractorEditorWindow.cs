using System;
using System.IO;
using ToolBox.Extensions;
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
        private const int MinToleranceSize = 0;
        private const int MaxToleranceSize = 255;
        
        private int _tileWidth = 16;
        private int _tileHeight = 16;
        private int _tolerance = 0;
        
        private Texture2D _loadedTexture;
        private Image _texturePreview;
        
        private Toggle includeFlipped;
        private Toggle useTolerance;
        private Toggle autoSlice;
        private Toggle generateJSON;
        private Toggle generateTileMapAsset;

        private string _currentTileMapName;
        

        [MenuItem("Tools/Tile Set Extractor")]
        public static void ShowWindow()
        {
            GetWindow<TileSetExtractor>( "TileSet Extractor" );
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Make root a flex column and center everything
            root.style.flexDirection = FlexDirection.Column;
            root.style.justifyContent = Justify.FlexStart;   // vertical centering
            root.style.alignItems = Align.Center;         // horizontal centering
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
            
            AddEditorFields(root);
            
            CreatePreviewTexture(root); 
            
            CreateDragDropOverlay(root);

            var spacer = new VisualElement();
           
            spacer.style.flexGrow = 1;  // takes all remaining vertical space
            
            root.Add(spacer);
            
            CreateOutPutTextField(root);
            
            root.Add(spacer);
            
            HandleButtonPositioning(root);
        }


        private TextField _savePathTextField;

        private void CreateOutPutTextField(VisualElement root)
        {
            _savePathTextField = new TextField("Save Path")
            {
                value = "Assets/Sprites/TileMaps",
                isReadOnly = false, // prevents editing, still selectable
                style =
                {
                    flexGrow = 1, // optional: makes it expand to fill row
                    minWidth = 800,  // pixels
                    minHeight = 1,
                    height = 5,
                }
            };

            rootVisualElement.Add(_savePathTextField);
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
            root.Add( CreateIntegerField( "Tile Width" , _tileWidth, value =>
            {
                _tileWidth = Mathf.Clamp(value, MinTileSize, MaxTileSize);
            }));
            
            root.Add( CreateIntegerField( "Tile Height" , _tileHeight, value =>
            {
                _tileHeight = Mathf.Clamp(value, MinTileSize, MaxTileSize);
            }));
            
            root.Add( CreateIntegerField( "Tolerance" , _tolerance, value =>
            {
                _tolerance = Mathf.Clamp(value, MinToleranceSize, MaxToleranceSize);
            }));
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
            row.Add(CreateButton( "Extract Tiles", ExtractUniqueTiles) );
            row.Add( CreateButton( "Set OutPut Path" , SetOutPutPath ) );
           
            root.Add(row);
        }

        private Button CreateButton( string buttonText, Action clickEvent )
        {
            return new ButtonBuilder(buttonText, clickEvent)
                .Width(ButtonWidth).Height(ButtonHeight).FontSize(ButtonFontSize).MarginBottom(ButtonMarginBottom).Build();
        }
        
        private IntegerField CreateIntegerField(string label, int initialValue, Action<int> onChanged)
        {
            if (string.IsNullOrEmpty(label)) return null;
            
            var integerField = new IntegerField(label) { value = initialValue };
            
            integerField.RegisterValueChangedCallback(evt =>
            {
                onChanged(evt.newValue);
            });
            
            return integerField;
        }
        
        private void DragAndDropImageAction(string path)
        {
            Logger.Log( $"Path: { path }");
            LoadTexture(path);
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
        
        private void ExtractUniqueTiles()
        {
            if (_loadedTexture == null)
            {
                Debug.LogWarning("No texture loaded!");
                return;
            }
            
            var filename = $"{_currentTileMapName}_{ DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
            
            ITileExtractor tileExtractor = new TileExtractor( _loadedTexture, _tileWidth, _tileHeight, filename );
            
            var uniqueTilesList = tileExtractor.ExtractTiles();

            if (uniqueTilesList.IsNullOrEmpty())
            {
                Debug.LogError($"No unique tiles found! probably due to the image size.... Aborting extraction.");
                return;
            }

            //Build the tile sheet from the extracted tiles
            
            Debug.Log( $"{_loadedTexture.width} {_loadedTexture.height}" );
            
            int columns = Mathf.CeilToInt(Mathf.Sqrt(uniqueTilesList.Count)); // square-ish
            int rows = Mathf.CeilToInt((float)uniqueTilesList.Count / columns);

            var tileSet = new TileSetBuilder(uniqueTilesList, _tileWidth, _tileHeight).SetColumnCount(columns).SetRowCount(rows).Build();
            
            _texturePreview.image = tileSet;
            
            SaveAsPng(tileSet, filename);
            
            SpriteAssetSlicer.Slice( $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png", 16,16);
            
            TileMapBuilder.BuildTileMap($"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.json", $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png", filename);
        }

        private void SaveAsPng(Texture2D texture, string filename)
        {
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes( $"{EditorPrefs.GetString("TileExtractor_SavePath")}/{filename}.png", pngData);
            
            AssetDatabase.Refresh();
        }

        private void SaveJsonMapFile(string jsonMap)
        {
            File.WriteAllText($"{EditorPrefs.GetString("TileExtractor_SavePath")}/jsonMap.json", jsonMap);
            
            AssetDatabase.Refresh();
        }

        private void SetOutPutPath()
        {
            var fileDialogHelper = new FileDialogHelper(_savePathTextField);
            
            fileDialogHelper.SetSavePathInProject();
        }

        private void AddOptions(VisualElement root)
        {
            // Add checkboxes
            includeFlipped = new Toggle("Include Flipped Tiles") { value = false };
            useTolerance = new Toggle("Use Tolerance") { value = false };
            autoSlice = new Toggle("Auto Slice After Save") { value = false };
            generateJSON = new Toggle("Generate JSON Map") { value = false };
            generateTileMapAsset = new Toggle("Generate Tile Map Asset") { value = false };
           
            root.Add(includeFlipped);
            root.Add(useTolerance);
            root.Add(autoSlice);
            root.Add(generateJSON);
            root.Add(generateTileMapAsset);
        }
        
        private bool IsTileEmpty(Color32[] tile, byte alphaThreshold = 10)
        {
            foreach (var c in tile)
            {
                Debug.Log( $"{c.a}" );
                if (c.a > alphaThreshold) return false; // consider pixel “visible” only if alpha above threshold
                return true;
            }

            return false;
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

