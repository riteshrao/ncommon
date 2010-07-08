using System;
using Autofac;
using NCommon.Configuration;

namespace NCommon.ContainerAdapter.Autofac
{
    public class AutofacContainerAdapter : IContainerAdapter
    {
        ContainerBuilder _builder;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="AutofacContainerAdapter"/> class.
        /// </summary>
        /// <param name="builder"></param>
        public AutofacContainerAdapter(ContainerBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Registers a default implementation type for a service type.
        /// </summary>
        /// <typeparam name="TService">The <typeparamref name="TService"/> type representing the service
        /// for which the implementation type is registered. </typeparam>
        /// <typeparam name="TImplementation">The <typeparamref name="TImplementation"/> type representing
        /// the implementation registered for the <typeparamref name="TService"/></typeparam>
        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            _builder.RegisterType<TImplementation>().As<TService>();
        }

        /// <summary>
        /// Registers a named implementation type of a service type.
        /// </summary>
        /// <typeparam name="TService">The <typeparamref name="TService"/> type representing the service
        /// for which the implementation type is registered. </typeparam>
        /// <typeparam name="TImplementation">The <typeparamref name="TImplementation"/> type representing
        /// the implementation registered for the <typeparamref name="TService"/></typeparam>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void Register<TService, TImplementation>(string named) where TImplementation : TService
        {
            _builder.RegisterType<TImplementation>().Named<TService>(named);
        }

        /// <summary>
        /// Registers a default implementation type for a service type.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing the service for which the
        ///  implementation type is registered.</param>
        /// <param name="implementation"><see cref="Type"/>. The type representing the implementation
        /// registered for the service type.</param>
        public void Register(Type service, Type implementation)
        {
            _builder.RegisterType(implementation).As(service);
        }

        /// <summary>
        /// Registers a named implementation type for a service type.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing the service for which the
        /// implementation type if registered.</param>
        /// <param name="implementation"><see cref="Type"/>. The type representing the implementaton
        /// registered for the service.</param>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void Register(Type service, Type implementation, string named)
        {
            _builder.RegisterType(implementation).Named(named, service);
        }

        ///<summary>
        /// Registers a open generic implementation for a generic service type.
        ///</summary>
        ///<param name="service">The type representing the service for which the implementation type is registered.</param>
        ///<param name="implementation">The type representing the implementation registered for the service.</param>
        public void RegisterGeneric(Type service, Type implementation)
        {
            _builder.RegisterGeneric(implementation).As(service);
        }

        ///<summary>
        /// Registers a named open generic implementation for a generic service type.
        ///</summary>
        ///<param name="service">The type representing the service for which the implementation is registered.</param>
        ///<param name="implementation">The type representing the implementation registered for the service.</param>
        ///<param name="named">string. The service name with which the implementation is registerd.</param>
        public void RegisterGeneric(Type service, Type implementation, string named)
        {
            _builder.RegisterGeneric(service).Named(named, service);
        }

        /// <summary>
        /// Registers a default implementation type for a service type as a singleton.
        /// </summary>
        /// <typeparam name="TService"><typeparamref name="TService"/>. The type representing the service
        /// for which the implementation type is registered as a singleton.</typeparam>
        /// <typeparam name="TImplementation"><typeparamref name="TImplementation"/>. The type representing
        /// the implementation that is registered as a singleton for the service type.</typeparam>
        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            _builder.RegisterType<TImplementation>().As<TService>().SingleInstance();
        }

        /// <summary>
        /// Registers a named implementation type for a service type as a singleton.
        /// </summary>
        /// <typeparam name="TService"><typeparamref name="TService"/>. The type representing the service
        /// for which the implementation type is registered as a singleton.</typeparam>
        /// <typeparam name="TImplementation"><typeparamref name="TImplementation"/>. The type representing
        /// the implementation that is registered as a singleton for the service type.</typeparam>
        /// <param name="named">string. The service name with which the implementation is registerd.</param>
        public void RegisterSingleton<TService, TImplementation>(string named) where TImplementation : TService
        {
            _builder.RegisterType<TImplementation>().Named<TService>(named).SingleInstance();
        }

        /// <summary>
        /// Registers a default implementation type for a service type as a singleton.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing the service
        /// for which the implementation type is registered as a singleton.</param>
        /// <param name="implementation"><see cref="Type"/>. The type representing
        /// the implementation that is registered as a singleton for the service type.</param>
        public void RegisterSingleton(Type service, Type implementation)
        {
            _builder.RegisterType(implementation).As(service).SingleInstance();
        }

        /// <summary>
        /// Registers a named implementation type for a service type as a singleton.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing the service
        /// for which the implementation type is registered as a singleton.</param>
        /// <param name="implementation"><see cref="Type"/>. The type representing
        /// the implementation that is registered as a singleton for the service type.</param>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void RegisterSingleton(Type service, Type implementation, string named)
        {
            _builder.RegisterType(implementation).Named(named, service).SingleInstance();
        }

        /// <summary>
        /// Registers an instance as an implementation for a service type.
        /// </summary>
        /// <typeparam name="TService"><typeparamref name="TService"/>. The type representing
        /// the service for which the instance is registered.</typeparam>
        /// <param name="instance">An instance of type <typeparamref name="TService"/> that is
        /// registered as an instance for <typeparamref name="TService"/>.</param>
        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            _builder.RegisterInstance(instance).As<TService>();
        }

        /// <summary>
        /// Registers an named instance as an implementation for a service type.
        /// </summary>
        /// <typeparam name="TService"><typeparamref name="TService"/>. The type representing
        /// the service for which the instance is registered.</typeparam>
        /// <param name="instance">An instance of type <typeparamref name="TService"/> that is
        /// registered as an instance for <typeparamref name="TService"/>.</param>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void RegisterInstance<TService>(TService instance, string named) where TService : class
        {
            _builder.RegisterInstance(instance).Named<TService>(named);
        }

        /// <summary>
        /// Registers an instance as an implementation for a service type.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing
        /// the service for which the instance is registered.</param>
        /// <param name="instance">An instance of <paramref name="service"/> that is
        /// registered as an instance for the service.</param>
        public void RegisterInstance(Type service, object instance)
        {
            _builder.RegisterInstance(instance).As(service);
        }

        /// <summary>
        /// Registers a named instance as an implementation for a service type.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing
        /// the service for which the instance is registered.</param>
        /// <param name="instance">An instance of <paramref name="service"/> that is
        /// registered as an instance for the service.</param>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void RegisterInstance(Type service, object instance, string named)
        {
            _builder.RegisterInstance(instance).Named(named, service);
        }
    }
}