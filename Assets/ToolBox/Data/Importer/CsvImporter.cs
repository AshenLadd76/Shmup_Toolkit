#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Data.Importer
{
    public class CsvImporter : AssetPostprocessor
    {
        private const string CsvFileExtensions = ".csv";
        private static string _fullPath;
        
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                if (assetPath.EndsWith(CsvFileExtensions))
                    ImportCsv(assetPath);
            }
        }

        private static void ImportCsv(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.Log($"Path not found");
                return;
            }
            
            _fullPath = GetFullPath(path);
            
            
            if (!File.Exists(_fullPath))
            {
                Logger.Log( $"File does not exist" );
                return;
            }
            
            using (var reader = new StreamReader(_fullPath))
            {
                string line;

                string headerLine = reader.ReadLine();
                
                if (string.IsNullOrEmpty(headerLine)) 
                {
                    Logger.LogWarning("CSV header is empty or missing.");
                    return;
                }
                
                var headers = CsvParser.ParseLine(headerLine);

                try
                {
                    while ((line = reader.ReadLine()) != null)
                        CsvRowProcessor.ProcessRow(line, headers, path, typeof(PlayerData));
                    
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception occurred while processing CSV rows: {ex.Message}\n{ex.StackTrace}");
                }
            }
            
            DeleteFile(_fullPath);
        }
        
        private static string GetFullPath(string path) => Path.Combine(Application.dataPath.Replace("Assets", ""), path);

        
        private static void DeleteFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                Logger.LogWarning($"No file found at {pathToFile}.");
                return;
            }
            
            File.Delete( pathToFile );
        }
    }
}
#endif
