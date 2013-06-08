using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.BLL;

namespace BMS.Web.Controllers
{
    public class DesktopController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
            
        }
       
        public ActionResult About()
        {
            return View();
        }
        
    }
}
