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

using System.Web;

namespace NCommon.Context
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets weather the current application is a web based application.
        /// </summary>
        /// <value>True if the application is a web based application, else false.</value>
        bool IsWebApplication { get; }
        /// <summary>
        /// Gets weather the current application is a WCF based application.
        /// </summary>
        /// <value>True if the application is a WCF based application, else false.</value>
        bool IsWcfApplication { get; }
        /// <summary>
        /// Gets weather ASP.Net compatability is enabled for the current WCF service.
        /// </summary>
        /// <value>True if <see cref="IsWcfApplication"/> is true and ASP.Net compatability is enabled for
        /// the current service, else false.</value>
        bool IsAspNetCompatEnabled { get; }
        /// <summary>
        /// Gets a <see cref="HttpContextBase"/> that wraps the current <see cref="HttpContext"/>
        /// </summary>
        /// <value>An <see cref="HttpContextBase"/> instnace if <see cref="IsWebApplication"/> is true,
        /// else null.</value>
        HttpContextBase HttpContext { get; }
        /// <summary>
        /// Gets a <see cref="IOperationContext"/> that wraps the current <see cref="OperationContext"/>
        /// for a WCF based application.
        /// </summary>
        /// <value>An  <see cref="IOperationContext"/> instance if <see cref="IsWcfApplication"/> is true,
        /// else null.</value>
        IOperationContext OperationContext { get; }
    }
}