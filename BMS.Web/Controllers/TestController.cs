using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.BLL;
using BMS.Web.Models;
namespace BMS.Web.Controllers
{
    public class TestController : BaseController
    {
        //
        // GET: /TestModel/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AssetCatalogs()
        {
            return View();
        }
        
    }
}
