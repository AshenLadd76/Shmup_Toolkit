using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace ToolBox.Services.Web
{
    public interface IServerResponseHandler<T>
    {
       // void HandleResponse(UnityWebRequest request);
        IServerResponseData ResponseData { get; }
    }
    
    
    public class ServerResponseHandler<T> : IServerResponseHandler<T>
    {
        private readonly Dictionary<ServerResponse, Action<T>> _handlers = new();
        public void AddHandler(ServerResponse response, Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[response] = handler;
        }

        public bool RemoveHandler(ServerResponse response)
        {
            return _handlers.Remove(response);
        }

        public bool HandleResponse(ServerResponse response, T param)
        {
            if (_handlers.TryGetValue(response, out var handler))
            {
                handler.Invoke(param);
                return true;
            }
            return false;
        }

        public void Handle(UnityWebRequest request)
        {
            throw new NotImplementedException();
        }
        
        public IServerResponseData ResponseData { get; }
    }
}
