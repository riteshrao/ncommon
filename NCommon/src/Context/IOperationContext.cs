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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace NCommon.Context
{
    /// <summary>
    /// Interface wrapper that wraps a <see cref="OperationContext"/>
    /// </summary>
    public interface IOperationContext
    {
        /// <summary>
        /// Gets the channel associated with the current <see cref="OperationContext"/> object.
        /// </summary>
        /// <value>A <see cref="IContextChannel"/> associated with the current <see cref="OperationContext"/></value>
        IContextChannel Channel { get; }
        /// <summary>
        /// Gets the <see cref="EndpointDispatcher"/> for the endpoint to inspect.
        /// </summary>
        /// <value>The <see cref="EndpointDispatcher"/> to inspect.</value>
        EndpointDispatcher EndpointDispatcher { get; }
        /// <summary>
        /// Gets a <see cref="IExtensionCollection{T}"/> of service extensions for the current message context.
        /// </summary>
        /// <value>A <see cref="IExtensionCollection{T}"/> of service extensions.</value>
        IExtensionCollection<OperationContext> Extensions { get; }
        /// <summary>
        /// Gets a value indicating weather the incoming message has supporting tokens.
        /// </summary>
        /// <value>True if the incoming message has supporting tokents, else false.</value>
        bool HasSupportingTokens { get; }
        /// <summary>
        /// Gets a <see cref="IServiceHost"/> wrapper for the current service object.
        /// </summary>
        /// <value>A <see cref="IServiceHost"/> wrapper.</value>
        IServiceHost Host { get; }
        /// <summary>
        /// Gets the incoming message headers for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageHeader"/> instance that contains the incoming message headers.</value>
        MessageHeaders IncomingMessageHeaders { get; }
        /// <summary>
        /// Gets the message properties for the incoming message in the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageProperties"/> instance that contains the message properties for
        /// the incoming message.</value>
        MessageProperties IncomingMessageProperties { get; }
        /// <summary>
        /// Gets the incoming SOAP messsage version for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageVersion"/> representing the SOAP version of the incoming message.</value>
        MessageVersion IncomingMessageVersion { get; }
        /// <summary>
        /// Gets a <see cref="IInstanceContext"/> wrapper that manages the current service instance.
        /// </summary>
        /// <value>A <see cref="IInstanceContext"/> wrapper for the current service instance.</value>
        IInstanceContext InstanceContext { get; }
        /// <summary>
        /// Gets the outgoing message headers for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageHeader"/> instance containing the outgoing message headers
        /// for the <see cref="OperationContext"/>.</value>
        MessageHeaders OutgoingMessageHeaders { get; }
        /// <summary>
        /// Gets the outgoing message properties for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageProperties"/> instance containing the outgoing message properties
        /// for the <see cref="OperationContext"/>.</value>
        MessageProperties OutgoingMessageProperties { get; }
        /// <summary>
        /// Gets the <see cref="RequestContext"/> implementation for the current executing method.
        /// </summary>
        /// <value>A <see cref="RequestContext"/> instance, or null if there is no request context.</value>
        RequestContext RequestContext { get; }
        /// <summary>
        /// Gets a string used to identify the current session.
        /// </summary>
        /// <value>A string used to identify the current session.</value>
        string SessionId { get; }
        /// <summary>
        /// Gets the collection of security tokens.
        /// </summary>
        /// <value>A <see cref="ICollection{T}"/>.</value>
        ICollection<SupportingTokenSpecification> SupportingTokens { get; }
        /// <summary>
        /// Gets a channel to the client instance that called the current operation.
        /// </summary>
        /// <typeparam name="T">The type of channel used to callback the client.</typeparam>
        /// <returns>A channel to the client instance that called the operation of the type specified in the
        /// <see cref="ServiceContractAttribute.CallbackContract"/> property.</returns>
        T GetCallbackChannel<T>();
        /// <summary>
        /// Commits the current executing transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">. Thrown when there is no transaction in the current context.</exception>
        void SetTransactionComplete();
    }
}