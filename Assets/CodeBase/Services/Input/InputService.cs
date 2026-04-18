using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;

namespace CodeBase.Services.Input
{
    public class InputService : BaseService
    {
        
        public const string OnRequestInputService = "OnRequestInputService";
        
        protected override void SubscribeToService() { }

        protected override void UnsubscribeFromService() { }


        private void Update()
        {
           // if(UnityEngine.Input.GetKeyDown( KeyCode.Space ) ) MessageBus.Broadcast(nameof(DocumentServiceMessages.OnRequestOpenDocument), DocumentID.Splash );
        }
    }
}
