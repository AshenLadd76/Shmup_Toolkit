using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Logger = ToolBox.Utils.Logger;
using Object = UnityEngine.Object;

namespace ToolBox.Editor
{
    public static class DragDropHandler
    {
        public static void RegisterDropArea( VisualElement element, Action<Object[]> onObjectsDropped, Action<string> onPathsDropped = null )
        {
            if (element == null)
            {
                Logger.Log( $"Required drag and drop visual element is missing" );
                return;
            }
            
            element.RegisterCallback<DragEnterEvent>(_ => {   element.style.backgroundColor = new Color(0f, 0.5f, 1f, 0.2f); // subtle highlight
            });
            
             // Drag leaves → reset highlight
             element.RegisterCallback<DragLeaveEvent>(_ => {   element.style.backgroundColor = new Color(0f, 0.0f, 0f, 0.0f); // Transparent
             });
             
             element.RegisterCallback<DragUpdatedEvent>(evt =>
             {
                 var validDrag = false || DragAndDrop.objectReferences.Length > 0 || DragAndDrop.paths != null && DragAndDrop.paths.Length > 0;

                 if (!validDrag) return;
                 
                 DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                 evt.StopPropagation();
             });
             
             element.RegisterCallback<DragPerformEvent>(_ =>
             {
                 DragAndDrop.AcceptDrag();
                 
                 var draggedObjects = DragAndDrop.objectReferences;
                 
                 var paths = DragAndDrop.paths;
                 
                 if( Path.IsPathRooted( paths[0] ) )
                     HandleExternalDragDrop( paths, onPathsDropped );
                 else
                     HandleInternalDragDrop(draggedObjects, onObjectsDropped );
                 
                 // Reset visuals
                 element.style.backgroundColor = new Color(0f, 0f, 0f, 0f);

             } );
        }
        
        private static void HandleInternalDragDrop(Object[] objects, Action<Object[]> onObjectsDropped )
        {
            if (objects.Length > 0)
                onObjectsDropped?.Invoke( objects );
        }

        private static void HandleExternalDragDrop(string[] paths, Action<string> onPathDropped)
        {
            if (paths == null || paths.Length <= 0) return;
            
            foreach (var path in paths)
            {
                if (Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase))
                    onPathDropped?.Invoke(path); // your method that takes a string
            }
        }
    }
}
