using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NCommon.Configuration;

namespace NCommon.ContainerAdapter.CastleWindsor
{
    public class WindsorContainerAdapter : IContainerAdapter
    {
        readonly IWindsorContainer _container;

        public WindsorContainerAdapter(IWindsorContainer container)
        {
            _container = container;
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation));
        }

        public void Register<TService, TImplementation>(string named) where TImplementation : TService
        {
            Register(typeof (TService), typeof (TImplementation), named);
        }

        public void Register(Type service, Type implementation)
        {
            _container.Register(Component.For(service).ImplementedBy(implementation));
        }

        public void Register(Type service, Type implementation, string named)
        {
            _container.Register(Component.For(service).ImplementedBy(implementation).Named(named));
        }

        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            RegisterSingleton(typeof (TService), typeof (TImplementation));
        }

        public void RegisterSingleton<TService, TImplementation>(string named) where TImplementation : TService
        {
            RegisterSingleton(typeof(TService), typeof(TImplementation), named);
        }

        public void RegisterSingleton(Type service, Type implementation)
        {
            _container.Register(Component.For(service)
                                    .ImplementedBy(implementation)
                                    .LifeStyle.Is(LifestyleType.Singleton));
        }

        public void RegisterSingleton(Type service, Type implementation, string named)
        {
            _container.Register(Component.For(service)
                                    .ImplementedBy(implementation)
                                    .LifeStyle.Is(LifestyleType.Singleton)
                                    .Named(named));
        }

        public void RegisterInstance<TService>(TService instance)
        {
            RegisterInstance(typeof (TService), instance);
        }

        public void RegisterInstance<TService>(TService instance, string named)
        {
            RegisterInstance(typeof(TService), instance, named);
        }

        public void RegisterInstance(Type service, object instance)
        {
            _container.Register(Component.For(service).Instance(instance));
        }

        public void RegisterInstance(Type service, object instance, string named)
        {
            _container.Register(Component.For(service).Instance(instance).Named(named));
        }
    }
}