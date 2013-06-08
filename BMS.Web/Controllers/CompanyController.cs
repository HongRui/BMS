using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.Models;
using BMS.Web.BLL;
using OUDAL;
using Dapper;
namespace BMS.Web.Controllers
{
    public class CompanyController : BaseController
    {
        private OUContext db = new OUContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);

        }
        public ActionResult List()
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult ListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Json(new Result { obj = "没有权限" });
            List<object> parameters = new List<object>();
            string sql =
                @"select * from Companies a ";

            Utilities.AddSqlFilterLike(collection, "name", ref sql, "a.name", parameters);

            if (!string.IsNullOrEmpty(sidx) && !sidx.Contains(';') && !sord.Contains(';'))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<Company>(sql, param: dynamicParams);
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
        public ActionResult View(int id)
        {

            Company s = db.Companies.Find(id);
            if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Company s = db.Companies.Find(id);
            if (s == null)
            {
                s = new Company();
            }
            if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Company s = db.Companies.Find(id);
            if (s == null)
            {
                s = new Company();
                db.Companies.Add(s);
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new string[] { }, new string[] { "" }, collection);
            if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Redirect("~/content/AccessDeny.htm");

            if (ModelState.IsValid)
            {

                db.SaveChanges();
                if (id == 0)
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Company.LogClass, "创建", "");

                }
                else
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Company.LogClass, "修改", "");
                }
                return Redirect("../View/" + s.Id);
            }

            return View(s);
        }

        [Authorize]
        [HttpPost]
        public JsonResult Delete(int id)
        {

            Models.Result result = new Models.Result();
            Company s = db.Companies.Find(id); if (!UserInfo.CurUser.HasRight("租赁管理-公司账号")) return Json(new Result { obj = "没有权限" });
            var query = (from o in db.RentIncomes where o.CompanyId == id select o.Id).Count();
            if (query > 0)
            {
                result.obj = "已有收款记录，不能删除";
            }
            else
            {
                db.Companies.Remove(s);
                db.SaveChanges(); BLL.Utilities.AddLogAndSave(s.Id, Company.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            }

            return Json(result);
        }
    }
}
