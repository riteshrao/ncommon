using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace NCommon.Context
{
    public interface IOperationContext
    {
        IContextChannel Channel { get; }
        EndpointDispatcher EndpointDispatcher { get; }
        IExtensionCollection<OperationContext> Extensions { get; }
        bool HasSupportingTokens { get; }
        IServiceHost Host { get; }
        MessageHeaders IncomingMessageHeaders { get; }
        MessageProperties IncomingMessageProperties { get; }
        MessageVersion IncomingMessageVersion { get; }
        IInstanceContext InstanceContext { get; }
        bool IsUserContext { get; }
        MessageHeaders OutgoingMessageHeaders { get; }
        MessageProperties OutgoingMessageProperties { get; }
        RequestContext RequestContext { get; }
        string SessionId { get; }
        ICollection<SupportingTokenSpecification> SupportingTokens { get; }

        T GetCallbackChannel<T>();
        void SetTransactionComplete();
    }
}