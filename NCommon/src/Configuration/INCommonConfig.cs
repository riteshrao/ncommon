using System;

namespace NCommon.Configuration
{
    public interface INCommonConfig
    {
        INCommonConfig ConfigureState<T>(Action<T> actions) where T : IStateConfiguration, new();
        INCommonConfig ConfigureData<T>(Action<T> actions) where T : IDataConfiguration, new();
    }
}