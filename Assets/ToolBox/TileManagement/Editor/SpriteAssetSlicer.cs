using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public static class SpriteAssetSlicer
    {
        public static void Slice(string atlasPath, int tileWidth, int tileHeight)
        {
            Debug.Log(atlasPath);
            
            Debug.Log($"{tileWidth} {tileHeight}");
            
            TextureImporter textureImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;

            if (textureImporter == null)
            {
                Debug.LogError("TextureImporter is not a texture importer");
                return;
            }

            textureImporter.isReadable = true;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.spritePixelsPerUnit = tileWidth;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            
            textureImporter.SaveAndReimport();

            Texture2D atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);

            if (atlas == null)
            {
                Debug.Log( $"Atlas {atlasPath} could not be loaded" );
                return;
            }

            int columns = atlas.width / tileWidth;
            int rows = atlas.height / tileHeight;
            
            Debug.Log( $"Slicer { columns } x {rows } height" );
            
            List<SpriteMetaData> metaData = new List<SpriteMetaData>();
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SpriteMetaData smd = new SpriteMetaData
                    {
                        alignment = (int)SpriteAlignment.Center,
                        pivot = new Vector2(0.5f, 0.5f),
                        rect = new Rect(
                            x * tileWidth,
                            atlas.height - (y + 1) * tileHeight, // flip Y
                            tileWidth,
                            tileHeight
                        ),
                        name = $"tile_{y}_{x}"
                    };
                    metaData.Add(smd);
                }
            }
            
            textureImporter.spritesheet = metaData.ToArray();
            textureImporter.spritePixelsPerUnit = tileWidth;
            EditorUtility.SetDirty(textureImporter);
            textureImporter.SaveAndReimport();
        }
    }
}
