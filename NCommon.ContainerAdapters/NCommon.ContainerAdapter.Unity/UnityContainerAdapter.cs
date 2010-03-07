using System;
using Microsoft.Practices.Unity;
using NCommon.Configuration;

namespace NCommon.ContainerAdapter.Unity
{
    public class UnityContainerAdapter : IContainerAdapter
    {
        readonly IUnityContainer _container;

        public UnityContainerAdapter(IUnityContainer container)
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
            _container.RegisterType(service, implementation, new TransientLifetimeManager());
        }

        public void Register(Type service, Type implementation, string named)
        {
            _container.RegisterType(service, implementation, named, new TransientLifetimeManager());
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
            _container.RegisterType(service, implementation, new ContainerControlledLifetimeManager());
        }

        public void RegisterSingleton(Type service, Type implementation, string named)
        {
            _container.RegisterType(service, implementation, named, new ContainerControlledLifetimeManager());
        }

        public void RegisterInstance<TService>(TService instance)
        {
            RegisterInstance(typeof(TService));
        }

        public void RegisterInstance<TService>(TService instance, string named)
        {
            RegisterInstance(typeof(TService), named);
        }

        public void RegisterInstance(Type service, object instance)
        {
            _container.RegisterInstance(service, instance, new ContainerControlledLifetimeManager());
        }

        public void RegisterInstance(Type service, object instance, string named)
        {
            _container.RegisterInstance(service, named, instance, new ContainerControlledLifetimeManager());
        }

    }
}