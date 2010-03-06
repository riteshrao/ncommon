using System.Web;

namespace NCommon.Context
{
    public interface IContext
    {
        bool IsWebApplication { get; }
        bool IsWcfApplication { get; }
        bool IsAspNetCompatEnabled { get; }

        HttpContextBase HttpContext { get; }
        IOperationContext OperationContext { get; }
    }
}