using System;
using UnityEngine.UIElements;

namespace ToolBox.Editor
{
    public class CheckBoxBuilder
    {
        private Toggle _toggle;

        public CheckBoxBuilder(string label, bool defaultValue = false)
        {
            _toggle = new Toggle(label)
            {
                value = defaultValue
            };
        }

        public CheckBoxBuilder OnValueChanged(Action<bool> callback)
        {
            _toggle.RegisterValueChangedCallback(evt => callback(evt.newValue));
            return this;
        }

        public CheckBoxBuilder Width(int w) { _toggle.style.width = w; return this; }
        public CheckBoxBuilder Height(int h) { _toggle.style.height = h; return this; return this; }
        
        public Toggle Build() => _toggle;
    }
}

