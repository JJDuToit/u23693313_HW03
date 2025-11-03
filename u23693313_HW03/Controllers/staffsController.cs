using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using u23693313_HW03.Models;

namespace u23693313_HW03.Controllers
{
    public class staffsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: staffs
        public async Task<ActionResult> Index()
        {
            var staffs = db.staffs.Include(s => s.staff1).Include(s => s.store);
            return View(await db.staffs.ToListAsync());
        }

        // GET: staffs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return PartialView(staff);
        }

        // GET: staffs/Create
        public ActionResult Create()
        {
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name");

            return PartialView("Create");
        }

        // POST: staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.staffs.Add(staff);
                await db.SaveChangesAsync();

                return Json(new { success = true, redirectTo = Url.Action("Index", "Home") });
            }

            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);

            return PartialView("Create", staff);
        }

        // GET: staffs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            return PartialView(staff);
        }

        // POST: staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            return PartialView("Create", staff);
        }

        // GET: staffs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return PartialView(staff);
        }

        // POST: staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            staff staff = await db.staffs.FindAsync(id);
            db.staffs.Remove(staff);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
