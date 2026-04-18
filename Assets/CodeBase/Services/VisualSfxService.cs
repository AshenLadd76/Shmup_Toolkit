using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services
{
    public class VisualSfxService : BaseService
    {
        public const string OnRequestVisualSfxMessage = "OnRequestVisualSfx";
        
        protected override void SubscribeToService()
        {
            MessageBus.AddListener<string, Vector3>( OnRequestVisualSfxMessage, OnRequestExplosion );
        }

        protected override void UnsubscribeFromService()
        {
            MessageBus.RemoveListener<string, Vector3>( OnRequestVisualSfxMessage, OnRequestExplosion );
        }

        private void OnRequestExplosion(string id, Vector3 position)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.Log( $"Required id is missing." );
            }
            
            Logger.Log( $"Request for Explosion with id: {id}" );
        }
    }
}
