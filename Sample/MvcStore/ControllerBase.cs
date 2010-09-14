using System.Web.Mvc;

namespace MvcStore
{
    public abstract class ControllerBase : Controller
    {
        protected ActionResult View<T>(T viewModel)
        {
            var viewName = typeof (T).Name.Replace("ViewModel", "");
            return View(viewName, viewModel);
        }
    }
}