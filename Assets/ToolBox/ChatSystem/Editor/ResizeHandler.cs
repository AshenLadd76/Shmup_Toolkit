using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.ChatSystem.Editor
{
    public class ResizeHandler : INodeBehaviour
    {
        private VisualElement _element;
        private VisualElement _handle;
        private bool _resizing;
        private Vector2 _startMouse;
        private Vector2 _startSize;

        
        public void Attach(VisualElement element)
        {
            _element = element;

            // Create handle internally
            _handle = new VisualElement
            {
                style =
                {
                    width = 12,
                    height = 12,
                    backgroundColor = Color.green,
                    position = Position.Absolute,
                    right = 0,
                    bottom = 0,
                    cursor = new StyleCursor((StyleKeyword)MouseCursor.ResizeHorizontal)
                }
            };
            _element.Add(_handle);

            // Register callbacks
            _handle.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _handle.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _handle.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }
        
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            _resizing = true;
            _startMouse = evt.mousePosition;
            _startSize = new Vector2(_element.resolvedStyle.width, _element.resolvedStyle.height);
            _handle.CaptureMouse();
            evt.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_resizing) return;
            Vector2 delta = evt.mousePosition - _startMouse;
            _element.style.width = Mathf.Max(50, _startSize.x + delta.x);
            _element.style.height = Mathf.Max(50, _startSize.y + delta.y);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _resizing = false;
            _handle.ReleaseMouse();
        }
        

        public void Detach()
        {
            throw new System.NotImplementedException();
        }
    }
}