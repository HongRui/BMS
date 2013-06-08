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
    public class BuildingController : BaseController
    {
        private OUContext db = new OUContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);

        }
        public ActionResult BuildingList()
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult BuildingListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Json(new Result { obj = "没有权限" });
            List<object> parameters = new List<object>();
            string sql =OUDAL.BuildingView.sql;

            Utilities.AddSqlFilterLike(collection, "name", ref sql, "b.name", parameters);
            Utilities.AddSqlFilterEqualInt(collection,"company",ref sql,"b.companyid");
            if (!string.IsNullOrEmpty(sidx) && !sidx.Contains(';') && !sord.Contains(';'))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<OUDAL.BuildingView>(sql, param: dynamicParams);
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
        public ActionResult BuildingView(int id)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Redirect("~/content/AccessDeny.htm");
            Building s = db.Buildings.Find(id);
            db.Database.Connection.Open();
            ViewBag.Rooms = db.Database.Connection.Query<OUDAL.RoomView>(OUDAL.RoomView.sql+" where r.buildingid="+id.ToString()).ToList();
            return View(s);
        }

        [Authorize]
        public ActionResult BuildingEdit(int id)
        {
            Building s = db.Buildings.Find(id);
            if (s == null)
            {
                s = new Building();
            }
            if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        [HttpPost]
        public ActionResult BuildingEdit(int id, FormCollection collection)
        {
            Building s = db.Buildings.Find(id);
            if (s == null)
            {
                s = new Building();
                db.Buildings.Add(s);
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new string[] { }, new string[] { "" }, collection);
            if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Redirect("~/content/AccessDeny.htm");

            if (ModelState.IsValid)
            {

                db.SaveChanges();
                if (id == 0)
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Building.LogClass, "创建", "");

                }
                else
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Building.LogClass, "修改", "");
                }
                return Redirect("../BuildingView/" + s.Id);
            }

            return View(s);
        }

        [Authorize]
        [HttpPost]
        public JsonResult BuildingDelete(int id)
        {

            Models.Result result = new Models.Result();
            Building s = db.Buildings.Find(id); if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Json(new Result { obj = "没有权限" });
            var query = (from o in db.Rooms where o.BuildingId == id select o.Id).Count();
            if (query > 0)
            {
                result.obj = "已有房屋记录，不能删除";
            }
            else
            {
                db.Buildings.Remove(s);
                db.SaveChanges(); BLL.Utilities.AddLogAndSave(s.Id, Building.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            }

            return Json(result);
        }
        public ActionResult RoomList()
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult RoomListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Json(new Result { obj = "没有权限" });
            List<object> parameters = new List<object>();
            string sql =OUDAL.RoomView.sql;

            Utilities.AddSqlFilterLike(collection, "name", ref sql, "a.name", parameters);
            Utilities.AddSqlFilterEqualInt(collection, "building", ref sql, "a.building");
            Utilities.AddSqlFilterInInts(collection, "state", ref sql, "a.state");
            if (!string.IsNullOrEmpty(sidx) && !sidx.Contains(';') && !sord.Contains(';'))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<OUDAL.RoomView>(sql, param: dynamicParams);
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
        public ActionResult RoomView(int id)
        {

            Room s = db.Rooms.Find(id);
            if (!UserInfo.CurUser.HasRight("租赁管理-项目查看")) return Redirect("~/content/AccessDeny.htm");
            return View(s);
        }

        [Authorize]
        public ActionResult RoomEdit(int id, int? building)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Redirect("~/content/AccessDeny.htm");
            Room s = null;
            if(id!=0)
            {s=db.Rooms.Find(id);
            }
            if (s == null)
            {
                s = new Room();
                s.BuildingId = (int) building;
            }
            return View(s);
        }

        [Authorize]
        [HttpPost]
        public ActionResult RoomEdit(int id, int? building, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Redirect("~/content/AccessDeny.htm");
            Room s = null;
            if (id != 0)
            {
                s = db.Rooms.Find(id);
            }
            if (s == null)
            {
                s = new Room();
                s.BuildingId = (int)building;
                db.Rooms.Add(s);
            }
            //collection.Remove("");
            TryUpdateModel(s, "", new string[] { }, new string[] { "" }, collection);

            if (ModelState.IsValid)
            {

                db.SaveChanges();
                if (id == 0)
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Room.LogClass, "创建", "");

                }
                else
                {
                    BLL.Utilities.AddLogAndSave(s.Id, Room.LogClass, "修改", "");
                }
                return Redirect(string.Format("../RoomView/{0}?reload=1" , s.Id));
            }

            return View(s);
        }

        [Authorize]
        [HttpPost]
        public JsonResult RoomDelete(int id)
        {

            Models.Result result = new Models.Result();
            Room s = db.Rooms.Find(id); if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Json(new Result { obj = "没有权限" });
            var query = (from o in db.RentContractRooms where o.RoomId == id select o.Id).Count();
            if (query > 0)
            {
                result.obj = "已有租赁记录，不能删除";
            }
            else
            {
                db.Rooms.Remove(s);
                db.SaveChanges(); BLL.Utilities.AddLogAndSave(s.Id, Room.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            }

            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public JsonResult GetRoomsByBuilding(int buildingid)
        {

            Models.Result result = new Models.Result();
            if (!UserInfo.CurUser.HasRight("租赁管理-项目维护")) return Json(new Result { obj = "没有权限" });
            var query =
                (from o in db.Rooms where o.BuildingId == buildingid select o).
                    ToList();
            result.success = true;
                result.obj = query;
            
            return Json(result);
        }
    }
}
