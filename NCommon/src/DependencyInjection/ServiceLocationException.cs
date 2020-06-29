using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCommon.DependencyInjection
{
    public class ServiceLocationException : ApplicationException
    {
        public ServiceLocationException(string message, Type service, string keyName, Exception innerException) :
            this("An Error occured while utilzing IOC Container to load Service: " + service.ToString() + " with KeyName: "
            + keyName + ". " + message, innerException)
        {

        }

        public ServiceLocationException(string message, Type service, Exception innerException) :
            this("An Error occured while utilzing IOC Container to load Service: " + service.ToString() + ". " + message, innerException)
        {

        }

        protected ServiceLocationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
