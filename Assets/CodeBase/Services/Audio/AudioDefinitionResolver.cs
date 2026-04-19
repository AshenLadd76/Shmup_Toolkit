using System.Collections.Generic;
using ToolBox.Utils;

namespace CodeBase.Services.Audio
{
    public static class AudioDefinitionResolver
    {
        public static string FormatID(string id) => string.IsNullOrEmpty(id) ? string.Empty : id.Trim().ToLowerInvariant();

        private static bool TryCreateKey(string id, out string key)
        {
            key = string.Empty;

            if (string.IsNullOrWhiteSpace(id))
            {
                Logger.LogError($"Audio key is null or empty");
                return false;
            }
            
            key = FormatID(id);
            return true;
        }
        
        private static bool TryGetAudioDefinition(string key, Dictionary<string, IAudioDefinition> dictionary, out IAudioDefinition audioDefinition)
        {
            audioDefinition = null;
            
            if (dictionary == null || dictionary.Count == 0)
            {
                Logger.LogError("Audio dictionary is not initialised or empty");
                return false;
            }

            
            if (dictionary.TryGetValue(key, out audioDefinition)) 
                return true;

            Logger.LogError($"Audio not found: {key}");
            return false;
        }
        
        public static bool TryResolveAudio(
            string id,
            Dictionary<string, IAudioDefinition> dictionary,
          
            out IAudioDefinition audioDefinition)
        {
            var key = string.Empty;
            audioDefinition = null;
            
            Logger.Log( $"Audio definition for {id}" );

            if (!TryCreateKey(id, out key))
            {
                Logger.LogError($"Audio key is not defined: {id}");
                return false;
            }
            
            Logger.Log( $"Audio definition Key: {key}" );

            if (TryGetAudioDefinition(key, dictionary, out audioDefinition)) return true;
            
            Logger.LogError( $"Failed to resolve audio definition for {id}" );
            
            return false;
        }
    }
}