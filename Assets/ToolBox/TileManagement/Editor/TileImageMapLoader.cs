using System;
using System.IO;
using Newtonsoft.Json;
using ToolBox.Utils;

namespace ToolBox.TileManagement.Editor
{
    public class TileImageMapLoader : ITileImageMapLoader
    {
        public TileImageMap Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.Log($"Path to json {path} is null or empty!");
                return null;
            }
            
            string json = File.ReadAllText(path);
            
            Logger.Log(path);
            
            if( string.IsNullOrEmpty(json) ) return null;
            
            Logger.Log(json);
            
            TileImageMap tileImageMap;
            
            try
            {
                tileImageMap = JsonConvert.DeserializeObject<TileImageMap>(json);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to parse JSON at {path}: {ex.Message}");
                return null;
            }
            
            return tileImageMap;
        }
    }
}