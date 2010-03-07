using System;
using NCommon.Configuration;
using StructureMap;

namespace NCommon.ContainerAdapter.StructureMap
{
    public class StructureMapContainerAdapter : IContainerAdapter
    {
        readonly IContainer _container;

        public StructureMapContainerAdapter(IContainer container)
        {
            _container = container;
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation));
        }

        public void Register<TService, TImplementation>(string named) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), named);
        }

        public void Register(Type service, Type implementation)
        {
            _container.Configure(config => config.For(service).Use(implementation));
        }

        public void Register(Type service, Type implementation, string named)
        {
            _container.Configure(config => config.For(service)
                .Use(implementation)
                .Named(named));
        }

        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            RegisterSingleton(typeof(TService), typeof(TImplementation));
        }

        public void RegisterSingleton<TService, TImplementation>(string named) where TImplementation : TService
        {
            RegisterSingleton(typeof(TService), typeof(TImplementation), named);
        }

        public void RegisterSingleton(Type service, Type implementation)
        {
            _container.Configure(config => config.For(service)
                .Singleton()
                .Use(implementation));
        }

        public void RegisterSingleton(Type service, Type implementation, string named)
        {
            _container.Configure(config => config.For(service)
                .Singleton()
                .Use(implementation)
                .Named(named));
        }

        public void RegisterInstance<TService>(TService instance)
        {
            RegisterInstance(typeof(TService), instance);
        }

        public void RegisterInstance<TService>(TService instance, string named)
        {
            RegisterInstance(typeof (TService), instance, named);
        }

        public void RegisterInstance(Type service, object instance)
        {
            _container.Configure(config => config.For(service).Use(instance));
        }

        public void RegisterInstance(Type service, object instance, string named)
        {
            _container.Configure(config => config.For(service).Use(instance).Named(named)); 
        }
    }
}