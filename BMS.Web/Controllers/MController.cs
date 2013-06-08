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
    public class MController : BaseController
    {
        private OUContext db = new OUContext();

        //
        // GET: /M/

        public ViewResult Index()
        {
            return View(db.MaterialIns.ToList());
        }

        //
        // GET: /M/Details/5

        public ViewResult Details(int id)
        {
            MaterialIn materialin = db.MaterialIns.Find(id);
            return View(materialin);
        }

        //
        // GET: /M/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /M/Create

        [HttpPost]
        public ActionResult Create(MaterialIn materialin)
        {
            if (ModelState.IsValid)
            {
                db.MaterialIns.Add(materialin);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(materialin);
        }
        
        //
        // GET: /M/Edit/5
 
        public ActionResult Edit(int id)
        {
            MaterialIn materialin = db.MaterialIns.Find(id);
            return View(materialin);
        }

        //
        // POST: /M/Edit/5

        [HttpPost]
        public ActionResult Edit(MaterialIn materialin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(materialin);
        }

        //
        // GET: /M/Delete/5
 
        public ActionResult Delete(int id)
        {
            MaterialIn materialin = db.MaterialIns.Find(id);
            return View(materialin);
        }

        //
        // POST: /M/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            MaterialIn materialin = db.MaterialIns.Find(id);
            db.MaterialIns.Remove(materialin);
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