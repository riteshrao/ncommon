using System;

namespace NCommon.Configuration
{
    ///<summary>
    /// Base interface for abstract specific containers that 
    ///</summary>
    public interface IContainerAdapter
    {
        void Register<TService, TImplementation>() where TImplementation : TService;
        void Register<TService, TImplementation>(string named) where TImplementation : TService;
        void Register(Type service, Type implementation) ;
        void Register(Type service, Type implementation, string named);

        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;
        void RegisterSingleton<TService, TImplementation>(string named) where TImplementation : TService;
        void RegisterSingleton(Type service, Type implementation);
        void RegisterSingleton(Type service, Type implementation, string named);

        void RegisterInstance<TService>(TService instance);
        void RegisterInstance<TService>(TService instance, string named);
        void RegisterInstance(Type service, object instance);
        void RegisterInstance(Type service, object instance, string named);
    }
}