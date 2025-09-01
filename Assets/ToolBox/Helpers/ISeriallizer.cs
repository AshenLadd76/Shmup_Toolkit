using System.Collections.Generic;

namespace ToolBox.Helpers
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);

        T Deserialize<T>(string json);

        public (bool IsValid, List<string> Errors) ValidateJson(string json, string schemaJson = null);
    }
}