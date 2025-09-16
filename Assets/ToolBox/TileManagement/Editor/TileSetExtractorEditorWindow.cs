using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
            
           root.Add(_texturePreview);
           
           var spacer = new VisualElement();
           spacer.style.flexGrow = 1;  // takes all remaining vertical space
           
           root.Add(spacer);
           
           root.Add( CreateButton( "Select Image", SelectImageAction) );
           root.Add(CreateButton( "Extract Tiles", ExtractUniqueTiles) );
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
                }
            };
            
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

       
        private void SelectImageAction(  )
        {
            var path = EditorUtility.OpenFilePanel("Choose a png image ", "", "png");

            if (string.IsNullOrEmpty(path)) return;
                
            _loadedTexture = TextureLoader.LoadTextureFromFile(path);
            _texturePreview.image = _loadedTexture;
        }
        
        private void ExtractUniqueTiles()
        {
            if (_loadedTexture == null)
            {
                Debug.LogWarning("No texture loaded!");
                return;
            }
            
            ITileExtractor tileExtractor = new TileExtractor( _loadedTexture, _tileWidth, _tileHeight );
            
            tileExtractor.ExtractTiles();
        }
    }
}

