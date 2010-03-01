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
using Microsoft.Practices.ServiceLocation;
using NCommon.State;

namespace NCommon.Data
{
    /// <summary>
    /// Manages the lifetime of a <see cref="IUnitOfWork"/>
    /// </summary>
    public static class UnitOfWork
    {
        #region const
        /// <summary>
        /// The Key used to store the current unit of work in <see cref="Store.Local"/>.
        /// </summary>
        private const string currentUnitOfWorkKey = "CurrentUnitOfWorkSession.Key";
        #endregion

        #region properties

        /// <summary>
        /// Gets a boolean value indicating whether a _unitOfWork has been started for the current
        /// thread or current session.
        /// </summary>
        /// <value>
        /// True if a _unitOfWork has already started for the current thread or request.
        /// </value>
        public static bool HasStarted
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IState>();
                return state.Local.Get<IUnitOfWork>(currentUnitOfWorkKey) == null;
            }
        }

        /// <summary>
        /// Gets the current <see cref="IUnitOfWork"/> instance.
        /// </summary>
        /// <value>
        /// A <see cref="IUnitOfWork"/> instance for the current thread or request.
        /// </value>
        public static IUnitOfWork Current
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IState>();
                return state.Local.Get<IUnitOfWork>(currentUnitOfWorkKey);
            }
            set
            {
                var state = ServiceLocator.Current.GetInstance<IState>();
                if (value == null)
                    //Remove if the value is sepcified as null);
                    state.Local.Remove<IUnitOfWork>(currentUnitOfWorkKey);
                else
                    state.Local.Put<IUnitOfWork>(currentUnitOfWorkKey, value);
            }
        }

        #endregion

        #region methods
        /// <summary>
        /// Starts a new <see cref="IUnitOfWork"/> session that implements a unit of work operation.
        /// </summary>
        /// <returns></returns>
        public static IUnitOfWork Start()
        {
            if (HasStarted)
                return Current; //returning the current uniut of work if it has already been started.
            var factory = ServiceLocator.Current.GetInstance<IUnitOfWorkFactory>();
            Current = factory.Create();
            return Current;
        }

        /// <summary>
        /// Finishes the current IUnitOfWork instance.
        /// </summary>
        /// <param name="flush">bool. True if the Finish operation should flush the changes made 
        /// to the current <see cref="IUnitOfWork"/> instance.</param>
        public static void Finish(bool flush)
        {
            Guard.Against<InvalidOperationException>(!HasStarted, "There is no running UnitOfWork session to finish.");
            if (flush)
                Current.TransactionalFlush();
            Current.Dispose();
            Current = null;
        }
        #endregion
    }
}