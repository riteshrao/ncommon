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
            
        }

        ///<summary>
        ///</summary>
        ///<param name="actions"></param>
        ///<returns></returns>
        public INCommonConfig ConfigureState<T>(Action<T> actions) where T : IStateConfiguration, new()
        {
            var configuration = (T) Activator.CreateInstance(typeof (T));
            actions(configuration);
            configuration.Configure(_containerAdapter);
            return this;
        }

        ///<summary>
        ///</summary>
        ///<param name="actions"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public INCommonConfig ConfigureData<T>(Action<T> actions) where T : IDataConfiguration, new()
        {
            var dataConfiguration = (T) Activator.CreateInstance(typeof (T));
            actions(dataConfiguration);
            dataConfiguration.Configure(_containerAdapter);
            return this;
        }
    }
}