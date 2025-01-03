using HomeShow.Models;
using System.Linq;
using System.Web.Mvc;

namespace HomeShow.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index(bool? Showhidden)
        {
            var mapping = Mapping.GetMappings();
            if (Showhidden != true)
                mapping = mapping.Where(x => x.Visible).ToList();
            return View(mapping);
        }
    }
}