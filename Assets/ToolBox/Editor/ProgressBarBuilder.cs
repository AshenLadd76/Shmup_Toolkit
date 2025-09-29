using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBox.Editor
{
    public class ProgressBarBuilder
    {
        private readonly ProgressBar _progressBar;
        
        private float _progress;

        public ProgressBarBuilder(string title = "")
        {
            _progressBar = new ProgressBar
            {
                title = title,
                lowValue = 0,
                highValue = 1,
                value = 0
            };
        }

        // ----------- Styling options -------------
        public ProgressBarBuilder Width(float w) { _progressBar.style.width = w; return this; }
        public ProgressBarBuilder Height(float h) { _progressBar.style.height = h; return this; }
        public ProgressBarBuilder BackgroundColor(Color c) { _progressBar.style.backgroundColor = c; return this; }
        public ProgressBarBuilder ProgressColor(Color c) 
        {
            // Child is .unity-progress-bar__progress
            var child = _progressBar.Q<VisualElement>("unity-progress-bar__progress");
            if (child != null) child.style.backgroundColor = c;
            return this;
        }
        public ProgressBarBuilder AlignSelf(Align align) { _progressBar.style.alignSelf = align; return this; }

        public ProgressBarBuilder Margin(float all)
        {
            _progressBar.style.marginTop = all; 
            _progressBar.style.marginBottom = all;
            _progressBar.style.marginLeft = all;
            _progressBar.style.marginRight = all;
            
            return this; 
            
        }

        // ----------- Value options -------------
        public ProgressBarBuilder Range(float min, float max) { _progressBar.lowValue = min; _progressBar.highValue = max; return this; }
        public ProgressBarBuilder Value(float v) { _progressBar.value = Mathf.Clamp(v, _progressBar.lowValue, _progressBar.highValue); return this; }
        public ProgressBarBuilder Title(string title) { _progressBar.title = title; return this; }

        // ----------- Build ----------
        public ProgressBar Build() => _progressBar;

        public ProgressBarBuilder SubscribeToEditorApplicationUpdate()
        {
            EditorApplication.update += UpdateProgress;
            return this;
        }

        public ProgressBarBuilder UnSubscribeFromEditorApplicationUpdate()
        {
            EditorApplication.update -= UpdateProgress;
            return this;
        }

        private  void UpdateProgress()
        {
            _progress += 0.01f;
            _progressBar.value = _progress;

            if (_progress >= 1f)
            {
                _progressBar.title = "Done!";
                EditorApplication.update -= UpdateProgress;
            }
        }
    }
}

