using ToolBox.Messaging;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services
{
    public class MessageBroadcaster : MonoBehaviour
    {
        [SerializeField] private string messageToSend;

        public void SendMessage()
        {
            if (string.IsNullOrEmpty(messageToSend))
            {
                Logger.Log("MessageBroadcaster: Message to send is null or empty.");
                return;
            }
            
            MessageBus.Broadcast(messageToSend);
        }

        public void Broadcast(string paramater)
        {
            if (string.IsNullOrEmpty(messageToSend) || string.IsNullOrEmpty(paramater))
            {
                Logger.Log("MessageBroadcaster: Message to send or parameter is null or empty.");
                return;
            }
            
            Logger.Log("MessageBroadcaster: Message to send: " + messageToSend);
            MessageBus.Broadcast(messageToSend, paramater);
        }
    }
}
