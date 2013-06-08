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
using BMS.Web.Models;
using BMS.Web.BLL;
using Dapper;
namespace BMS.Web.Controllers
{
    public class SupplierController : BaseController
    {
        private OUContext db = new OUContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult SupplierList()
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商查看"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            return View();
        }
        [HttpPost]
        public JsonResult SupplierListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商查看"))
            {
                return Json("没有权限");
            }
            List<object> parameters = new List<object>();
            string sql =
                @"select * from suppliers a 
where 1=1";

            Utilities.AddSqlFilterInInts(collection,"State[]",ref sql,"a.state");
            Utilities.AddSqlFilterInInts(collection, "Level[]", ref sql, "a.level");
            Utilities.AddSqlFilterLike(collection,"name",ref sql,"a.name",parameters);
            if (collection["catalog"] != null)
            {
                string[] strCatalogs = collection["catalog"].Split(',');
                //ToDo:需要增加类型的条件
            }
           
            if (!string.IsNullOrEmpty(sidx))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<ViewSupplier>(sql, param: dynamicParams);
            var list = query.ToList();
            int totalrow = list.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in list
                            select o).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            newquery.ForEach(o =>
                                 {
                                    
            });
            var jsonData = new
            {
                total = pagenum,
                page = page,
                records = totalrow,
                rows = newquery
            };
            return Json(jsonData);
        }
        
        
      
        [Authorize]
        public ActionResult SupplierView(int id,string code)
        {
            Supplier s = db.Suppliers.Find(id);
            if(s==null)
            {
                return View("ShowError", "", "找不到供应商");
            }
            if(s.AccessCode!=code)
            {
                if (!UserInfo.CurUser.HasRight("供应商管理-供应商查看"))
                {
                    return Redirect("~/content/AccessDeny.htm");
                }
            }
            ViewBag.Attachments =
               (from o in db.Attachments where o.MasterType == Supplier.LogClass && o.MasterId == s.Id select o).ToList();
            return View(s);
        }

        [Authorize]
        public ActionResult SupplierEdit(int id)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商资料修改"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            Supplier s = db.Suppliers.Find(id);
            if(s==null)
            {
                s=new Supplier();
            }
            return View(s);
        }

       
        [Authorize]
        [HttpPost]
        public ActionResult SupplierEdit(int id,  FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商资料修改"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            Supplier s = db.Suppliers.Find(id);
            if(s==null)
            {
                s = new Supplier ();
                db.Suppliers.Add(s);
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new  string[]{}, new string[] { "Password","State"},collection);           
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
                return Redirect("../SupplierView/" + s.Id);
            }
            
            return View(s);
        }
        [Authorize]
        public ActionResult ChangeLevel(int id)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商评级"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            Supplier s = db.Suppliers.Find(id);
            if (s == null)
            {
                return View("ShowError", "", "找不到供应商");
            }
            return View(s);
        }


        [Authorize]
        [HttpPost]
        public ActionResult ChangeLevel(int id, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商评级"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            Supplier s = db.Suppliers.Find(id);
            if (s == null)
            {
                return View("ShowError", "", "找不到供应商");
            }
            int oldLevel = s.Level;
            int newLevel = 0;
            if(!int.TryParse(collection["Level"],out newLevel))
            {
                return View("ShowError", "", "错误的等级参数");
            }
            if(oldLevel!=newLevel)
            {
                s.Level=newLevel;
                db.SaveChanges();
                BLL.Utilities.AddLog(s.Id, Supplier.LogClass,"供应商等级变更",Enum.GetName(typeof(SupplierLevel), oldLevel)+"=>"+ Enum.GetName(typeof(SupplierLevel), newLevel) );     
            }
            return Redirect("~/content/close.htm");
        }
        
        [Authorize]
        [HttpPost]
        public JsonResult SetState(int id, int state)
        {

            Models.Result result = new Models.Result();
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商评级"))
            {
                result.obj = "没有权限";
                return Json(result);
            }
            Supplier s = db.Suppliers.Find(id);
            if (s == null)
            {
                result.success = false;
                result.obj = ("找不到供应商 id=" + id);
            }
            
                if (!Enum.IsDefined(typeof(SupplierState), state))
                {
                    result.success = false;
                    result.obj = "处置参数异常，不能处置";
                }
                if(s.State!=state)
                {
                    int oldState=s.State;
                    s.State = state;
                    db.SaveChanges();
                    BLL.Utilities.AddLog(s.Id, Supplier.LogClass,"供应商状态修改",Enum.GetName(typeof(SupplierState), oldState)+"=>"+ Enum.GetName(typeof(SupplierState), state) );
                    result.success = true;
                    result.obj = "设置成功";
                }
                else
                {
                    result.success = true;
                    result.obj = "没有变化";
                }
            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            Models.Result result = new Models.Result();
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商资料修改"))
            {
                result.obj = "没有权限";
                return Json(result);
            }
            Supplier s = db.Suppliers.Find(id);
                  db.Suppliers.Remove(s);
                  db.SaveChanges(); BLL.Utilities.AddLog(s.Id, Supplier.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            return Json(result);
        }
        public ActionResult ChangePassword(int id)
        {
            Supplier s=new Supplier();
            return View(s);
        }
        [HttpPost]
        public ActionResult ChangePassword(int id,FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("供应商管理-供应商资料修改"))
            {
                return Redirect("~/content/AccessDeny.htm");
            }
            Supplier s = db.Suppliers.Find(id);
            if (s == null)
                return View("ShowError", "", "找不到供应商");
            if(collection["password"]!=collection["ConfirmPassword"])
            {
                ModelState.AddModelError("ConfirmPassword","两个密码不一致");
            }
            if (collection["password"] =="")
            {
                ModelState.AddModelError("Password", "密码不能为空");
            }
            if(ModelState.IsValid)
            {
                s.Password = collection["password"];
                AccessLog.AddLog(db,UserInfo.CurUser.Name,s.Id,Supplier.LogClass,"变更密码","");
                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            return View(new Supplier());
        }
    }
}