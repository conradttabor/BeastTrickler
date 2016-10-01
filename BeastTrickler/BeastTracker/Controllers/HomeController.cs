using System.Web.Mvc;

namespace BeastTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["ForwardingAddress"]);
        }
    }
}