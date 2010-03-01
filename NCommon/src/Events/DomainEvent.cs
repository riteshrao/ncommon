using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NCommon.Extensions;
using NCommon.State;

namespace NCommon.Events
{
    ///<summary>
    /// DomainEvent class that allowes raising domain events from domain entities and allow registring
    /// custom callbacks to execute when a <see cref="IDomainEvent"/> is raised.
    ///</summary>
    public class DomainEvent
    {
        static readonly string CallbackListKey = "DomainEvent.Callbacks";

        ///<summary>
        /// Registers a callback to be called when a domain event is raised.
        ///</summary>
        ///<param name="callback">An <see cref="Action{T}"/> to be invoked.</param>
        ///<typeparam name="T">The domain event that the callback is registered to handle.</typeparam>
        public static void RegisterCallback<T>(Action<T> callback) where T : IDomainEvent
        {
            var state = ServiceLocator.Current.GetInstance<IState>();
            var callbacks = state.Local.Get<IList<Delegate>>(CallbackListKey);
            if (callbacks == null)
            {
                callbacks = new List<Delegate>();
                state.Local.Put(CallbackListKey, callbacks);
            }
            callbacks.Add(callback);
        }

        ///<summary>
        /// Clears all callbacks registered on the current thread.
        ///</summary>
        public static void ClearCallbacks()
        {
            var state = ServiceLocator.Current.GetInstance<IState>();
            state.Application.Remove<IList<Delegate>>(CallbackListKey);
        }

        ///<summary>
        ///</summary>
        ///<param name="event"></param>
        ///<typeparam name="T"></typeparam>
        public static void Raise<T>(T @event) where T : IDomainEvent
        {
            var state = ServiceLocator.Current.GetInstance<IState>();
            var handlers = ServiceLocator.Current.GetAllInstances<Handles<T>>();
            if (handlers != null)
                handlers.ForEach(x => x.Handle(@event));

            var callbacks = state.Local.Get<IList<Delegate>>(CallbackListKey);
            if (callbacks != null && callbacks.Count > 0)
                callbacks.OfType<Action<T>>().ForEach(x => x(@event));
        }
    }
}