using NCommon.Configuration;

namespace MvcStore.Common
{
    public interface IStoreConfiguration
    {
        IDataConfiguration Create();
    }
}