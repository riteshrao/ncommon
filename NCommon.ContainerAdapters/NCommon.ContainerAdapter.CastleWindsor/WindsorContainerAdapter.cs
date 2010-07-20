#region license
//Copyright 2010 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NCommon.Configuration;

namespace NCommon.ContainerAdapter.CastleWindsor
{
    /// <summary>
    /// <see cref="IContainerAdapter"/> implementation for Castle Windsor.
    /// </summary>
    public class WindsorContainerAdapter : IContainerAdapter
    {
        readonly IWindsorContainer _container;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="WindsorContainerAdapter"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> instance used by the WindsorContainerAdapter
        /// to register components.</param>
        public WindsorContainerAdapter(IWindsorContainer container)
        {
            _container = container;
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
            Register(typeof(TService), typeof(TImplementation));
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
            Register(typeof (TService), typeof (TImplementation), named);
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
            _container.Register(Component.For(service).ImplementedBy(implementation));
        }

        /// <summary>
        /// Registers a named implementation type for a service type.
        /// </summary>
        /// <param name="service"><see cref="Type"/>. The type representing the service fow which the
        /// implementation type if registered.</param>
        /// <param name="implementation"><see cref="Type"/>. The type representing the implementaton
        /// registered for the service.</param>
        /// <param name="named">string. The service name with which the implementation is registered.</param>
        public void Register(Type service, Type implementation, string named)
        {
            _container.Register(Component.For(service)
                .ImplementedBy(implementation)
                .LifeStyle.Is(LifestyleType.Transient)
                .Named(named));
        }

        ///<summary>
        /// Registers a open generic implementation for a generic service type.
        ///</summary>
        ///<param name="service">The type representing the service for which the implementation type is registered.</param>
        ///<param name="implementation">The type representing the implementation registered for the service.</param>
        public void RegisterGeneric(Type service, Type implementation)
        {
            Register(service, implementation);
        }

        ///<summary>
        /// Registers a named open generic implementation for a generic service type.
        ///</summary>
        ///<param name="service">The type representing the service for which the implementation is registered.</param>
        ///<param name="implementation">The type representing the implementation registered for the service.</param>
        ///<param name="named">string. The service name with which the implementation is registerd.</param>
        public void RegisterGeneric(Type service, Type implementation, string named)
        {
            Register(service, implementation, named);
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
            RegisterSingleton(typeof (TService), typeof (TImplementation));
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
            RegisterSingleton(typeof(TService), typeof(TImplementation), named);
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
            _container.Register(Component.For(service)
                                    .ImplementedBy(implementation)
                                    .LifeStyle.Is(LifestyleType.Singleton));
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
            _container.Register(Component.For(service)
                                    .ImplementedBy(implementation)
                                    .LifeStyle.Is(LifestyleType.Singleton)
                                    .Named(named));
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
            RegisterInstance(typeof (TService), instance);
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
            RegisterInstance(typeof(TService), instance, named);
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
            _container.Register(Component.For(service).Instance(instance));
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
            _container.Register(Component.For(service).Instance(instance).Named(named));
        }
    }
}