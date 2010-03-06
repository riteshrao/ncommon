using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace NCommon.Context
{
    public interface IInstanceContext
    {
        IExtensionCollection<InstanceContext> Extensions { get; }
        IServiceHost Host { get; }
        ICollection<IChannel> IncomingChannels { get; }
        int ManualFlowControlLimit { get; set; }
        ICollection<IChannel> OutgoinChannels { get; }
        SynchronizationContext SynchronizationContext { get; set; }

        void IncrementManualFlowControlLimit(int limit);
        object GetServiceInstance();
        object GetServiceInstance(Message message);
    }
}