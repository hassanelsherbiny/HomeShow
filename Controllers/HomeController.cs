using HomeShow.Models;
using System.Web.Mvc;

namespace HomeShow.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
          
            return View( Mapping.GetMappings());
        }
    }
}