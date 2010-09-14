using System.Web;
using System.Web.Mvc;
using Castle.Windsor;

namespace MvcStore
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            _container = container;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, System.Type controllerType)
        {
            if (controllerType == null)
                throw new HttpException(404, string.Format(
                    "The controller for path {0} could not be found or it does not implement IController.",
                    requestContext.HttpContext.Request.Path));
            return (IController) _container.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            _container.Release(controller);
        }
    }
}