using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SupplierReg.Web.Models;
using OUDAL;
namespace SupplierReg.Web.Controllers
{
    public class AccountController : Controller
    {

        

        protected override void Initialize(RequestContext requestContext)
        {
            
           

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                OUContext db=new OUContext();
                OUDAL.Supplier supplier =
                    (from o in db.Suppliers where model.UserName == o.Name && model.Password == o.Password select o).
                        FirstOrDefault();

                if (supplier != null)
                {
                    FormsAuthentication.SetAuthCookie(supplier.Id.ToString(), false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("SupplierView", "Supplier");
                    }
                }

                else
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

       

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            HttpContext.Session["User"] = null;
            
            FormsAuthentication.SignOut();

            return RedirectToAction("SupplierView", "Supplier;");
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string password, string confirmPassword)
        {
            int id = BLL.UserInfo.CurUser.Id;
            if(password!=confirmPassword)
            {
                ModelState.AddModelError("confirmPassword","两次密码不一致");
                return View();
            }
            if(password.Trim()=="")
            {
                ModelState.AddModelError("password", "密码不能为空");
                return View();
            }
            using (OUContext db = new OUContext())
            {
                OUDAL.Supplier supplier = (from o in db.Suppliers where o.Id == id select o).First();
                if (supplier.Password == oldpassword)
                {
                    supplier.Password = password;
                    db.SaveChanges();
                    return Redirect("~/Supplier/SupplierView");
                }
                else
                {
                    ModelState.AddModelError("oldpassword", "旧密码错误");                    
                }
            }return View();
        }
        

    }
}
