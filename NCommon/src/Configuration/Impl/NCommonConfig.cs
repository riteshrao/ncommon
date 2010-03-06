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
        readonly IContainerAdapter _containerAdapter;

        ///<summary>
        ///</summary>
        ///<param name="containerAdapter"></param>
        public NCommonConfig(IContainerAdapter containerAdapter)
        {
            _containerAdapter = containerAdapter;
            InitializeDefaults();
        }

        void InitializeDefaults()
        {
            _containerAdapter.Register<IContext, Context.Impl.Context>();
            _containerAdapter.Register<ILocalStateSelector, DefaultLocalStateSelector>();
            _containerAdapter.Register<ISessionStateSelector, DefaultSessionStateSelector>();
            _containerAdapter.Register<ILocalState, LocalStateWrapper>();
            _containerAdapter.Register<ISessionState, SessionStateWrapper>();
            _containerAdapter.RegisterSingleton<IApplicationState, ApplicationState>();
        }

        ///<summary>
        ///</summary>
        ///<param name="config"></param>
        ///<returns></returns>
        public INCommonConfig ConfigureState(Action<IStateConfiguration> config)
        {
            config(new StateConfiguration(_containerAdapter));
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
            dataConfiguration.Configure(_containerAdapter);
            return this;
        }
    }
}