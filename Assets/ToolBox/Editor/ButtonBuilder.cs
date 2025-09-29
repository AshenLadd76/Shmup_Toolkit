using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.Editor
{
    public class ButtonBuilder
    {
        private Button _button;
        private Action _onClick;
        
        public ButtonBuilder(string text, Action onClick)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Button text cannot be null or empty.", nameof(text));

            if (onClick == null)
                throw new ArgumentNullException(nameof(onClick), "Click action cannot be null");

            // Create the button
            _button = new Button(onClick) { text = text };
        }
        
        public ButtonBuilder Width(int w) { _button.style.width = w; return this; }
        public ButtonBuilder Height(int h) { _button.style.height = h; return this; }
        public ButtonBuilder FontSize(int size) { _button.style.fontSize = size; return this; }
        public ButtonBuilder MarginTop(int m) { _button.style.marginTop = m; return this; }
        public ButtonBuilder MarginBottom(int m) { _button.style.marginBottom = m; return this; }
        public ButtonBuilder MarginLeft(int m) { _button.style.marginLeft = m; return this; }
        public ButtonBuilder MarginRight(int m) { _button.style.marginRight = m; return this; }
        public ButtonBuilder BackgroundColor(Color c) { _button.style.backgroundColor = c; return this; }
        public ButtonBuilder AlignSelf(Align align) { _button.style.alignSelf = align; return this; }


        public Button Build() => _button;
    }
}