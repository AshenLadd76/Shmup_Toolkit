using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Editor
{
    public class PopUpBuilder<T>
    {
        private string _label = "";
        private Vector2 _size = new Vector2(150, 20);
        private List<T> _choices = new List<T>();
        private Func<T> _getter;
        private Action<T> _setter;
        
        private Func<T, string> _formatSelectedValue;

        private T _defaultValue ;

        private Action<T> _onValueChanged;
        
        
        public PopUpBuilder<T> Label(string label)
        {
            _label = label;
            return this;
        }

        public PopUpBuilder<T> Size(float width, float height)
        {
            _size = new Vector2(width, height);
            return this;
        }

        public PopUpBuilder<T> Choices(List<T> choices)
        {
            _choices = choices;
            return this;
        }

        public PopUpBuilder<T> DefaultValue(T defaultValue)
        {
            _defaultValue = defaultValue;
            return this;
        }
        
        public PopUpBuilder<T> WithFormatter(Func<T, string> formatter)
        {
            _formatSelectedValue = formatter;
            return this;
        }
        
        public PopUpBuilder<T> OnValueChanged(Action<T> callback)
        {
            _onValueChanged = callback;
            return this;
        }
        
        public PopupField<T> Build()
        {
            var popup = new PopupField<T>(_label, _choices, _defaultValue, _formatSelectedValue)
            {
                style =
                {
                    width = _size.x,
                    height = _size.y,
                    marginBottom = 10
                }
            };
            
            if (_onValueChanged != null)
                popup.RegisterValueChangedCallback(evt => _onValueChanged(evt.newValue));
            
            return popup;
        }
    }
}