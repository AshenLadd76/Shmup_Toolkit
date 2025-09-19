using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.TileManagement.Editor
{
    public  class FileDialogHelper 
    {
        
        private const string SavePathKey = "TileExtractor_SavePath";
        private const string DefaultSavePath = "Assets/Sprites/TileExtractor/Exports";
        private readonly TextField _savePathField;


        public FileDialogHelper(TextField savePathField = null)
        {
            _savePathField = savePathField;
        }
        
        public  string GetSavePathInProject(string defaultName = "" , string defaultPath = "" , string defaultExt = "")
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save TileMap JSON",  // dialog title
                defaultName,          // default file name
                "json",               // extension
                "Choose a location to save your file" // help message
            );

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Save cancelled by user.");
                return null;
            }

            return path;
        }

        public void SetSavePathInProject()
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Output Directory", "Assets", "");
            
            SavePath(DefaultSavePath);
            
            if (!string.IsNullOrEmpty(newPath))
            {
                // Convert absolute path to relative project path if possible
                if (newPath.StartsWith(Application.dataPath))
                {
                    newPath = "Assets" + newPath.Substring(Application.dataPath.Length);
                }

                SavePath(newPath);
            }
            
        }

        private void SavePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            
            EditorPrefs.SetString(SavePathKey, path);
            _savePathField.value = path;
        }
    }
}
