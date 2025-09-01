using System;

namespace ToolBox.Patterns.Factory
{
    public interface IFactory
    {
        public void Register(Type type, Func<object> recipe);
        object Get(Type type);
        void Unregister(Type type); 
        void ClearAll();
    }
}