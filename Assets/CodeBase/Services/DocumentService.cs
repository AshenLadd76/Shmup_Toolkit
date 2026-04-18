using System;
using System.Collections.Generic;
using CodeBase.Documents.Intro;
using CodeBase.GameSections.Intro;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using CoroutineRunner = ToolBox.Helpers.CoroutineRunner;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services
{
    public class DocumentService: BaseService
    {
        [SerializeField] private Transform root;
        
        private readonly Dictionary<DocumentID, Func<IDocument>> _documents = new();

        private readonly Dictionary<DocumentID, IDocument> _cachedDocuments = new();
        
        private readonly Dictionary<DocumentID, IDocument> _openedDocuments = new();

        private ICoroutineRunner _coroutineRunner;

        
        protected override void SubscribeToService()
        {
            MessageBus.AddListener<DocumentID>( nameof(DocumentServiceMessages.OnRequestOpenDocument), OnRequestOpenDocument);
        }

        protected override void UnsubscribeFromService()
        {
            MessageBus.RemoveListener<DocumentID>( nameof(DocumentServiceMessages.OnRequestOpenDocument), OnRequestOpenDocument);
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _coroutineRunner = new CoroutineRunner(this);
            
            _documents[DocumentID.Splash] = () => new IntroDocument(_coroutineRunner , root);
            _documents[DocumentID.PlayerSelect] = () => new PlayerSelectDocument(_coroutineRunner, root);
            _documents[DocumentID.TitleScreen] = () => new TitleScreenDocument(_coroutineRunner, root);
        }

        private void OnRequestOpenDocument(DocumentID documentID)
        {
            if (_cachedDocuments.TryGetValue(documentID, out var activeDocument))
            {
                _cachedDocuments[documentID].Open();
                return;
            }
            
            if (!_documents.TryGetValue(documentID, out Func<IDocument> documentFunc))
            {
                Logger.LogError($"Document {documentID} not found");
                return;
            }
            
            IDocument document = documentFunc();
            
            if (document == null)
            {
                Logger.LogError($"Document {documentID} factory returned null.");
                return;
            }
            
            _openedDocuments[documentID] = document;
            
            document.Build();
            document.Open();
            
            if (document.ShouldCache)
            {
                Logger.Log($"Document {documentID} cached");
                _cachedDocuments[documentID] = document;
            }
        }

        private void OnRequestCloseDocument(DocumentID documentID)
        {
            if (_cachedDocuments.TryGetValue(documentID, out var cachedDocumentToClose))
            {
                cachedDocumentToClose.Close();
                return;
            }
            
            if (_openedDocuments.TryGetValue(documentID, out var documentToClose))
            {
                documentToClose.Close();
                _openedDocuments.Remove(documentID);
                
                if( documentToClose.ShouldCache )
                    _cachedDocuments.Remove(documentID);
                
                return;
            }
        }
    }

    public interface IDocument
    {
        bool ShouldCache => false;
        
        void Build();
        void Open();
        void Close();
    }
    
    public enum DocumentID
    { 
        Splash,
        PlayerSelect,
        TitleScreen
    }
    
    public enum DocumentServiceMessages
    {
        OnRequestOpenDocument,
        OnRequestCloseDocument
    }
}
