using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace NCommon.Context
{
    public interface IServiceHost
    {
        ServiceAuthorizationBehavior Authorization { get; }
        ReadOnlyCollection<Uri> BaseAddresses { get; }
        ChannelDispatcherCollection ChannelDispatchers { get; }
        TimeSpan CloseTimeout { get; set; }
        ServiceCredentials Credentials { get; }
        ServiceDescription Description { get;}
        IExtensionCollection<ServiceHostBase> Extensions { get; }
        int ManualFlowControlLimit { get; set; }
        TimeSpan OpenTimeout { get; set; }

        ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address);
        ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address);
        ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, string address, Uri listenUri);
        ServiceEndpoint AddServiceEndpoint(string implementedContract, Binding binding, Uri address, Uri listenUri);
    }
}