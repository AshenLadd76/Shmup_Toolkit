using System;
using System.Collections.Generic;
using ToolBox.Utils;

namespace ToolBox.Patterns.Factory
{
    public class LazyInstantiatedFactory : ITryGetFactory
    {
        private readonly Dictionary<Type, Func<object>> _recipesDictionary = new();
        private readonly Dictionary<Type, object> _cachedInstances = new();
        
        /// <summary>
        ///  Registers a service type, adding it to the _recipesDictionary
        /// </summary>
      
        public void Register(Type type,  Func<object> recipe)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (recipe == null) throw new ArgumentNullException(nameof(recipe));
            
            if (!_recipesDictionary.TryAdd(type, recipe))
            {
                Logger.LogError( $"Object of {type} exists in the factory already." );
                return;
            }

            Logger.Log( $"Created a pool of type : { type }" );
        }
        
   
        
        /// <summary>
        /// Unregisters a service type, removing its factory and cached instance.
        /// </summary>
        public  void Unregister(Type type)
        {
            _recipesDictionary.Remove(type);
            _cachedInstances.Remove(type);
        }

        public object Get(Type type)
        {
            // Check if an instance is already cached for this factory object
            if (_cachedInstances.TryGetValue(type, out var instance)) 
                return instance;
            
            // If not cached, check if a factory (recipe) is registered.
            if (_recipesDictionary.TryGetValue(type, out var recipe))
            {
                // Invoke the factory to create the service instance.
                var service = recipe();
                
                // Cache the created instance for future requests.
                _cachedInstances[type] = service;

                return service;
            }
            
            // If no factory registered, throw an exception indicating missing registration.
            throw new ServiceNotRegisteredException(type);
        }
        
        public bool TryGet(Type type, out object service)
        {
            if (_cachedInstances.TryGetValue(type, out var instance))
            {
                service = instance;
                return true;
            }
            if (_recipesDictionary.TryGetValue(type, out var recipe))
            {
                var created = recipe();
                _cachedInstances[type] = created;
                service = created;
                return true;
            }
            service = null;
            return false;
        }
        
        
        /// <summary>
        /// Clears all registered factories and cached instances.
        /// Effectively resets the Service Locator to an empty state.
        /// </summary>
        public  void ClearAll()
        {
            _recipesDictionary.Clear();
            _cachedInstances.Clear();
        }
     }
}
