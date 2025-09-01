using System;

namespace ToolBox.Patterns.Factory
{
    public interface ITryGetFactory : IFactory
    {
        public bool TryGet(Type type, out object service);
    }
}