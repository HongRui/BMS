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

using System.Reflection;
namespace BMS.Web.Controllers
{
    public class AssetController : BaseController
    {
        private OUContext db = new OUContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult AssetList(int mallid)
        {
            return View(mallid);
        }
        [HttpPost]
        public ActionResult AssetListQuery(string sidx, string sord, int page, int rows, int mallid, FormCollection collection)
        {

            List<object> parameters = new List<object>();
            string sql =
                @"select a.id,a.catalogid,a.name,a.sn,a.supplier,a.assetno,a.spec,a.price,a.buydate,a.state,a.isbad,a.remark 
,au.Location,u.Name as username,u.id as userid
from Assets a 
left outer join AssetUsers au on a.Id=au.AssetId and au.EndDate is null
left outer join SystemUsers u on u.Id=au.UserId
where a.mallid=" +mallid.ToString();

            if (collection["name"] != null & collection["name"] != "")
            {
                parameters.Add(new SqlParameter("name", string.Format("%{0}%",collection["name"])));
                sql += string.Format(" and ( {1} like @{0} or {2} like @{0} )","name", "a.name", "a.spec");
            }
           
            Utilities.AddSqlFilterLike(collection, "assetno", ref sql, "a.assetno", parameters);
            Utilities.AddSqlFilterEqual(collection,"catalog",ref sql,"a.catalogid",parameters);
            Utilities.AddSqlFilterLike(collection, "supplier", ref sql, "a.supplier", parameters);
            Utilities.AddSqlFilterLike(collection, "sn", ref sql, "a.sn", parameters);
            Utilities.AddSqlFilterDateGreaterThen(collection, "buydatebegin", ref sql, "a.buydate", parameters);
            Utilities.AddSqlFilterDateLessThen(collection, "buydateend", ref sql, "a.buydate", parameters);
            Utilities.AddSqlFilterLike(collection, "area", ref sql, "au.area", parameters);
            Utilities.AddSqlFilterLike(collection, "user", ref sql, "u.name", parameters);

            Utilities.AddSqlFilterInInts(collection, "state", ref sql, "a.state");
            if(collection["isnouser"]!=null&collection["isnouser"]!="")
            {
                sql +=" and au.userid!=null";
            }
            Utilities.AddSqlFilterEqualInt(collection,"isbad",ref sql,"a.isbad");
            Utilities.AddSqlFilterInInts(collection,"assettype",ref sql,"a.assettype");
            if (!string.IsNullOrEmpty(sidx))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<ViewAssetList>(sql, param: dynamicParams);
            var list = query.ToList();
            int totalrow = list.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in list
                            select o).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            newquery.ForEach(o =>
                                 {
                                     o.Catalog = AssetCatalogBLL.GetFullNameById(o.CatalogId);
                                     o.StateName = Enum.GetName(typeof (AssetState), o.State);
                                     o.Department = DepartmentBLL.GetUserDepartmentString(o.UserId);
                ;
            });
            if (collection["toexcel"] == "true")
            {                
                ExcelHelper.Output(this.Response, newquery, "AssetList.xlsx");
                return Content("");
            }
            var jsonData = new
            {
                total = pagenum,
                page = page,
                records = totalrow,
                rows = newquery
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jsonData));
            //return Json(jsonData);
        }

        public string Test<T>(List<T> source)
        {
            //Type t = source[0].GetType();
            PropertyInfo p1 =typeof( T).GetProperty("Name");
            return Convert.ToString( p1.GetValue(source[0], null));

        }
        
        //
        // GET: /Asset/Details/5
        [Authorize]
        public ActionResult AssetView(int id, int mallid)
        {
           
            Asset asset = db.Assets.Find(id);
            var queryUser = from au in db.AssetUsers
                            join u in db.SystemUsers on au.UserId equals u.Id into tl_user
                            where au.AssetId == id
                            from u in tl_user.DefaultIfEmpty()
                            orderby au.BeginDate
                            select new BMS.Web.Models.AssetUserView
                            {
                                AssetId = au.AssetId
                                ,
                                Id = au.Id
                                ,
                                UserId = au.UserId
                                ,
                                Location = au.Location
                                ,
                                BeginDate = au.BeginDate
                                ,
                                EndDate = au.EndDate
                                ,
                                UserName =u.Name
                                ,
                                UseType = au.UseType
                            };
            ViewBag.UserLog = queryUser.ToList();
            var queryChangeLog = from c in db.AssetChangeLogs where c.AssetId == id orderby c.Id descending select c;
            ViewBag.ChangeLog = queryChangeLog.ToList();
            var maintains = from o in db.AssetMaintains where o.AssetId == id orderby o.PlanDate select o;
            ViewBag.Maintains = maintains;
            if (asset.State ==(int) AssetState.在用)
            {
                ViewBag.ButtonRetirement = true;
            }
            return View(asset);
        }

        
        //
        // GET: /Asset/Edit/5
        [Authorize]
        public ActionResult AssetEdit(int id, int mallid)
        {
            Asset asset = db.Assets.Find(id);
            if(asset==null)
            {
                asset=new Asset();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult AssetEdit(int id, int mallid,FormCollection collection)
        {
            Asset a = db.Assets.Find(id);
            if(a==null)
            {
                a = new Asset {MallId = mallid};
                db.Assets.Add(a);
            }
            //collection.Remove("");
            TryUpdateModel(a, "", new  string[]{}, new string[] {"Id", "MallId","Price","ReceiptNo","BuyDate","NCNo" },collection);
            if(a.CatalogId==0)
            {
                ModelState.AddModelError("CatalogId", "请选择类型");
            }
            if (ModelState.IsValid)
            {
                var query = from o in db.Assets where o.AssetNo == a.AssetNo && o.Id != id select o;
                if (query.Count() > 0)
                {
                    ModelState.AddModelError("AssetNo", "固资编号不能重复");

                }
                else
                {
                    db.SaveChanges(); BLL.Utilities.AddLog(a.Id, "设施设备", "修改", "");
                    //return RedirectToAction("Index");
                    if(id==0)
                    {
                        return Redirect("../AssetView/" + a.Id);
                    }
                    else
                    {
                        return Redirect("~/content/close.htm");
                    }
                    
                }
            }
            
            return View(a);
        }
        [Authorize]
        public ActionResult AssetEditFinance(int id, int mallid)
        {
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                asset = new Asset();
            }
            return View(asset);
        }
        [Authorize]
        [HttpPost]
        public ActionResult AssetEditFinance(int id, int mallid, FormCollection collection)
        {
            Asset a = db.Assets.Find(id);
            if (a == null)
            {
                return View("ShowError", "", "找不到设施设备");
            }
            //collection.Remove("");
            TryUpdateModel(a, "", new string[] { "Price", "ReceiptNo", "BuyDate", "NCNo"  },null, collection);
            
            if (ModelState.IsValid)
            {
                    db.SaveChanges(); BLL.Utilities.AddLog(a.Id, "设施设备", "财务信息修改", "");
                    return Redirect("~/content/close.htm");
            }
            Asset post = new Asset();
            TryUpdateModel(post);
            post.MallId = a.MallId;
            return View(post);
        }
        
        [Authorize]
        public ActionResult UserBook(int id, int? assetid)
        {
            AssetUser au; Asset asset;
            if (id == 0)
            {
                asset = db.Assets.Find(assetid);
                if (asset == null)
                {//ToDo:异常处理
                }
                au = new AssetUser();
                au.BeginDate = DateTime.Today;
                var query = from o in db.AssetUsers where o.AssetId == asset.Id && o.EndDate == null select o;
                AssetUser[] list = query.ToArray();

                if (list.Length > 0)
                {
                    //ViewBag.Error = "没有待归还的设备";
                    return View("ShowError", "", "设备已被领用");
                }
            }
            else
            {
                au = db.AssetUsers.Find(id);
                if (au == null)
                {
                    //ToDo:异常处理
                }
                asset = db.Assets.Find(au.AssetId);
            }
            
            ViewBag.asset = asset;
            ViewBag.Locations = 
            "";
            return View(au);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserBook(int id, int? AssetId, FormCollection collection)
        {

            AssetUser a = null;
            int assetid = 0; int userid = 0;
            int.TryParse(collection["userid"], out userid);
            DateTime beginDate = DateTime.MinValue;
            DateTime.TryParse(collection["BeginDate"], out beginDate);
            if (id == 0)
            {
                a = new AssetUser();
                assetid = (int)AssetId;
                a.AssetId = assetid;
                db.AssetUsers.Add(a);
            }
            else
            {
                a = db.AssetUsers.Find(id);
                if (a == null) throw new Exception("找不到设备领用记录");
                assetid = a.AssetId;
            }
            //delete 
            if (collection["act"] == "delete")
            {
                db.AssetUsers.Remove(a);
                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            //create or modify
            a.BeginDate = beginDate;
            a.Location = collection["Location"];
            if(userid!=0)
                a.UserId = userid;            
            a.UseType = collection["UseType"];
            if (a.UseType == null) a.UseType = "";
            Asset asset = db.Assets.Find(assetid);
            if (asset == null) throw new Exception("找不到设备");
            //var queryLastUser = from au in db.AssetUser where au.AssetId == assetid && au.EndDate == null select au;
            //List<AssetUser> list = queryLastUser.ToList();
            //if (list.Count != 0)
            //{
            //    foreach (AssetUser au in list) au.EndDate = beginDate.AddDays(-1);
            //}
            db.SaveChanges(); BLL.Utilities.AddLog(asset.Id, Asset.LogClass, "领用", "");
            ViewBag.Message = "ok";
            ViewBag.asset = asset;
            //return Redirect("~/content/close.htm");
            return Redirect("~/content/close.htm");
        }
        [Authorize]
        public ActionResult UserReturn(int id, int? assetid)
        {
            Asset asset;
            asset = db.Assets.Find(assetid);
            if (asset == null)
            {
                throw new Exception("没找到设备");
            }
            var query = from o in db.AssetUsers where o.AssetId == asset.Id && o.EndDate == null select o;
            AssetUser[] list = query.ToArray();

            if (list.Length == 0)
            {
                //ViewBag.Error = "没有待归还的设备";
                return View("ShowError", "", "没有待归还的设备");
            }
            AssetUser au = list[0];
            ViewBag.asset = asset;
            return View(au);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserReturn(int id, int? assetid, FormCollection collection)
        {
            Asset asset;
            asset = db.Assets.Find(assetid);
            if (asset == null)
            {
                throw new Exception("没找到设备");
            }
            var query = from o in db.AssetUsers where o.AssetId == asset.Id && o.EndDate == null select o;
            AssetUser[] list = query.ToArray();

            if (list.Length == 0)
            {
                return View("ShowError", "", "没有待归还的设备");
            }
            AssetUser au = list[0];
            DateTime enddate;
            if (DateTime.TryParse(collection["enddate"], out enddate) == false)
            {
                ModelState.AddModelError("enddate", "请填写归还日期");
                ViewBag.asset = asset;
                return View(au);
            }
            au.EndDate = enddate;
            db.SaveChanges();
            return Redirect("~/content/close.htm");
        }
        [Authorize]
        public ActionResult ChangeLog(int id, int? assetid)
        {
            AssetChangeLog au; Asset asset;
            if (id == 0)
            {
                asset = db.Assets.Find(assetid);
                if (asset == null)
                {//ToDo:异常处理
                }
                au = new AssetChangeLog();
                au.ChangeDate = DateTime.Today;
                au.Title = "其他";
            }
            else
            {
                au = db.AssetChangeLogs.Find(id);
                if (au == null)
                {
                    //ToDo:异常处理
                }
                asset = db.Assets.Find(au.AssetId);
            }
            ViewBag.asset = asset;
            
            return View(au);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangeLog(int id, int? AssetId, FormCollection collection)
        {

            AssetChangeLog a = null;
            int assetid = 0;
            DateTime ChangeDate = DateTime.MinValue;
            DateTime.TryParse(collection["ChangeDate"], out ChangeDate);
            if (id == 0)
            {
                a = new AssetChangeLog();
                assetid = (int)AssetId;
                a.AssetId = assetid;
                db.AssetChangeLogs.Add(a);
            }
            else
            {
                a = db.AssetChangeLogs.Find(id);
                if (a == null) throw new Exception("找不到变动记录");
                assetid = a.AssetId;
            }
            //delete 
            if (collection["act"] == "delete")
            {
                db.AssetChangeLogs.Remove(a);
                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            //create or modify
            Asset asset = db.Assets.Find(a.AssetId);
            //a.ChangeDate = ChangeDate;
            //a.Title = collection["Title"];
            //a.Detail = collection["Detail"];
            //a.UseType = collection["UseType"];
            TryUpdateModel(a, "", null, new string[] { "AssetId" });
            if (ModelState.IsValid)
            {
                switch (a.Title)
                {
                    case "故障":
                        asset.IsBad = 1;
                        break;
                    case "修复":
                        asset.IsBad = 0;
                        break;
                    case "其他":
                    default:
                        break;
                }
                db.SaveChanges();
                ViewBag.Message = "ok";
                ViewBag.asset = asset;
                return Redirect("~/content/close.htm");
            }
            else
            {
                ViewBag.asset = asset;
                return View(a);
            }


        }

        [Authorize]
        public ActionResult MaintainPlan(int id, int? assetid)
        {
            AssetMaintain au; Asset asset;
            if (id == 0)
            {
                asset = db.Assets.Find(assetid);
                if (asset == null)
                {//ToDo:异常处理
                }
                au = new AssetMaintain();
            }
            else
            {
                au = db.AssetMaintains.Find(id);
                if (au == null)
                {
                    //ToDo:异常处理
                }
                asset = db.Assets.Find(au.AssetId);
            }
            ViewBag.asset = asset;

            return View(au);
        }
        [Authorize]
        [HttpPost]
        public ActionResult MaintainPlan(int id, int? AssetId, FormCollection collection)
        {
            AssetMaintain a = db.AssetMaintains.Find(id);;
            if(a==null)
            {
                a = new AssetMaintain();
            }
            

            TryUpdateModel(a, "", new string[] { }, new string[] { "DoneDate", "DoneDetail" }, collection);
            if(ModelState.IsValid)
            {
                if(id==0)
                {
                    a.AssetId = (int) AssetId;
                    db.AssetMaintains.Add(a);
                }

                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            return View(a);
        }

        [Authorize]
        public ActionResult MaintainDone(int id)
        {
            AssetMaintain au=db.AssetMaintains.Find(id);
            if (au == null)
            {
                return View("ShowError", "找不到养护记录");
            }
            return View(au);
        }
        [Authorize]
        [HttpPost]
        public ActionResult MaintainDone(int id, FormCollection collection)
        {
            AssetMaintain a = db.AssetMaintains.Find(id);
            TryUpdateModel(a, "",  new string[] { "DoneDate", "DoneDetail" },new string[] { }, collection);
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return Redirect("~/content/close.htm");
            }
            return View(a);
        }
        [HttpPost]
        public JsonResult MaintainDelete(int id)
        {
            Models.Result result = new Models.Result();
            AssetMaintain a = db.AssetMaintains.Find(id);
            if(a==null)
            {
                result.obj = "找不到保养记录";
            }
            else if(a.DoneDate!=null)
            {
                result.obj = "养护操作已完成，不能删除";

            }
            else
            {
                db.AssetMaintains.Remove(a);
                db.SaveChanges();
                result.success = true;
            }
            return Json(result);
        }

        
        [Authorize]
        [HttpPost]
        public JsonResult Retirement(int id,int state)
        {
            Models.Result result = new Models.Result();

            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                result.success= false;
                result.obj = ("找不到设备 id=" + id);
            }
            else if (asset.State != (int)AssetState.在用)
            {
                result.success = false;
                result.obj = "设备不在在用状态，不能处置";
            }
            else
            {
                AssetState assetState;
                if (!Enum.IsDefined(typeof(AssetState), state))
                {
                    result.success = false;
                    result.obj = "处置参数异常，不能处置";
                }
                asset.State = state;
                db.SaveChanges();
                BLL.Utilities.AddLog(asset.Id, Asset.LogClass, Enum.GetName(typeof(AssetState),state), "");
                result.success = true;
                result.obj = "处置成功";
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult Normal(int id)
        {
            Models.Result result = new Models.Result();

            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                result.success= false;
                result.obj = ("找不到设备 id=" + id);
            }
            
            else
            {
                asset.State = (int)AssetState.在用;
                db.SaveChanges();
                BLL.Utilities.AddLog(asset.Id, Asset.LogClass, "恢复在用", "状态恢复为在用");
                result.success = true;
                result.obj = "设备恢复为在用状态";
            }
            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            Models.Result result = new Models.Result();
            Asset asset = db.Assets.Find(id);
            var query = from o in db.AssetUsers where o.AssetId == id select o;
            if (query.Count() > 0)
            {
                result.success= false;
                result.obj = "有用户领用记录，无法删除";
            }
            else
            {
                db.Assets.Remove(asset);
                db.SaveChanges(); BLL.Utilities.AddLog(asset.Id, Asset.LogClass, "删除", "");
                result.success = true;
                result.obj = "已删除";
            }
            return Json(result);
        }

        [Authorize]
        public ActionResult Transfer(int id)
        {
            Asset asset = db.Assets.Find(id);
            return View(asset);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Transfer(int id, FormCollection collection)
        {
            Asset asset = db.Assets.Find(id);
            int newcompany; int oldcompany = asset.MallId;
            int.TryParse(collection["newcompany"], out newcompany);
            asset.MallId = newcompany;
            BLL.Utilities.AddLog(id, Asset.LogClass, "资产转移", string.Format("从{0}转移到{1} {2}", Company.GetName(oldcompany), Company.GetName(newcompany), collection["remark"]));
            db.SaveChanges();
            return RedirectToAction("Details", new { id = asset.Id });
        }
        [Authorize]
        
        public JsonResult GetHelperString(string area,string catalog, int num)
        {
           
            return Json("",JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintUserSign(int id)
        {
            Asset asset = db.Assets.Find(id);
            ViewBag.UserLog=( from o in db.AssetUsers where o.AssetId == asset.Id && o.EndDate == null select o).FirstOrDefault();
            return View(asset);
        }

        /*----------------------User------------------------------*/
        public ActionResult UserIndex()
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult UserQuery(int mallid,string sidx, string sord, int page, int rows, FormCollection collection)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Json(new Result { success = false, obj = "没有权限" });
            List<object> parameters = new List<object>();
            string s = ViewUser.BaseSql;

            
            
            BLL.Utilities.AddSqlFilterLike(collection, "Name", ref s, "u.name", parameters);
            List<Department> departFilter = DepartmentBLL.GetDepartmentByParent(mallid);
            string mallStr = "0";
            departFilter.ForEach(o => mallStr += "," + o.Id.ToString());
            s += string.Format(" and u.id in (select userid from departmentusers du where departmentid in ({0})) ", mallStr);
            if (collection["departments"] != null && collection["departments"] != "")
            {
                if (!collection["departments"].Contains(";"))
                    s += string.Format(" and u.id in (select userid from departmentusers du where departmentid in ({0})) ", collection["departments"]);
            }
                    
            
            bool state1; bool state2;
            bool.TryParse(collection["State1"], out state1); bool.TryParse(collection["State2"], out state2);
            if (state1 ^ state2)
            {
                if (state1) s += " and u.state=0";
                if (state2) s += " and u.state=1";
            }
            string order = "";
            if (sidx != "")
            {
                order = sidx + " " + sord;
                s += " order by " + order;
            }
            var query = db.Database.SqlQuery<ViewUser>(s, parameters.ToArray()).ToArray();

            int totalrow = query.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in query select o).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            foreach (ViewUser u in newquery)
            {
                var q1 = (from o in db.DepartmentUsers from p in db.Departments where o.UserId == u.id && o.DepartmentId == p.Id select p.Name);
                foreach (string departname in q1)
                {
                    if (u.Departments == null)
                    {
                        u.Departments = departname;
                    }
                    else
                    {
                        u.Departments += "," + departname;
                    }
                }
            }
            //int records = newquery.Count();
            var jsonData = new
            {
                total = pagenum,
                page = page,
                records = totalrow,
                rows = newquery
            };
            return Json(jsonData);
        }
        public ActionResult PersonalInfo(int id)
        {
            SystemUser user = UserBLL.GetById(id);
            string sql =string.Format( "select a.Name as AssetName, a.AssetNo,a.Spec,a.SN,u.Location,u.BeginDate,u.AssetId from assets a join assetusers u on a.id=u.assetid where u.userid={0} and u.enddate is null",id);
            db.Database.Connection.Open();
            
            var query = db.Database.Connection.Query(sql);
            ViewBag.AssetList = query.ToList();
            return View(user);
        }
    }
}