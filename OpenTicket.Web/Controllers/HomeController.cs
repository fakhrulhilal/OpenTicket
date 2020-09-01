using System.Web.Mvc;

namespace OpenTicket.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();
    }
}