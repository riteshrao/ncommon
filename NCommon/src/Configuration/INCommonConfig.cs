using System;

namespace NCommon.Configuration
{
    public interface INCommonConfig
    {
        INCommonConfig ConfigureState(Action<IStateConfiguration> config);
        INCommonConfig ConfigureData<T>(Action<T> config) where T : IDataConfiguration;
    }
}