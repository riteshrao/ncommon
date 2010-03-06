using System.ServiceModel.Activation;
using System.Web;

namespace NCommon.Context.Impl
{
    public class Context : IContext
    {
        public bool IsWebApplication
        {
            get
            {
                return HttpContext != null;
            }
        }

        public bool IsWcfApplication
        {
            get { return OperationContext != null; }
        }

        public bool IsAspNetCompatEnabled
        {
            get
            {
                if (!IsWcfApplication)
                    return false;
                var aspnetCompat = this.OperationContext.Host
                    .Description
                    .Behaviors
                    .Find<AspNetCompatibilityRequirementsAttribute>();

                return (aspnetCompat != null &&
                        (aspnetCompat.RequirementsMode == AspNetCompatibilityRequirementsMode.Allowed ||
                         aspnetCompat.RequirementsMode == AspNetCompatibilityRequirementsMode.Required) && IsWebApplication);
            }
        }

        public virtual HttpContextBase HttpContext
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return new HttpContextWrapper(System.Web.HttpContext.Current);
            }
        }

        public virtual IOperationContext OperationContext
        {
            get
            {
                if (System.ServiceModel.OperationContext.Current == null)
                    return null;
                return new OperationContextWrapper(System.ServiceModel.OperationContext.Current);
            }
        }
    }
}