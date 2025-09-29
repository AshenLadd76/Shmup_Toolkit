using System;
using ToolBox.Editor;
using ToolBox.TileManagement.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.ChatSystem.Editor
{
    public class GuiEditor : EditorWindow
    {
        private VisualElement _root;
        
        private const int DefaultNodeWidth = 250;
        private const int DefaultNodeHeight = 50;
        private const int DefaultNodePositionX = 20;
        private const int DefaultNodePositionY = 20;
        

        [MenuItem("Tools/Conversation System")]
        public static void ShowWindow()
        {
            GetWindow<GuiEditor>("Conversation Editor");
        }

        public void CreateGUI()
        {
            _root = rootVisualElement;

            _root.Add(CreateButton("Node Test", OnButtonClicked));
        }

        private void OnButtonClicked()
        {
            new NodeBuilder(_root)
                .SetSize(new Vector2(250, 50))
                .SetPosition(new Vector2(20, 20))
                .WithLabel("This is a message")
                .AddConnectorPoints()
                .Draggable()
                .Resizable()
                .Build();
        }


        private Button CreateButton(string buttonText, Action clickEvent)
        {
            return new ButtonBuilder(buttonText, clickEvent)
                .Width(120).Height(30).FontSize(16).MarginBottom(10).Build();
        }
        
    }

    public class BoxConnection
    {
        
        public VisualElement fromConnector;
        public VisualElement toConnector;

        public BoxConnection(VisualElement from, VisualElement to)
        {
            fromConnector = from;
            toConnector = to;
        }
    }
}
