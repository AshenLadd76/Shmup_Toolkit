#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Data.Importer
{
    public static class CsvRowProcessor
    {
        private static int _assetIndex = 0;
        
        public static void ProcessRow(string row, List<string> headers, string basePath, Type soType)
        {
            if (string.IsNullOrWhiteSpace(row))
            {
                Logger.LogWarning("Skipped empty or whitespace row");
                return;
            }

            var rowValues = CsvParser.ParseLine(row);

            if (headers.Count != rowValues.Count)
            {
                Logger.LogWarning($"Skipped row: Column count mismatch. Headers: {headers.Count}, Row: {rowValues.Count}");
                return;
            }
            
            if (soType == null)
            {
                Logger.LogError("ScriptableObject type cannot be null.");
                return;
            }

            var soInstance = CreateInstance(soType);

            try
            {
                CsvToScriptableObjectMapper.PopulateSoFromCsvRow(soInstance, headers, rowValues);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to map CSV row to ScriptableObject: {ex.Message}");
                return;
            }

            SaveScriptableObjectAsset(soInstance, basePath);
        }
        
        
        private static ScriptableObject CreateInstance(Type soType) => ScriptableObject.CreateInstance(soType);
        
        private static void SaveScriptableObjectAsset(ScriptableObject soInstance, string path)
        {
            try
            {
                var assetPath = CreateAssetPath(path);
                AssetDatabase.CreateAsset(soInstance, assetPath);
                Logger.Log($"Created asset at: {assetPath}");
                AssetDatabase.SaveAssets();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to save ScriptableObject: {ex.Message}");
            }
        }

        private static string CreateAssetPath(string path)
        {
            var fileName = $"Row_{_assetIndex++}.asset";
            var assetFolder = Path.GetDirectoryName(path);
            var assetPath = Path.Combine(assetFolder, fileName).Replace("\\", "/");
            
            return assetPath;
        }
    }
}
#endif

