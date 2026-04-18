using System.Collections;
using CodeBase.Services;
using ToolBox.Helpers;
using ToolBox.Messaging;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Documents.Intro
{
    public class TitleScreenDocument : IDocument
    {
        private const string TitleScreenAssetPath = "UI/TitleScreenSequenceSo";
        
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _root;
        private readonly WaitForSeconds _countdownWait;
        
        private AnimationSequenceSo _titleScreenSequenceSo;
        
        private Coroutine _startTitleScreenCoroutine;
        
        public TitleScreenDocument(ICoroutineRunner coroutineRunner, Transform root)
        {
            _coroutineRunner =  coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _root = root ?? throw new System.ArgumentNullException(nameof(root));
            
            _countdownWait = new WaitForSeconds(10f);
        }
        
        public void Build()
        {
            _titleScreenSequenceSo = Resources.Load<AnimationSequenceSo>(TitleScreenAssetPath);

            if (_titleScreenSequenceSo == null)
                Logger.LogError("Intro Asset Not Found at path: " + TitleScreenAssetPath);
        }

        public void Open() => StartTitleScreen();
        
        public void Close() => StopTitleScreenCoroutine();
        
     
        private void StartTitleScreen()
        {
            if (_startTitleScreenCoroutine != null) return;

            _startTitleScreenCoroutine = _coroutineRunner.StartCoroutine(TitleScreenCoroutine());
        }

        private void StopTitleScreenCoroutine()
        {
            if (_startTitleScreenCoroutine == null) return;
            
            _coroutineRunner.StopCoroutine(_startTitleScreenCoroutine);
            _startTitleScreenCoroutine = null;
            
            foreach (Transform child in _root)
                Object.Destroy(child.gameObject);
        }

        private IEnumerator TitleScreenCoroutine()
        {
            IAnimationSegment previousSegment = null;
            
            var currentSegment = _titleScreenSequenceSo.Segments[0];

            if (currentSegment == null)
            {
                Logger.LogError($"Segment not found in title screen.");
                yield break;
            }

            if(!CreditsCheck())
                currentSegment.PlaySegment(_coroutineRunner, _root, previousSegment);
            
            yield return _countdownWait;
            
            Logger.Log( $"Trigger title screen fade out animation......" );

            yield return _countdownWait;

            Close();
            
            MessageBus.Broadcast(nameof(DocumentServiceMessages.OnRequestOpenDocument), DocumentID.PlayerSelect);
        }

        private bool CreditsCheck()
        {
            //Check if the player has paid any cash to play this game....if not return false
            //this controls what title screen animation is played.
            return false;
        }
        private void OnAnyKeyPressed()
        {
            Logger.Log($"OnAnyKeyPressed: That means its time to stop the countdown coroutine and load the player select screen");
        }
    }
}
