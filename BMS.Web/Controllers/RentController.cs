using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.Models;
using BMS.Web.BLL;
using Newtonsoft.Json;
using OUDAL;
using Dapper;
using System.Transactions;
namespace BMS.Web.Controllers
{
    public class RentController : BaseController
    {
        private OUContext db = new OUContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);

        }
        public ActionResult ContractList()
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同查看")) return Redirect("~/content/AccessDeny.htm");
            return View();
        }
        [HttpPost]
        public JsonResult ListQuery(string sidx, string sord, int page, int rows, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同查看")) return Json(new Result { obj = "没有权限" });
            List<object> parameters = new List<object>();
            string sql =
                @"select * from RentContracts a ";

            Utilities.AddSqlFilterLike(collection, "name", ref sql, "a.name", parameters);

            if (!string.IsNullOrEmpty(sidx) && !sidx.Contains(';') && !sord.Contains(';'))
            {
                string pre = "";
                sql += string.Format(" order by {0} {1}", sidx, sord);
            }
            db.Database.Connection.Open();
            var dynamicParams = new DynamicParameters();
            parameters.ForEach(o => { var p = o as SqlParameter; dynamicParams.Add(p.ParameterName, p.Value, p.DbType); });
            var query = db.Database.Connection.Query<RentContract>(sql, param: dynamicParams);
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
        public ActionResult ContractView(int id)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同查看")) return Redirect("~/content/AccessDeny.htm");
            RentContract s = db.RentContracts.Find(id);
            return View(s);
        }
        ActionResult _ContractEdit(int id, int? room, FormCollection collection)
        {
            db.Database.Connection.Open();
List<IdName> buildings =
                db.Database.Connection.Query<IdName>("select id,name from buildings order by name").ToList();
            db.Database.Connection.Close();
            RentContract s = null;
            List<ViewRentContractRoom> rooms = null;
            List<RentContractPrice> prices = null;
            List<Removeable<RentContractPrice>> newprices = null;
            if(id!=0)
            {
                s=db.RentContracts.Find(id);
            }
            if (s == null)
            {
                s = new RentContract();
            }
            if (collection != null)
            {
                rooms = JsonConvert.DeserializeObject<List<ViewRentContractRoom>>(collection["rentRoomsString"]);
                newprices =
                    JsonConvert.DeserializeObject<List<Removeable<RentContractPrice>>>(collection["pricesString"]);
                TryUpdateModel(s);
                if (s.ClientId == 0)ModelState.AddModelError("ClientId","请选择客户");
                if(s.BillDay<1||s.BillDay>30)ModelState.AddModelError("BillDay","不在允许范围中");
                if(s.BillMonth<1)ModelState.AddModelError("BillMonth","不能小于1");
                bool vaildCheck = true;
                foreach (var removeable in newprices)
                {
                    if (!removeable.IsRemoved)
                    {
                        if (removeable.obj.BillBeginDate == DateTime.MinValue) vaildCheck = false;
                        if (removeable.obj.BillEndDate == DateTime.MinValue) vaildCheck = false;
                    }
                }
                foreach (var _room in rooms)
                {
                    var r = (from o in db.Rooms where o.Id == _room.RoomId select o).FirstOrDefault();
                    if (r == null)
                    {
                        ModelState.AddModelError("","租赁房屋不存在");
                    }
                    else
                    {
                        var rr = (from o in db.RentContractRooms
                                  where o.RoomId == _room.RoomId && o.ContractId != id &&
                                        (o.RentBeginDate >= s.RentBeginDate && o.RentBeginDate <= s.RentEndDate ||
                                         o.RentEndDate >= s.RentBeginDate && o.RentEndDate <= s.RentEndDate)
                                  select o).FirstOrDefault();
                        if (rr != null)
                        {
                            ModelState.AddModelError("","不能选择已出租房屋"+rr.RoomId);
                        }
                    }
                }
                if (vaildCheck == false)
                {
                    ModelState.AddModelError("","付款条款中，日期不能为空");
                }
                if (ModelState.IsValid)
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        //List<RentContractPrice> oldPrices =
                        //    (from o in db.RentContractPrices where o.ContractId == id select o).ToList();
                        if (s.Id == 0) db.RentContracts.Add(s);
                        db.SaveChanges();
                        foreach (var obj in newprices)
                        {
                            RentContractPrice price = null;
                            if (obj.obj.Id > 0)
                            {
                                price =
                                    (from o in db.RentContractPrices where o.Id == obj.obj.Id select o).FirstOrDefault();
                            }
                            else
                            {
                                price = new RentContractPrice {ContractId = s.Id};
                                db.RentContractPrices.Add(price);
                            }
                            if (obj.IsRemoved)
                            {
                                db.RentContractPrices.Remove(price);
                            }
                            else
                            {
                                price.BillBeginDate = obj.obj.BillBeginDate;
                                price.BillEndDate = obj.obj.BillEndDate;
                                price.Remark = obj.obj.Remark;
                                price.RentPrice = obj.obj.RentPrice;
                            }
                        }
                        foreach (var _room in rooms)
                        {
                            RentContractRoom rentRoom = null;
                            if (_room.Id > 0)
                            {
                                rentRoom =
                                    (from o in db.RentContractRooms where o.Id == _room.Id select o).FirstOrDefault();

                            }
                            else
                            {
                                rentRoom=new RentContractRoom{ContractId = s.Id,RoomId = _room.RoomId};
                                Room r = (from o in db.Rooms where o.Id == rentRoom.RoomId select o).FirstOrDefault();
                                rentRoom.RentDim = r.RentDim;
                                rentRoom.Dim = r.Dim;
                                //仅仅是新增房屋才变更rentdim字段
                                db.RentContractRooms.Add(rentRoom);
                            }
                            if (_room.IsRemoved)
                                {
                                    db.RentContractRooms.Remove(rentRoom);
                                }
                                else
                            {
                        //目前将room里面的date取值与rentcontract保持一致
                                rentRoom.Remark = _room.Remark;
                                rentRoom.RentBeginDate = s.RentBeginDate;
                                rentRoom.RentEndDate = s.RentEndDate;
                                rentRoom.RentPrice = _room.RentPrice;
                            }
                        }
                        db.SaveChanges();
                        tran.Complete();
                        return Redirect("../ContractView/" + s.Id.ToString());
                    }
                }
            }
            else
            {
                db.Database.Connection.Open();
                rooms =
                db.Database.Connection.Query<ViewRentContractRoom>(string.Format("{0} where rent.contractid={1}",
                                                                                 ViewRentContractRoom.sql, id)).ToList();
            
                prices =
                    db.Database.Connection.Query<RentContractPrice>(
                        string.Format("select * from rentcontractprices where contractid={0}", id)).ToList();
                db.Database.Connection.Close();
                newprices = new List<Removeable<RentContractPrice>>();
                prices.ForEach(o=>newprices.Add(new Removeable<RentContractPrice>{obj=o}));
            }
            if(s.ClientId!=0)
            {
                ViewBag.ClientName = (from o in db.Clients where o.Id == s.ClientId select o.Name).FirstOrDefault();
            }else
            {
                ViewBag.ClientName = "";
            }
            
            ViewBag.Buildings =JsonConvert.SerializeObject(buildings);
            ViewBag.RoomsString = JsonConvert.SerializeObject(rooms);
            ViewBag.Prices = JsonConvert.SerializeObject(newprices,Formatter.DateConverter);
            return View(s);
        }
        [Authorize]
        public ActionResult ContractEdit(int id,int?room)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同维护")) return Redirect("~/content/AccessDeny.htm");
          
            return _ContractEdit(id, room,null); 
        }
        [HttpPost]
        [Authorize]
        public ActionResult ContractEdit(int id, int? room,FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同维护")) return Redirect("~/content/AccessDeny.htm");

            return _ContractEdit(id, room, collection);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id,int?room, FormCollection collection)
        {
            if (!UserInfo.CurUser.HasRight("租赁管理-合同维护")) return Redirect("~/content/AccessDeny.htm");
            using (TransactionScope transcope = new TransactionScope())
            {
                RentContract s = null;
                if (id != 0)
                {
                    s = db.RentContracts.Find(id);
                }
                if (s == null)
                {
                    s = new RentContract();
                    db.RentContracts.Add(s);
                }
                //collection.Remove("");
                TryUpdateModel(s, "", new string[] {}, new string[] {""}, collection);

                if (ModelState.IsValid)
                {

                    db.SaveChanges();
                    if (id == 0)
                    {
                        BLL.Utilities.AddLogAndSave(s.Id, RentContract.LogClass, "创建", "");

                    }
                    else
                    {
                        BLL.Utilities.AddLogAndSave(s.Id, RentContract.LogClass, "修改", "");
                    }
                    return Redirect("../View/" + s.Id);
                }

                return View(s);
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult Delete(int id)
        {

            Models.Result result = new Models.Result();
            RentContract s = db.RentContracts.Find(id); if (!UserInfo.CurUser.HasRight("租赁管理-客户维护")) return Json(new Result { obj = "没有权限" });
            //var query = (from o in db.RentContractRooms where o.RentContractId == id select o.Id).Count();
            //if (query > 0)
            //{
            //    result.obj = "已有合同记录，不能删除";
            //}
            //else
            //{
            //    db.RentContracts.Remove(s);
            //    db.SaveChanges(); BLL.Utilities.AddLogAndSave(s.Id, RentContract.LogClass, "删除", "");
            //    result.success = true;
            //    result.obj = "已删除";
            //}

            return Json(result);
        }
    }
}
