using System;

namespace ToolBox.Patterns.Factory
{
    public class ServiceNotRegisteredException : Exception
    {
        public ServiceNotRegisteredException(Type serviceType) : base($"Service not registered: {serviceType.FullName}") { }
    }
}