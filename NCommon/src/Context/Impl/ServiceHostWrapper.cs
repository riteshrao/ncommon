#region license
//Copyright 2008 Ritesh Rao 

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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace NCommon.Context.Impl
{
    /// <summary>
    /// Default <see cref="IServiceHost"/> wrapper.
    /// </summary>
    public class ServiceHostWrapper : IServiceHost
    {
        readonly ServiceHostBase _serviceHost;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ServiceHostWrapper"/> class.
        /// </summary>
        /// <param name="serviceHost">The <see cref="ServiceHost"/> instance to wrap.</param>
        public ServiceHostWrapper(ServiceHostBase serviceHost)
        {
            _serviceHost = serviceHost;
        }

        /// <summary>
        /// Gets the authorization behavior for the service.
        /// </summary>
        /// <value>A <see cref="ServiceAuthorizationBehavior"/> for the service host.</value>
        public ServiceAuthorizationBehavior Authorization
        {
            get { return _serviceHost.Authorization; }
        }

        /// <summary>
        /// Gets the base addresses used by the service host.
        /// </summary>
        /// <value>A <see cref="ReadOnlyCollection{T}"/> that contains the base addresses used by the service host.</value>
        public ReadOnlyCollection<Uri> BaseAddresses
        {
            get { return _serviceHost.BaseAddresses; }
        }

        /// <summary>
        /// Gets a collection of channel dispatchers used by he service host.
        /// </summary>
        /// <value>A <see cref="ChannelDispatcherCollection"/> containing the channel dispatchers used by the service host.</value>
        public ChannelDispatcherCollection ChannelDispatchers
        {
            get { return _serviceHost.ChannelDispatchers; }
        }

        /// <summary>
        /// Gets or sets the interval of time allowed for the service host to close.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> that specifies the interval of time allowed for the service host to close.</value>
        public TimeSpan CloseTimeout
        {
            get { return _serviceHost.CloseTimeout; }
            set { _serviceHost.CloseTimeout = value; }
        }

        /// <summary>
        /// Gets the crendentials for the service host.
        /// </summary>
        /// <value>A <see cref="ServiceCredentials"/> instance.</value>
        public ServiceCredentials Credentials
        {
            get { return _serviceHost.Credentials; }
        }

        /// <summary>
        /// Gets the description of the service host.
        /// </summary>
        /// <value>A <see cref="ServiceDescription"/> instance.</value>
        public ServiceDescription Description
        {
            get { return _serviceHost.Description; }
        }

        /// <summary>
        /// Gets the extensions registered for the service host.
        /// </summary>
        /// <value>A <see cref="IExtensionCollection{T}"/> contianing extensions registered for the service host.</value>
        public IExtensionCollection<ServiceHostBase> Extensions
        {
            get { return _serviceHost.Extensions; }
        }

        /// <summary>
        /// Gets or sets the flow control limit for messages recieved by the service host.
        /// </summary>
        /// <value>int. The flow control limit for messages recieved by the service host.</value>
        public int ManualFlowControlLimit
        {
            get { return _serviceHost.ManualFlowControlLimit; }
            set { _serviceHost.ManualFlowControlLimit = value; }
        }

        /// <summary>
        /// Gets or sets the interval of time allowed for the service host to open.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> that specifies the interval of time allowed for the service host to open.</value>
        public TimeSpan OpenTimeout
        {
            get { return _serviceHost.OpenTimeout; }
            set { _serviceHost.OpenTimeout = value; }
        }

        /// <summary>
        /// Adds a service endpoint to the service host with the specified contract, binding and address.
        /// </summary>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="binding">A <see cref="Binding"/> instnace for the endpoint.</param>
        /// <param name="address">The address for the endpoint.</param>
        /// <returns>A <see cref="ServiceEndpoint"/> instance that was added to the service host.</returns>
        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address);
        }

        /// <summary>
        /// Adds a service endpoint to the service host with the specified contract, binding and address.
        /// </summary>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="binding">A <see cref="Binding"/> instnace for the endpoint.</param>
        /// <param name="address">The address for the endpoint.</param>
        /// <returns>A <see cref="ServiceEndpoint"/> instance that was added to the service host.</returns>
        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address);
        }

        /// <summary>
        /// Adds a service endpoint to the service host with the specified contract, binding and address.
        /// </summary>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="binding">A <see cref="Binding"/> instnace for the endpoint.</param>
        /// <param name="address">The address for the endpoint.</param>
        /// <param name="listenUri">The address at which the endpoint listens for incoming messages.</param>
        /// <returns>A <see cref="ServiceEndpoint"/> instance that was added to the service host.</returns>
        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address, Uri listenUri)
        {
           return _serviceHost.AddServiceEndpoint(implementedContract, binding, address, listenUri);
        }

        /// <summary>
        /// Adds a service endpoint to the service host with the specified contract, binding and address.
        /// </summary>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="binding">A <see cref="Binding"/> instnace for the endpoint.</param>
        /// <param name="address">The address for the endpoint.</param>
        /// <param name="listenUri">The address at which the endpoint listens for incoming messages.</param>
        /// <returns>A <see cref="ServiceEndpoint"/> instance that was added to the service host.</returns>
        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address, Uri listenUri)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address, listenUri);
        }
    }
}