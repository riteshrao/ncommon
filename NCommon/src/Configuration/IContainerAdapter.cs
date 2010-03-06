using System;

namespace NCommon.Configuration
{
    ///<summary>
    /// Base interface for abstract specific containers that 
    ///</summary>
    public interface IContainerAdapter
    {
        void Register<TService, TImplementation>();
        void Register<TService, TImplementation>(string named);
        void Register(Type service, Type implementation);
        void Register(Type service, Type implementation, string named);

        void RegisterSingleton<TService, TImplementation>();
        void RegisterSingleton<TService, TImplementation>(string named);
        void RegisterSingleton(Type service, Type implementation);
        void RegisterSingleton(Type service, Type implementation, string named);

        void RegisterInstance<TService>(TService instance);
        void RegisterInstance<TService>(TService instance, string named);
        void RegisterInstance(Type service, object instance);
        void RegisterInstance(Type service, object instance, string named);

        void ReplaceComponent<TService, TImplementation>();
    }
}