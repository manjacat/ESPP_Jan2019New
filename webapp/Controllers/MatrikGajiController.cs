 using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class MatrikGajiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        private GenerateProcedures ge = new GenerateProcedures();

        // GET: MatrikGaji


        //public ActionResult SenaraiGaji()
        //{
        //    return View(db.HR_MATRIKS_GAJI.ToList());
        //}


        public List<HR_MATRIKS_GAJI> CariPekerja(string value)
         {
            List<HR_MATRIKS_GAJI> sGaji = new List<HR_MATRIKS_GAJI>();
       
       
             sGaji = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == value).ToList();
    
            
            return sGaji;
        }


        public ActionResult SenaraiGaji( string value)
        {
            List<HR_MATRIKS_GAJI> mGaji = new List<HR_MATRIKS_GAJI>();
            mGaji = CariPekerja( value);

           
            ViewBag.value = value;

            
            return View(mGaji);
        }

        
        public PartialViewResult GajiList( string value)
        {
            ViewBag.value = value;

            ViewBag.HR_GRED_GAJI = value;

            List<HR_MATRIKS_GAJI> matrik = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == value).ToList();
            

            ViewBag.detail = db.HR_MATRIKS_GAJI.ToList<HR_MATRIKS_GAJI>();

            return PartialView("_GajiList", matrik);
        }

          
        public ActionResult MatrikInfo(string id, string value, int? kod, decimal? exp,decimal? min, string jenis)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_MATRIKS_GAJI matrik = db.HR_MATRIKS_GAJI.FirstOrDefault(s => s.HR_SISTEM_SARAAN == id && s.HR_GRED_GAJI == value && s.HR_PERINGKAT == kod && s.HR_TAHAP == exp && s.HR_GAJI_MIN == min);

            if (matrik == null)
            {
                return HttpNotFound();
            }

            
            return PartialView ("_GajiList" + jenis, matrik);
        }


        public ActionResult GajiListInfo(string id, string value, int? kod, decimal? exp, decimal? min)
        {
            return MatrikInfo(id, value, kod,exp,min, "Info");
        }

        public ActionResult GajiListEdit(string id, string value, int? kod, decimal? exp, decimal? min)
        {
            return MatrikInfo(id, value, kod, exp,min, "Edit");
        }

        public ActionResult GajiListPadam(string id, string value, int? kod, decimal? exp, decimal? min)
        {
            return MatrikInfo(id, value, kod, exp,min, "Padam");
        }



        //public ActionResult EditMatrik(string saraan, string gaji, int peringkat, decimal? tahap)
        //{
        //    if (saraan == null || gaji == null || tahap == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    }
        //    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.SingleOrDefault(s => s.HR_SISTEM_SARAAN == saraan && s.HR_GRED_GAJI == gaji && s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap);

        //    if (matriks == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.GroupBy(c => c.HR_PERINGKAT).Select(c => c.FirstOrDefault()).OrderBy(c => c.HR_PERINGKAT), "HR_PERINGKAT", "HR_PERINGKAT");
        //    ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
        //    return PartialView("_GajiEditList", matriks);
        //}

        [HttpPost, ActionName("GajiListEdit")]
        [ValidateAntiForgeryToken]
        public JsonResult EditMatrik(HR_MATRIKS_GAJI matrik)
        {
            try
            {
                String[] arr = Convert.ToString(matrik.HR_TAHAP).Split('.');
                decimal n = Convert.ToDecimal(arr[0]);
                List<HR_MATRIKS_GAJI> mat = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == matrik.HR_SISTEM_SARAAN && s.HR_GRED_GAJI == matrik.HR_GRED_GAJI && s.HR_PERINGKAT == matrik.HR_PERINGKAT && (s.HR_TAHAP >= n && s.HR_TAHAP < (n + 1))).ToList();

                db.HR_MATRIKS_GAJI.RemoveRange(mat);
                db.SaveChanges();
                ge.Z_INSERT_MATRIKS_GAJI_IND_TEST(matrik.HR_GRED_GAJI, matrik.HR_GAJI_POKOK, new ObjectParameter("pi_BEZA", typeof(int)), n, matrik.HR_KOD_GAJI, matrik.HR_PERINGKAT, new ObjectParameter("tAHAPMAX", typeof(int)));
                return Json(new { msg = "Data berjaya dikemaskini", gred = matrik.HR_GRED_GAJI, error = false }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { msg = "Data tidak berjaya dikemaskini", gred = matrik.HR_GRED_GAJI, error = true }, JsonRequestBehavior.AllowGet);
            }
        }



      
        [HttpPost, ActionName("GajiListPadam")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(HR_MATRIKS_GAJI matrik)
        {
            try
            {
                String[] arr = Convert.ToString(matrik.HR_TAHAP).Split('.');
                decimal n = Convert.ToDecimal(arr[0]);
                List<HR_MATRIKS_GAJI> mat = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == matrik.HR_SISTEM_SARAAN && s.HR_GRED_GAJI == matrik.HR_GRED_GAJI && s.HR_PERINGKAT == matrik.HR_PERINGKAT && (s.HR_TAHAP >= n && s.HR_TAHAP < (n + 1))).ToList();

                db.HR_MATRIKS_GAJI.RemoveRange(mat);
                db.SaveChanges();
                return Json(new { msg = "Data berjaya dipadam", gred = matrik.HR_GRED_GAJI, error = false }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { msg = "Data tidak berjaya dipadam", gred = matrik.HR_GRED_GAJI, error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TambahMatrik(HR_MATRIKS_GAJI model)
        {
            
            ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.AsEnumerable().Where(s=> s.HR_GRED_GAJI == model.HR_GRED_GAJI).Select( s=> new { HR_PERINGKAT = Convert.ToInt16(s.HR_PERINGKAT.Substring(1,1))}).ToList(), "HR_PERINGKAT", "HR_PERINGKAT");
            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");

            ViewBag.gred = model.HR_GRED_GAJI;
            return PartialView("_TambahMatrik");
        }


        [HttpPost, ActionName("TambahMatrik")]
        [ValidateAntiForgeryToken]
        public JsonResult GenerateMatrik(HR_MATRIKS_GAJI model)
        {  
            try
            {
                ge.Z_INSERT_MATRIKS_GAJI_IND_TEST(model.HR_GRED_GAJI, model.HR_GAJI_POKOK,new ObjectParameter("pi_BEZA", typeof(int)), model.HR_TAHAP, model.HR_KOD_GAJI ,model.HR_PERINGKAT, new ObjectParameter("tAHAPMAX", typeof(int)));//PEGANGDATADARIDZATUL
                return Json( new { msg = "Data berjaya dimasukkan", gred = model.HR_GRED_GAJI, error = false}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { msg = "Data tidak berjaya dimasukkan", gred = model.HR_GRED_GAJI, error = true}, JsonRequestBehavior.AllowGet);

            }  
        }


        public JsonResult SenaraiPeringkat (string gred)
        {
            var jadual = db.HR_JADUAL_GAJI.AsEnumerable().Where(s => s.HR_GRED_GAJI == gred).Select(s => new { HR_PERINGKAT = Convert.ToInt16(s.HR_PERINGKAT.Substring(1, 1)) }).ToList();

            return  Json(jadual,JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariMatrik(string peringkat, string gred)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HR_JADUAL_GAJI item = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == gred &&  s.HR_PERINGKAT == "P" +peringkat);
            
           
            if (item == null)
            {
                item = new HR_JADUAL_GAJI();
            }    
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariGajiPokok( decimal? kod, string gaji)
        {
            List<HR_MATRIKS_GAJI> matriks = new List<HR_MATRIKS_GAJI>();
         
            if (kod != null)
            {
                matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == gaji && s.HR_GAJI_POKOK == kod).ToList();
            }
            string msg = null;
            if (matriks.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

    }
}