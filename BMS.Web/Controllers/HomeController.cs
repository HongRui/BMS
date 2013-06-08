using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.BLL;
using OUDAL;
using BMS;
using System.Data.Objects.SqlClient;
namespace BMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private OUContext db = new OUContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Index()
        {
            return View(Request.Browser);
            
        }
        public ActionResult MallIndex(int mallid)
        {
            var outGuaranteeAsset =
                (from o in db.Assets
                 where SqlFunctions.DateAdd("M",1,o.GuaranteeDate)> DateTime.Today
                 select o).ToList();
            ViewBag.OutGuaranteeAsset = outGuaranteeAsset;
            var maintainAlert = (from o in db.AssetMaintains
                                 join p in db.Assets on o.AssetId equals p.Id
                                 where o.DoneDate == null && SqlFunctions.DateAdd("M", 1, o.PlanDate) > DateTime.Today
                                 select p).ToList();
            ViewBag.MaintainAlert = maintainAlert;
            return View(mallid);

        }
        public ActionResult About()
        {
            return View();
        }
        
    }
}
