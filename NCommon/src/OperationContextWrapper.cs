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

namespace NCommon
{
    /// <summary>
    /// Default implementation of <see cref="IOperationContext"/>
    /// </summary>
    public class OperationContextWrapper : IOperationContext
    {
        readonly OperationContext _context;
        readonly IInstanceContext _instanceContext;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="OperationContextWrapper"/> class.
        /// </summary>
        /// <param name="context">The <see cref="OperationContext"/> to wrap.</param>
        public OperationContextWrapper(OperationContext context)
        {
            _context = context;
            _instanceContext = new InstanceContextWrapper(context.InstanceContext);
        }

        /// <summary>
        /// Gets the channel associated with the current <see cref="OperationContext"/> object.
        /// </summary>
        /// <value>A <see cref="IContextChannel"/> associated with the current <see cref="OperationContext"/></value>
        public IContextChannel Channel
        {
            get { return _context.Channel; }
        }

        /// <summary>
        /// Gets the <see cref="EndpointDispatcher"/> for the endpoint to inspect.
        /// </summary>
        /// <value>The <see cref="EndpointDispatcher"/> to inspect.</value>
        public EndpointDispatcher EndpointDispatcher
        {
            get { return _context.EndpointDispatcher; }
        }

        /// <summary>
        /// Gets a <see cref="IExtensionCollection{T}"/> of service extensions for the current message context.
        /// </summary>
        /// <value>A <see cref="IExtensionCollection{T}"/> of service extensions.</value>
        public IExtensionCollection<OperationContext> Extensions
        {
            get { return _context.Extensions; }
        }

        /// <summary>
        /// Gets a value indicating weather the incoming message has supporting tokens.
        /// </summary>
        /// <value>True if the incoming message has supporting tokents, else false.</value>
        public bool HasSupportingTokens
        {
            get { return _context.HasSupportingTokens; }
        }

        /// <summary>
        /// Gets a <see cref="IServiceHost"/> wrapper for the current service object.
        /// </summary>
        /// <value>A <see cref="IServiceHost"/> wrapper.</value>
        public IServiceHost Host
        {
            get { return new ServiceHostWrapper(_context.Host); }
        }

        /// <summary>
        /// Gets the incoming message headers for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageHeader"/> instance that contains the incoming message headers.</value>
        public MessageHeaders IncomingMessageHeaders
        {
            get { return _context.IncomingMessageHeaders; }
        }

        /// <summary>
        /// Gets the message properties for the incoming message in the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageProperties"/> instance that contains the message properties for
        /// the incoming message.</value>
        public MessageProperties IncomingMessageProperties
        {
            get { return _context.IncomingMessageProperties; }
        }

        /// <summary>
        /// Gets the incoming SOAP messsage version for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageVersion"/> representing the SOAP version of the incoming message.</value>
        public MessageVersion IncomingMessageVersion
        {
            get { return _context.IncomingMessageVersion; }
        }

        /// <summary>
        /// Gets a <see cref="IInstanceContext"/> wrapper that manages the current service instance.
        /// </summary>
        /// <value>A <see cref="IInstanceContext"/> wrapper for the current service instance.</value>
        public IInstanceContext InstanceContext
        {
            get { return _instanceContext; }
        }

        /// <summary>
        /// Gets the outgoing message headers for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageHeader"/> instance containing the outgoing message headers
        /// for the <see cref="OperationContext"/>.</value>
        public MessageHeaders OutgoingMessageHeaders
        {
            get { return _context.OutgoingMessageHeaders; }
        }

        /// <summary>
        /// Gets the outgoing message properties for the <see cref="OperationContext"/>.
        /// </summary>
        /// <value>A <see cref="MessageProperties"/> instance containing the outgoing message properties
        /// for the <see cref="OperationContext"/>.</value>
        public MessageProperties OutgoingMessageProperties
        {
            get { return _context.OutgoingMessageProperties; }
        }

        /// <summary>
        /// Gets the <see cref="RequestContext"/> implementation for the current executing method.
        /// </summary>
        /// <value>A <see cref="RequestContext"/> instance, or null if there is no request context.</value>
        public RequestContext RequestContext
        {
            get { return _context.RequestContext; }
        }

        /// <summary>
        /// Gets a string used to identify the current session.
        /// </summary>
        /// <value>A string used to identify the current session.</value>
        public string SessionId
        {
            get { return _context.SessionId; }
        }

        /// <summary>
        /// Gets the collection of security tokens.
        /// </summary>
        /// <value>A <see cref="ICollection{T}"/>.</value>
        public ICollection<SupportingTokenSpecification> SupportingTokens
        {
            get { return _context.SupportingTokens; }
        }

        /// <summary>
        /// Gets a channel to the client instance that called the current operation.
        /// </summary>
        /// <typeparam name="T">The type of channel used to callback the client.</typeparam>
        /// <returns>A channel to the client instance that called the operation of the type specified in the
        /// <see cref="ServiceContractAttribute.CallbackContract"/> property.</returns>
        public T GetCallbackChannel<T>()
        {
            return _context.GetCallbackChannel<T>();
        }

        /// <summary>
        /// Commits the current executing transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">. Thrown when there is no transaction in the current context.</exception>
        public void SetTransactionComplete()
        {
            _context.SetTransactionComplete();
        }
    }
}