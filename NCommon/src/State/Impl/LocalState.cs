using System;
using System.Collections;
using System.ServiceModel;
using System.Web;

namespace NCommon.State.Impl
{
    public class LocalState : ILocalState
    {
        #region internal classes
        interface ILocalStateInstance
        {
            Hashtable InternalStorage { get; }
        }

        class ThreadLocalState : ILocalStateInstance
        {
            [ThreadStatic] 
            static ThreadLocalState _instance;
            readonly Hashtable _internalStorage = new Hashtable();

            public Hashtable InternalStorage
            {
                get { return _internalStorage; }
            }

            public static ThreadLocalState Current
            {
                get
                {
                    if (_instance == null)
                        _instance = new ThreadLocalState();
                    return _instance;
                }
            }
        }

        class HttpRequestState : ILocalStateInstance
        {
            readonly Hashtable _internalStorage = new Hashtable();

            public Hashtable InternalStorage
            {
                get { return _internalStorage; }
            }

            public static HttpRequestState Current
            {
                get
                {
                    var instance = HttpContext.Current.Items[typeof (HttpRequestState).FullName] as HttpRequestState;
                    if (instance == null)
                        HttpContext.Current.Items[typeof (HttpRequestState).FullName] = (instance = new HttpRequestState());
                    return instance;
                }
            }
        }

        class WcfRequestState : ILocalStateInstance, IExtension<OperationContext>
        {
            readonly Hashtable _internalStorage = new Hashtable();

            public Hashtable InternalStorage
            {
                get { return _internalStorage; }
            }

            public static WcfRequestState Current
            {
                get
                {
                    var instance = OperationContext.Current.Extensions.Find<WcfRequestState>();
                    if (instance == null)
                        OperationContext.Current.Extensions.Add((instance = new WcfRequestState()));
                    return instance;
                }
            }

            public void Attach(OperationContext owner) {}

            public void Detach(OperationContext owner){}
        }
        #endregion

        readonly ILocalStateInstance _state;
        static Func<ILocalStateInstance> _instanceResolver;
        static readonly object _instanceResolverLock = new object();

        public LocalState()
        {
            _state = ResolveStateInstance();
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
                                                 " instance to store.");
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

        static ILocalStateInstance ResolveStateInstance()
        {
            if (_instanceResolver == null)
            {
                lock (_instanceResolverLock)
                {
                    if (_instanceResolver == null)
                        _instanceResolver = () =>
                        {
                            if (OperationContext.Current != null)
                                return WcfRequestState.Current;
                            else if (HttpContext.Current != null)
                                return HttpRequestState.Current;
                            return ThreadLocalState.Current;
                        };
                }
            }
            return _instanceResolver();
        }
    }
}