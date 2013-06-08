using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.BLL;
using OUDAL;

namespace BMS.Web.Controllers
{ 
    public class M1Controller : BaseController
    {
        private OUContext db = new OUContext();

        //
        // GET: /M1/

        public ViewResult Index()
        {
            return View(db.MaterialInItems.ToList());
        }

        //
        // GET: /M1/Details/5

        public ViewResult Details(int id)
        {
            MaterialInItem materialinitem = db.MaterialInItems.Find(id);
            return View(materialinitem);
        }

        //
        // GET: /M1/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /M1/Create

        [HttpPost]
        public ActionResult Create(MaterialInItem materialinitem)
        {
            if (ModelState.IsValid)
            {
                db.MaterialInItems.Add(materialinitem);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(materialinitem);
        }
        
        //
        // GET: /M1/Edit/5
 
        public ActionResult Edit(int id)
        {
            MaterialInItem materialinitem = db.MaterialInItems.Find(id);
            return View(materialinitem);
        }

        //
        // POST: /M1/Edit/5

        [HttpPost]
        public ActionResult Edit(MaterialInItem materialinitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialinitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(materialinitem);
        }

        //
        // GET: /M1/Delete/5
 
        public ActionResult Delete(int id)
        {
            MaterialInItem materialinitem = db.MaterialInItems.Find(id);
            return View(materialinitem);
        }

        //
        // POST: /M1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            MaterialInItem materialinitem = db.MaterialInItems.Find(id);
            db.MaterialInItems.Remove(materialinitem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}