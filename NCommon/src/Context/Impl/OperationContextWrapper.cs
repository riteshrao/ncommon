using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace NCommon.Context.Impl
{
    public class OperationContextWrapper : IOperationContext
    {
        readonly OperationContext _context;
        readonly IInstanceContext _instanceContext;

        public OperationContextWrapper(OperationContext context)
        {
            _context = context;
            _instanceContext = new InstanceContextWrapper(context.InstanceContext);
        }

        public IContextChannel Channel
        {
            get { return _context.Channel; }
        }

        public EndpointDispatcher EndpointDispatcher
        {
            get { return _context.EndpointDispatcher; }
        }

        public IExtensionCollection<OperationContext> Extensions
        {
            get { return _context.Extensions; }
        }

        public bool HasSupportingTokens
        {
            get { return _context.HasSupportingTokens; }
        }

        public IServiceHost Host
        {
            get { return new ServiceHostWrapper(_context.Host); }
        }

        public MessageHeaders IncomingMessageHeaders
        {
            get { return _context.IncomingMessageHeaders; }
        }

        public MessageProperties IncomingMessageProperties
        {
            get { return _context.IncomingMessageProperties; }
        }

        public MessageVersion IncomingMessageVersion
        {
            get { return _context.IncomingMessageVersion; }
        }

        public IInstanceContext InstanceContext
        {
            get { return _instanceContext; }
        }

        public bool IsUserContext
        {
            get { return _context.IsUserContext; }
        }

        public MessageHeaders OutgoingMessageHeaders
        {
            get { return _context.OutgoingMessageHeaders; }
        }

        public MessageProperties OutgoingMessageProperties
        {
            get { return _context.OutgoingMessageProperties; }
        }

        public RequestContext RequestContext
        {
            get { return _context.RequestContext; }
        }

        public string SessionId
        {
            get { return _context.SessionId; }
        }

        public ICollection<SupportingTokenSpecification> SupportingTokens
        {
            get { return _context.SupportingTokens; }
        }

        public T GetCallbackChannel<T>()
        {
            return _context.GetCallbackChannel<T>();
        }

        public void SetTransactionComplete()
        {
            _context.SetTransactionComplete();
        }
    }
}