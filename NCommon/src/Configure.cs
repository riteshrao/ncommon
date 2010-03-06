using System;
using NCommon.Configuration;
using NCommon.Configuration.Impl;

namespace NCommon
{
    public static class Configure
    {
        public static INCommonConfig Using(IContainerAdapter containerAdapter)
        {
            return new NCommonConfig(containerAdapter);
        }
    }
}