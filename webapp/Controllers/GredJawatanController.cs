using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class GredJawatanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        // GET: GredJawatan
        public ActionResult Index()
        {
            List<HR_JAWATAN> senaraiJawatan = db.HR_JAWATAN.ToList();
              
            List<GredJawatanModels> GJM = new List<GredJawatanModels>();

            foreach (HR_JAWATAN jawatan in senaraiJawatan)
            {
                
                GredJawatanModels gjm = new GredJawatanModels();
                gjm.HR_JAWATAN = new HR_JAWATAN();
                gjm.HR_GELARAN_JAWATAN = new HR_GELARAN_JAWATAN();
                gjm.GE_PARAMTABLE = new GE_PARAMTABLE();
                gjm.HR_JAWATAN = jawatan;
                List<HR_GELARAN_JAWATAN> senaraiGelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_JAWATAN == jawatan.HR_KOD_JAWATAN).ToList();
                if(senaraiGelaran.Count() > 0)
                {
                    foreach(HR_GELARAN_JAWATAN gelaran in senaraiGelaran)
                    {
                        if (gelaran.HR_GRED != null)
                        {
                            gelaran.HR_GRED = gelaran.HR_GRED.Trim();
                        }
                        GE_PARAMTABLE gred = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == gelaran.HR_GRED);

                        gjm.GE_PARAMTABLE = gred;
                        gjm.HR_GELARAN_JAWATAN = gelaran;
                    }
                }
                else
                {
                    gjm.GE_PARAMTABLE = new GE_PARAMTABLE();
                    gjm.HR_GELARAN_JAWATAN = new HR_GELARAN_JAWATAN();
                }
                GJM.Add(gjm);
                
                
            }            
            return View(GJM);
        }

      

        public ActionResult InfoGredJawatan (string id)
        {
           
            HR_JAWATAN mJawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == id);
            HR_GELARAN_JAWATAN mGelaran = db.HR_GELARAN_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == id);

 
           
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GredJawatanModels gredJawatan = new GredJawatanModels();
            gredJawatan.HR_JAWATAN = mJawatan;
            gredJawatan.HR_GELARAN_JAWATAN = mGelaran;
            
             
            if (gredJawatan == null)
            {
                return HttpNotFound();
            }
           
            return PartialView("_InfoGredJawatan", gredJawatan);
        }


       
        public ActionResult TambahGredJawatan()
        {
            return PartialView("_TambahGredJawatan");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahGredJawatan(GredJawatanModels model)
        {
            if (ModelState.IsValid)
            {
                
                

                var checkJawatan = db.HR_JAWATAN.Where(s => s.HR_NAMA_JAWATAN == model.HR_JAWATAN.HR_NAMA_JAWATAN).ToList(); //check samada da ada atau belum //tak boleh guna yg dah ada so panggil HR_NAMA_JAWATAN
                if (checkJawatan.Count <= 0)
                {
                    var SelectLastID = db.HR_JAWATAN.OrderByDescending(s => s.HR_KOD_JAWATAN).FirstOrDefault().HR_KOD_JAWATAN;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == 'J' || x == '0').ToArray());
                    var Increment = Convert.ToSingle(LastID) + 1;
                    var KodJawatan = "J" +  Convert.ToString(Increment).PadLeft(4, '0');

                    model.HR_JAWATAN.HR_KOD_JAWATAN = KodJawatan;
                    db.HR_JAWATAN.Add(model.HR_JAWATAN);
                }
                else
                {
                    db.Entry(model.HR_JAWATAN).State = EntityState.Modified;
                }

                var selectGred = db2.GE_PARAMTABLE.Where(s => s.SHORT_DESCRIPTION == model.GE_PARAMTABLE.SHORT_DESCRIPTION && s.STRING_PARAM == model.GE_PARAMTABLE.STRING_PARAM && s.GROUPID == 109).ToList();
                if(selectGred.Count() <= 0)
                {
                    int IncrementGE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).OrderByDescending(s => s.ORDINAL).FirstOrDefault().ORDINAL;
                    model.GE_PARAMTABLE.LONG_DESCRIPTION = model.GE_PARAMTABLE.SHORT_DESCRIPTION;
                    model.GE_PARAMTABLE.AUDIT_WHEN = DateTime.Now;

                    model.GE_PARAMTABLE.GROUPID = 109;
                    model.GE_PARAMTABLE.ORDINAL = IncrementGE + 1;
                    db2.GE_PARAMTABLE.Add(model.GE_PARAMTABLE);
                }
                else
                {
                    db2.Entry(model.GE_PARAMTABLE).State = EntityState.Modified;
                }

                model.HR_GELARAN_JAWATAN.HR_KOD_JAWATAN = model.HR_JAWATAN.HR_KOD_JAWATAN;
                model.HR_GELARAN_JAWATAN.HR_GRED = model.GE_PARAMTABLE.SHORT_DESCRIPTION;

                var checkGelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_PENERANGAN == model.HR_GELARAN_JAWATAN.HR_PENERANGAN).ToList(); //check samada da ada atau belum //tak boleh guna yg dah ada so panggil HR_NAMA_JAWATAN
                if (checkGelaran.Count <= 0)
                {
                    var SelectLastID = db.HR_GELARAN_JAWATAN.OrderByDescending(s => s.HR_KOD_GELARAN).FirstOrDefault().HR_KOD_GELARAN;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == 'G' || x == '0').ToArray());
                    var Increment = Convert.ToSingle(LastID) + 1;
                    var KodGelaran = "G" + Convert.ToString(Increment).PadLeft(4, '0');

                    model.HR_GELARAN_JAWATAN.HR_KOD_GELARAN = KodGelaran;
                    db.HR_GELARAN_JAWATAN.Add(model.HR_GELARAN_JAWATAN);
                }
                else
                {
                    db.Entry(model.HR_GELARAN_JAWATAN).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public JsonResult cariJawatan(string item)
        {
            var data = "";
            var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_NAMA_JAWATAN == item);
            if(jawatan != null)
            {
                data = jawatan.HR_KOD_JAWATAN;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cariGred(string item)
        {
            GE_PARAMTABLE data = new GE_PARAMTABLE();
            var gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == item);
            if (gred != null)
            {
                data.ORDINAL = gred.ORDINAL;
                data.GROUPID = gred.GROUPID;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cariGelaran(string item)
        {
            var data = "";
            var gelaran = db.HR_GELARAN_JAWATAN.SingleOrDefault(s => s.HR_PENERANGAN == item);
            if (gelaran != null)
            {
                data = gelaran.HR_KOD_JAWATAN;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGredJawatan()
        {
            return PartialView("_EditGredJawatan");
        }


        public ActionResult PadamGredJawatan()
        {
            return PartialView("_PadamGredJawatan");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamGredJawatan(GredJawatanModels model)
        {
            HR_JAWATAN jawatan = db.HR_JAWATAN.Find(model.HR_JAWATAN.HR_KOD_JAWATAN);
            db.HR_JAWATAN.Remove(jawatan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }


}