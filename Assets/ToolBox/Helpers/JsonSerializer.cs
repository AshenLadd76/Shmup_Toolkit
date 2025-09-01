using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Logger = ToolBox.Utils.Logger;



namespace ToolBox.Helpers
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Serialization failed: {ex.Message}");
                return null; // or "{}" or some safe fallback
            }
        }

        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Deserialize failed: {ex.Message}");
                return default;
            }
        }

        public (bool IsValid, List<string> Errors) ValidateJson(string json, string schemaJson = null)
        {
            throw new NotImplementedException();
        }
    }
}