using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.Common;
using OUDAL;
using SupplierReg.Web.Models;
using SupplierReg.Web.BLL;
namespace SupplierReg.Web.Controllers
{
    public class SupplierController : Controller
    {
        private OUContext db = new OUContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
       
        [Authorize]
        public ActionResult SupplierView()
        {
             int id = 0;
                    int.TryParse(System.Web.HttpContext.Current.User.Identity.Name, out id);
                    OUContext db=new OUContext();
                    Supplier s = (from o in db.Suppliers where o.Id == id select o).FirstOrDefault();
            ViewBag.Attachments =
                (from o in db.Attachments where o.MasterType == Supplier.LogClass && o.MasterId == s.Id select o).ToList();
            
            
            return View(s);
        }

        [Authorize]
        public ActionResult SupplierEdit()
        {
            int id = 0;
            int.TryParse(System.Web.HttpContext.Current.User.Identity.Name, out id);
            OUContext db = new OUContext();
            Supplier s = (from o in db.Suppliers where o.Id == id select o).FirstOrDefault();
            if(s==null)
            {
                s=new Supplier();
            }
            return View(s);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SupplierEdit( FormCollection collection)
        {
            int id = 0;
            int.TryParse(System.Web.HttpContext.Current.User.Identity.Name, out id);
            OUContext db = new OUContext();
            Supplier s = (from o in db.Suppliers where o.Id == id select o).FirstOrDefault();
            if(s==null)
            {
                return View(s);
            }
            if(s.State==(int)SupplierState.往来)
            {
                return View("ShowError", "", "已是往来供应商，不能修改资料");
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new string[] { }, new string[] { "Name", "Password", "Level", "State","AuditRemark" ,"AccessCode"}, collection);           
            if (ModelState.IsValid)
            {
                    s.Save(db);
                    db.SaveChanges(); 
                    if(id==0)
                    {
                        BLL.Utilities.AddLog(s.Id,Supplier.LogClass, "创建", "");
                        
                    }
                    else
                    {
                        BLL.Utilities.AddLog(s.Id, Supplier.LogClass, "修改", "");
                    }
                return Redirect("SupplierView" );
            }
            
            return View(s);
        }
        public ActionResult SupplierRegister()
        {
            Supplier s = new Supplier();
            return View(s);
        }
        [HttpPost]
        public ActionResult SupplierRegister( FormCollection collection)
        {
            Supplier s = new Supplier();
            s.Level = (int) SupplierLevel.F;
            TryUpdateModel(s, "", new string[] { }, new string[] { "Level", "State" }, collection);
            s.Name = s.Name.Trim();
            if(collection["ConfirmPassword"]!=s.Password)
                {
                    ModelState.AddModelError("ConfirmPassword", "两个密码不一致");
                }
                if( (from o in db.Suppliers where o.Name==s.Name select o).Count()>0)
                {
                    ModelState.AddModelError("","供应商名称已存在");
                }
            if (ModelState.IsValid)
            {
                    s.Save(db);
                    db.SaveChanges();
                    BLL.Utilities.AddLog(s.Id, Supplier.LogClass, "创建", "");
                System.Web.Security.FormsAuthentication.SetAuthCookie(s.Id.ToString(),false);
                    return Redirect("SupplierView" );
               
            }
            return View(s);
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            Supplier s = new Supplier();
            return View(s);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            int id = 0;
            int.TryParse(System.Web.HttpContext.Current.User.Identity.Name, out id);
            Supplier s = db.Suppliers.Find(id);
            if (s == null)
                return View("ShowError", "", "找不到供应商");
            if (collection["password"] != collection["ConfirmPassword"])
            {
                ModelState.AddModelError("ConfirmPassword", "两个密码不一致");
            }
            if (collection["password"] == "")
            {
                ModelState.AddModelError("Password", "密码不能为空");
            }
            if (ModelState.IsValid)
            {
                s.Password = collection["password"];
                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            return View(new Supplier());
        }
    }
}