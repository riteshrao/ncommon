using System;
using NCommon.Configuration;
using Ninject;

namespace NCommon.ContainerAdapter.NInject
{
    public class NInjectContainerAdapter : IContainerAdapter
    {
        readonly IKernel _kernel;

        public NInjectContainerAdapter(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Register(typeof (TService), typeof (TImplementation));
        }

        public void Register<TService, TImplementation>(string named) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), named);
        }

        public void Register(Type service, Type implementation)
        {
            _kernel.Bind(service).To(implementation);
        }

        public void Register(Type service, Type implementation, string named)
        {
            _kernel.Bind(service).To(implementation).Named(named);
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
            _kernel.Bind(service).To(implementation).InSingletonScope();
        }

        public void RegisterSingleton(Type service, Type implementation, string named)
        {
            _kernel.Bind(service).To(implementation).InSingletonScope().Named(named);
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
            _kernel.Bind(service).ToConstant(instance);
        }

        public void RegisterInstance(Type service, object instance, string named)
        {
            _kernel.Bind(service).ToConstant(instance).Named(named);
        }
    }
}