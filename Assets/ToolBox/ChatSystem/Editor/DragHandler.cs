using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.ChatSystem.Editor
{
    public class DragHandler : INodeBehaviour
    {
        private VisualElement _element;
        private bool _dragging;
        private Vector2 _offset;
        
        
        public void Attach(VisualElement element)
        {
            _element = element;
            _element.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _element.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _element.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            _dragging = true;
            _offset = evt.localMousePosition;
            _element.CaptureMouse();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_dragging) return;
            _element.style.left = evt.mousePosition.x - _offset.x;
            _element.style.top = evt.mousePosition.y - _offset.y;
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _dragging = false;
            _element.ReleaseMouse();
        }

        public void Detach()
        {
            // optionally unregister callbacks
        }
        
    }
}