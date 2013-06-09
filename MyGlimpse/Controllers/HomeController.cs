using System;
using System.Threading;
using System.Web.Mvc;

namespace MyGlimpse.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            var random = new Random(1000);
            
            // simulate work of between 0 & 1 seconds
            Thread.Sleep(random.Next(1000));

            return View();
        }

    }
}
