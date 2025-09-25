using UnityEngine.UIElements;

namespace ToolBox.ChatSystem.Editor
{
    public interface INodeBehaviour
    {
        void Attach(VisualElement element);
        void Detach();
    }
}