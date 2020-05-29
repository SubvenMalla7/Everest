using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Everest_Video_Library.Controllers.ViewModel;
using Everest_Video_Library.Models;
using Everest_Video_Library.Models.VideoLibrary;

namespace Everest_Video_Library.Controllers.VideoLibrary
{
    [AuthLog(Roles = "Manager")]
    public class LoansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Loans
        public ActionResult Index()
        {
            var loans = db.Loans.Include(l => l.Dvds).Include(l => l.Members);
            return View(loans.ToList());
        }

        // GET: Loans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            ViewBag.Error = Request.Cookies["Error"] != null ? Request.Cookies["Error"].Value : null;
            ViewBag.Success = Request.Cookies["Success"] != null ? Request.Cookies["Success"].Value : null;
            HttpCookie cokie = new HttpCookie("Error");
            cokie["Error"] = null;
            cokie["Success"] = null;
            Response.Cookies.Add(cokie);
            AddLoan loan = new AddLoan
            {
                Albums = db.Albums.ToList(),
                Members = db.Members.ToList()
            };

            return View(loan);
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult Create(int MemberId, int AlbumId)
        {
            HttpCookie cokie = new HttpCookie("Error");

            if (ModelState.IsValid)
            {

                Album album = db.Albums.Find(AlbumId);
                Member member = db.Members.Find(MemberId);
                int memberAge = Convert.ToInt32((DateTime.Today - member.DateOfBirth).TotalDays / 365);


                if (album.AgeContent & memberAge < 18)
                {
                    cokie["Error"] = "Age must be 18+ to view this content";
                    Response.Cookies.Add(cokie);

                    return RedirectToAction("Create");
                }
                if (album.NoOfStock < 1)
                {
                    cokie["Error"] = "Sorry! we dont have stock";
                    Response.Cookies.Add(cokie);

                    return RedirectToAction("Create");

                }
                var loan = (from lon in db.Loans
                            where
                            (lon.ReturnedDate == null
                                && lon.MemberId == MemberId)
                            select new
                            {
                                lon.DvdId,
                                lon.ReturnedDate,
                                lon.MemberId
                            }).ToList();


                int noOfDvdsMemberCanLone = member.Catagory.NoOfDvdRent;
                int noOfDaysMemberCanLone = member.Catagory.LoanDays;
                var count = loan.Count();

                if (noOfDvdsMemberCanLone <= loan.Count)
                {
                    cokie["Error"] = "Return Dvd first to take loan!";
                    Response.Cookies.Add(cokie);


                    return RedirectToAction("Create");
                }

                Dvd dvd =
                    db.Dvds.FirstOrDefault(X => X.AlbumId == AlbumId & X.OnStock);


                Loan newLoan = new Loan
                {
                    DvdId = dvd.Id,
                    MemberId = member.Id,
                    LoanDate = DateTime.Today,
                    ReturnDate = DateTime.Today.AddDays(member.Catagory.LoanDays),
                };
                dvd.OnStock = false;
                album.NoOfStock -= album.NoOfStock;
                db.Loans.Add(newLoan);
                db.SaveChanges();
                cokie["Success"] = "Sucessfully loned";
                Response.Cookies.Add(cokie);
                return RedirectToAction("Create");

            }

            cokie["Error"] = "Not Valid Input";
            Response.Cookies.Add(cokie);

            return RedirectToAction("Create");


        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            ViewBag.DvdId = new SelectList(db.Dvds, "Id", "Id", loan.DvdId);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", loan.MemberId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MemberId,DvdId")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DvdId = new SelectList(db.Dvds, "Id", "Id", loan.DvdId);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Albumdvd = db.Dvds.FirstOrDefault(X => X.Id == id);
            Albumdvd.OnStock = true;
            var loan = db.Loans.FirstOrDefault(X => X.DvdId == id);
            loan.ReturnedDate = DateTime.Today;
            if (DateTime.Today > loan.ReturnDate)
            {
                int daysMore = (DateTime.Today - (DateTime)loan.ReturnDate).Days;

                decimal finrPerday = loan.Members.Catagory.FinePerDays;
                loan.FineAmount = finrPerday * daysMore;
            }
            var album = db.Albums.FirstOrDefault(X => X.Id == loan.Dvds.AlbumId);
            album.NoOfStock += 1;
            db.SaveChanges();
            return Redirect("/Members");
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Albumdvd = db.Dvds.FirstOrDefault(X => X.Id == id);
            Albumdvd.OnStock = true;
            var loan = db.Loans.FirstOrDefault(X => X.DvdId == id);
            loan.ReturnedDate = DateTime.Today;
            if (DateTime.Today > loan.ReturnDate)
            {
                int daysMore = (DateTime.Today - (DateTime)loan.ReturnDate).Days;

                decimal finrPerday = loan.Members.Catagory.FinePerDays;
                loan.FineAmount = finrPerday * daysMore;
            }
            var album = db.Albums.FirstOrDefault(X => X.Id == loan.Dvds.AlbumId);
            album.NoOfStock += 1;
            db.SaveChanges();
            return Redirect("/Members");
           
        }


        [Route("/{dvd:int?}/{member:int?}")]
        public ActionResult Return(int? dvd, int? member)
        {
            if (dvd == null & member == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Albumdvd = db.Dvds.FirstOrDefault(X => X.Id == dvd);
            Albumdvd.OnStock = true;
            var loan = db.Loans.FirstOrDefault(X => X.DvdId == dvd);
            loan.ReturnedDate = DateTime.Today;
            if (DateTime.Today > loan.ReturnDate)
            {
                int daysMore = (DateTime.Today - (DateTime)loan.ReturnDate).Days;

                decimal finrPerday = loan.Members.Catagory.FinePerDays;
                loan.FineAmount = finrPerday * daysMore;
            }
            var album = db.Albums.FirstOrDefault(X => X.Id == loan.Dvds.AlbumId);
            album.NoOfStock += 1;
            db.SaveChanges();
            return Redirect("/Members");
        }
        
        public ActionResult LatestLone()
        {
            var dayBefore = DateTime.Today.AddDays(-31);
            var loans = db.Loans.Where(X => X.LoanDate > dayBefore).ToList();

            return View(loans);
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
