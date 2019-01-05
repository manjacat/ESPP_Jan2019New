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
    public class MaklumatPekerjaanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MaklumatPekerjaan
        public ActionResult Index()
        {
            var hR_MAKLUMAT_PEKERJAAN = db.HR_MAKLUMAT_PEKERJAAN.Include(h => h.HR_MAKLUMAT_ELAUN_POTONGAN).Include(h => h.HR_MAKLUMAT_PERIBADI).Include(h => h.HR_PERSARAAN);
            return View(hR_MAKLUMAT_PEKERJAAN.ToList());
        }

        // GET: MaklumatPekerjaan/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            if (hR_MAKLUMAT_PEKERJAAN == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_PEKERJAAN);
        }

        // GET: MaklumatPekerjaan/Create
        public ActionResult Create()
        {
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_ELAUN_POTONGAN, "HR_NO_PEKERJA", "HR_KOD_ELAUN_POTONGAN");
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NO_KPBARU");
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_PERSARAAN, "HR_NO_PEKERJA", "HR_ALASAN");
            return View();
        }

        // POST: MaklumatPekerjaan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HR_NO_PEKERJA,HR_GELARAN,HR_JABATAN,HR_BAHAGIAN,HR_JAWATAN,HR_GRED,HR_KATEGORI,HR_KUMP_PERKHIDMATAN,HR_TARAF_JAWATAN,HR_GAJI_POKOK,HR_NO_AKAUN_BANK,HR_BULAN_KENAIKAN_GAJI,HR_TARIKH_MASUK,HR_TARIKH_SAH_JAWATAN,HR_TARIKH_TAMAT_KONTRAK,HR_TARIKH_TAMAT,HR_SISTEM,HR_NO_PENYELIA,HR_STATUS_KWSP,HR_STATUS_SOCSO,HR_STATUS_PCB,HR_STATUS_PENCEN,HR_NILAI_KWSP,HR_NILAI_SOCSO,HR_KOD_PCB,HR_GAJI_PRORATA,HR_MATRIKS_GAJI,HR_UNIT,HR_KUMPULAN,HR_KOD_BANK,HR_TINGKATAN,HR_KAKITANGAN_IND,HR_FAIL_PERKHIDMATAN,HR_NO_SIRI,HR_BAYARAN_CEK,HR_TARIKH_KE_JABATAN,HR_KOD_GAJI,HR_KELAS_PERJALANAN,HR_TARIKH_LANTIKAN,HR_TARIKH_TIDAK_AKTIF,HR_GAJI_IND,HR_TARIKH_GAJI,HR_PCB_TARIKH_MULA,HR_PCB_TARIKH_AKHIR,HR_NILAI_PCB,HR_KOD_GELARAN_J,HR_TANGGUH_GERAKGAJI_IND,HR_TARIKH_KEYIN,HR_NP_KEYIN,HR_TARIKH_UBAH,HR_NP_UBAH,HR_SKIM,HR_PERGERAKAN_GAJI,HR_NO_KWSP,HR_NO_PENCEN,HR_NO_SOCSO,HR_NO_PCB,HR_INITIAL,HR_AM_YDP,HR_TARIKH_MASUK_KERAJAAN,HR_UNIFORM,HR_TEKNIKAL,HR_TARIKH_KELUAR_MBPJ")] HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN)
        {
            if (ModelState.IsValid)
            {
                db.HR_MAKLUMAT_PEKERJAAN.Add(hR_MAKLUMAT_PEKERJAAN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_ELAUN_POTONGAN, "HR_NO_PEKERJA", "HR_KOD_ELAUN_POTONGAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NO_KPBARU", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_PERSARAAN, "HR_NO_PEKERJA", "HR_ALASAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            return View(hR_MAKLUMAT_PEKERJAAN);
        }

        // GET: MaklumatPekerjaan/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            if (hR_MAKLUMAT_PEKERJAAN == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_ELAUN_POTONGAN, "HR_NO_PEKERJA", "HR_KOD_ELAUN_POTONGAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NO_KPBARU", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_PERSARAAN, "HR_NO_PEKERJA", "HR_ALASAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            return View(hR_MAKLUMAT_PEKERJAAN);
        }

        // POST: MaklumatPekerjaan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_NO_PEKERJA,HR_GELARAN,HR_JABATAN,HR_BAHAGIAN,HR_JAWATAN,HR_GRED,HR_KATEGORI,HR_KUMP_PERKHIDMATAN,HR_TARAF_JAWATAN,HR_GAJI_POKOK,HR_NO_AKAUN_BANK,HR_BULAN_KENAIKAN_GAJI,HR_TARIKH_MASUK,HR_TARIKH_SAH_JAWATAN,HR_TARIKH_TAMAT_KONTRAK,HR_TARIKH_TAMAT,HR_SISTEM,HR_NO_PENYELIA,HR_STATUS_KWSP,HR_STATUS_SOCSO,HR_STATUS_PCB,HR_STATUS_PENCEN,HR_NILAI_KWSP,HR_NILAI_SOCSO,HR_KOD_PCB,HR_GAJI_PRORATA,HR_MATRIKS_GAJI,HR_UNIT,HR_KUMPULAN,HR_KOD_BANK,HR_TINGKATAN,HR_KAKITANGAN_IND,HR_FAIL_PERKHIDMATAN,HR_NO_SIRI,HR_BAYARAN_CEK,HR_TARIKH_KE_JABATAN,HR_KOD_GAJI,HR_KELAS_PERJALANAN,HR_TARIKH_LANTIKAN,HR_TARIKH_TIDAK_AKTIF,HR_GAJI_IND,HR_TARIKH_GAJI,HR_PCB_TARIKH_MULA,HR_PCB_TARIKH_AKHIR,HR_NILAI_PCB,HR_KOD_GELARAN_J,HR_TANGGUH_GERAKGAJI_IND,HR_TARIKH_KEYIN,HR_NP_KEYIN,HR_TARIKH_UBAH,HR_NP_UBAH,HR_SKIM,HR_PERGERAKAN_GAJI,HR_NO_KWSP,HR_NO_PENCEN,HR_NO_SOCSO,HR_NO_PCB,HR_INITIAL,HR_AM_YDP,HR_TARIKH_MASUK_KERAJAAN,HR_UNIFORM,HR_TEKNIKAL,HR_TARIKH_KELUAR_MBPJ")] HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_MAKLUMAT_PEKERJAAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_ELAUN_POTONGAN, "HR_NO_PEKERJA", "HR_KOD_ELAUN_POTONGAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NO_KPBARU", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            ViewBag.HR_NO_PEKERJA = new SelectList(db.HR_PERSARAAN, "HR_NO_PEKERJA", "HR_ALASAN", hR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA);
            return View(hR_MAKLUMAT_PEKERJAAN);
        }

        // GET: MaklumatPekerjaan/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            if (hR_MAKLUMAT_PEKERJAAN == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_PEKERJAAN);
        }

        // POST: MaklumatPekerjaan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HR_MAKLUMAT_PEKERJAAN hR_MAKLUMAT_PEKERJAAN = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            db.HR_MAKLUMAT_PEKERJAAN.Remove(hR_MAKLUMAT_PEKERJAAN);
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
