using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace NCommon.Context.Impl
{
    public class InstanceContextWrapper : IInstanceContext
    {
        readonly InstanceContext _context;

        public InstanceContextWrapper(InstanceContext context)
        {
            _context = context;
        }

        public IExtensionCollection<InstanceContext> Extensions
        {
            get { return _context.Extensions; }
        }

        public IServiceHost Host
        {
            get { return new ServiceHostWrapper(_context.Host); }
        }

        public ICollection<IChannel> IncomingChannels
        {
            get { return _context.IncomingChannels; }
        }

        public int ManualFlowControlLimit
        {
            get { return _context.ManualFlowControlLimit; }
            set { _context.ManualFlowControlLimit = value; }
        }

        public ICollection<IChannel> OutgoinChannels
        {
            get { return _context.OutgoingChannels; }
        }

        public SynchronizationContext SynchronizationContext
        {
            get { return _context.SynchronizationContext; }
            set { _context.SynchronizationContext = value; }
        }

        public void IncrementManualFlowControlLimit(int limit)
        {
            _context.IncrementManualFlowControlLimit(limit);
        }

        public object GetServiceInstance()
        {
            return _context.GetServiceInstance();
        }

        public object GetServiceInstance(Message message)
        {
            return _context.GetServiceInstance(message);
        }
    }
}