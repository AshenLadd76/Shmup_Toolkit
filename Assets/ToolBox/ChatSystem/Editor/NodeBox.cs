using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.ChatSystem.Editor
{
    public class NodeBuilder : VisualElement
    {
        private VisualElement _root;
        private VisualElement _box;

        private Vector2 _size = new Vector2( 250, 50 );
        private Vector2 _position = new Vector2( 100, 100 );
        private string _labelText = "??";
        
        private readonly INodeBehaviour _dragHandler;
        private readonly INodeBehaviour _resizeHandler;
        
        public NodeBuilder(VisualElement root)
        {
            _root = root;
            
            CreateNode();
            
            _dragHandler = new DragHandler();
            _resizeHandler = new ResizeHandler();
        }
        
        public NodeBuilder SetSize(Vector2 size) { _size = size; return this; }
        public NodeBuilder SetPosition(Vector2 pos) { _position = pos; return this; }
        public NodeBuilder WithLabel(string text)
        { 
            _labelText = text;
            
            // Add some label inside
            var label = new Label(_labelText)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    flexGrow = 1
                }
            };
            _box.Add(label);
            
            return this;
        }


        private void CreateNode()
        {
            var box = new VisualElement
            {
                style =
                {
                    width = _size.x,
                    height = _size.y,
                    backgroundColor = new Color(0.3f, 0.5f, 0.8f, 1f),
                    position = Position.Absolute,
                    left = _position.x,
                    top = _position.y,
                }
            };
            _box = box;
        }

       
        
        public NodeBuilder AddConnectorPoints()
        {
            float size = 10; // connector size
            Color connectorColor = Color.red;

            // Helper to create a connector
            VisualElement CreateConnector()
            {
                var connector = new VisualElement
                {
                    name = "Connector",
                    style =
                    {

                        width = size,
                        height = size,
                        backgroundColor = connectorColor,
                        position = Position.Absolute
                    }
                };
                return connector;
            }

            // Create connectors once
            var top = CreateConnector();
            var bottom = CreateConnector();
            var left = CreateConnector();
            var right = CreateConnector();

            // Top connector

            _box.Add(top);
            _box.Add(bottom);
            _box.Add(left);
            _box.Add(right);

            // Position connectors after layout (stays updated on resize)
            _box.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                float width = _box.resolvedStyle.width;
                float height = _box.resolvedStyle.height;

                top.style.left = (width - size) / 2;
                top.style.top = -size / 2;

                bottom.style.left = (width - size) / 2;
                bottom.style.top = height - size / 2;

                left.style.left = -size / 2;
                left.style.top = (height - size) / 2;

                right.style.left = width - size / 2;
                right.style.top = (height - size) / 2;
            });

            return this;
        }
        
        public NodeBuilder Draggable()
        {
            _dragHandler.Attach(_box);
            
            return this;
        }
        
        public NodeBuilder Resizable()
        {
            _resizeHandler.Attach(_box);
            return this;
        }

        public void Build() => _root.Add(_box);
    }
}
