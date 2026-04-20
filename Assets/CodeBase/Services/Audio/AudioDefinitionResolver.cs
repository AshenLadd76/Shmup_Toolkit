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
            
            return dictionary.TryGetValue(key, out audioDefinition);
        }
        
        public static bool TryResolveAudio(string id, Dictionary<string, IAudioDefinition> dictionary, out IAudioDefinition audioDefinition)
        {
            audioDefinition = null;
            
            if (!TryCreateKey(id, out var key)) return false;

            if (TryGetAudioDefinition(key, dictionary, out audioDefinition)) return true;
            
            Logger.LogError( $"Failed to resolve audio definition for {id}" );
            
            return false;
        }
    }
}