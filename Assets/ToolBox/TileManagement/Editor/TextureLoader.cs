using System.IO;
using UnityEngine;

namespace ToolBox.TileManagement.Editor
{
    public static class TextureLoader
    {
        public static Texture2D LoadTextureFromFile(string path)
        {
            Debug.Log( "Loading " + path );
            
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;
            
            byte[] fileData = File.ReadAllBytes(path);
            
            Texture2D tex = new Texture2D(2, 2);
            
            return tex.LoadImage(fileData) ? tex : null;
        }

        public static Texture2D CreateTexture(int width, int height, int tileWidth, int tileHeight)
        {
            Texture2D tex = new Texture2D(width, height);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = ((x / tileWidth + y / tileHeight) % 2 == 0) ? Color.gray : Color.white;
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }
    }
}