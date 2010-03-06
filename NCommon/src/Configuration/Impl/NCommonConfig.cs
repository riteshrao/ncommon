using System;
using NCommon.Context;
using NCommon.State;
using NCommon.State.Impl;

namespace NCommon.Configuration.Impl
{
    ///<summary>
    ///</summary>
    public class NCommonConfig : INCommonConfig
    {
        readonly IContainer _container;

        ///<summary>
        ///</summary>
        ///<param name="container"></param>
        public NCommonConfig(IContainer container)
        {
            _container = container;
            InitializeDefaults();
        }

        void InitializeDefaults()
        {
            _container.Register<IContext, Context.Impl.Context>();
            _container.Register<ILocalStateSelector, DefaultLocalStateSelector>();
            _container.Register<ISessionStateSelector, DefaultSessionStateSelector>();
            _container.Register<ILocalState, LocalStateWrapper>();
            _container.Register<ISessionState, SessionStateWrapper>();
            _container.RegisterSingleton<IApplicationState, ApplicationState>();
        }

        ///<summary>
        ///</summary>
        ///<param name="config"></param>
        ///<returns></returns>
        public INCommonConfig ConfigureState(Action<IStateConfiguration> config)
        {
            config(new StateConfiguration(_container));
            return this;
        }

        ///<summary>
        ///</summary>
        ///<param name="config"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public INCommonConfig ConfigureData<T>(Action<T> config) where T : IDataConfiguration
        {
            var dataConfiguration = (IDataConfiguration) Activator.CreateInstance(typeof (T));
            dataConfiguration.Configure(_container);
            return this;
        }
    }
}