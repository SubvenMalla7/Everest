using Everest_Video_Library.Models;
using Everest_Video_Library.Models.VideoLibrary;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Everest_Video_Library.Controllers.VideoLibrary
{
    [AuthLog(Roles = "Manager")]


    public class MemberCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MemberCategories
        public ActionResult Index()
        {
            return View(db.MemberCategories.ToList());
        }

        // GET: MemberCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberCategory memberCatagory = db.MemberCategories.Find(id);
            if (memberCatagory == null)
            {
                return HttpNotFound();
            }
            return View(memberCatagory);
        }

        // GET: MemberCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MemberCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LoanDays,FinePerDays,NoOfDvdRent,Name")] MemberCategory memberCatagory)
        {
            if (ModelState.IsValid)
            {
                db.MemberCategories.Add(memberCatagory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(memberCatagory);
        }

        // GET: MemberCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberCategory memberCatagory = db.MemberCategories.Find(id);
            if (memberCatagory == null)
            {
                return HttpNotFound();
            }
            return View(memberCatagory);
        }

        // POST: MemberCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LoanDays,FinePerDays,NoOfDvdRent,Name")] MemberCategory memberCatagory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberCatagory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(memberCatagory);
        }

        // GET: MemberCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberCategory memberCatagory = db.MemberCategories.Find(id);
            if (memberCatagory == null)
            {
                return HttpNotFound();
            }
            return View(memberCatagory);
        }

        // POST: MemberCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MemberCategory memberCatagory = db.MemberCategories.Find(id);
            db.MemberCategories.Remove(memberCatagory);
            db.SaveChanges();
            return RedirectToAction("Index");
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
