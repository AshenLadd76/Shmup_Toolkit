using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Messenger
{
    /// <summary>
    /// A centralized ScriptableObject-based message hub for broadcasting and subscribing to events
    /// using strongly-typed delegates. Supports zero to three parameters per event.
    /// 
    /// Example usage:
    /// <code>
    /// MessagBus.Instance.AddListener("OnPlayerDeath", OnPlayerDeath);
    /// MessageBus.Instance.RemoveListener("OnPlayerDeath", OnPlayerDeath);
    /// 
    /// MessageBus.Instance.Broadcast("OnPlayerDeath");
    /// </code>
    /// </summary>
    /// <remarks>
    /// Ensures a single instance across the project. All event listeners are cleared when the asset is disabled.
    /// </remarks>
    
    
    [CreateAssetMenu(menuName = "Messaging/MessageHub")]
    public class MessageBus : ScriptableObject
    {
        private static MessageBus _instance;

        private const string MessageBusName = "MessageBus";
        
        public static MessageBus Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = Resources.Load<MessageBus>(MessageBusName);
                    
                if (_instance == null)
                    Logger.LogError("MessageBus asset missing in Resources folder!");

                return _instance;
            }
        }

        private Dictionary<string, Delegate> _eventTable;
        private Dictionary<string, Delegate> _funcTable;

        private void OnEnable()
        {
            _eventTable = new Dictionary<string, Delegate>();
            _funcTable = new Dictionary<string, Delegate>();
            
           CheckInstance();
        }

        private void OnDisable()
        {
            _eventTable?.Clear();
        }

        private void CheckInstance()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[MessageHub] Duplicate instance detected: {name}. Destroying this instance.");
                // Optionally destroy this instance to enforce uniqueness
                #if UNITY_EDITOR
                // DestroyImmediate only allowed in editor
                    DestroyImmediate(this, true);
                #else
                    Destroy(this);
                #endif
            }
        }

        //Fire and forget
        public void AddListener(string eventName, Action callBack) => AddInternal(eventName, callBack);
        public void AddListener<T1>(string eventName, Action<T1> callBack) => AddInternal(eventName, callBack);
        public void AddListener<T1,T2>(string eventName, Action<T1,T2> callBack) => AddInternal(eventName, callBack);
        public void AddListener<T1,T2,T3>(string eventName, Action<T1,T2,T3> callBack) => AddInternal(eventName, callBack);
        
        
        //Callbacks using func
        public void AddListener<TIn, TOut>(string eventName, Func<TIn, TOut> callback) => AddInternal(eventName, callback);


        //Fire and forget
        public void RemoveListener(string eventName, Action callback) => RemoveInternal(eventName, callback);
        public void RemoveListener<T1>(string eventName, Action<T1> callback) => RemoveInternal(eventName, callback);
        public void RemoveListener<T1,T2>(string eventName, Action<T1,T2> callback) => RemoveInternal(eventName, callback);
        public void RemoveListener<T1,T2,T3>(string eventName, Action<T1,T2,T3> callback) => RemoveInternal(eventName, callback);
        
        
        //Callbacks using func
        public void RemoveListener<TIn, TOut>(string eventName, Func<TIn, TOut> callback) => RemoveInternal(eventName, callback);
        
        
        //Use for fire and forget
        private void AddInternal(string eventName, Delegate callback)
        {
            if (string.IsNullOrWhiteSpace(eventName) || callback == null)
            {
                Debug.LogError("[MessageHub] Invalid event name or callback.");
                return;
            }

            if (_eventTable.TryGetValue(eventName, out var existingDelegate))
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
                
                _eventTable[eventName] = Delegate.Combine(existingDelegate, callback);
            }
            else
            {
                _eventTable[eventName] = callback;
            }
        }

        private void RemoveInternal(string eventName, Delegate callback)
        {
            if (string.IsNullOrWhiteSpace(eventName) || callback == null)
            {
                Debug.LogError("[MessageHub] Invalid event name or callback.");
                return;
            }

            if (_eventTable.TryGetValue(eventName, out var existingDelegate))
            {
                if (existingDelegate.GetType() != callback.GetType())
                {
                    Debug.LogWarning($"[MessageHub] Listener type mismatch for event '{eventName}'. Expected {existingDelegate.GetType().Name}, got {callback.GetType().Name}.");
                    return;
                }

                var newDelegate = Delegate.Remove(existingDelegate, callback);
                if (newDelegate == null)
                    _eventTable.Remove(eventName);
                else
                    _eventTable[eventName] = newDelegate;
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No listeners found for event '{eventName}' to remove.");
            }
        }
        
        
        public void Broadcast(string eventName)
        {
            if (_eventTable.TryGetValue(eventName, out var del) && del is Action action)
            {
                action.Invoke();
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching zero-parameter listener found for event '{eventName}'.");
            }
        }

        
        
        public void Broadcast<T1>(string eventName, T1 param1)
        {
            if (_eventTable.TryGetValue(eventName, out var del) && del is Action<T1> action)
            {
                action.Invoke(param1);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching one-parameter listener found for event '{eventName}'.");
            }
        }

        public void Broadcast<T1, T2>(string eventName, T1 param1, T2 param2)
        {
            if (_eventTable.TryGetValue(eventName, out var del) && del is Action<T1, T2> action)
            {
                action.Invoke(param1, param2);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching two-parameter listener found for event '{eventName}'.");
            }
        }

        public void Broadcast<T1, T2, T3>(string eventName, T1 param1, T2 param2, T3 param3)
        {
            if (_eventTable.TryGetValue(eventName, out var del) && del is Action<T1, T2, T3> action)
            {
                action.Invoke(param1, param2, param3);
            }
            else
            {
                Debug.LogWarning($"[MessageHub] No matching three-parameter listener found for event '{eventName}'.");
            }
        }
    }
}
