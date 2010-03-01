using System;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace NCommon.State.Impl
{
    public class SessionState : ISessionState
    {
        #region internal classes
        interface ISessionStateInstance
        {
            Hashtable InternalStorage { get; }
        }

        class HttpSessionStateInstance : ISessionStateInstance
        {
            readonly Hashtable _internalStorage = new Hashtable();
            readonly static object _instanceLock = new object();

            public Hashtable InternalStorage
            {
                get { return _internalStorage; }
            }

            public static HttpSessionStateInstance Current
            {
                get
                {
                    var instance =
                        HttpContext.Current.Session[typeof(HttpSessionStateInstance).FullName] as
                        HttpSessionStateInstance;
                    if (instance == null)
                    {
                        lock (_instanceLock)
                        {
                            instance = HttpContext.Current.Session[typeof(HttpSessionStateInstance).FullName] as
                                       HttpSessionStateInstance;
                            if (instance == null)
                                HttpContext.Current.Session[typeof(HttpSessionStateInstance).FullName] =
                                    (instance = new HttpSessionStateInstance());
                        }
                    }
                    return instance;
                }
            }
        }

        class WcfSessionStateInstance : ISessionStateInstance, IExtension<InstanceContext>
        {
            readonly Hashtable _internalStorage = new Hashtable();
            readonly static object _instanceLock = new object();

            public Hashtable InternalStorage
            {
                get { return _internalStorage; }
            }

            public static WcfSessionStateInstance Current
            {
                get
                {
                    var instance = OperationContext.Current.InstanceContext.Extensions.Find<WcfSessionStateInstance>();
                    if (instance == null)
                    {
                        lock(_instanceLock)
                        {
                            instance = OperationContext.Current.InstanceContext.Extensions.Find<WcfSessionStateInstance>();
                            if (instance == null)
                                OperationContext.Current.InstanceContext.Extensions.Add((instance = new WcfSessionStateInstance()));
                        }
                    }
                    return instance;
                }
            }

            public void Attach(InstanceContext owner) {}

            public void Detach(InstanceContext owner)
            {
                _internalStorage.Clear();
            }
        } 
        #endregion

        readonly ISessionStateInstance _state;
        static Func<ISessionStateInstance> _instanceResolver;
        static readonly object _instanceResolverLock = new object();

        public SessionState()
        {
            _state = ResolveInstance();
        }

        public T Get<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to retrieve.");
            var fullKey = typeof (T).FullName + key;
            return (T) _state.InternalStorage[fullKey];
        }

        public void Put<T>(object key, T instance)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to put.");
            var fullKey = typeof (T).FullName + key;
            _state.InternalStorage[fullKey] = instance;
        }

        public void Remove<T>(object key)
        {
            Guard.Against<ArgumentNullException>(key == null,
                                                 "Expected a non-null key identifying the " + typeof(T).FullName +
                                                 " instance to remove.");
            var fullKey = typeof (T).FullName + key;
            _state.InternalStorage.Remove(fullKey);
        }

        static ISessionStateInstance ResolveInstance()
        {
            if (_instanceResolver == null)
            {
                lock(_instanceResolverLock)
                {
                    if (_instanceResolver == null)
                        _instanceResolver = () =>
                        {
                            if (OperationContext.Current != null)
                            {
                                //Wcf application detected. Checking if AspNetCompatMode is specified
                                var aspnetCompat = OperationContext.Current.Host
                                    .Description
                                    .Behaviors
                                    .Find<AspNetCompatibilityRequirementsAttribute>();

                                if (aspnetCompat != null &&
                                    (aspnetCompat.RequirementsMode == AspNetCompatibilityRequirementsMode.Allowed ||
                                     aspnetCompat.RequirementsMode == AspNetCompatibilityRequirementsMode.Required) &&
                                    HttpContext.Current != null)
                                    return HttpSessionStateInstance.Current;
                                return WcfSessionStateInstance.Current;
                            }
                            return HttpSessionStateInstance.Current;
                        };
                }
            }
            return _instanceResolver();
        }
    }
}