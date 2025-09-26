using System.IO;

namespace ToolBox.Data
{
    public static class JsonSaver
    {
        public static void SaveJson(string jsonText, string path)
        {
            File.WriteAllText(path, jsonText);
        }
    }
}
