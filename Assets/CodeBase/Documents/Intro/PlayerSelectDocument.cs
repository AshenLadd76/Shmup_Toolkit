using CodeBase.Services;
using ToolBox.Helpers;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;


namespace CodeBase.GameSections.Intro
{
    public class PlayerSelectDocument : IDocument
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _root;
        
        public PlayerSelectDocument(ICoroutineRunner coroutineRunner, Transform root)
        {
            _coroutineRunner =  coroutineRunner ?? throw new System.ArgumentNullException(nameof(coroutineRunner));
            _root = root ?? throw new System.ArgumentNullException(nameof(root));
        }
        
        public void Build()
        {
            Logger.Log( $"Building PlayerSelect document" );
        }

        public void Open()
        {
            Logger.Log( $"Opening PlayerSelect document" );
        }

        public void Close()
        {
            Logger.Log( $"Closing PlayerSelect document" );
        }
    }
}
