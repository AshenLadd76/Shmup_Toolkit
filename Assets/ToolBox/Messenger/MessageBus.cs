using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Messaging
{
    
    /// <summary>
    /// A centralized message hub for broadcasting and subscribing to events
    /// using strongly-typed delegates. Supports zero to three parameters per event.
    /// 
    /// Example usage:
    /// <code>
    /// MessagBus.AddListener("OnPlayerDeath", OnPlayerDeath);
    /// MessageBus.RemoveListener("OnPlayerDeath", OnPlayerDeath);
    /// 
    /// MessageBus.Broadcast("OnPlayerDeath");
    /// </code>
    /// </summary>
    /// <remarks>
    /// Ensures a single instance across the project. All event listeners are cleared when the asset is disabled.
    /// </remarks>
    
    public static class MessageBus
    {
        private static readonly Dictionary<string, Delegate> ListenersDictionary;
        
        public static bool EnableTracing = true;
        
        static MessageBus()
        {
            ListenersDictionary = new Dictionary<string, Delegate>();
       
        }
        
        //Fire and forget
        public static void AddListener(string eventName, Action callBack) => AddInternal(eventName, callBack);
        public static void AddListener<T1>(string eventName, Action<T1> callBack) => AddInternal(eventName, callBack);
        public static void AddListener<T1,T2>(string eventName, Action<T1,T2> callBack) => AddInternal(eventName, callBack);
        public static void AddListener<T1,T2,T3>(string eventName, Action<T1,T2,T3> callBack) => AddInternal(eventName, callBack);
        
        //Fire and forget
        public static void RemoveListener(string eventName, Action callback) => RemoveInternal(eventName, callback);
        public static void RemoveListener<T1>(string eventName, Action<T1> callback) => RemoveInternal(eventName, callback);
        public static void RemoveListener<T1,T2>(string eventName, Action<T1,T2> callback) => RemoveInternal(eventName, callback);
        public static void RemoveListener<T1,T2,T3>(string eventName, Action<T1,T2,T3> callback) => RemoveInternal(eventName, callback);
        
        
        //Use for fire and forget
        private static void AddInternal(string eventName, Delegate callback)
        {
            if( EnableTracing ) Debug.Log( $"Adding event {eventName}" );
            
            if (string.IsNullOrWhiteSpace(eventName) || callback == null)
            {
                Debug.LogError("[MessageHub] Invalid event name or callback.");
                return;
            }

            if (ListenersDictionary.TryGetValue(eventName, out var existingDelegate))
            {
                if (existingDelegate.GetType() != callback.GetType())
                {
                    Debug.LogWarning($"[MessageHub] Listener type mismatch for event '{eventName}'. Expected {existingDelegate.GetType().Name}, got {callback.GetType().Name}.");
                    return;
                }
                
                // Optional: prevent duplicate subscriptions
                foreach (Delegate existing in existingDelegate.GetInvocationList())
                {
                    if (existing == callback)
                    {
                        Debug.LogWarning($"[MessageHub] Listener already subscribed for event '{eventName}'. Not adding again.");
                        return;
                    }
                }
                
                ListenersDictionary[eventName] = Delegate.Combine(existingDelegate, callback);
            }
            else
            {
                ListenersDictionary[eventName] = callback;
            }
        }
        
        private static void RemoveInternal(string eventName, Delegate callback)
        {
            if( EnableTracing ) Debug.Log( $"Removing event {eventName}" );
            
            if (string.IsNullOrWhiteSpace(eventName) || callback == null)
            {
                Debug.LogError("[MessageHub] Invalid event name or callback.");
                return;
            }

            if (ListenersDictionary.TryGetValue(eventName, out var existingDelegate))
            {
                if (existingDelegate.GetType() != callback.GetType())
                {
                    Debug.LogWarning($"[MessageHub] Listener type mismatch for event '{eventName}'. Expected {existingDelegate.GetType().Name}, got {callback.GetType().Name}.");
                    return;
                }

                var newDelegate = Delegate.Remove(existingDelegate, callback);
                if (newDelegate == null)
                    ListenersDictionary.Remove(eventName);
                else
                    ListenersDictionary[eventName] = newDelegate;
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No listeners found for event '{eventName}' to remove.");
            }
        }
        
        public static void Broadcast(string eventName, object sender = null)
        {
            if (ListenersDictionary.TryGetValue(eventName, out var del) && del is Action action)
            {
                Trace( eventName, sender );
                
                action.Invoke();
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching zero-parameter listener found for event '{eventName}'.");
            }
        }

        
        public static void Broadcast<T1>(string eventName, T1 param1, object sender = null)
        {
            if (ListenersDictionary.TryGetValue(eventName, out var del) && del is Action<T1> action)
            {
                Trace( eventName, sender, param1);
                
                action.Invoke(param1);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching one-parameter listener found for event '{eventName}'.");
            }
        }

        public static void Broadcast<T1, T2>(string eventName, T1 param1, T2 param2, object sender = null)
        {
            if (ListenersDictionary.TryGetValue(eventName, out var del) && del is Action<T1, T2> action)
            {
                Trace( eventName, sender, param1, param2);
                
                action.Invoke(param1, param2);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching two-parameter listener found for event '{eventName}'.");
            }
        }

        public static void Broadcast<T1, T2, T3>(string eventName, T1 param1, T2 param2, T3 param3, object sender = null)
        {
            if (ListenersDictionary.TryGetValue(eventName, out var del) && del is Action<T1, T2, T3> action)
            {
                Trace( eventName, sender, param1, param2, param3);
                
                action.Invoke(param1, param2, param3);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching three-parameter listener found for event '{eventName}'.");
            }
        }
        
        private static void Trace(string eventName, object sender = null, params object[] parameters)
        {
            if( !EnableTracing ) return;
            
            var paramStr = parameters is { Length: > 0 } 
                ? string.Join(", ", parameters) 
                : "No parameters";
    
            var senderStr = sender != null ? sender.ToString() : "Unknown sender";
    
            Debug.Log($"[MessageBus] Event '{eventName}' requested by {senderStr} with params: {paramStr}");
        }
        
        public static void ClearListeners() => ListenersDictionary.Clear();
    }
}
