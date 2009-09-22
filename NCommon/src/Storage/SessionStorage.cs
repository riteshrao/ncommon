#region license
//Copyright 2008 Ritesh Rao 

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
using System.Collections;
using System.Web;

namespace NCommon.Storage
{
    ///<summary>
    /// Inherits from the <see cref="Store"/> class to allow storage of application specific data
    /// in the current HttpSession.
    ///</summary>
    public class SessionStorage : Store
    {
        #region properties

        /// <summary>
        /// Overriden. Configures the storage to use locking when getting and setting values.
        /// </summary>
        protected override bool UseLocking
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the object used by the <see cref="Store"/> for locking when retrieving and setting values.
        /// </summary>
        protected override object LockInstance
        {
            get { return SessionStorageLock; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Overriden. Gets the <see cref="Hashtable"/> used to store thread specific data.
        /// </summary>
        /// <returns>A <see cref="Hashtable"/> that can be used to store Session specific data.</returns>
        /// <remarks>
        /// This code block uses locking to create the Hashtable as multiple requests can execute under the same session in
        /// the case AJAX calls. 
        /// </remarks>
        protected override Hashtable GetInternalHashtable()
        {
            Guard.Against<InvalidOperationException>(!IsWebApplication || HttpContext.Current.Session == null,
                                                     "An ASP.Net session must be available when using Session storage. No ASP.Net session was found in the current context.");

            var internalStorage = HttpContext.Current.Session[typeof (SessionStorage).FullName] as Hashtable;
            if (internalStorage == null)
            {
                lock (SessionStorageLock)
                {
                    internalStorage = HttpContext.Current.Session[typeof (SessionStorage).FullName] as Hashtable;
                    if (internalStorage == null)
                        HttpContext.Current.Session[typeof (SessionStorage).FullName] =
                            internalStorage = new Hashtable();
                }
            }
            return internalStorage;
        }

        #endregion
    }
}