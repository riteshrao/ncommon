using System;
using NCommon.Configuration;
using NCommon.Configuration.Impl;

namespace NCommon
{
    public static class Configure
    {
        public static INCommonConfig Using(IContainer container)
        {
            return new NCommonConfig(container);
        }
    }
}