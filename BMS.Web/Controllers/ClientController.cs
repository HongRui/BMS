﻿using System;
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
    public class ClientController : BaseController
    {
        private OUContext db = new OUContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);

        }
        public ActionResult List()
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-客户查看")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult ListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-客户查看")) return Json(new Result { obj = "没有权限" });
            List<object> parameters = new List<object>();
            string sql =
                @"select * from Clients a ";

            Utilities.AddSqlFilterLike(collection, "name", ref sql, "a.name", parameters);

            if (!string.IsNullOrEmpty(sidx) && !sidx.Contains(';') && !sord.Contains(';'))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<Client>(sql, param: dynamicParams);
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

            Client s = db.Clients.Find(id);
            if (!UserInfo.CurUser.HasRight("租赁管理-客户查看")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Client s = db.Clients.Find(id);
            if (s == null)
            {
                s = new Client();
            }
            if (!UserInfo.CurUser.HasRight("租赁管理-客户维护")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Client s = db.Clients.Find(id);
            if (s == null)
            {
                s = new Client();
                db.Clients.Add(s);
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new string[] { }, new string[] { "" }, collection);
            if (!UserInfo.CurUser.HasRight("租赁管理-客户维护")) return Redirect("~/content/AccessDeny.htm");

            if (ModelState.IsValid)
            {

                db.SaveChanges();
                if (id == 0)
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Client.LogClass, "创建", "");

                }
                else
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Client.LogClass, "修改", "");
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
            Client s = db.Clients.Find(id); if (!UserInfo.CurUser.HasRight("租赁管理-客户维护")) return Json(new Result { obj = "没有权限" });
            var query = (from o in db.RentContracts where o.ClientId == id select o.Id).Count();
            if (query > 0)
            {
                result.obj = "已有合同记录，不能删除";
            }
            else
            {
                db.Clients.Remove(s);
                db.SaveChanges(); BLL.Utilities.AddLogAndSave(s.Id, Client.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            }

            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public JsonResult SearchClient(string name,int matchCount )
        {
            var list = (from o in db.Clients where o.Name.Contains(name) select o.Name).Take(matchCount).ToList();

            return Json(list);
        }
        [Authorize]
        [HttpPost]
        public JsonResult CheckClient(string name)
        {
            var c = (from o in db.Clients where o.Name == name select o).FirstOrDefault();
            Result result=new Result();
            if(c!=null)
            {
                result.success = true;
                result.obj = c.Id;
            }
            return Json(result);
        }
    }
}
