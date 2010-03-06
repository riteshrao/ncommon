using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace NCommon.Context.Impl
{
    public class ServiceHostWrapper : IServiceHost
    {
        readonly ServiceHostBase _serviceHost;

        public ServiceHostWrapper(ServiceHostBase serviceHost)
        {
            _serviceHost = serviceHost;
        }

        public ServiceAuthorizationBehavior Authorization
        {
            get { return _serviceHost.Authorization; }
        }

        public ReadOnlyCollection<Uri> BaseAddresses
        {
            get { return _serviceHost.BaseAddresses; }
        }

        public ChannelDispatcherCollection ChannelDispatchers
        {
            get { return _serviceHost.ChannelDispatchers; }
        }

        public TimeSpan CloseTimeout
        {
            get { return _serviceHost.CloseTimeout; }
            set { _serviceHost.CloseTimeout = value; }
        }

        public ServiceCredentials Credentials
        {
            get { return _serviceHost.Credentials; }
        }

        public ServiceDescription Description
        {
            get { return _serviceHost.Description; }
        }

        public IExtensionCollection<ServiceHostBase> Extensions
        {
            get { return _serviceHost.Extensions; }
        }

        public int ManualFlowControlLimit
        {
            get { return _serviceHost.ManualFlowControlLimit; }
            set { _serviceHost.ManualFlowControlLimit = value; }
        }

        public TimeSpan OpenTimeout
        {
            get { return _serviceHost.OpenTimeout; }
            set { _serviceHost.OpenTimeout = value; }
        }

        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address);
        }

        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address);
        }

        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address, Uri listenUri)
        {
           return _serviceHost.AddServiceEndpoint(implementedContract, binding, address, listenUri);
        }

        public ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address, Uri listenUri)
        {
            return _serviceHost.AddServiceEndpoint(implementedContract, binding, address, listenUri);
        }
    }
}