using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services
{
    public class ExplosionService : BaseService
    {
        public const string OnRequestExplosionMessage = "OnRequestExplosion";
        
        protected override void SubscribeToService()
        {
            MessageBus.AddListener<string, Vector3>( OnRequestExplosionMessage, OnRequestExplosion );
        }

        protected override void UnsubscribeFromService()
        {
            MessageBus.RemoveListener<string, Vector3>( OnRequestExplosionMessage, OnRequestExplosion );
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
