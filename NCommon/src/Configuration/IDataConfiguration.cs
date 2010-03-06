using System.Data;

namespace NCommon.Configuration
{
    public interface IDataConfiguration
    {
        void Configure(IContainer container);
    }
}