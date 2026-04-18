using System.Collections;
using CodeBase.Services;
using ToolBox.Helpers;
using ToolBox.Messaging;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;
using Object = UnityEngine.Object;

namespace CodeBase.Documents.Intro
{
    public class IntroDocument : IDocument
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _root;
        
        private const string IntroAssetPath = "UI/IntroAimationSequenceSo";
        
        private AnimationSequenceSo _introAnimationSequenceSo;
        
        public IntroDocument(ICoroutineRunner coroutineRunner, Transform root)
        {
            _coroutineRunner =  coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _root = root ?? throw new System.ArgumentNullException(nameof(root));
        }

        public void Build()
        {
            Logger.Log("Building Splash Section");
            
            _introAnimationSequenceSo = Resources.Load<AnimationSequenceSo>(IntroAssetPath);

            if (_introAnimationSequenceSo == null)
                Logger.LogError("Intro Asset Not Found at path: " + IntroAssetPath);
        }
        
        public void Open() => StartIntro();
        
        public void Close() => StopIntro();
        
        
        private Coroutine _startIntroCoroutine;
        private void StartIntro()
        {
            if (_startIntroCoroutine != null) return;

            _startIntroCoroutine = _coroutineRunner.StartCoroutine(IntroCoroutine());
        }

        private void StopIntro()
        {
            if (_startIntroCoroutine == null) return;
            
            _coroutineRunner.StopCoroutine(_startIntroCoroutine);
            _startIntroCoroutine = null;
            
            foreach (Transform child in _root)
                Object.Destroy(child.gameObject);
            
        }

        private IEnumerator IntroCoroutine()
        {
            IAnimationSegment previousSegment = null;

            foreach (var segment in _introAnimationSequenceSo.Segments)
            {
                yield return segment.PlaySegment(_coroutineRunner, _root, previousSegment);
                
                previousSegment = segment;
            }
            
            Logger.Log("Intro Animation Sequence Complete");
            
            Close();
            
            MessageBus.Broadcast( nameof(DocumentServiceMessages.OnRequestOpenDocument), DocumentID.TitleScreen  );
        }
    }
}