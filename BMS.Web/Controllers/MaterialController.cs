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
    public class MaterialController : BaseController
    {

        private OUContext db = new OUContext();

        
        public ActionResult MaterialList(int mallid)
        {
            return View(mallid);
        }
        [HttpPost]
        public JsonResult MaterialListQuery(string sidx, string sord, int page, int rows,int mallid, string Name)
        {

            //string s = string.Format(@" {0} where p.userid={1} ", ViewPlan.BaseSql, UserInfo.CurUser.Id);
            //s += ViewPlan.GetOrder(sidx, sord);
            //var query = db.Database.SqlQuery<ViewPlan>(s).ToArray();
            var query = (from o in db.Materials where o.MallId == mallid  select o);
            if (!string.IsNullOrEmpty(Name))
            {
                query = (from o in query where o.Name.Contains(Name) select o);
            }
            if(!string.IsNullOrEmpty(sidx))
            {
                if(sord=="desc")
                {
                    if(sidx=="Name")
                    {
                        query = (from o in query orderby o.Name descending select o);
                    }else if(sidx=="CatalogId")
                    {
                        query = (from o in query orderby o.CatalogId descending select o);
                    }else if(sidx=="DepartmentId")
                    {
                        query = (from o in query orderby o.DepartmentId descending select o);
                    }
                }
                else
                {
                    if (sidx == "Name")
                    {
                        query = (from o in query orderby o.Name  select o);
                    }
                    else if (sidx == "CatalogId")
                    {
                        query = (from o in query orderby o.CatalogId  select o);
                    }
                    else if(sidx=="DepartmentId")
                    {
                        query = (from o in query orderby o.DepartmentId select o);
                    }
                }
                
            }
            var list = query.ToList(); 
            int totalrow = list.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in list select new { id = o.Id, Name = o.Name, Spec = o.Spec,
                Remark=o.Remark,Department=DepartmentBLL.GetNameById(o.DepartmentId), Catalog = AssetCatalogBLL.GetFullNameById(o.CatalogId) }).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            
            var jsonData = new
            {
                total = pagenum,
                page = page,
                records = totalrow,
                rows = newquery
            };
            return Json(jsonData);
        }
        [HttpPost]
        public JsonResult GetMaterialName(int id)
        {
            var m = (from o in db.Materials where o.Id == id select o.Name).FirstOrDefault();
            if (m == null) m = "";
            return Json(m);
             
        }
        [HttpPost]
        public JsonResult GetMaterialByName(string name)
        {
            var m = (from o in db.Materials where o.Name.StartsWith(name) || o.Remark.StartsWith(name) select o).FirstOrDefault();
            //if (m == null) m = "";
            return Json(m);

        }
        public ActionResult MaterialSelectList(int mallid)
        {
            return View(mallid);
        }
        
        public ActionResult MaterialView(int id, int mallid)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.Materials where o.Id == id select o).FirstOrDefault();

            if (r == null)
            {
                r = new Material();
            }

            return View(r);
        }
        public ActionResult MaterialEdit(int id, int mallid)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.Materials where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new Material();
            }
            return View(r);
        }
        [HttpPost]
        public ActionResult MaterialEdit(int id, int mallid, string departmentid,string name, int catalogId, string spec, string remark, FormCollection collection)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            Material r = (from o in db.Materials where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new Material { MallId = mallid }; db.Materials.Add(r);
            }
            if (name.Length == 0)
            {
                ModelState.AddModelError("Name", "名称不能为空");
            }
            int departmentId;
            if(!int.TryParse(departmentid,out departmentId))
            {
                ModelState.AddModelError("DepartmentId","部门不能为空");
            }
            r.Name = name;
            r.CatalogId = catalogId;
            r.Spec = spec;
            r.Remark = remark;
            r.DepartmentId = departmentId;
            if (ModelState.IsValid == false)
            {
                return View(r);
            }
            else
            {

                db.SaveChanges();

                if (id == 0)
                {
                    return Redirect("../MaterialView/" + r.Id);
                }
                else
                {
                    return Redirect("~/content/close.htm");
                }
            }
            
        }
        [HttpPost]
        public JsonResult MaterialDelete(int id, FormCollection collection)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-部门管理")) return Json(new Result { success = false, obj = "没有权限" });
            Models.Result result = new Models.Result();
            result.success = false;
            string checksql1 = "select top 1 1 from materialinitems where materialid={0} union select top 1 1 from materialoutitems where materialid={0}";
            if(db.Database.SqlQuery<int>(string.Format(checksql1,id)).Count()>0)
            {
                
                result.obj = "有入库或出库单使用过此物资名称，不能删除";
            }
            else
            {
                Material m = (from o in db.Materials where o.Id == id select o).FirstOrDefault();
                if(m==null)
                {
                    result.obj = "找不到物资名称";
                }
                else
                {
                    db.Materials.Remove(m);
                    db.SaveChanges();
                    result.success = true;
                }
            }
            return Json(result);
        }
        #region In
        public ActionResult MaterialInView(int id, int mallid,string act)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.MaterialIns where o.Id == id select o).FirstOrDefault();

            if (r == null)
            {
                r = new MaterialIn();
            }
            ViewBag.Items = (from o in db.MaterialInItems where o.MaterialId == id select o).ToList();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                var items =
                    conn.Query(
                        string.Format("select m.DepartmentId, m.Name,m.CatalogId,i.UnitPrice,i.Num from materialinitems i join materials m on i.materialid=m.id where i.inid={0}", r.Id));
                ViewBag.Items2 = items;
            }
            //ViewBag.Items1 = (from o in db.MaterialInItems select new { UnitPrice = o.UnitPrice, Num = o.Num }).ToList();
            if (act == "print")
                return View("MaterialInPrint", r);
            return View(r);
        }
        public ActionResult MaterialInEdit(int id, int mallid)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.MaterialIns where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new MaterialIn();
            }
            List<MaterialInItem> items = (from o in db.MaterialInItems where o.InId == id select o).ToList();
            ViewBag.ItemsString = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            return View(r);
        }

        [HttpPost]
        public ActionResult MaterialInEdit(int id, int mallid, string itemsString, FormCollection collection)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            MaterialIn r = (from o in db.MaterialIns where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new MaterialIn {MallId = mallid}; db.MaterialIns.Add(r);
            }
            MaterialIn v=new MaterialIn();
            TryUpdateModel(r, collection);
            r.PersonId = UserInfo.CurUser.Id;
            List<MaterialInItem> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MaterialInItem>>(itemsString);
            foreach (var materialInItem in items)
            {
                if (materialInItem.MaterialId == 0) ModelState.AddModelError("", "请选择物料");
            }
            if(items.Count==0)
            {
                ModelState.AddModelError("", "入库单的物料明细不能为空");
            }
            if (ModelState.IsValid == false)
            {
                ViewBag.ItemsString = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                return View(r);
            }
            else
            {
                db.SaveChanges();
                (from o in db.MaterialInItems where o.InId == r.Id select o).ToList().ForEach(o => db.MaterialInItems.Remove(o));
                foreach (var item in items)
                {
                    item.InId = r.Id;
                    db.MaterialInItems.Add(item);
                }
                db.SaveChanges();
                return Redirect("../MaterialInView/" + r.Id);

            }

        }
        [HttpPost]
        public JsonResult MaterialInDelete(int id, FormCollection collection)
        {
            Models.Result result = new Models.Result();
            var items = (from o in db.MaterialInItems where o.InId == id select o).ToList();
            items.ForEach(o=>db.MaterialInItems.Remove(o));
            var materialIn = (from o in db.MaterialIns where o.Id == id select o).FirstOrDefault();
            if (materialIn != null) db.MaterialIns.Remove(materialIn);
            db.SaveChanges();
            result.success = true;
            return Json(result);
        }
        public ActionResult MaterialInList(int mallid)
        {
            return View();
        }
        [HttpPost]
        public JsonResult MaterialInListQuery(string sidx, string sord, int page, int rows, int mallid,FormCollection collection)
        {
            List<object> parameters = new List<object>();
            string sql =ViewMaterialIn.sql+" where i.mallid=" + mallid.ToString();
            Utilities.AddSqlFilterEqual(collection,"name",ref sql,"u.name",parameters);
            Utilities.AddSqlFilterLike(collection, "materialname", ref sql, "m.name", parameters);
            Utilities.AddSqlFilterInInts(collection,"departments",ref sql,"d.id");
            Utilities.AddSqlFilterDateGreaterThen(collection,"DateFrom",ref sql,"i.InDate",parameters);
            Utilities.AddSqlFilterDateLessThen(collection, "DateTo", ref sql, "i.InDate", parameters);
            if (!string.IsNullOrEmpty(sidx))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
                

            } 
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o=> { var p = o as SqlParameter;dynamicParams.Add(p.ParameterName,p.Value, p.DbType); });
            var query = db.Database.Connection.Query<Models.ViewMaterialIn>(sql, param: dynamicParams);
            
            var list = query.ToList();
            int totalrow = list.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in list
                            select o).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            newquery.ForEach(o=>
                                 {
                                     o.InType = Utilities.EnumToString(typeof (MaterialInType), (string) o.InType);
                                     ;
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
        #endregion

        #region Out
        public ActionResult MaterialOutView(int id, int mallid,string act)
        {
            //if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.MaterialOuts where o.Id == id select o).FirstOrDefault();

            if (r == null)
            {
                r = new MaterialOut();
            }
            ViewBag.Items = (from o in db.MaterialOutItems where o.MaterialId == id select o).ToList();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                var items =
                    conn.Query(
                        string.Format("select m.Name,m.CatalogId,i.Num from MaterialOutitems i join materials m on i.materialid=m.id where i.outid={0}", r.Id));
                ViewBag.Items2 = items;
            }
            ViewBag.Items1 = (from o in db.MaterialOutItems select new {  Num = o.Num }).ToList();
            if (act=="print")
                return View("MaterialOutPrint",r);
            return View(r);
        }
        public ActionResult MaterialOutEdit(int id, int mallid)
        {
           // if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            var r = (from o in db.MaterialOuts where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new MaterialOut();
            }
            List<MaterialOutItem> items = (from o in db.MaterialOutItems where o.OutId == id select o).ToList();
            var editItems=(from o in items select new MaterialOutItemEdit {Item=o,Msg=""}).ToList();
            ViewBag.ItemsString = Newtonsoft.Json.JsonConvert.SerializeObject(editItems);
            return View(r);
        }

        [HttpPost]
        public ActionResult MaterialOutEdit(int id, int mallid, string itemsString, FormCollection collection)
        {
           // if (!UserInfo.CurUser.HasRight("系统管理-用户管理")) return Redirect("~/content/AccessDeny.htm");
            MaterialOut r = (from o in db.MaterialOuts where o.Id == id select o).FirstOrDefault();
            if (r == null)
            {
                r = new MaterialOut { }; db.MaterialOuts.Add(r);
            }
            
            TryUpdateModel(r, collection);
            r.MallId = mallid;
            
            if (r.DepartmentId == 0)
            {
                ModelState.AddModelError("DepartmentId", "请选择部门");
            }
            if(r.PersonId==0)
            {
                ModelState.AddModelError("PersonId", "请选择人员");
            }
            List<MaterialOutItemEdit> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MaterialOutItemEdit>>(itemsString);
            foreach (var materialOutItem in items)
            {
                if (materialOutItem.Item.MaterialId == 0) ModelState.AddModelError("", "请选择物料");
                decimal inNum = db.Database.SqlQuery<decimal>(string.Format("select isnull(sum(num),0) from materialinitems where materialid={0}", materialOutItem.Item.MaterialId)).First();
                
                decimal outNum=db.Database.SqlQuery<decimal>(string.Format("select isnull(sum(num),0) from materialoutitems where materialid={0} and outid!={1}", materialOutItem.Item.MaterialId, r.Id)).First();
                
                if (inNum - outNum - materialOutItem.Item.Num < 0)
                {
                    ModelState.AddModelError("", "出库数量不能大于库存数");
                    materialOutItem.Msg = "!";
                }
            }
            if (ModelState.IsValid == false)
            {
                ViewBag.ItemsString = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                return View(r);
            }
            else
            {
                db.SaveChanges();
                (from o in db.MaterialOutItems where o.OutId == r.Id select o).ToList().ForEach(o => db.MaterialOutItems.Remove(o));
                foreach (var item in items)
                {
                    item.Item.OutId = r.Id;
                    db.MaterialOutItems.Add(item.Item);
                }
                db.SaveChanges();
                return Redirect("../MaterialOutView/" + r.Id);

            }

        }
        [HttpPost]
        public JsonResult MaterialOutDelete(int id, FormCollection collection)
        {
            Models.Result result = new Models.Result();
            var items = (from o in db.MaterialOutItems where o.OutId == id select o).ToList();
            items.ForEach(o => db.MaterialOutItems.Remove(o));
            var materialOut = (from o in db.MaterialOuts where o.Id == id select o).FirstOrDefault();
            if (materialOut != null) db.MaterialOuts.Remove(materialOut);
            db.SaveChanges();
            result.success = true;
            return Json(result);
        }
        public ActionResult MaterialOutList(int mallid)
        {
            return View();
        }
        [HttpPost]
        public JsonResult MaterialOutListQuery(string sidx, string sord, int page, int rows, int mallid, FormCollection collection)
        {
            List<object> parameters = new List<object>();
             string sql =
                @"select i.Id ,i.code,i.OutDate,i.Remark
,m.name,m.spec,item.num,d.Name as Department ,u.Name as UserName
from MaterialOuts i 
join materialoutitems item on i.id=item.outid
join materials m on item.materialid=m.id
join departments d on m.departmentid=d.id
join systemusers u on i.personid=u.id
where i.mallid=" +
                mallid.ToString();

            
            if (!string.IsNullOrEmpty(sidx))
            {
                
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            Utilities.AddSqlFilterLike (collection, "name", ref sql, "u.name", parameters);
            Utilities.AddSqlFilterInInts(collection, "departments", ref sql, "d.id");
            Utilities.AddSqlFilterLike(collection, "materialname", ref sql, "m.name", parameters);
            Utilities.AddSqlFilterDateGreaterThen(collection, "DateFrom", ref sql, "i.OutDate", parameters);
            Utilities.AddSqlFilterDateLessThen(collection, "DateTo", ref sql, "i.OutDate", parameters);
            
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<Models.ViewMaterialOut>(sql, param: dynamicParams);
            var list = query.ToList();
            int totalrow = list.Count();
            int pagenum = (totalrow - totalrow % rows) / rows + 1;
            var newquery = (from o in list
                            select o).Take(rows * page).Skip(page * rows - rows).ToList();// 这种写法是在内存中运算
            
            var jsonData = new
            {
                total = pagenum,
                page = page,
                records = totalrow,
                rows = newquery
            };
            return Json(jsonData);
        }
        #endregion
        #region Report
        public ActionResult Report(int mallid)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Report(int mallid ,FormCollection collection)
        {
            //加权平均法。因为没有保存期初数，统计时会出现本月期初数与上月期末数不一致的情况
            ViewData["DateFrom"] = collection["DateFrom"];
            ViewData["DateTo"] = collection["DateTo"];
            string error = "";
            DateTime d1;
            DateTime d2;
            if (!DateTime.TryParse(collection["DateFrom"], out d1) )
                error = "请输入起止日期";
            if( !DateTime.TryParse(collection["DateTo"], out d2))
                error = "请输入起止日期";
            if(error!="")
            {
                ViewBag.Error = error;
                return View();
            }
            string sql =
                @"declare @d1 datetime ,@d2 datetime
set @d1='{1}' ;set @d2='{2}'
select d.name as department, m.id, m.catalogid, m.Name,m.Spec,m.Remark 
,ii.num as innum ,ii.price as inprice
,oi.num as outnum
from Materials m 
left outer join (select ii.materialid, sum(ii.num) as num ,sum(ii.unitprice*ii.num) as price
from MaterialInItems ii join MaterialIns i on ii.InId=i.Id where i.InDate between @d1 and @d2 group by ii.MaterialId) 
as ii on m.Id=ii.MaterialId
left outer join( select oi.materialid,Sum(oi.num) as num from  MaterialOutItems oi 
	join MaterialOuts o on oi.OutId=o.Id where o.outDate between @d1 and @d2 group by oi.MaterialId)
as oi on m.Id=oi.MaterialId
join departments d on d.id=m.departmentid
where m.mallid={0}
";
            List<ViewMaterialReport> list=new List<ViewMaterialReport>();
            using(IDbConnection conn = db.Database.Connection)
            {
                conn.Open();
                var preList =
                    conn.Query<_ViewMaterialReport>(string.Format(sql, mallid, "1900-1-1",
                                                                 d1.AddDays(-1).ToShortDateString()));
                var perioList =
                    conn.Query<_ViewMaterialReport>(string.Format(sql, mallid, d1, d2));
                preList.ToList().ForEach(o=>
                                             {
                                                 list.Add(new ViewMaterialReport
                                                              {
                                                                  Id = o.Id,
                                                                  Department = o.Department,
                                                                  CatalogName =
                                                                      AssetCatalogBLL.GetFullNameById(o.CatalogId),
                                                                  preNum = o.InNum - o.OutNum,
                                                                  prePrice =
                                                                      o.InNum == 0
                                                                          ? 0
                                                                          : (o.InPrice/o.InNum)*(o.InNum - o.OutNum),
                                                                  Name = o.Name,
                                                                  Spec = o.Spec
                                                              });

                                             }
                    );
                perioList.ToList().ForEach(o=>
                                               {
                                                   ViewMaterialReport v = list.Find(p => p.Id == o.Id);
                                                   if(v==null)
                                                   {
                                                       v = new ViewMaterialReport
                                                               {
                                                                   CatalogName =
                                                                       AssetCatalogBLL.GetFullNameById(o.CatalogId),
                                                                   Id = o.Id,
                                                                   Department = o.Department,
                                                                   Name = o.Name,
                                                                   Spec = o.Spec
                                                               };
                                                       list.Add(v);
                                                   }
                                                   v.InNum = o.InNum;
                                                   v.InPrice = o.InPrice;
                                                   v.OutNum = o.OutNum;
                                                   v.LastNum = v.preNum + o.InNum - o.OutNum;
                                                   v.UnitPrice = (v.InNum+v.preNum)==0?0: (v.prePrice+v.InPrice)/(v.preNum+o.InNum);
                                                   v.OutPrice = v.OutNum * v.UnitPrice;
                                                   v.LastPrice = o.InPrice + v.prePrice-v.OutPrice;
                                               });
                
            }
            list = (from o in list where o.preNum != 0 || o.InNum != 0 select o).ToList();
            return View(list);
        }
        public ActionResult Report2(int mallid)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Report2(int mallid, FormCollection collection)
        {
//先进先出法统计
            ViewData["DateFrom"] = collection["DateFrom"];
            ViewData["DateTo"] = collection["DateTo"];
            string error = "";
            DateTime d1;
            DateTime d2;
            if (!DateTime.TryParse(collection["DateFrom"], out d1))
                error = "请输入起止日期";
            if (!DateTime.TryParse(collection["DateTo"], out d2))
                error = "请输入起止日期";
            if (error != "")
            {
                ViewBag.Error = error;
                return View();
            }
            string sqlMaterial = string.Format(@"select d.name as department, m.id, m.catalogid, m.Name,m.Spec,m.Remark 
from Materials m join departments d on d.id=m.departmentid
where m.mallid={0}", mallid);
            string sqlIn = string.Format("{0} where mallid={1} order by i.indate", MaterialDealDate.sqlIn, mallid);
            string sqlOut = string.Format("{0} where mallid={1} order by i.outdate", MaterialDealDate.sqlOut, mallid);
            using (IDbConnection conn = db.Database.Connection)
            {
                conn.Open();
                List<MaterialDealDate> inList = conn.Query<MaterialDealDate>(sqlIn).ToList();
                List<MaterialDealDate> outList = conn.Query<MaterialDealDate>(sqlOut).ToList();
                List<ViewMaterialReport> materialList = conn.Query<ViewMaterialReport>(sqlMaterial).ToList();
                
                //计算每笔出库金额
                foreach (var outDeal in outList)
                {
                    int inIndex = 0;
                    while(outDeal.Left>0&&inIndex<inList.Count)
                    {
                        MaterialDealDate inDeal = inList[inIndex];
                        if(inDeal.MaterialId!=outDeal.MaterialId)
                        {
                            inIndex++;continue;
                        }
                        if(outDeal.Left>=inDeal.Left)
                        {
                            outDeal.Price += inDeal.Left*inDeal.UnitPrice;
                            outDeal.Left -= inDeal.Left;
                            inDeal.Left = 0;
                            inIndex++;
                            if (outDeal.MaterialId == 9)
                                LogHelper.LogHelper.Info(string.Format("入库{0} {1} {2} - 出库 {3} {4}", inDeal.Num, inDeal.Left, inDeal.DealDate,outDeal.Num,outDeal.Left));
                        }
                        else
                        {
                            outDeal.Price += outDeal.Left*inDeal.UnitPrice;
                            inDeal.Left -= outDeal.Left;
                            outDeal.Left = 0;
                            if (outDeal.MaterialId == 9)
                                LogHelper.LogHelper.Info(string.Format("入库{0} {1} {2} - 出库 {3} {4}", inDeal.Num, inDeal.Left, inDeal.DealDate,outDeal.Num,outDeal.Left));
                        }
                    }
                }
                foreach (var deal in inList)
                {
                    if (deal.MaterialId == 9)
                        LogHelper.LogHelper.Info(string.Format("入库{0} {1} {2}", deal.Num, deal.Left, deal.DealDate));
                }
                foreach (var deal in outList)
                {
                    if (deal.MaterialId == 9) LogHelper.LogHelper.Info(string.Format("出库{0} {1} {2}", deal.Num, deal.Left, deal.DealDate));
                }

                //分别累计出入库
                foreach (var deal in inList)
                {
                    ViewMaterialReport vmr = null;
                    foreach (var viewMaterialReport in materialList)
                    {
                        if(viewMaterialReport.Id==deal.MaterialId)
                        {
                            vmr = viewMaterialReport;
                            break;
                        }
                    }
                    if(vmr!=null)
                    {
                        if(deal.DealDate<d1)
                        {
                            vmr.preNum += deal.Num;
                            vmr.prePrice += deal.Price;
                        }else if(deal.DealDate<=d2)
                        {
                            vmr.InNum += deal.Num;
                            vmr.InPrice += deal.Num;
                        }
                    }
                }
                foreach (var deal in outList)
                {
                    ViewMaterialReport vmr = null;
                    foreach (var viewMaterialReport in materialList)
                    {
                        if (viewMaterialReport.Id == deal.MaterialId)
                        {
                            vmr = viewMaterialReport;
                            break;
                        }
                    }
                    if (vmr != null)
                    {
                        if (deal.DealDate < d1)
                        {
                            vmr.preNum -= deal.Num;
                            vmr.prePrice -= deal.Price;
                        }
                        else if (deal.DealDate <= d2)
                        {
                            vmr.OutNum += deal.Num;
                            vmr.OutPrice += deal.Price;
                        }
                    }
                }
                List<ViewMaterialReport> newList=new List<ViewMaterialReport>();
                foreach (var vmr in materialList)
                {
                    if(vmr.InNum!=0||vmr.preNum!=0||vmr.OutNum!=0)
                    {
                        vmr.LastNum = vmr.preNum + vmr.InNum - vmr.OutNum;
                        vmr.LastPrice = vmr.prePrice + vmr.InPrice - vmr.OutNum;
                        if (vmr.LastNum != 0) vmr.UnitPrice = vmr.LastPrice/vmr.LastNum;
                        newList.Add(vmr);
                    }
                }
                return View(newList);
            }

            
        }
        
        public ActionResult ReportOutStoreDetail(int mallid)
        {
            return View();
        }
        [HttpPost]
        public ActionResult ReportOutStoreDetail(int mallid, FormCollection collection)
        {
            string error = "";
            DateTime d1;
            DateTime d2;
            ViewData["DateFrom"] = collection["DateFrom"];
            ViewData["DateTo"] = collection["DateTo"];
            
            if (!DateTime.TryParse(collection["DateFrom"], out d1))
                error = "请输入起止日期";
            if (!DateTime.TryParse(collection["DateTo"], out d2))
                error = "请输入起止日期";
            if (error != "")
            {
                ViewBag.Error = error;
                return View();
            }
            //这个是汇总的语句，现在改为按清单出，不再执行此语句
//            string sql =@"declare @d1 datetime ,@d2 datetime
//set @d1='{1}' ;set @d2='{2}'
//select d.id as departmentid,  d.name as department, m.id, m.catalogid, m.Name,m.Spec ,sum(oi.num) as outnum
//from Materials m 
//join MaterialOutItems oi on oi.MaterialId=m.Id
//join MaterialOuts o on oi.OutId=o.Id
//join departments d on d.id=o.DepartmentId
//where m.mallid={0}
//group by d.id,d.Name, m.Id,m.CatalogId,m.Name,m.Spec
//";
            string listSql = @"declare @d1 datetime ,@d2 datetime
set @d1='{1}' ;set @d2='{2}'
select d.id as departmentid, d.name as department,u.Name as username, m.catalogid, m.Name,m.Spec ,oi.num as outnum, o.id
,o.outdate,o.remark,(oi.num*m.totalprice/m.num) as totalprice
from (select m.Id,m.MallId,m.CatalogId,m.Name,m.Spec,case when i.num=0 then 1 else i.num end as num ,case when i.num=0 then 0 else i.totalprice end as totalprice from Materials m 
	left outer join  
	(select ii.MaterialId,sum(ii.Num) as num,sum(ii.num*ii.UnitPrice) as totalprice
		from MaterialInItems ii join MaterialIns i on ii.InId=i.Id
		where i.InDate<=@d2 group by ii.MaterialId) as i
	on m.Id=i.MaterialId) as m 
join MaterialOutItems oi on oi.MaterialId=m.Id
join MaterialOuts o on oi.OutId=o.Id
join departments d on d.id=o.DepartmentId
join SystemUsers u on u.Id=o.PersonId
where m.mallid={0} and o.outdate between @d1 and @d2
order by  d.name,u.name,m.name
";
            using (IDbConnection conn = db.Database.Connection)
            {
                conn.Open();
                var list =
                    conn.Query<ViewMaterialOutListReport>(string.Format(listSql, mallid, d1, d2)).ToList();
                if (collection["departments"] != "")
                {
                    string[] departStrs = collection["departments"].Split(',');
                    List<int> departmentIds = new List<int>();
                    foreach (var departStr in departStrs)
                    {
                        int id = 0;
                        if (int.TryParse(departStr, out id))
                        {
                            departmentIds.Add(id);
                        }
                    }

                    List<int> departFilter = new List<int>();
                    DepartmentBLL.GetDepartmentByParent(departmentIds).ForEach(o => departFilter.Add(o.Id));

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        ViewMaterialOutListReport item = list[i];
                        if (!departFilter.Contains(item.DepartmentId)) list.RemoveAt(i);
                    }
                }
                list.ForEach(o => { o.CatalogName = AssetCatalogBLL.GetFullNameById(o.CatalogId); });
                return View(list);
            }
            
        }
        public ActionResult ReportOutStore(int mallid)
        {
            return View();
        }
        [HttpPost]
        public ActionResult ReportOutStore(int mallid, FormCollection collection)
        {
            string error = "";
            DateTime d1;
            DateTime d2;
            ViewData["DateFrom"]= collection["DateFrom"];
            ViewData["DateTo"] = collection["DateTo"];
            if (!DateTime.TryParse(collection["DateFrom"], out d1))
                error = "请输入起止日期";
            if (!DateTime.TryParse(collection["DateTo"], out d2))
                error = "请输入起止日期";
            if (error != "")
            {
                ViewBag.Error = error;
                return View();
            }
            string sql = @"declare @d1 datetime ,@d2 datetime
set @d1='{1}' ;set @d2='{2}'
select d.id as departmentid,  d.name as department
, m.id, m.catalogid, m.Name,m.Spec ,sum(oi.num) as outnum,SUM(oi.num*m.totalprice/m.num) as totalprice
from (select m.Id,m.MallId,m.CatalogId,m.Name,m.Spec,i.num,i.totalprice from Materials m 
	left outer join  
	(select ii.MaterialId,sum(ii.Num) as num,sum(ii.num*ii.UnitPrice) as totalprice
		from MaterialInItems ii join MaterialIns i on ii.InId=i.Id
		where i.InDate<=@d2 group by ii.MaterialId) as i
	on m.Id=i.MaterialId) as m
join MaterialOutItems oi on oi.MaterialId=m.Id
join MaterialOuts o on oi.OutId=o.Id
join departments d on d.id=o.DepartmentId
where m.mallid={0} and o.outdate between @d1 and @d2
group by d.id,d.Name, m.Id,m.CatalogId,m.Name,m.Spec
order by d.id
            ";
            
            using (IDbConnection conn = db.Database.Connection)
            {
                conn.Open();
                var list =
                    conn.Query<ViewMaterialOutReport>(string.Format(sql, mallid, d1, d2)).ToList();
                if(collection["departments"]!="")
                {
                    string[] departStrs = collection["departments"].Split(',');
                    List<int> departmentIds=new List<int>();
                    foreach (var departStr in departStrs)
                    {
                        int id = 0;
                        if(int.TryParse(departStr,out id))
                        {
                            departmentIds.Add(id);
                        }
                    }

                    List<int> departFilter =new List<int>(); 
                    DepartmentBLL.GetDepartmentByParent(departmentIds).ForEach(o=>departFilter.Add(o.Id));

                    for (int i =list.Count-1; i >=0;i-- )
                    {
                        ViewMaterialOutReport item = list[i];
                        if(!departFilter.Contains(item.DepartmentId))list.RemoveAt(i);
                    }
                }
                
                list.ForEach(o => { o.CatalogName = AssetCatalogBLL.GetFullNameById(o.CatalogId); });
                return View(list);
            }

        }
        #endregion

    }
}
