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
using System.Configuration;

namespace NCommon.Util
{
    ///<summary>
    /// Utility class that helps get connection stirngs from the configuration file that contain
    /// machine names as identifiers for the connection strings.
    ///</summary>
    public class ConnectionString
    {
        /// <summary>
        /// Gets the default connection string for the application. 
        /// </summary>
        /// <returns>string. The default connection stirng of the application.</returns>
        /// <remarks>
        /// This methods attemps to find a app setting named "DefaultConnectionStringKey" in the application's configuration file. This
        /// setting should contain the name of the connection string defined in the &lt;connectionStrings&gt; that will be used as the
        /// application's default connection string.
        /// </remarks>
        public static string Default()
        {
            var @default = ConfigurationManager.AppSettings["DefaultConnectionStringKey"];
            if (string.IsNullOrEmpty(@default))
                throw new ApplicationException(
                    "No default connection string was found for the application. To add a default connection string " +
                    "add an entry in <appSettings> configuration section with the key \"DefaultConnectionStringKey\" and specify the " +
                    "connection string name that will be used as the default connection string.");

            return ConfigurationManager.ConnectionStrings[@default].ConnectionString;
        }

        /// <summary>
        /// Gets a connection stirng that can be used for the current machine, or if no applicable connection string found
        /// then gets the default connection string by calling <see cref="Default"/>.
        /// </summary>
        /// <returns>string. A connection stirng applicable for the current machine, or the default connection string.</returns>
        public static string Get()
        {
            var machineBasedConnection = ConfigurationManager.ConnectionStrings[Environment.MachineName];
            if (machineBasedConnection == null)
                return Default();
            return machineBasedConnection.ConnectionString;
        }

        /// <summary>
        /// Gets a connection stirng that can be used for the current machine and specific database name, 
        /// or if no applicable connection string found then gets the default connection string by calling <see cref="Default"/>.
        /// </summary>
        /// <param name="forDatabase">The database name for which the connection string is looked up.</param>
        /// <returns>string. A connection stirng applicable for the current machine, or the default connection string.</returns>
        /// <remarks>
        /// When connection strings are specified for multi-database scenarios, the naming convention of [MachineName]_[DatabaseName] is
        /// used. For e.g. if value of <paramref name="forDatabase"/> is OrderDB and current machine name is MachineA, then
        /// this method will look for a machine connection string entry in the config file named MachineA_OrderDB.
        /// </remarks>
        public static string Get(string forDatabase)
        {
            var connectionName = Environment.MachineName + "_" + forDatabase;
            var machineBasedConnection = ConfigurationManager.ConnectionStrings[forDatabase];
            if (machineBasedConnection == null)
                return Default();
            return machineBasedConnection.ConnectionString;
        }
    }
}