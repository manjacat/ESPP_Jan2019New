using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;

namespace eSPP.Controllers
{
    public class PergerakanGajisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult SenaraiPergerakanGaji()
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            List<HR_MAKLUMAT_PEKERJAAN> mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.ToList();
            List<MaklumatKakitanganModels> listKakitangan = new List<MaklumatKakitanganModels>();
            

            foreach (var item in mPeribadi)
            {
                MaklumatKakitanganModels kakitangan = new MaklumatKakitanganModels();
                kakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
                kakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                kakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU = item.HR_NO_KPBARU;
                HR_MAKLUMAT_PEKERJAAN pekerjaan = mPekerjaan.SingleOrDefault(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA);
                if (pekerjaan == null) 
                {
                    pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }
                kakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN = pekerjaan.HR_JAWATAN;
                kakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN = pekerjaan.HR_JABATAN;
                
                listKakitangan.Add(kakitangan);
            }
            
            return View(listKakitangan);
        }

        // GET: PergerakkanGaji
        public ActionResult Index()
        {
            return View(db.HR_MAKLUMAT_KEWANGAN8.ToList());
        }

        // GET: PergerakkanGaji/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8 = db.HR_MAKLUMAT_KEWANGAN8.Find(id);
            if (hR_MAKLUMAT_KEWANGAN8 == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_KEWANGAN8);
        }

        // GET: PergerakkanGaji/Create
        public ActionResult Create()
        {
            return View();
        }
      
        // POST: PergerakkanGaji/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND")] HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8)
        {
            if (ModelState.IsValid)
            {
                db.HR_MAKLUMAT_KEWANGAN8.Add(hR_MAKLUMAT_KEWANGAN8);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hR_MAKLUMAT_KEWANGAN8);
        }

        // GET: PergerakkanGaji/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8 = db.HR_MAKLUMAT_KEWANGAN8.Find(id);
            if (hR_MAKLUMAT_KEWANGAN8 == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_KEWANGAN8);
        }

        // POST: PergerakkanGaji/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND")] HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_MAKLUMAT_KEWANGAN8).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hR_MAKLUMAT_KEWANGAN8);
        }

        // GET: PergerakkanGaji/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8 = db.HR_MAKLUMAT_KEWANGAN8.Find(id);
            if (hR_MAKLUMAT_KEWANGAN8 == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_KEWANGAN8);
        }

        // POST: PergerakkanGaji/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HR_MAKLUMAT_KEWANGAN8 hR_MAKLUMAT_KEWANGAN8 = db.HR_MAKLUMAT_KEWANGAN8.Find(id);
            db.HR_MAKLUMAT_KEWANGAN8.Remove(hR_MAKLUMAT_KEWANGAN8);
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
