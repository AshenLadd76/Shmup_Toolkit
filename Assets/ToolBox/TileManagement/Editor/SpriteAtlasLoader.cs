using System;
using System.Linq;
using ToolBox.Extensions;
using UnityEditor;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.TileManagement.Editor
{
    public static class SpriteAtlasLoader
    {
        public static Sprite[] LoadSpriteAssets(string atlasPath)
        {
            var emptyArray = Array.Empty<Sprite>();
            
            if (string.IsNullOrEmpty(atlasPath))
            {
                Logger.Log( $"Path to atlas {atlasPath} is null or empty!" );
                return emptyArray;
            }
            
            var assetArray = AssetDatabase.LoadAllAssetsAtPath(atlasPath).OfType<Sprite>().ToArray();

            if (!assetArray.IsNullOrEmpty()) return assetArray;

            Logger.Log($"Failed to load assets from atlas {atlasPath}!");

            return emptyArray;
        }
    }
}