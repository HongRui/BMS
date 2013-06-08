using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OUDAL;
using BMS.Web.BLL;
using BMS.Web.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace BMS.Web.Controllers
{
    public class AssetCatalogController : BaseController
    {
        private OUContext db = new OUContext();
        [Authorize]
        public ActionResult Index()
        {
            //if (BLL.Utilities.CheckRight(Asset.ViewRole) == false)
            //{
            //    return BLL.Utilities.NoRight();
            //}
            
            return View();
        }
        public ActionResult CatalogView()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CatalogJson()
        {
            Models.Result result = new Models.Result();
            var list = from o in db.AssetCatalogs select new TreeNode { id = o.Id, name = o.Name, pId = o.PId, tag = o.CatalogType };
            result.success = true;
            result.obj = list;

            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateCatalog(int id, FormCollection collection)
        {
            Models.Result result = new Models.Result();
            var c = (from o in db.AssetCatalogs where o.Id == id select o).FirstOrDefault();
            if (c == null)
            {
                c = new AssetCatalog();
                c.PId = int.Parse(collection["pid"]);
                db.AssetCatalogs.Add(c);
            }
            c.Name = collection["name"];
            c.CatalogType = collection["type"];
            if (c.CatalogType == null) c.CatalogType = "";
            db.SaveChanges();
            result.success = true;
            result.obj = c;
            AssetCatalogBLL.Update();
            return Json(result);
        }
        [HttpPost]
        public JsonResult DeleteCatalog(int id, FormCollection collection)
        {
            Models.Result result = new Models.Result();
            var assetCatalog = (from o in db.AssetCatalogs where o.Id == id select o).FirstOrDefault();
            if (assetCatalog == null)
            {
                result.success = false;
                result.obj = "找不到节点";
            }
            else
            {
                bool candelete = true;
                if ((from o in db.AssetCatalogs where o.PId == id select o).Count() > 0)
                {
                    candelete = true;
                }
                if ((from o in db.Materials where o.CatalogId == id select o).Count() > 0)
                {
                    candelete = true;
                }
                if (!candelete)
                {
                    result.success = false;
                    result.obj = "要删除的节点有子节点或已有物资为此物料类型，不能删除";
                }
                else
                {
                    //db.Database.ExecuteSqlCommand(string.Format("delete departmentusers where departmentid={0}", id));
                    db.AssetCatalogs.Remove(assetCatalog);
                    db.SaveChanges();
                    result.success = true;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateCatalogParent(int id, int pid)
        {
            if (!UserInfo.CurUser.HasRight("系统管理-部门管理")) return Json(new Result { success = false, obj = "没有权限" });
            Models.Result result = new Models.Result();
            var assetCatalog = (from o in db.AssetCatalogs where o.Id == id select o).FirstOrDefault();
            if (assetCatalog == null)
            {
                result.success = false;
                result.obj = "找不到节点";
            }
            else if (DepartmentBLL.IsParnet(id, pid))
            {
                result.success = false;
                result.obj = "不能拖动到子节点下";
            }
            else
            {
                assetCatalog.PId = pid;
                db.SaveChanges();
                DepartmentBLL.UpdateDepartments();
                result.success = true;
            }
            AssetCatalogBLL.Update();
            return Json(result);
        }

    }
}
