using System.IO;
using UnityEditor;

namespace ToolBox.TileManagement.Editor
{
    public static class JsonSaver
    {
        public static void SaveJson(string jsonText, string path)
        {
            File.WriteAllText(path, jsonText);
        }
    }
}
