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


using System.Collections;
using System.Web;

namespace NCommon.Storage
{
    ///<summary>
    /// Inherits from <see cref="Store"/> to allow storing of application specific data
    /// in the AppDomain / current HttpContext.
    ///</summary>
    public class AppStorage : Store
    {
        #region fields

        private static Hashtable _internalStorage;

        #endregion

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
            get { return AppStorageLock; }
        }

        #endregion

        #region methods

        ///<summary>
        /// Overriden. Gets the internal hash table that is used to store and retrieve application
        /// specific data.
        ///</summary>
        ///<returns>A <see cref="Hashtable"/> that is used to store application specific data.</returns>
        /// <remarks>
        /// This method implementation uses locking as multiple threads (or requests in the case of a web app) can call
        /// the GetInternalHashtable at the same time.
        /// </remarks>
        protected override Hashtable GetInternalHashtable()
        {
            if (IsWebApplication)
            {
                //This code is executing under a WebSite. Use the Application context to retrieve the hash table.
                var internalHashtable = HttpContext.Current.Application[typeof (AppStorage).FullName] as Hashtable;
                if (internalHashtable == null)
                {
                    lock (AppStorageLock)
                    {
                        internalHashtable = HttpContext.Current.Application[typeof(AppStorage).FullName] as Hashtable;
                        if (internalHashtable == null)
                            HttpContext.Current.Application[typeof(AppStorage).FullName] =
                                internalHashtable = new Hashtable();
                    }
                }
                return internalHashtable;
            }

            //The code is running under a normal windows application. Use the static property.
            if (_internalStorage == null)
            {
                lock (AppStorageLock)
                {
                    if (_internalStorage == null)
                        _internalStorage = new Hashtable();
                }
            }
            return _internalStorage;
        }

        #endregion
    }
}