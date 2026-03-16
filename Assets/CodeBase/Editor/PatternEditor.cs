using CodeBase.Patterns;
using CodeBase.Patterns.CirclePattern;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor
{
    [CustomEditor(typeof(PatternController))]
    public class PatternEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Draws normal fields
            
            EditorGUILayout.Space();
            // Add a header
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            // Add space
            EditorGUILayout.Space();

            var myTarget = (PatternController)target;

            if (GUILayout.Button("Start Pattern Generation"))
                myTarget.StartPattern(); // Call your method
            

            if (GUILayout.Button("Stop Pattern Generation"))
                myTarget.StopPattern();
            
        }
    }
}