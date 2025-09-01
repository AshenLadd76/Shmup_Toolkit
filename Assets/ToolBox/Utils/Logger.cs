using UnityEngine;

namespace ToolBox.Utils
{
    public static class Logger
    {
        private static bool _enableLogging = true;

        public static bool EnableLogging
        {
            get => _enableLogging;
            set => _enableLogging = value;
        }

        private static readonly string DefaultContext = "General";

        public static void Log<T>(T message)
        {
            if (!_enableLogging) return;
            Log(DefaultContext, message);
        }

        public static void Log<T>(string context, T message)
        {
            if (!_enableLogging) return;
            Debug.Log($"[{context}] {message}");
        }

        public static void LogWarning<T>(T message)
        {
            LogWarning(DefaultContext, message);
        }

        public static void LogWarning<T>(string context, T message)
        {
            if (!_enableLogging) return;
            Debug.LogWarning($"[{context}] {message}");
        }

        public static void LogError<T>(T message)
        {
            LogError(DefaultContext, message);
        }

        public static void LogError<T>(string context, T message)
        {
            if (!_enableLogging) return;
            Debug.LogError($"[{context}] {message}");
        }
        
        // Custom Assert method
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Debug.LogError($"Assertion failed: {message}");
                // You can also add additional logic to stop or handle the failure, depending on the use case.
                Debug.Break(); // Optionally stop the game in the editor if the assertion fails
            }
        }
    }
}

