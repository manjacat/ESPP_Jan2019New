using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.tool.xml;
using ClosedXML.Excel;

namespace eSPP.Controllers
{
    public class Kewangan8Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext mc = new MajlisContext();
        private Entities db2 = new Entities();
        private SPGContext spg = new SPGContext();

        public List<HR_MAKLUMAT_PERIBADI> CariPekerja(string key, string value, int? bulan, string kod)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (key == "1")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
                
            }
            else if (key == "2")
            {
                value = value.ToUpper();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper())  && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
            }
            else if (key == "3")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA).Count() > 0).ToList();
            }

            else if (key == "4" && bulan != null)
            {
                string dateStr = "01/" + bulan + "/" + DateTime.Now.Year;
                DateTime date = Convert.ToDateTime(dateStr);

                List<HR_MAKLUMAT_PERIBADI> tmpPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

                //sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => (s.HR_AKTIF_IND != "T" && s.HR_AKTIF_IND != "P") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T" || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "K") && (Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Year < DateTime.Now.Year)  && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_AKTIF_IND == "Y" && (s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y" || s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T" || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "K") && (Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList();
                //foreach (HR_MAKLUMAT_PERIBADI p in sPeribadi)
                //{
                //    if(p.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null && p.HR_MAKLUMAT_PEKERJAAN.HR_GRED != "")
                //    {
                //        GE_PARAMTABLE checkGred = mc.GE_PARAMTABLE.AsEnumerable().Where(g => g.ORDINAL == Convert.ToInt32(p.HR_MAKLUMAT_PEKERJAAN.HR_GRED) && g.GROUPID == 109 && g.STRING_PARAM == "SSM").FirstOrDefault();
                //        if(checkGred != null)
                //        {
                //            var checkJadualGaji = db.HR_JADUAL_GAJI.Where(j => j.HR_GRED_GAJI == checkGred.SHORT_DESCRIPTION && j.HR_PERINGKAT == p.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0,2)).Count();
                //            if(checkJadualGaji > 0)
                //            {
                //                tmpPeribadi.Add(p);
                //            }
                //        }
                //    }

                //}
                //sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
                //sPeribadi = tmpPeribadi;

                foreach (HR_MAKLUMAT_PERIBADI peribadi in sPeribadi)
                {
                    int gred = 0;
                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                    }

                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                    {
                        var peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                        GE_PARAMTABLE pTable = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                        if (pTable != null)
                        {
                            HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.FirstOrDefault(s => s.HR_SISTEM_SARAAN == "SSM" && s.HR_PERINGKAT == peringkat && s.HR_GRED_GAJI == pTable.SHORT_DESCRIPTION);

                            if (jadualGaji != null)
                            {
                                tmpPeribadi.Add(peribadi);
                            }
                        }
                    }
                }

                sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
                sPeribadi = tmpPeribadi;

            }

            //if (kod != "00025")
            //{
            //    sPeribadi = sPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y").ToList();
            //}
            return sPeribadi;
        }

        //public List<ErrorListModels> ErrorList(string key, string value, int? bulan, string kod)
        //{
        //    List<HR_MAKLUMAT_PERIBADI> mPekerja = CariPekerja(key, value, bulan, kod);
        //    List<ErrorListModels> error = new List<ErrorListModels>();
        //    foreach(HR_MAKLUMAT_PERIBADI pekerja in mPekerja)
        //    {
        //        var msg = "";
        //        if(pekerja.HR_AKTIF_IND == "T")
        //        {
        //            msg = "Pekerja tidak boleh buat " + ;
        //        }
        //        ErrorListModels err = new ErrorListModels();
        //        if (pekerja.HR_AKTIF_IND != "Y")
        //        {
        //            err.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
        //            err.MESEJ = "";
        //        }
        //    }
        //    return error;
        //}

        public string RedirectLink(string HR_KOD_PERUBAHAN)
        {
            var redirect = "";
            if (HR_KOD_PERUBAHAN == "00031")
            {
                redirect = "GanjaranKontrak";
            }
            if (HR_KOD_PERUBAHAN == "00025")
            {
                redirect = "TahanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00030")
            {
                redirect = "PotonganGaji";
            }
            if(HR_KOD_PERUBAHAN == "kew8")
            {
                redirect = "Kewangan8Manual";
            }
            if (HR_KOD_PERUBAHAN == "00026")
            {
                redirect = "BayarGaji";
            }
            if (HR_KOD_PERUBAHAN == "TP")
            {
                redirect = "TamatPerkhidmatan";
            }
            if (HR_KOD_PERUBAHAN == "00022")
            {
                redirect = "TangguhPergerakanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00037")
            {
                redirect = "SambungPergerakanGaji";
            }
            if (HR_KOD_PERUBAHAN == "00036")
            {
                redirect = "PindaanGaji";
            }
            if (HR_KOD_PERUBAHAN == "CUTI")
            {
                redirect = "Cuti";
            }
            if (HR_KOD_PERUBAHAN == "00015")
            {
                redirect = "SambungKontrak";
            }
            if (HR_KOD_PERUBAHAN == "00039")
            {
                redirect = "SemuaJenisPotongan";
            }
            if (HR_KOD_PERUBAHAN == "00024")
            {
                redirect = "SemuaJenisElaun";
            }
            if (HR_KOD_PERUBAHAN == "LNTKN")
            {
                redirect = "Perlantikan";
            }
            if (HR_KOD_PERUBAHAN == "TMK")
            {
                redirect = "TanggungMemangkuKerja";
            }
            if (HR_KOD_PERUBAHAN == "00032")
            {
                redirect = "TanggungKerja";
            }
            if (HR_KOD_PERUBAHAN == "00004")
            {
                redirect = "MemangkuKerja";
            }
            return redirect;
        }

        public PartialViewResult TableKewangan8(string id)
        {
            ViewBag.id = id;
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = new List<HR_MAKLUMAT_KEWANGAN8>();

            kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == id && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045")).ToList<HR_MAKLUMAT_KEWANGAN8>();

            foreach (var item in kewangan8)
            {
                PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                pergerakanGaji.HR_KOD_PERUBAHAN = item.HR_KOD_PERUBAHAN;
                pergerakanGaji.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                pergerakanGaji.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                pergerakanGaji.HR_BULAN = item.HR_BULAN;
                pergerakanGaji.HR_TAHUN = item.HR_TAHUN;
                pergerakanGaji.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                pergerakanGaji.HR_BUTIR_PERUBAHAN = item.HR_BUTIR_PERUBAHAN;
                pergerakanGaji.HR_CATATAN = item.HR_CATATAN;
                pergerakanGaji.HR_NO_SURAT_KEBENARAN = item.HR_NO_SURAT_KEBENARAN;
                pergerakanGaji.HR_AKTIF_IND = item.HR_AKTIF_IND;
                pergerakanGaji.HR_NP_UBAH_HR = item.HR_NP_UBAH_HR;
                pergerakanGaji.HR_TARIKH_UBAH_HR = item.HR_TARIKH_UBAH_HR;
                pergerakanGaji.HR_NP_FINALISED_HR = item.HR_NP_FINALISED_HR;
                pergerakanGaji.HR_TARIKH_FINALISED_HR = item.HR_TARIKH_FINALISED_HR;
                pergerakanGaji.HR_FINALISED_IND_HR = item.HR_FINALISED_IND_HR;
                pergerakanGaji.HR_NP_UBAH_PA = item.HR_NP_UBAH_PA;
                pergerakanGaji.HR_TARIKH_UBAH_PA = item.HR_TARIKH_UBAH_PA;
                pergerakanGaji.HR_NP_FINALISED_PA = item.HR_NP_FINALISED_PA;
                pergerakanGaji.HR_TARIKH_FINALISED_PA = item.HR_TARIKH_FINALISED_PA;
                pergerakanGaji.HR_FINALISED_IND_PA = item.HR_FINALISED_IND_PA;
                pergerakanGaji.HR_EKA = item.HR_EKA;
                pergerakanGaji.HR_ITP = item.HR_ITP;
                pergerakanGaji.HR_KEW8_IND = item.HR_KEW8_IND;
                pergerakanGaji.HR_BIL = item.HR_BIL;
                pergerakanGaji.HR_KOD_JAWATAN = item.HR_KOD_JAWATAN;
                pergerakanGaji.HR_KEW8_ID = item.HR_KEW8_ID;
                pergerakanGaji.HR_LANTIKAN_IND = item.HR_LANTIKAN_IND;
                pergerakanGaji.HR_TARIKH_SP = item.HR_TARIKH_SP;
                pergerakanGaji.HR_SP_IND = item.HR_SP_IND;
                pergerakanGaji.HR_JUMLAH_BULAN = item.HR_JUMLAH_BULAN;
                pergerakanGaji.HR_NILAI_EPF = item.HR_NILAI_EPF;
                model.Add(pergerakanGaji);
            }
            return PartialView("_TableKewangan8", model.GroupBy(s => s.HR_KEW8_ID).Select(s => s.FirstOrDefault()));
        }

        public ActionResult Kewangan8Manual(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "kew8");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        private void InfoPekerja(PergerakanGajiModels model)
        {
            HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_AKTIF_IND == "Y").Count() > 0);
            if (Peribadi == null)
            {
                Peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            int HR_GRED = Convert.ToInt32(Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == HR_GRED);
            if (gred == null)
            {
                gred = new GE_PARAMTABLE();
            }

            ViewBag.HR_GRED = gred.SHORT_DESCRIPTION;
            ViewBag.HR_KOD_GAJI = Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_GAJI_POKOK = Peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

            model.HR_ITP = 0;
            model.HR_EKA = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> itp = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (itp.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in itp)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        var j = item.HR_JUMLAH;
                        jumElaun = jumElaun + j;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        var j = item.HR_JUMLAH;
                        jumAwam = jumAwam + j;
                    }
                }
                model.HR_ITP = jumElaun;
                model.HR_EKA = jumAwam;
            }
        }

        public ActionResult TambahKewangan8(PergerakanGajiModels model)
        {
            InfoPekerja(model);

            var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
            var kewID = (lastID.HR_KEW8_ID + 1);

            model.HR_KEW8_ID = kewID;

            List<HR_KEWANGAN8> kewangan8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kewangan8, "HR_KOD_KEW8", "HR_PENERANGAN");
            return PartialView("_TambahKewangan8", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKewangan8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, PergerakanGajiModels pergerakanGaji, HR_MAKLUMAT_PEKERJAAN pekerjaan)
        {
            if (ModelState.IsValid)
            {
                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var kewID = (lastID.HR_KEW8_ID + 1);

                model.HR_KEW8_ID = kewID;

                model.HR_TARIKH_KEYIN = DateTime.Now;
                model.HR_FINALISED_IND_HR = "T";
                model.HR_BULAN = DateTime.Now.Month;
                model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);

                //modelDetail.HR_STATUS_IND = "T";
                //modelDetail.HR_JUMLAH_PERUBAHAN = 0;

                db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                db.SaveChanges();

                return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = model.HR_NO_PEKERJA });
            }
            List<HR_KEWANGAN8> kewangan8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kewangan8, "HR_KOD_KEW8", "HR_PENERANGAN");
            return PartialView("_TambahKewangan8", pergerakanGaji);
        }

        public ActionResult InfoKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_InfoKewangan8", model);
        }

        public ActionResult EditKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_EditKewangan8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKewangan8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, PergerakanGajiModels pergerakanGaji)
        {
            if(ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }
                model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                model.HR_TARIKH_UBAH_HR = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = model.HR_NO_PEKERJA });
            }
            return PartialView("_EditKewangan8", pergerakanGaji);
        }


        public ActionResult PadamKewangan8(string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, int? HR_KEW8_ID)
        {
            if (HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KEW8_ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tarikhMula = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikhMula && s.HR_KEW8_ID == HR_KEW8_ID);
            if (kewangan8 == null)
            {
                return HttpNotFound();
            }

            

            PergerakanGajiModels model = new PergerakanGajiModels();
            model.HR_NO_PEKERJA = kewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = kewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = kewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = kewangan8.HR_BULAN;
            model.HR_TAHUN = kewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = kewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = kewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = kewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = kewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = kewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = kewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = kewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = kewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = kewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = kewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = kewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = kewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = kewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = kewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = kewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = kewangan8.HR_EKA;
            model.HR_ITP = kewangan8.HR_ITP;
            model.HR_KEW8_IND = kewangan8.HR_KEW8_IND;
            model.HR_BIL = kewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = kewangan8.HR_KOD_JAWATAN;
            model.HR_KEW8_ID = kewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = kewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = kewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = kewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = kewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = kewangan8.HR_NILAI_EPF;

            InfoPekerja(model);

            List<HR_KEWANGAN8> kew8 = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").ToList();
            ViewBag.HR_KOD_PERUBAHAN = new SelectList(kew8, "HR_KOD_KEW8", "HR_PENERANGAN");

            return PartialView("_PadamKewangan8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamKewangan8(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail)
        {
            //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(modelDetail);
            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
            db.HR_MAKLUMAT_KEWANGAN8.Remove(model);
                
            db.SaveChanges();
            return RedirectToAction("Kewangan8", "Kewangan8", new { key = "1", value = kewangan8.HR_NO_PEKERJA });
        }

        //GET
        public ActionResult Borang(string key, string value)
        {
            MaklumatKakitanganModels model = new MaklumatKakitanganModels();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (key == "1" || key == "4")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_PERIBADI>();
            }
            else if (key == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NAMA_PEKERJA.Contains(value)).ToList<HR_MAKLUMAT_PERIBADI>();
            }
            else if (key == "3")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(a => a.HR_NO_KPBARU == value).ToList<HR_MAKLUMAT_PERIBADI>();
            }

            if(key == "4" && mPeribadi.Count() > 0)
            {
                //HR_MAKLUMAT_KEWANGAN8 kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.si
            }
            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Borang()
        {

            return View();
        }

        public ActionResult Penyata()
        {
            return View();
        }

        public List<HR_MAKLUMAT_PERIBADI> CariPekerja3(string tarafpekerja, string tarafjawatan, string jenisperubahan, int? bulan)
        {
            List<HR_MAKLUMAT_PERIBADI> model = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> ListPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            if (jenisperubahan != "")
            {
                if(tarafjawatan != "")
                {
                    ListPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_AKTIF_IND == jenisperubahan && (s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y" || s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND != "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja && s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == tarafjawatan && (s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month == bulan) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList<HR_MAKLUMAT_PERIBADI>();
                }
                else
                {
                    ListPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_AKTIF_IND == jenisperubahan && (s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y" || s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND != "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T" || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "K") && (s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month == bulan) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList<HR_MAKLUMAT_PERIBADI>();
                }
            }
            else
            {
                if(tarafjawatan != "")
                {
                    ListPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_AKTIF_IND == "Y" && (s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y" || s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja && s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == tarafjawatan && (s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month == bulan) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList<HR_MAKLUMAT_PERIBADI>();
                }
                else
                {
                    ListPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_AKTIF_IND == "Y" && (s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y" || s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T") && s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T" || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "K") && (s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month == bulan) && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_ELAUN_POTONGAN_IND == "G" && p.HR_AKTIF_IND == "Y").Count() > 0).ToList<HR_MAKLUMAT_PERIBADI>();

                }
            }

            ListPeribadi.OrderByDescending(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).ThenBy(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).ThenBy(s => s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT).ToList<HR_MAKLUMAT_PERIBADI>();

            foreach (HR_MAKLUMAT_PERIBADI peribadi in ListPeribadi)
            {
                int gred = 0;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }

                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                {
                    var peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                    GE_PARAMTABLE pTable = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (pTable != null)
                    {
                        HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.FirstOrDefault(s => s.HR_SISTEM_SARAAN == "SSM" && s.HR_PERINGKAT == peringkat && s.HR_GRED_GAJI == pTable.SHORT_DESCRIPTION);

                        if (jadualGaji != null)
                        {
                            model.Add(peribadi);
                        }
                    }
                }
            }
            return model;
        }

        public ActionResult SejarahPergerakanGaji(string key, string value, string kod)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00030");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SenaraiPergerakanGaji()
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            int bulan2 = DateTime.Now.Month;

            List<HR_MAKLUMAT_PERIBADI> model = CariPekerja3("Y", "", "", bulan2);

            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.jabatan = mc.GE_JABATAN.ToList();
            ViewBag.bahagian = mc.GE_BAHAGIAN.ToList();
            ViewBag.Unit = mc.GE_UNIT.ToList();
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            ViewBag.tarafpekerja = "Y";
            ViewBag.tarafjawatan = "";
            ViewBag.jenisperubahan = "";
            ViewBag.bulan = bulan2;


            //ViewBag.year = DateTime.Now.Year;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            //List<SelectListItem> JenisTindakan = new List<SelectListItem>();
            //JenisTindakan.Add(new SelectListItem { Text = "Belum Dibayar Gaji", Value = "1" });
            //JenisTindakan.Add(new SelectListItem { Text = "Teleh Dibayar Gaji", Value = "2" });
            //ViewBag.JenisTindakan = JenisTindakan;

            List<SelectListItem> TarafJawatan = new List<SelectListItem>();
            TarafJawatan.Add(new SelectListItem { Text = "Semua", Value = "" });
            TarafJawatan.AddRange(new SelectList(mc.GE_PARAMTABLE.Where(s => s.GROUPID == 104), "STRING_PARAM", "SHORT_DESCRIPTION"));
            ViewBag.TarafJawatan = TarafJawatan;

            List<SelectListItem> JenisPerubahan = new List<SelectListItem>();
            JenisPerubahan.Add(new SelectListItem { Text = "Semua", Value = "" });
            JenisPerubahan.Add(new SelectListItem { Text = "Tahan Gaji", Value = "N" });
            JenisPerubahan.Add(new SelectListItem { Text = "Cuti Tanpa Gaji", Value = "C" });
            ViewBag.JenisPerubahan = JenisPerubahan;

            return View(model);
        }

        [HttpPost]
        public ActionResult SenaraiPergerakanGaji(string tarafpekerja, string tarafjawatan, string jenisperubahan, int? bulan)
        {
            List<HR_MAKLUMAT_PERIBADI>  model = CariPekerja3(tarafpekerja, tarafjawatan, jenisperubahan, bulan);

            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.jabatan = mc.GE_JABATAN.ToList();   
            ViewBag.bahagian = mc.GE_BAHAGIAN.ToList();
            ViewBag.Unit = mc.GE_UNIT.ToList();
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            ViewBag.tarafpekerja = tarafpekerja;
            ViewBag.tarafjawatan = tarafjawatan;
            ViewBag.jenisperubahan = jenisperubahan;
            ViewBag.bulan = bulan;
            //ViewBag.jenistindakan = jenistindakan;

            //ViewBag.year = tahun;
            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            //List<SelectListItem> JenisTindakan = new List<SelectListItem>();
            //JenisTindakan.Add(new SelectListItem { Text = "Belum Dibayar Gaji", Value = "1" });
            //JenisTindakan.Add(new SelectListItem { Text = "Teleh Dibayar Gaji", Value = "2" });
            //ViewBag.JenisTindakan = JenisTindakan;

            List<SelectListItem> TarafJawatan = new List<SelectListItem>();
            TarafJawatan.Add(new SelectListItem { Text = "Semua", Value = "" });
            TarafJawatan.AddRange(new SelectList(mc.GE_PARAMTABLE.Where(s => s.GROUPID == 104), "STRING_PARAM", "SHORT_DESCRIPTION"));
            ViewBag.TarafJawatan = TarafJawatan;

            List<SelectListItem> JenisPerubahan = new List<SelectListItem>();
            JenisPerubahan.Add(new SelectListItem { Text = "Semua", Value = "" });
            JenisPerubahan.Add(new SelectListItem { Text = "Tahan Gaji", Value = "N" });
            JenisPerubahan.Add(new SelectListItem { Text = "Cuti Tanpa Gaji", Value = "C" });
            ViewBag.JenisPerubahan = JenisPerubahan;

            return View(model);
        }

        public JsonResult SenaraiPergerakanGaji2(int? bulan, int? tahun)
        {
            List<MaklumatKakitanganModels> model = new List<MaklumatKakitanganModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();
            
            foreach(HR_MAKLUMAT_KEWANGAN8 kewangan in kewangan8)
            {
                MaklumatKakitanganModels model2 = new MaklumatKakitanganModels();
                model2.HR_MAKLUMAT_KEWANGAN8 = new HR_MAKLUMAT_KEWANGAN8();
                model2.HR_MAKLUMAT_KEWANGAN8 = kewangan;

                
                model2.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan.HR_NO_PEKERJA);
                if(peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }
                model2.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = peribadi.HR_NO_PEKERJA;
                model2.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;

                model2.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan.HR_NO_PEKERJA);
                if(pekerjaan == null)
                {
                    pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }

                GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == pekerjaan.HR_JABATAN);
                if(pekerjaan == null || jabatan == null)
                {
                    jabatan = new GE_JABATAN();
                }

                model2.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == pekerjaan.HR_JAWATAN);
                if (pekerjaan == null || jawatan == null)
                {
                    jawatan = new HR_JAWATAN();
                }
                model2.HR_MAKLUMAT_KEWANGAN8.HR_KOD_JAWATAN = jawatan.HR_NAMA_JAWATAN;
                model.Add(model2);
            }

            JsonResult json = Json(model, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = 2147483647;

            return json;
        }

        [HttpPost]
        public FileStreamResult PDFSenaraiPergerakanGaji(string tarafpekerja, string tarafjawatan, string jenisperubahan, int? bulan)
        {
            List<HR_MAKLUMAT_PERIBADI> ListPeribadi = CariPekerja3(tarafpekerja, tarafjawatan, jenisperubahan, bulan);
            //List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == DateTime.Now.Year).ToList<HR_MAKLUMAT_KEWANGAN8>();
            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();
            List<GE_BAHAGIAN> sBahagian = new List<GE_BAHAGIAN>();
            List<GE_UNIT> sUnit = new List<GE_UNIT>();
            List<HR_MAKLUMAT_PERIBADI> sTaraf = new List<HR_MAKLUMAT_PERIBADI>();


            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A3.Rotate(), 40, 40, 90, 40);
            var writer = PdfWriter.GetInstance(document, output);
            PageEventHelper page = new PageEventHelper();
            page.bulan = bulan;
            page.tahun = DateTime.Now.Year;
            page.tarafpekerja = tarafpekerja;
            writer.PageEvent = page;
            writer.CloseStream = false;
            document.Open();

            iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);
            int no = 1;
            foreach (HR_MAKLUMAT_PERIBADI peribadi in ListPeribadi)
            {
                int gred = 0;
                if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                
                GE_JABATAN ListJabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                if (ListJabatan == null)
                {
                    ListJabatan = new GE_JABATAN();
                }
                sJabatan.Add(ListJabatan);
                GE_BAHAGIAN ListBahagian = mc.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
                if (ListBahagian == null)
                {
                    ListBahagian = new GE_BAHAGIAN();
                }
                sBahagian.Add(ListBahagian);
                GE_UNIT ListUnit = mc.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_UNIT == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT);
                if (ListBahagian == null)
                {
                    ListUnit = new GE_UNIT();
                }
                sUnit.Add(ListUnit);
                sTaraf.Add(peribadi);
            }

            foreach (GE_JABATAN jabatan in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()).OrderBy(s => s.GE_KOD_JABATAN).ToList())
            {
                foreach (GE_BAHAGIAN bahagian in sBahagian.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN).GroupBy(s => s.GE_KOD_BAHAGIAN).Select(s => s.FirstOrDefault()).ToList())
                {
                    document.NewPage();
                    iTextSharp.text.Paragraph jabatanTxt = new iTextSharp.text.Paragraph("[" + jabatan.GE_KOD_JABATAN + "] :   " + jabatan.GE_KETERANGAN_JABATAN, contentFont);
                    document.Add(jabatanTxt);
                    iTextSharp.text.Paragraph breakTxt = new iTextSharp.text.Paragraph("\n", contentFont);
                    document.Add(breakTxt);

                    //float[] columnWidths = { 2f, 5f, 5f, 5f, 15f, 5f, 4f, 4f, 5f, 4f, 3f, 4f };
                    List<float> columnWidths = new List<float>();
                    columnWidths.Add(2f);
                    columnWidths.Add(5f);
                    columnWidths.Add(5f);
                    columnWidths.Add(5f);
                    columnWidths.Add(15f);
                    columnWidths.Add(5f);
                    columnWidths.Add(4f);
                    columnWidths.Add(4f);
                    columnWidths.Add(5f);
                    columnWidths.Add(4f);
                    columnWidths.Add(3f);
                    columnWidths.Add(4f);

                    if(tarafpekerja == "T")
                    {
                        columnWidths.Insert(1, 4f);
                    }

                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/icon-laporan.png"));
                    pic.ScaleAbsolute(20f, 12f);
                    //pic.SetAbsolutePosition(40f, 465f);
                    pic.SetAbsolutePosition(40f, document.PageSize.GetTop(130));
                    pic.Alignment = Image.TEXTWRAP;
                    pic.IndentationRight = 3f;
                    pic.SpacingBefore = 3f;

                    document.Add(pic);
                    iTextSharp.text.Paragraph bahagianTxt = new iTextSharp.text.Paragraph("             [" + bahagian.GE_KOD_BAHAGIAN + "] : " + bahagian.GE_KETERANGAN, contentFont);
                    document.Add(bahagianTxt);

                    decimal? jumBezaGaji = 0;
                    foreach (GE_UNIT unit in sUnit.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN && s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN).GroupBy(s => s.GE_KOD_UNIT).Select(s => s.FirstOrDefault()).ToList())
                    {
                        iTextSharp.text.Paragraph unitTxt = new iTextSharp.text.Paragraph("                     [" + unit.GE_KOD_UNIT + "] :     " + unit.GE_KETERANGAN, contentFont);
                        document.Add(unitTxt);
                        
                        foreach (HR_MAKLUMAT_PERIBADI mTaraf in sTaraf.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jabatan.GE_KOD_JABATAN && s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT == unit.GE_KOD_UNIT).GroupBy(s => new { s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN, s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN, s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT, s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN }).Select(s => s.FirstOrDefault()).ToList())
                        {
                            GE_PARAMTABLE taraf = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == mTaraf.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                            if(taraf == null)
                            {
                                taraf = new GE_PARAMTABLE();
                            }

                            iTextSharp.text.Paragraph jawatanTxt = new iTextSharp.text.Paragraph("TARAF JAWATAN :   (" + taraf.STRING_PARAM + ")  " + taraf.SHORT_DESCRIPTION, contentFont);
                            document.Add(jawatanTxt);

                            iTextSharp.text.Paragraph breakTxt2 = new iTextSharp.text.Paragraph("\n", contentFont);
                            document.Add(breakTxt2);

                            
                            PdfPTable table = new PdfPTable(columnWidths.ToArray());
                            PdfPCell cell1 = new PdfPCell(new iTextSharp.text.Paragraph("BIL.", contentFont));
                            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            PdfPCell cell2 = new PdfPCell(new iTextSharp.text.Paragraph("NO. PEKERJA", contentFont));
                            PdfPCell cell3 = new PdfPCell(new iTextSharp.text.Paragraph("NO. K/P BARU", contentFont));
                            PdfPCell cell4 = new PdfPCell(new iTextSharp.text.Paragraph("NO. K/P LAMA", contentFont));
                            PdfPCell cell5 = new PdfPCell(new iTextSharp.text.Paragraph("NAMA", contentFont));
                            PdfPCell cell6 = new PdfPCell(new iTextSharp.text.Paragraph("GAJI SEKARANG", contentFont));
                            PdfPCell cell7 = new PdfPCell(new iTextSharp.text.Paragraph("GAJI BARU", contentFont));
                            PdfPCell cell8 = new PdfPCell(new iTextSharp.text.Paragraph("BEZA GAJI", contentFont));
                            PdfPCell cell9 = new PdfPCell(new iTextSharp.text.Paragraph("JUMLAH TUNGGAKAN", contentFont));
                            PdfPCell cell10 = new PdfPCell(new iTextSharp.text.Paragraph("TARIKH K/GAJI", contentFont));
                            PdfPCell cell11 = new PdfPCell(new iTextSharp.text.Paragraph("JENIS P/GAJI", contentFont));
                            PdfPCell cell12 = new PdfPCell(new iTextSharp.text.Paragraph("MAX GAJI", contentFont));

                            cell1.Border = 1;
                            cell1.BorderWidthBottom = 1;
                            cell1.PaddingBottom = 4f;
                            cell2.Border = 1;
                            cell2.BorderWidthBottom = 1;
                            cell2.PaddingBottom = 4f;
                            cell3.Border = 1;
                            cell3.BorderWidthBottom = 1;
                            cell3.PaddingBottom = 4f;
                            cell4.Border = 1;
                            cell4.BorderWidthBottom = 1;
                            cell4.PaddingBottom = 4f;
                            cell5.Border = 1;
                            cell5.BorderWidthBottom = 1;
                            cell5.PaddingBottom = 4f;
                            cell6.Border = 1;
                            cell6.BorderWidthBottom = 1;
                            cell6.PaddingBottom = 4f;
                            cell7.Border = 1;
                            cell7.BorderWidthBottom = 1;
                            cell7.PaddingBottom = 4f;
                            cell8.Border = 1;
                            cell8.BorderWidthBottom = 1;
                            cell8.PaddingBottom = 4f;
                            cell9.Border = 1;
                            cell9.BorderWidthBottom = 1;
                            cell9.PaddingBottom = 4f;
                            cell10.Border = 1;
                            cell10.BorderWidthBottom = 1;
                            cell10.PaddingBottom = 4f;
                            cell11.Border = 1;
                            cell11.BorderWidthBottom = 1;
                            cell11.PaddingBottom = 4f;
                            cell12.Border = 1;
                            cell12.BorderWidthBottom = 1;
                            cell12.PaddingBottom = 4f;

                            table.AddCell(cell1);

                            if (tarafpekerja == "T")
                            {
                                PdfPCell cell25 = new PdfPCell(new iTextSharp.text.Paragraph("ID", contentFont));
                                cell25.Border = 1;
                                cell25.BorderWidthBottom = 1;
                                cell25.PaddingBottom = 4f;
                                table.AddCell(cell25);
                            }

                            table.AddCell(cell2);
                            table.AddCell(cell3);
                            table.AddCell(cell4);
                            table.AddCell(cell5);
                            table.AddCell(cell6);
                            table.AddCell(cell7);
                            table.AddCell(cell8);
                            table.AddCell(cell9);
                            table.AddCell(cell10);
                            table.AddCell(cell11);
                            table.AddCell(cell12);

                            foreach (HR_MAKLUMAT_PERIBADI peribadi in ListPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jabatan.GE_KOD_JABATAN && s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT == unit.GE_KOD_UNIT && s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == mTaraf.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN))
                            {
                                decimal? gaji = 0;
                                decimal? kenaikan = 0;
                                decimal? gaji_maxsimum = 0;
                                var peringkat = "";

                                int gred = 0;
                                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                                {
                                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                                }
                                HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G" && s.HR_AKTIF_IND == "Y");
                                if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                                {
                                    peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                                }
                                
                                GE_PARAMTABLE pTable = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM" && s.ORDINAL == gred);
                                if (pTable == null)
                                {
                                    pTable = new GE_PARAMTABLE();
                                }

                                HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.FirstOrDefault(s => s.HR_PERINGKAT == peringkat && s.HR_GRED_GAJI == pTable.SHORT_DESCRIPTION);
                                if (jadualGaji != null)
                                {
                                    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                                    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                                }
                                else
                                {
                                    jadualGaji = new HR_JADUAL_GAJI();
                                    kenaikan = 0;
                                    gaji_maxsimum = 0;
                                }

                                //if(elaunPotongan != null && jadualGaji != null && pTable != null)
                                //{
                                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null)
                                {
                                    //DateTime date = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                                    //DateTime? date2 = date.AddMonths(1);

                                    //PA_TRANSAKSI_GAJI transaksiGaji = spg.PA_TRANSAKSI_GAJI.Where(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN_GAJI >= date2.Value.Month && s.PA_TAHUN_GAJI >= date2.Value.Year).OrderBy(s => s.PA_TAHUN_GAJI).ThenBy(s => s.PA_BULAN_GAJI).FirstOrDefault();
                                    //if (transaksiGaji != null)
                                    //{
                                    //    gaji = transaksiGaji.PA_GAJI_POKOK;
                                    //}
                                    //else
                                    //{
                                    //    date2 = date;
                                    //    transaksiGaji = spg.PA_TRANSAKSI_GAJI.Where(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN_GAJI <= date2.Value.Month && s.PA_TAHUN_GAJI <= date2.Value.Year).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
                                    //    if(transaksiGaji != null)
                                    //    {
                                    //        gaji = transaksiGaji.PA_GAJI_POKOK;
                                    //    }
                                    //}

                                    if(peribadi.HR_AKTIF_IND == "N" || peribadi.HR_AKTIF_IND == "G")
                                    {
                                        gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    }

                                    gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                                    if (gaji > gaji_maxsimum)
                                    {
                                        gaji = gaji_maxsimum;
                                    }

                                }

                                    int bulanKenaikan = 0;

                                    if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null)
                                    {
                                        bulanKenaikan = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month;
                                    }

                                    int kiraBln = (DateTime.Now.Month - bulanKenaikan);
                                    decimal? tunggakan = kiraBln * jadualGaji.HR_RM_KENAIKAN;

                                    PA_PELARASAN pelarasan = spg.PA_PELARASAN.FirstOrDefault(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN == bulan && s.PA_TAHUN == DateTime.Now.Year);
                                    if(pelarasan == null)
                                    {
                                        pelarasan = new PA_PELARASAN();
                                    }

                                    decimal? bezaGaji = gaji - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    jumBezaGaji += bezaGaji;

                                    PdfPCell cell13 = new PdfPCell(new iTextSharp.text.Paragraph(no + ")", contentFont2));
                                    cell13.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    PdfPCell cell14 = new PdfPCell(new iTextSharp.text.Paragraph(peribadi.HR_NO_PEKERJA, contentFont2));
                                    PdfPCell cell15 = new PdfPCell(new iTextSharp.text.Paragraph(peribadi.HR_NO_KPBARU, contentFont2));
                                    PdfPCell cell16 = new PdfPCell(new iTextSharp.text.Paragraph(peribadi.HR_NO_KPLAMA, contentFont2));
                                    PdfPCell cell17 = new PdfPCell(new iTextSharp.text.Paragraph(peribadi.HR_NAMA_PEKERJA, contentFont2));
                                    PdfPCell cell18 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK), contentFont2));
                                    PdfPCell cell19 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", gaji), contentFont2));
                                    PdfPCell cell20 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", bezaGaji), contentFont2));
                                    PdfPCell cell21 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", tunggakan), contentFont2));
                                    PdfPCell cell22 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI), contentFont2));
                                    PdfPCell cell23 = new PdfPCell(new iTextSharp.text.Paragraph(null, contentFont2));
                                    PdfPCell cell24 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", jadualGaji.HR_GAJI_MAX), contentFont2));


                                    cell13.Border = 0;
                                    cell14.Border = 0;
                                    cell15.Border = 0;
                                    cell16.Border = 0;
                                    cell17.Border = 0;
                                    cell18.Border = 0;
                                    cell19.Border = 0;
                                    cell20.Border = 0;
                                    cell21.Border = 0;
                                    cell22.Border = 0;
                                    cell23.Border = 0;
                                    cell24.Border = 0;

                                    table.AddCell(cell13);

                                    if (tarafpekerja == "T")
                                    {
                                        PdfPCell cell26 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                                        cell26.Border = 0;
                                        table.AddCell(cell26);
                                    }

                                    table.AddCell(cell14);
                                    table.AddCell(cell15);
                                    table.AddCell(cell16);
                                    table.AddCell(cell17);
                                    table.AddCell(cell18);
                                    table.AddCell(cell19);
                                    table.AddCell(cell20);
                                    table.AddCell(cell21);
                                    table.AddCell(cell22);
                                    table.AddCell(cell23);
                                    table.AddCell(cell24);

                                    

                                    no++;
                                //}
                            }
                            table.TotalWidth = document.PageSize.Width - 80f;
                            table.WidthPercentage = 100;

                            document.Add(table);
                            iTextSharp.text.Paragraph breakTxt3 = new iTextSharp.text.Paragraph("\n", contentFont);
                            document.Add(breakTxt3);
                        }
                        
                    }
                    PdfPTable tableBahagian = new PdfPTable(columnWidths.ToArray());
                    PdfPCell cellBahagian1 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    cellBahagian1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PdfPCell cellBahagian2 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian3 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian4 = new PdfPCell(new iTextSharp.text.Paragraph("JUMLAH UNTUK BAHAGIAN["+bahagian.GE_KOD_BAHAGIAN+"] : ", contentFont));
                    cellBahagian4.Colspan = 2;
                    cellBahagian4.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell cellBahagian6 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian7 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian8 = new PdfPCell(new iTextSharp.text.Paragraph(string.Format("{0:#,0.00}", jumBezaGaji), contentFont2));
                    PdfPCell cellBahagian9 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian10 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian11 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                    PdfPCell cellBahagian12 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));

                    cellBahagian1.Border = 0;
                    cellBahagian2.Border = 0;
                    cellBahagian3.Border = 0;
                    cellBahagian4.Border = 0;

                    cellBahagian6.Border = 0;
                    cellBahagian7.Border = 0;
                    cellBahagian8.Border = 0;
                    cellBahagian9.Border = 0;
                    cellBahagian10.Border = 0;
                    cellBahagian11.Border = 0;
                    cellBahagian12.Border = 0;


                    tableBahagian.AddCell(cellBahagian1);
                    if (tarafpekerja == "T")
                    {
                        PdfPCell cellBahagian13 = new PdfPCell(new iTextSharp.text.Paragraph("", contentFont));
                        cellBahagian13.Border = 0;
                        tableBahagian.AddCell(cellBahagian13);
                    }
                    tableBahagian.AddCell(cellBahagian2);
                    tableBahagian.AddCell(cellBahagian3);
                    tableBahagian.AddCell(cellBahagian4);

                    tableBahagian.AddCell(cellBahagian6);
                    tableBahagian.AddCell(cellBahagian7);
                    tableBahagian.AddCell(cellBahagian8);
                    tableBahagian.AddCell(cellBahagian9);
                    tableBahagian.AddCell(cellBahagian10);
                    tableBahagian.AddCell(cellBahagian11);
                    tableBahagian.AddCell(cellBahagian12);

                    tableBahagian.TotalWidth = document.PageSize.Width - 80f;
                    tableBahagian.WidthPercentage = 100;

                    document.Add(tableBahagian);
                }
                
            }
            
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(" ");

                
            paragraph.Alignment = Element.ALIGN_JUSTIFIED;
                
            paragraph.SpacingBefore = 10f;
                
            document.Add(paragraph);
               
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        [HttpPost]
        public FileResult EXCSenaraiPergerakanGaji(string tarafpekerja, string tarafjawatan, string jenisperubahan, int? bulan)
        {
            List<HR_MAKLUMAT_PERIBADI> ListPeribadi = CariPekerja3(tarafpekerja, tarafjawatan, jenisperubahan, bulan);
            //List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == DateTime.Now.Year).ToList<HR_MAKLUMAT_KEWANGAN8>();
            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();
            List<GE_BAHAGIAN> sBahagian = new List<GE_BAHAGIAN>();
            List<GE_UNIT> sUnit = new List<GE_UNIT>();
            List<HR_MAKLUMAT_PERIBADI> sTaraf = new List<HR_MAKLUMAT_PERIBADI>();

            foreach (HR_MAKLUMAT_PERIBADI peribadi in ListPeribadi)
            {
                int gred = 0;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }

                GE_JABATAN ListJabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                if (ListJabatan == null)
                {
                    ListJabatan = new GE_JABATAN();
                }
                sJabatan.Add(ListJabatan);
                GE_BAHAGIAN ListBahagian = mc.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
                if (ListBahagian == null)
                {
                    ListBahagian = new GE_BAHAGIAN();
                }
                sBahagian.Add(ListBahagian);
                GE_UNIT ListUnit = mc.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_UNIT == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT);
                if (ListBahagian == null)
                {
                    ListUnit = new GE_UNIT();
                }
                sUnit.Add(ListUnit);
                sTaraf.Add(peribadi);
            }

            //List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();

            //if(model.Count() <= 0)
            //{
            //    model = new List<HR_MAKLUMAT_KEWANGAN8>();
            //}
            DataSet ds = new DataSet();

            //List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();

            //foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            //{
            //    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA);
            //    GE_JABATAN jabatan2 = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            //    sJabatan.Add(jabatan2);
            //}
            //var no2 = 0;
            //foreach (GE_JABATAN jab in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()))
            //{

            //    no2++;
            //    DataTable dt = new DataTable("SENARAI PERGERAKAN GAJI "+ jab.GE_KOD_JABATAN);
            //    dt.Columns.AddRange(new DataColumn[6] { new DataColumn("#"),
            //                                new DataColumn("No Pekerja"),
            //                                new DataColumn("Nama Pekerja"),
            //                                new DataColumn("Tarikh Pergerakan"),
            //                                new DataColumn("Jabatan"),
            //                                new DataColumn("Jawatan")});


            //    var no = 0;
            //    foreach (var l in ListPeribadi)
            //    {
            //        HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_NO_PEKERJA);
            //        if (peribadi == null)
            //        {
            //            peribadi = new HR_MAKLUMAT_PERIBADI();
            //        }

            //        if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jab.GE_KOD_JABATAN)
            //        {
            //            GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            //            if (jabatan == null)
            //            {
            //                jabatan = new GE_JABATAN();
            //            }
            //            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            //            if (jawatan == null)
            //            {
            //                jawatan = new HR_JAWATAN();
            //            }
            //            ++no;
            //            dt.Rows.Add(no, l.HR_NO_PEKERJA, peribadi.HR_NAMA_PEKERJA, string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN), jabatan.GE_KETERANGAN_JABATAN, jawatan.HR_NAMA_JAWATAN);
            //        }
                    
            //    }
            //    ds.Tables.Add(dt);
            //}

            using (XLWorkbook wb = new XLWorkbook())
            {
                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "Appril" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "september" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };
                var Bulan = "";
                foreach (var m in associativeArray)
                {
                    if (bulan == m.Key)
                    {
                        Bulan = m.Value;
                    }

                }

                var associativeArray2 = new Dictionary<string, string>() { { "Y", "Kakitangan" }, { "T", "Pekerja" } };
                var kakitangan = "";
                foreach (var m in associativeArray2)
                {
                    if (tarafpekerja == m.Key)
                    {
                        kakitangan = m.Value;
                    }

                }
                int no = 1;
                foreach (GE_JABATAN jabatan in sJabatan.GroupBy(s => s.GE_KOD_JABATAN).Select(s => s.FirstOrDefault()).OrderBy(s => s.GE_KOD_JABATAN).ToList())
                {

                    foreach (GE_BAHAGIAN bahagian in sBahagian.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN).GroupBy(s => s.GE_KOD_BAHAGIAN).Select(s => s.FirstOrDefault()).ToList())
                    {
                        var cellleft = 1;
                        var cellright = 3;
                        var ws = wb.Worksheets.Add("JABATAN " + jabatan.GE_KOD_JABATAN + " BAHAGIAN " + bahagian.GE_KOD_BAHAGIAN);

                        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                        ws.PageSetup.AdjustTo(80);
                        ws.PageSetup.PaperSize = XLPaperSize.A3Paper;
                        ws.PageSetup.VerticalDpi = 400;
                        ws.PageSetup.HorizontalDpi = 400;

                        
                        ws.Column(1).Width = 5;
                        ws.Column(2).Width = 15;
                        ws.Column(3).Width = 15;
                        ws.Column(4).Width = 15;
                        ws.Column(5).Width = 60;
                        ws.Column(6).Width = 15;
                        ws.Column(7).Width = 15;
                        ws.Column(8).Width = 15;
                        ws.Column(9).Width = 15;
                        ws.Column(10).Width = 15;
                        ws.Column(11).Width = 15;
                        ws.Column(12).Width = 15;

                        var imagePath = Server.MapPath("~/Content/img/logo-mbpj.gif");
                        var image = ws.AddPicture(imagePath)
                            .MoveTo(ws.Cell("B" + cellleft).Address)
                            .Scale(0.1); // optional: resize picture
                        ws.Cell("A" + cellleft).Value = "MAJLIS BANDARAYA PETALING JAYA\nSENARAI PERGERAKAN GAJI UNTUK BULAN "+ Bulan.ToUpper() +" BAGI "+ kakitangan.ToUpper() + "\nMBPJ";
                        ws.Cell("A" + cellleft).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("A" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("A" + cellleft).Style.Font.FontSize = 12;
                        ws.Cell("A" + cellleft).Style.Font.Bold = true;
                        ws.Range("A" + cellleft + ":I" + cellright).Merge();

                        cellleft += 5;
                        cellright += 3;

                        ws.Cell("A"+ cellleft).Value = "[" + jabatan.GE_KOD_JABATAN + "] :   " + jabatan.GE_KETERANGAN_JABATAN;
                        ws.Cell("A" + cellleft).Style.Font.Bold = true;
                        ws.Range("A"+ cellleft + ":I"+ cellright).Merge();

                        cellleft += 2;
                        cellright += 2;

                        var imagePath2 = Server.MapPath("~/Content/img/icon-laporan.png");
                        var image2 = ws.AddPicture(imagePath2)
                            .MoveTo(ws.Cell("A"+ cellleft).Address)
                            .Scale(0.05); // optional: resize picture

                        ws.Cell("A"+ cellleft).Value = "             [" + bahagian.GE_KOD_BAHAGIAN + "] : " + bahagian.GE_KETERANGAN;
                        ws.Cell("A" + cellleft).Style.Font.Bold = true;
                        ws.Range("A" + cellleft + ":I" + cellright).Merge();

                        decimal? jumBezaGaji = 0;
                        foreach (GE_UNIT unit in sUnit.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN && s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN).GroupBy(s => s.GE_KOD_UNIT).Select(s => s.FirstOrDefault()).ToList())
                        {
                            cellleft += 2;
                            cellright += 2;

                            ws.Cell("A" + cellleft).Value = "                     [" + unit.GE_KOD_UNIT + "] :     " + unit.GE_KETERANGAN;
                            ws.Cell("A" + cellleft).Style.Font.Bold = true;
                            ws.Range("A" + cellleft + ":I" + cellright).Merge();
                            var tblNo = 0;
                            foreach (HR_MAKLUMAT_PERIBADI mTaraf in sTaraf.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jabatan.GE_KOD_JABATAN && s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT == unit.GE_KOD_UNIT).GroupBy(s => new { s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN, s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN, s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT, s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN }).Select(s => s.FirstOrDefault()).ToList())
                            {
                                GE_PARAMTABLE taraf = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == mTaraf.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                                if (taraf == null)
                                {
                                    taraf = new GE_PARAMTABLE();
                                }
                                cellleft += 2;
                                cellright += 2;
                                ws.Cell("A" + cellleft).Value = "TARAF JAWATAN :   (" + taraf.STRING_PARAM + ")  " + taraf.SHORT_DESCRIPTION;
                                ws.Cell("A" + cellleft).Style.Font.Bold = true;
                                ws.Range("A" + cellleft + ":I" + cellright).Merge();

                                DataTable dt = new DataTable(tblNo.ToString());
                                dt.Columns.Add("BIL.");
                                dt.Columns.Add("NO. PEKERJA");
                                dt.Columns.Add("NO. K/P BARU");
                                dt.Columns.Add("NO. K/P LAMA");
                                dt.Columns.Add("NAMA");
                                dt.Columns.Add("GAJI SEKARANG");
                                dt.Columns.Add("GAJI BARU");
                                dt.Columns.Add("BEZA GAJI");
                                dt.Columns.Add("JUMLAH\nTUNGGAKAN");
                                dt.Columns.Add("TARIKH K/GAJI");
                                dt.Columns.Add("JENIS P/GAJI");
                                dt.Columns.Add("MAX GAJI");

                                foreach (HR_MAKLUMAT_PERIBADI peribadi in ListPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jabatan.GE_KOD_JABATAN && s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT == unit.GE_KOD_UNIT && s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == mTaraf.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN))
                                {
                                    decimal? gaji = 0;
                                    decimal? kenaikan = 0;
                                    decimal? gaji_maxsimum = 0;
                                    var peringkat = "";

                                    int gred = 0;
                                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                                    {
                                        gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                                    }
                                    HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G" && s.HR_AKTIF_IND == "Y");
                                    if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                                    {
                                        peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                                    }
                                    
                                    GE_PARAMTABLE pTable = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM" && s.ORDINAL == gred);
                                    if (pTable == null)
                                    {
                                        pTable = new GE_PARAMTABLE();
                                    }
                                    
                                    HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.FirstOrDefault(s => s.HR_PERINGKAT == peringkat && s.HR_GRED_GAJI == pTable.SHORT_DESCRIPTION);
                                    if (jadualGaji != null)
                                    {
                                        kenaikan = jadualGaji.HR_RM_KENAIKAN;
                                        gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                                    }
                                    else
                                    {
                                        jadualGaji = new HR_JADUAL_GAJI();
                                        kenaikan = 0;
                                        gaji_maxsimum = 0;
                                    }

                                    //if(elaunPotongan != null && jadualGaji != null && pTable != null)
                                    //{
                                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null)
                                    {
                                        //DateTime date = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                                        //DateTime? date2 = date.AddMonths(1);

                                        //PA_TRANSAKSI_GAJI transaksiGaji = spg.PA_TRANSAKSI_GAJI.Where(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN_GAJI >= date2.Value.Month && s.PA_TAHUN_GAJI >= date2.Value.Year).OrderBy(s => s.PA_TAHUN_GAJI).ThenBy(s => s.PA_BULAN_GAJI).FirstOrDefault();
                                        //if (transaksiGaji != null)
                                        //{
                                        //    gaji = transaksiGaji.PA_GAJI_POKOK;
                                        //}
                                        //else
                                        //{
                                        //    date2 = date;
                                        //    transaksiGaji = spg.PA_TRANSAKSI_GAJI.Where(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN_GAJI <= date2.Value.Month && s.PA_TAHUN_GAJI <= date2.Value.Year).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
                                        //    if (transaksiGaji != null)
                                        //    {
                                        //        gaji = transaksiGaji.PA_GAJI_POKOK;
                                        //    }
                                        //}

                                        if (peribadi.HR_AKTIF_IND == "N" || peribadi.HR_AKTIF_IND == "G")
                                        {
                                            gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                        }

                                        gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                                        if (gaji > gaji_maxsimum)
                                        {
                                            gaji = gaji_maxsimum;
                                        }
                                    }

                                    int bulanKenaikan = 0;

                                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null)
                                    {
                                        bulanKenaikan = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI.Value.Month;
                                    }

                                    int kiraBln = (DateTime.Now.Month - bulanKenaikan);
                                    decimal? tunggakan = kiraBln * jadualGaji.HR_RM_KENAIKAN;

                                    PA_PELARASAN pelarasan = spg.PA_PELARASAN.FirstOrDefault(s => s.PA_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.PA_BULAN == bulan && s.PA_TAHUN == DateTime.Now.Year);
                                    if (pelarasan == null)
                                    {
                                        pelarasan = new PA_PELARASAN();
                                    }

                                    decimal? bezaGaji = gaji - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    jumBezaGaji += bezaGaji;

                                    dt.Rows.Add(no, peribadi.HR_NO_PEKERJA, peribadi.HR_NO_KPBARU, peribadi.HR_NO_KPLAMA, peribadi.HR_NAMA_PEKERJA, string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK), string.Format("{0:#,0.00}", gaji), string.Format("{0:#,0.00}", bezaGaji), string.Format("{0:#,0.00}", tunggakan), string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI), pelarasan.PA_JENIS_PELARASAN, string.Format("{0:#,0.00}", jadualGaji.HR_GAJI_MAX));
                                    

                                    no++;
                                }
                                cellleft += 2;
                                cellright += 2;
                                ws.Cell(cellleft, 1).InsertTable(dt.AsEnumerable());

                                var countPeribadi = ListPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jabatan.GE_KOD_JABATAN && s.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.HR_MAKLUMAT_PEKERJAAN.HR_UNIT == unit.GE_KOD_UNIT && s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == mTaraf.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN).Count();
                                cellleft += countPeribadi;
                                cellright += countPeribadi;

                                
                            }
                        }

                        cellleft += 3;
                        cellright += 3;

                        ws.Cell("D" + cellleft).Value = "JUMLAH UNTUK BAHAGIAN [" + jabatan.GE_KOD_JABATAN + "] :   ";
                        ws.Cell("D" + cellleft).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("D" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Range("D" + cellleft + ":E" + cellright).Merge();
                        ws.Cell("D" + cellleft).Style.Font.Bold = true;

                        ws.Cell("H" + cellleft).Value = string.Format("{0:#,0.00}", jumBezaGaji);


                        cellleft += 2;
                        cellright += 4;
                    }
                }
                    
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Senarai_Pergerakan_gaji.xlsx");
                }
            }
        }

        //public ActionResult PergerakanGaji(string key, string value)
        //{
        //    List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
        //    List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

        //    if (key == "1" || key == "4")
        //    {
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == value).ToList();

        //    }
        //    else if (key == "2")
        //    {
        //        value = value.ToUpper();
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper())).ToList();
        //    }
        //    else if (key == "3")
        //    {
        //        mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU.Contains(value)).ToList();
        //    }

        //    if(mPeribadi.Count() > 0)
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == "00001").ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    ViewBag.key = key;
        //    ViewBag.mPeribadi = mPeribadi;
        //    ViewBag.detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
        //    ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
        //    ViewBag.Matriks = db.HR_MATRIKS_GAJI.ToList();
        //    ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
        //    return View(model);
        //}

        public string JawatanInd(string status, string taraf)
        {
            var jawatan_ind = "";

            if (status == "Y")
            {
                jawatan_ind = "K" + taraf;
            }
            else if (status == "T")
            {
                jawatan_ind = "P" + taraf;
            }
            return jawatan_ind;
        }

        public PergerakanGajiModels mElaunPotongan(string HR_NO_PEKERJA, string HR_JAWATAN_IND)
        {
            PergerakanGajiModels data = new PergerakanGajiModels();
            data.HR_KRITIKAL = 0;
            data.HR_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E").ToList();
            if (elaunPotongan.Count() > 0)
            {
                foreach (var item3 in elaunPotongan)
                {
                    HR_ELAUN elaun11 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item3.HR_KOD_ELAUN_POTONGAN);
                    if (elaun11 != null)
                    {
                        data.HR_WILAYAH = Convert.ToSingle(item3.HR_JUMLAH);
                        data.HR_KOD_WILAYAH = item3.HR_KOD_ELAUN_POTONGAN;
                    }
                    HR_ELAUN awam12 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item3.HR_KOD_ELAUN_POTONGAN);
                    if (awam12 != null)
                    {
                        data.HR_KRITIKAL = Convert.ToSingle(item3.HR_JUMLAH);
                        data.HR_KOD_KRITIKAL = item3.HR_KOD_ELAUN_POTONGAN;
                    }
                }

            }
            return data;
        }

        public ActionResult PergerakanGajiTest(string key, string value, int? bulan, ManageMessageId? message)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();

            sPeribadi = CariPekerja(key, value, bulan, "00001");

            HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            if (peribadi2 == null)
            {
                peribadi2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = peribadi2.HR_NAMA_PEKERJA;
            ViewBag.HR_NP_FINALISED_HR = peribadi2.HR_NO_PEKERJA;
            int belumMuktamad = 0;
            List<HR_MAKLUMAT_KEWANGAN8> Kew8 = db.HR_MAKLUMAT_KEWANGAN8.AsEnumerable().Where(s => s.HR_KOD_PERUBAHAN == "00001" && (((Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && Convert.ToDateTime(s.HR_TARIKH_MULA).Year < DateTime.Now.Year) && s.HR_FINALISED_IND_HR != "Y" && s.HR_UBAH_IND == "1") || ((Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && s.HR_TAHUN == DateTime.Now.Year) && s.HR_FINALISED_IND_HR == "Y" && s.HR_UBAH_IND == "0"))).OrderByDescending(s => s.HR_KEW8_ID).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();
            List<PergerakanGajiModels> editKew8 = new List<PergerakanGajiModels>();
            foreach (HR_MAKLUMAT_KEWANGAN8 item2 in Kew8)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA);
                if(peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }
                
                var ubahGred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                GE_PARAMTABLE sGred2 = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == ubahGred);
                if (sGred2 == null)
                {
                    sGred2 = new GE_PARAMTABLE();
                }

                if (item2.HR_FINALISED_IND_HR != "Y")
                {
                    belumMuktamad++;
                }

                PergerakanGajiModels edit = new PergerakanGajiModels();
                edit.HR_WILAYAH = 0;
                edit.HR_PERUBAHAN_WILAYAH = 0;
                edit.HR_PERGERAKAN_EWIL = 0;

                edit.HR_KRITIKAL = 0;
                edit.HR_PERUBAHAN_KRITIKAL = 0;
                edit.HR_PERGERAKAN_EKAL = 0;

                edit.HR_JUMLAH_PERUBAHAN = 0;
                edit.HR_GAJI_BARU = 0;

                var jawatan_ind = JawatanInd(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                int? gred = null;
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item2.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item2.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item2.HR_KEW8_ID).ToList();
                foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in kew8Detail)
                {
                    if (detail.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(detail.HR_GRED);
                    }

                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }

                    int? peringkat = CariPeringkat(detail.HR_MATRIKS_GAJI);

                    //var pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gaji_maxsimum = 0;
                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == detail.HR_GAJI_BARU).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    edit.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    edit.HR_GAJI_MAX = matriks.HR_GAJI_MAX;

                    edit.HR_JENIS_PERGERAKAN = detail.HR_JENIS_PERGERAKAN;
                    edit.HR_GAJI_BARU = detail.HR_GAJI_BARU;
                    edit.HR_MATRIKS_GAJI = detail.HR_MATRIKS_GAJI;
                    edit.HR_GRED = sGred.SHORT_DESCRIPTION;

                    HR_ELAUN elaun = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                    if (elaun != null)
                    {
                        PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                        edit.HR_WILAYAH = Convert.ToSingle(elaunPotongan.HR_WILAYAH);
                        edit.HR_KOD_WILAYAH = detail.HR_KOD_PELARASAN;
                        edit.HR_PERUBAHAN_WILAYAH = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_PERGERAKAN_EWIL = detail.HR_PERGERAKAN_EWIL;
                    }

                    HR_ELAUN awam12 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                    if (awam12 != null)
                    {
                        PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                        edit.HR_KRITIKAL = Convert.ToSingle(elaunPotongan.HR_KRITIKAL);
                        edit.HR_KOD_KRITIKAL = detail.HR_KOD_PELARASAN;
                        edit.HR_PERUBAHAN_KRITIKAL = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_PERGERAKAN_EKAL = detail.HR_PERGERAKAN_EKAL;
                    }

                    HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN );
                    if (tggkk != null)
                    {
                        var tunggakan = detail.HR_GAJI_BARU - detail.HR_GAJI_LAMA;
                        edit.HR_KOD_PELARASAN = detail.HR_KOD_PELARASAN;
                        //edit.HR_JUMLAH_PERUBAHAN = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_JUMLAH_PERUBAHAN = tunggakan;
                    }
                }

                edit.HR_NO_PEKERJA = item2.HR_NO_PEKERJA;
                edit.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;
                edit.HR_NO_KPBARU = peribadi.HR_NO_KPBARU;
                edit.HR_KOD_PERUBAHAN = item2.HR_KOD_PERUBAHAN;
                edit.HR_TARIKH_MULA = item2.HR_TARIKH_MULA;
                edit.HR_KEW8_ID = item2.HR_KEW8_ID;
                
                edit.HR_GRED_LAMA = sGred2.SHORT_DESCRIPTION;
                
                edit.HR_GAJI_LAMA = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                
                edit.HR_MATRIKS_GAJI_LAMA = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                edit.HR_BUTIR_PERUBAHAN = item2.HR_BUTIR_PERUBAHAN;
                edit.HR_NO_SURAT_KEBENARAN = item2.HR_NO_SURAT_KEBENARAN;
                edit.HR_NP_FINALISED_HR = item2.HR_NP_FINALISED_HR;
                edit.HR_FINALISED_IND_HR = item2.HR_FINALISED_IND_HR;


                HR_MAKLUMAT_PERIBADI pgw = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NP_FINALISED_HR);
                if (pgw == null)
                {
                    pgw = new HR_MAKLUMAT_PERIBADI();
                }
                ViewBag.HR_NAMA_PEGAWAI2 = pgw.HR_NAMA_PEKERJA;
                editKew8.Add(edit);
            }
            ViewBag.EditKew8 = editKew8;
            ViewBag.JumEditKew8 = editKew8.GroupBy(s => new { s.HR_KOD_PERUBAHAN, s.HR_KEW8_ID, s.HR_TARIKH_MULA }).Select(s => s.FirstOrDefault()).ToList();
            ViewBag.countEditKew8 = editKew8.Count();
            ViewBag.belumMuktamad = belumMuktamad;

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                if (editKew8.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001").Count() <= 0)
                {
                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                    int? gred = null;
                    if (item.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                    }
                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }


                    HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == item.HR_NO_PEKERJA).Count() > 0);
                    if (gajiUpah == null)
                    {
                        gajiUpah = new HR_GAJI_UPAHAN();
                    }

                    HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                    if (potongan2 == null)
                    {
                        potongan2 = new HR_POTONGAN();
                    }
                    ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

                    var jawatan_ind = JawatanInd(item.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, item.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);

                    int? peringkat = CariPeringkat(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI);
                    string pkt = "P" + peringkat;
                    decimal? kenaikan = 0;
                    decimal? gajiPokokBaru = 0;
                    decimal? gajiPokokBaru2 = 0;
                    decimal? gaji_maxsimum = 0;
                    decimal? tunggakan = 0;
                    HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    if (jadualGaji != null)
                    {
                        kenaikan = jadualGaji.HR_RM_KENAIKAN;
                        gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    }

                    gajiPokokBaru = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                    if (gajiPokokBaru > gaji_maxsimum)
                    {
                        gajiPokokBaru2 = gaji_maxsimum;
                    }
                    else
                    {
                        gajiPokokBaru2 = gajiPokokBaru;
                    }

                    PergerakanGajiModels elaun = mElaunPotongan(item.HR_NO_PEKERJA, jawatan_ind);

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    tunggakan = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    //tunggakan = (bulan - Convert.ToDateTime(item.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month) * kenaikan;

                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
                    pergerakanGaji.HR_NO_KPBARU = item.HR_NO_KPBARU;
                    pergerakanGaji.HR_GAJI_POKOK = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerakanGaji.HR_GRED_LAMA = sGred.SHORT_DESCRIPTION;
                    pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;
                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    pergerakanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    pergerakanGaji.HR_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerakanGaji.HR_MATRIKS_GAJI = "P" + matriks.HR_PERINGKAT + "T" + Convert.ToDouble(matriks.HR_TAHAP);
                    pergerakanGaji.HR_MATRIKS_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = tunggakan;

                    pergerakanGaji.HR_JENIS_PERGERAKAN = "D";

                    pergerakanGaji.HR_KRITIKAL = elaun.HR_KRITIKAL;
                    pergerakanGaji.HR_KOD_KRITIKAL = elaun.HR_KOD_KRITIKAL;
                    pergerakanGaji.HR_PERUBAHAN_KRITIKAL = 0;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = 0;

                    pergerakanGaji.HR_WILAYAH = elaun.HR_WILAYAH;
                    pergerakanGaji.HR_KOD_WILAYAH = elaun.HR_KOD_WILAYAH;
                    pergerakanGaji.HR_PERUBAHAN_WILAYAH = 0;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = 0;

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        string HR_PERUBAHAN_WILAYAH = string.Format("{0:0.00}", pergerakanGaji.HR_JUMLAH_PERUBAHAN * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100));
                        pergerakanGaji.HR_PERUBAHAN_WILAYAH = Convert.ToDecimal(HR_PERUBAHAN_WILAYAH);
                    }

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        string HR_PERGERAKAN_EWIL = string.Format("{0:0.00}", pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100));
                        pergerakanGaji.HR_PERGERAKAN_EWIL = Convert.ToDecimal(HR_PERGERAKAN_EWIL);
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        string HR_PERUBAHAN_KRITIKAL = string.Format("{0:0.00}", pergerakanGaji.HR_JUMLAH_PERUBAHAN * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100));
                        pergerakanGaji.HR_PERUBAHAN_KRITIKAL = Convert.ToDecimal(HR_PERUBAHAN_KRITIKAL);
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        string HR_PERGERAKAN_EKAL = string.Format("{0:0.00}", pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100));
                        pergerakanGaji.HR_PERGERAKAN_EKAL = Convert.ToDecimal(HR_PERGERAKAN_EKAL);
                    }

                    //pergerakanGaji.CHECKED = 0;
                    //var countKew8 = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(k => k.HR_KEW8_ID).Where(k => k.HR_KOD_PERUBAHAN == "00001" && k.HR_NO_PEKERJA == item.HR_NO_PEKERJA && k.HR_FINALISED_IND_HR == "Y").Count();

                    //if (countKew8 <= 0)
                    //{
                    //    pergerakanGaji.CHECKED = 1;
                    //}
                    model.Add(pergerakanGaji);
                }
            }
            //ViewBag.countKew8 = model.Where(s => s.CHECKED == 1).Count();
            ViewBag.countKew8 = model.Count();
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
            ViewBag.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            return View(model);
        }

        public List<PergerakanGajiModels> ViewEditKew8(int? bulan, string tindakan, string tarafpekerja)
        {
            List<HR_MAKLUMAT_KEWANGAN8> Kew8 = db.HR_MAKLUMAT_KEWANGAN8.AsEnumerable().Where(s => s.HR_KOD_PERUBAHAN == "00001" && Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && s.HR_TAHUN == DateTime.Now.Year).OrderByDescending(s => s.HR_KEW8_ID).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();

            if (tindakan == "K")
            {
                Kew8 = Kew8.AsEnumerable().Where(s => s.HR_FINALISED_IND_HR != "Y" && s.HR_UBAH_IND == "1").ToList();
            }

            if (tindakan == "M")
            {
                Kew8 = Kew8.AsEnumerable().Where(s => s.HR_FINALISED_IND_HR == "Y" && s.HR_UBAH_IND == "0").ToList();
            }

            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            foreach (HR_MAKLUMAT_KEWANGAN8 item2 in Kew8)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA && s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja);
                if (peribadi != null)
                {
                    var ubahGred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);

                    GE_PARAMTABLE sGred2 = cariGred(ubahGred, null);


                    //if (item2.HR_FINALISED_IND_HR != "Y")
                    //{
                    //    belumMuktamad++;
                    //}

                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                    pergerakanGaji.HR_WILAYAH = 0;
                    pergerakanGaji.HR_PERUBAHAN_WILAYAH = 0;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = 0;

                    pergerakanGaji.HR_KRITIKAL = 0;
                    pergerakanGaji.HR_PERUBAHAN_KRITIKAL = 0;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = 0;

                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = 0;
                    pergerakanGaji.HR_GAJI_BARU = 0;

                    var jawatan_ind = JawatanInd(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                    int? gred = null;
                    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item2.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item2.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item2.HR_KEW8_ID).ToList();
                    foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in kew8Detail)
                    {
                        if (detail.HR_GRED != null)
                        {
                            gred = Convert.ToInt32(detail.HR_GRED);
                        }

                        GE_PARAMTABLE sGred = cariGred(gred, null);

                        int? peringkat = CariPeringkat(detail.HR_MATRIKS_GAJI);

                        //var pkt = "P" + peringkat;
                        //decimal? kenaikan = 0;
                        //decimal? gaji_maxsimum = 0;
                        //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                        //if (jadualGaji != null)
                        //{
                        //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                        //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                        //}

                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == detail.HR_GAJI_BARU).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                        if (matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                            matriks.HR_GAJI_MIN = 0;
                            matriks.HR_GAJI_MAX = 0;
                            matriks.HR_GAJI_POKOK = 0;
                        }

                        pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                        pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;

                        pergerakanGaji.HR_JENIS_PERGERAKAN = detail.HR_JENIS_PERGERAKAN;
                        pergerakanGaji.HR_GAJI_BARU = detail.HR_GAJI_BARU;
                        pergerakanGaji.HR_MATRIKS_GAJI = detail.HR_MATRIKS_GAJI;
                        pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;

                        HR_ELAUN elaun = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                        if (elaun != null)
                        {
                            PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                            pergerakanGaji.HR_WILAYAH = Convert.ToSingle(elaunPotongan.HR_WILAYAH);
                            pergerakanGaji.HR_KOD_WILAYAH = detail.HR_KOD_PELARASAN;
                            pergerakanGaji.HR_PERUBAHAN_WILAYAH = detail.HR_JUMLAH_PERUBAHAN;
                            pergerakanGaji.HR_PERGERAKAN_EWIL = detail.HR_PERGERAKAN_EWIL;
                        }

                        HR_ELAUN awam12 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                        if (awam12 != null)
                        {
                            PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                            pergerakanGaji.HR_KRITIKAL = Convert.ToSingle(elaunPotongan.HR_KRITIKAL);
                            pergerakanGaji.HR_KOD_KRITIKAL = detail.HR_KOD_PELARASAN;
                            pergerakanGaji.HR_PERUBAHAN_KRITIKAL = detail.HR_JUMLAH_PERUBAHAN;
                            pergerakanGaji.HR_PERGERAKAN_EKAL = detail.HR_PERGERAKAN_EKAL;
                        }

                        HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN);
                        if (tggkk != null)
                        {
                            var tunggakan = detail.HR_GAJI_BARU - detail.HR_GAJI_LAMA;
                            pergerakanGaji.HR_KOD_PELARASAN = detail.HR_KOD_PELARASAN;
                            pergerakanGaji.HR_JUMLAH_PERUBAHAN = detail.HR_JUMLAH_PERUBAHAN;
                            pergerakanGaji.HR_KENAIKAN = tunggakan;
                            //edit.HR_JUMLAH_PERUBAHAN = tunggakan;
                        }
                    }

                    pergerakanGaji.HR_NO_PEKERJA = item2.HR_NO_PEKERJA;
                    pergerakanGaji.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;
                    pergerakanGaji.HR_NO_KPBARU = peribadi.HR_NO_KPBARU;
                    pergerakanGaji.HR_KOD_PERUBAHAN = item2.HR_KOD_PERUBAHAN;
                    pergerakanGaji.HR_TARIKH_MULA = item2.HR_TARIKH_MULA;
                    pergerakanGaji.HR_KEW8_ID = item2.HR_KEW8_ID;
                    pergerakanGaji.HR_ANSURAN_ID = item2.HR_ANSURAN_ID;

                    if(item2.HR_GRED_LAMA != null)
                    {
                        pergerakanGaji.HR_GRED_LAMA = cariGred(Convert.ToInt32(item2.HR_GRED_LAMA), null).SHORT_DESCRIPTION;
                    }
                    
                    pergerakanGaji.HR_GAJI_LAMA = item2.HR_GAJI_LAMA;
                    pergerakanGaji.HR_MATRIKS_GAJI_LAMA = item2.HR_MATRIKS_GAJI_LAMA;

                    pergerakanGaji.HR_BUTIR_PERUBAHAN = item2.HR_BUTIR_PERUBAHAN;
                    pergerakanGaji.HR_NO_SURAT_KEBENARAN = item2.HR_NO_SURAT_KEBENARAN;
                    pergerakanGaji.HR_NP_FINALISED_HR = item2.HR_NP_FINALISED_HR;
                    pergerakanGaji.HR_FINALISED_IND_HR = item2.HR_FINALISED_IND_HR;


                    HR_MAKLUMAT_PERIBADI pgw = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NP_FINALISED_HR);
                    if (pgw == null)
                    {
                        pgw = new HR_MAKLUMAT_PERIBADI();
                    }
                    ViewBag.HR_NAMA_PEGAWAI2 = pgw.HR_NAMA_PEKERJA;
                    model.Add(pergerakanGaji);
                }
            }
            return model;
        }

        public List<PergerakanGajiModels> ViewTambahKew8(List<HR_MAKLUMAT_PERIBADI> sPeribadi, int? bulan, string tindakan, string tarafpekerja)
        {
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            List<PergerakanGajiModels> editKew8 = ViewEditKew8(bulan, tindakan, tarafpekerja);

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja))
            {
                if (editKew8.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001").Count() <= 0)
                {
                    int? gred = null;
                    if (item.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                    }

                    GE_PARAMTABLE sGred = cariGred(gred, null);

                    int? peringkat = CariPeringkat(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI);
                    string pkt = "P" + peringkat;
                    decimal? kenaikan = 0;
                    decimal? kenaikan2 = 0;
                    decimal? gajiPokokBaru = 0;
                    decimal? gajiPokokBaru2 = 0;
                    decimal? gaji_maxsimum = 0;

                    //decimal? tunggakan = 0;
                    decimal? jumPerubahan = 0;

                    HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    if (jadualGaji != null)
                    {
                        kenaikan = jadualGaji.HR_RM_KENAIKAN;
                        gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    }

                    gajiPokokBaru = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                    if (gajiPokokBaru > gaji_maxsimum)
                    {
                        gajiPokokBaru2 = gaji_maxsimum;
                    }
                    else
                    {
                        gajiPokokBaru2 = gajiPokokBaru;
                    }

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }
                    var jumBulan = (DateTime.Now.Month - Convert.ToDateTime(item.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month);
                    kenaikan2 = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    jumPerubahan = jumBulan * kenaikan2;


                    var jawatan_ind = JawatanInd(item.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, item.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);

                    PergerakanGajiModels elaun = mElaunPotongan(item.HR_NO_PEKERJA, jawatan_ind);

                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();

                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
                    pergerakanGaji.HR_NO_KPBARU = item.HR_NO_KPBARU;
                    pergerakanGaji.HR_GAJI_POKOK = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerakanGaji.HR_JENIS_PERGERAKAN = "D";

                    pergerakanGaji.HR_GRED_LAMA = sGred.SHORT_DESCRIPTION;
                    pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;

                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;

                    pergerakanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    pergerakanGaji.HR_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = jumPerubahan;
                    pergerakanGaji.HR_KENAIKAN = kenaikan2;

                    pergerakanGaji.HR_MATRIKS_GAJI = "P" + matriks.HR_PERINGKAT + "T" + Convert.ToDouble(matriks.HR_TAHAP);
                    pergerakanGaji.HR_MATRIKS_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    pergerakanGaji.HR_KRITIKAL = elaun.HR_KRITIKAL;
                    pergerakanGaji.HR_KOD_KRITIKAL = elaun.HR_KOD_KRITIKAL;
                    pergerakanGaji.HR_PERUBAHAN_KRITIKAL = 0;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = 0;

                    pergerakanGaji.HR_WILAYAH = elaun.HR_WILAYAH;
                    pergerakanGaji.HR_KOD_WILAYAH = elaun.HR_KOD_WILAYAH;
                    pergerakanGaji.HR_PERUBAHAN_WILAYAH = 0;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = 0;

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        Decimal? HR_PERUBAHAN_WILAYAH = pergerakanGaji.HR_KENAIKAN * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        pergerakanGaji.HR_PERUBAHAN_WILAYAH = Math.Abs(Convert.ToDecimal(HR_PERUBAHAN_WILAYAH));
                    }

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        Decimal? HR_PERGERAKAN_EWIL_BARU = pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        Decimal? HR_PERGERAKAN_EWIL_LAMA = pergerakanGaji.HR_GAJI_LAMA * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        Decimal? HR_PERGERAKAN_EWIL = (HR_PERGERAKAN_EWIL_LAMA - HR_PERGERAKAN_EWIL_BARU) * jumBulan;
                        pergerakanGaji.HR_PERGERAKAN_EWIL = Math.Abs(Convert.ToDecimal(HR_PERGERAKAN_EWIL));
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        Decimal? HR_PERUBAHAN_KRITIKAL = pergerakanGaji.HR_KENAIKAN * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        pergerakanGaji.HR_PERUBAHAN_KRITIKAL = Math.Abs(Convert.ToDecimal(HR_PERUBAHAN_KRITIKAL));
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        Decimal? HR_PERGERAKAN_EKAL_BARU = pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        Decimal? HR_PERGERAKAN_EKAL_LAMA = pergerakanGaji.HR_GAJI_LAMA * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        Decimal? HR_PERGERAKAN_EKAL = (HR_PERGERAKAN_EKAL_BARU - HR_PERGERAKAN_EKAL_LAMA) * jumBulan;
                        pergerakanGaji.HR_PERGERAKAN_EKAL = Convert.ToDecimal(HR_PERGERAKAN_EKAL);
                    }
                    model.Add(pergerakanGaji);
                }
            }

            return model;
        }

        public ActionResult PergerakanGaji(string key, string value, int? bulan, string tarafpekerja, string tindakan, ManageMessageId? message)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();

            if(tarafpekerja == null)
            {
                tarafpekerja = "Y";
            }

            ViewBag.tarafpekerja = tarafpekerja;

            if(tindakan == null)
            {
                tindakan = "T";
            }

            ViewBag.ViewTindakan = tindakan;
            //bulan = DateTime.Now.Month;

            //dapatkan senarai pekerja yg layak dapat PGT
            sPeribadi = CariPekerja(key, value, bulan, "00001");


            // daparkan id user yg login
            HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T" && s.HR_AKTIF_IND != "P").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            if (peribadi2 == null)
            {
                peribadi2 = new HR_MAKLUMAT_PERIBADI();
            }

            ViewBag.HR_NAMA_PEGAWAI = peribadi2.HR_NAMA_PEKERJA;
            ViewBag.HR_NP_FINALISED_HR = peribadi2.HR_NO_PEKERJA;

            if(tindakan == "T")
            {
                model = ViewTambahKew8(sPeribadi, bulan, tindakan, tarafpekerja);
            }
            else
            {
                model = ViewEditKew8(bulan, tindakan, tarafpekerja);
            }

            ViewBag.countKew8 = model.Count();
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
            ViewBag.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            ViewBag.TARIKH_MULA = null;
            ViewBag.ID = null;

            if(model.Count() > 0)
            {
                ViewBag.TARIKH_MULA = model.ElementAt(0).HR_TARIKH_MULA;
                ViewBag.ID = model.ElementAt(0).HR_KEW8_ID;
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "-- Pilih --", Value = null });
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_KEWANGAN8> bil = BilPekerja2(key, value, bulan, tarafpekerja);

            List<SelectListItem> Tindakan = new List<SelectListItem>();
            Tindakan.Add(new SelectListItem { Value = "T", Text = "Tambah ( " + bil.FirstOrDefault(s => s.HR_KEW8_IND == "T").HR_BIL + " )" });
            Tindakan.Add(new SelectListItem { Value = "K", Text = "Kemaskini ( " + bil.FirstOrDefault(s => s.HR_KEW8_IND == "K").HR_BIL + " )" });
            Tindakan.Add(new SelectListItem { Value = "M", Text = "Muktamad ( " + bil.FirstOrDefault(s => s.HR_KEW8_IND == "M").HR_BIL + " )" });
            ViewBag.Tindakan = Tindakan;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PergerakanGaji(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, string btnSub, int? bulan2, string tarafpekerja2, string tindakan2)
        {
            if (ModelState.IsValid)
            {
                var mgsErr = "";
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T" && s.HR_AKTIF_IND != "P").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var incrementID = lastID.HR_KEW8_ID + 1;

                //var lastAnsuranID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_ANSURAN_ID).FirstOrDefault();
                //var ansuranID = incrementID;

                foreach (var kew8 in pergerakanGaji)
                {
                    List<HR_MAKLUMAT_KEWANGAN8> SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA).SingleOrDefault();

                    HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA).Count() > 0);
                    if (gajiUpah == null)
                    {
                        gajiUpah = new HR_GAJI_UPAHAN();
                    }

                    HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                    if (potongan2 == null)
                    {
                        potongan2 = new HR_POTONGAN();
                    }
                    ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

                    var jawatan_ind = "";
                    string kodTGGAJ = null;
                    if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T")
                        {
                            kodTGGAJ = "TGGKT";
                        }
                        else
                        {
                            kodTGGAJ = "TGGKK";
                        }
                        jawatan_ind = "K" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                    }
                    else if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T")
                        {
                            kodTGGAJ = "TGGPT";
                        }
                        else
                        {
                            kodTGGAJ = "TGGPK";
                        }
                        jawatan_ind = "P" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                    }

                    HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_KOD_UPAH == kodTGGAJ && s.HR_SINGKATAN == "TGGAJ");
                    if (gajiUpah == null)
                    {
                        tggkk = new HR_GAJI_UPAHAN();
                    }
                    ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    //var peringkat = "";
                    //var tahap = "";

                    GE_PARAMTABLE sGred = cariGred(null, kew8.HR_GRED);

                    //if (kew8.HR_JENIS_PERGERAKAN == "D" || kew8.HR_JENIS_PERGERAKAN == "S")
                    //{
                    //    if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                    //    {
                    //        peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                    //        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                    //        {

                    //            var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    //            tahap = t;
                    //        }
                    //    }
                    //}
                    //else if (kew8.HR_JENIS_PERGERAKAN == "J")
                    //{
                    //    //HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == kew8.HR_GRED && s.HR_GAJI_POKOK == kew8.HR_GAJI_BARU).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).FirstOrDefault();
                    //    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == kew8.HR_GAJI_MIN && s.HR_GAJI_MAX == kew8.HR_GAJI_MAX && s.HR_GRED_GAJI == kew8.HR_GRED && s.HR_GAJI_POKOK == kew8.HR_GAJI_BARU).SingleOrDefault();
                    //    if (matriks == null)
                    //    {
                    //        matriks = new HR_MATRIKS_GAJI();
                    //    }
                    //    peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                    //    tahap = Convert.ToString(matriks.HR_TAHAP);
                    //}

                    //var pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gaji_maxsimum = 0;

                    //if(kew8.HR_GRED != null)
                    //{

                    //}

                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    //var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred);

                    HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID);
                    //HR_MAKLUMAT_KEWANGAN8 semakModel = model;
                    if (btnSub == "Tambah" || btnSub == "Edit")
                    {
                        if (btnSub == "Tambah")
                        {
                            model = new HR_MAKLUMAT_KEWANGAN8();
                            model.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                            model.HR_KOD_PERUBAHAN = pergerakanGajiDetail.HR_KOD_PERUBAHAN;
                            model.HR_TARIKH_MULA = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                            model.HR_KEW8_ID = incrementID;
                            model.HR_ANSURAN_ID = incrementID;
                            model.HR_TARIKH_KEYIN = DateTime.Now;
                            model.HR_TARIKH_AKHIR = null;
                            model.HR_BULAN = DateTime.Now.Month;
                            model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                            incrementID++;
                        }

                        model.HR_BUTIR_PERUBAHAN = pergerakanGajiDetail.HR_BUTIR_PERUBAHAN;
                        model.HR_CATATAN = pergerakanGajiDetail.HR_CATATAN;
                        model.HR_NO_SURAT_KEBENARAN = pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN;
                        model.HR_AKTIF_IND = kew8.HR_AKTIF_IND;
                        model.HR_NP_UBAH_HR = peribadi2.HR_NO_PEKERJA;
                        model.HR_TARIKH_UBAH_HR = DateTime.Now;
                        model.HR_NP_FINALISED_HR = pergerakanGajiDetail.HR_NP_FINALISED_HR;
                        model.HR_TARIKH_FINALISED_HR = DateTime.Now;
                        model.HR_FINALISED_IND_HR = pergerakanGajiDetail.HR_FINALISED_IND_HR;
                        model.HR_NP_UBAH_PA = pergerakanGajiDetail.HR_NP_UBAH_PA;
                        model.HR_TARIKH_UBAH_PA = pergerakanGajiDetail.HR_TARIKH_UBAH_PA;
                        model.HR_NP_FINALISED_PA = pergerakanGajiDetail.HR_NP_FINALISED_PA;
                        model.HR_TARIKH_FINALISED_PA = pergerakanGajiDetail.HR_TARIKH_FINALISED_PA;
                        model.HR_FINALISED_IND_PA = pergerakanGajiDetail.HR_FINALISED_IND_PA;
                        model.HR_EKA = 0;
                        model.HR_ITP = 0;
                        model.HR_KEW8_IND = pergerakanGajiDetail.HR_KEW8_IND;
                        model.HR_BIL = pergerakanGajiDetail.HR_BIL;
                        model.HR_KOD_JAWATAN = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

                        model.HR_LANTIKAN_IND = pergerakanGajiDetail.HR_LANTIKAN_IND;
                        model.HR_TARIKH_SP = pergerakanGajiDetail.HR_TARIKH_SP;
                        model.HR_SP_IND = pergerakanGajiDetail.HR_SP_IND;
                        model.HR_JUMLAH_BULAN = pergerakanGajiDetail.HR_JUMLAH_BULAN;
                        model.HR_NILAI_EPF = pergerakanGajiDetail.HR_NILAI_EPF;
                        model.HR_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        model.HR_MATRIKS_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        model.HR_GRED_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED;

                        model.HR_UBAH_IND = "1";
                        SMK8.Add(model);
                        if (btnSub == "Tambah")
                        {
                            db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                        }
                        else
                        {
                            db.Entry(model).State = EntityState.Modified;
                        }
                    }
                    if (btnSub == "Padam")
                    {
                        db.HR_MAKLUMAT_KEWANGAN8.Remove(model);
                    }
                    db.SaveChanges();

                    if (kew8.HR_KRITIKAL == null)
                    {
                        kew8.HR_KRITIKAL = 0;
                    }
                    if (kew8.HR_WILAYAH == null)
                    {
                        kew8.HR_WILAYAH = 0;
                    }

                    //decimal? tunggakan = (bulan2 - Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month) * kenaikan;

                    List<PergerakanGajiModels> sElaun = new List<PergerakanGajiModels>
                    {
                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = 0,
                            HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_JUMLAH_PERUBAHAN,
                            //HR_JUMLAH_PERUBAHAN = tunggakan,
                            HR_PERGERAKAN_EKAL = kew8.HR_PERGERAKAN_EKAL,
                            HR_PERGERAKAN_EWIL = kew8.HR_PERGERAKAN_EWIL
                        },

                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = Convert.ToDecimal(kew8.HR_KRITIKAL),
                            HR_KOD_PELARASAN = kew8.HR_KOD_KRITIKAL,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_PERUBAHAN_KRITIKAL,
                            HR_PERGERAKAN_EKAL = kew8.HR_PERGERAKAN_EKAL,
                            HR_PERGERAKAN_EWIL = 0
                        },

                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = Convert.ToDecimal(kew8.HR_WILAYAH),
                            HR_KOD_PELARASAN = kew8.HR_KOD_WILAYAH,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_PERUBAHAN_WILAYAH,
                            HR_PERGERAKAN_EKAL = 0,
                            HR_PERGERAKAN_EWIL = kew8.HR_PERGERAKAN_EWIL
                        }
                    };

                    if (btnSub == "Tambah" || btnSub == "Edit")
                    {
                        HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        List<HR_POTONGAN> potongan = new List<HR_POTONGAN>();
                        if (sElaun.Count() > 0)
                        {
                            foreach (PergerakanGajiModels elaun in sElaun)
                            {
                                if (elaun.HR_KOD_PELARASAN != null)
                                {
                                    modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID && s.HR_KOD_PELARASAN == elaun.HR_KOD_PELARASAN);
                                    //HR_MAKLUMAT_KEWANGAN8_DETAIL semakModelDetail = modelDetail;

                                    if (btnSub == "Tambah")
                                    {
                                        modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                        modelDetail.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                                        modelDetail.HR_KOD_PERUBAHAN = pergerakanGajiDetail.HR_KOD_PERUBAHAN;
                                        modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                                        modelDetail.HR_KOD_PELARASAN = elaun.HR_KOD_PELARASAN;
                                        modelDetail.HR_KEW8_ID = model.HR_KEW8_ID;
                                    }

                                    modelDetail.HR_MATRIKS_GAJI = kew8.HR_MATRIKS_GAJI;
                                    modelDetail.HR_GRED = Convert.ToString(sGred.ORDINAL);
                                    modelDetail.HR_JUMLAH_PERUBAHAN = elaun.HR_JUMLAH_PERUBAHAN;
                                    modelDetail.HR_GAJI_BARU = kew8.HR_GAJI_BARU;
                                    modelDetail.HR_JENIS_PERGERAKAN = kew8.HR_JENIS_PERGERAKAN;
                                    modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = elaun.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    modelDetail.HR_STATUS_IND = kew8.HR_KEW8_IND;
                                    modelDetail.HR_ELAUN_KRITIKAL_BARU = kew8.HR_ELAUN_KRITIKAL_BARU;

                                    modelDetail.HR_NO_PEKERJA_PT = kew8.HR_NO_PEKERJA_PT;
                                    modelDetail.HR_PERGERAKAN_EKAL = elaun.HR_PERGERAKAN_EKAL;
                                    modelDetail.HR_PERGERAKAN_EWIL = elaun.HR_PERGERAKAN_EWIL;
                                    modelDetail.HR_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                    SMK8D.Add(modelDetail);
                                    //db.Entry(modelDetail).State = EntityState.Modified;
                                    if (btnSub == "Tambah")
                                    {
                                        //HR_MAKLUMAT_KEWANGAN8_DETAIL padamDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == modelDetail.HR_NO_PEKERJA && s.HR_TARIKH_MULA == modelDetail.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == modelDetail.HR_KOD_PERUBAHAN && s.HR_KOD_PELARASAN == modelDetail.HR_KOD_PELARASAN);
                                        //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(padamDetail);
                                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                                    }
                                    else
                                    {
                                        db.Entry(modelDetail).State = EntityState.Modified;
                                    }

                                    //HR_MAKLUMAT_ELAUN_POTONGAN editElaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == modelDetail.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == elaun.HR_KOD_PELARASAN && s.HR_ELAUN_POTONGAN_IND == "E");
                                    //if(editElaunPotongan != null)
                                    //{
                                    //    editElaunPotongan.HR_JUMLAH = elaun.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    //    db.Entry(editElaunPotongan).State = EntityState.Modified;
                                    //}

                                    db.SaveChanges();

                                    //if (pergerakanGajiDetail.HR_FINALISED_IND_HR == "Y")
                                    //{

                                    //    if (modelDetail.HR_JUMLAH_PERUBAHAN != 0)
                                    //    {
                                    //        HR_POTONGAN pelarasan = new HR_POTONGAN();
                                    //        pelarasan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                    //        pelarasan.HR_NILAI = modelDetail.HR_JUMLAH_PERUBAHAN;
                                    //        potongan.Add(pelarasan);
                                    //    }


                                    //}
                                }

                            }
                        }
                        if (pergerakanGajiDetail.HR_FINALISED_IND_HR == "Y")
                        {
                            //Muktamad(model, modelDetail, null, "00001", potongan, gajiUpah, potongan2, tggkk, mPeribadi, null, model.HR_TARIKH_MULA, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                            Muktamad2(SMK8, SMK8D, "00001", gajiUpah, tggkk, potongan2);
                        }
                    }

                    if (btnSub == "Padam")
                    {
                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID).ToList();
                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(modelDetail2);
                        db.SaveChanges();
                        mgsErr = "Data berjaya dipadam";
                    }

                    //incrementID++;
                }

                if (tindakan2 == "K" && btnSub != "Padam")
                {
                    mgsErr = "Data berjaya dikemaskini";
                }

                if (tindakan2 == "T")
                {
                    tindakan2 = "K";
                    mgsErr = "Data berjaya dimasukkan";
                }

                if(pergerakanGajiDetail.HR_FINALISED_IND_HR == "Y")
                {
                    tindakan2 = "M";

                    if (btnSub != "Padam")
                        mgsErr = mgsErr + " dan dimuktamadkan";
                }

                return Json(new { error = false, msg = mgsErr + ".",  key = key2, value = value2, bulan = bulan2, tarafpekerja = tarafpekerja2, tindakan = tindakan2 }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
            ViewBag.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            return Json(new { error = true, msg = "Maaf ralat!!. Sila hubungi pihak ICT untuk menyelesaikan masalah ini. Terima Kasih.", key = key2, value = value2, bulan = bulan2, tarafpekerja = tarafpekerja2, tindakan = tindakan2 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PergerakanGaji_bak(string key, string value, int? bulan, ManageMessageId? message)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();

            //bulan = DateTime.Now.Month;

            //dapatkan senarai pekerja yg layak dapat PGT
            sPeribadi = CariPekerja(key, value, bulan, "00001");


            // daparkan id user yg login
            HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T" && s.HR_AKTIF_IND != "P").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            if (peribadi2 == null)
            {
                peribadi2 = new HR_MAKLUMAT_PERIBADI();
            }

            ViewBag.HR_NAMA_PEGAWAI = peribadi2.HR_NAMA_PEKERJA;
            ViewBag.HR_NP_FINALISED_HR = peribadi2.HR_NO_PEKERJA;

            //Edit
            int belumMuktamad = 0;
            List<HR_MAKLUMAT_KEWANGAN8> Kew8 = db.HR_MAKLUMAT_KEWANGAN8.AsEnumerable().Where(s => s.HR_KOD_PERUBAHAN == "00001" && ((Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && s.HR_FINALISED_IND_HR != "Y" && s.HR_UBAH_IND == "1") || ((Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && s.HR_TAHUN == DateTime.Now.Year) && s.HR_FINALISED_IND_HR == "Y" && s.HR_UBAH_IND == "0"))).OrderByDescending(s => s.HR_KEW8_ID).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();
            List<PergerakanGajiModels> editKew8 = new List<PergerakanGajiModels>();
            foreach (HR_MAKLUMAT_KEWANGAN8 item2 in Kew8)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA);
                if (peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }

                var ubahGred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                GE_PARAMTABLE sGred2 = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == ubahGred);
                if (sGred2 == null)
                {
                    sGred2 = new GE_PARAMTABLE();
                }

                if (item2.HR_FINALISED_IND_HR != "Y")
                {
                    belumMuktamad++;
                }

                PergerakanGajiModels edit = new PergerakanGajiModels();
                edit.HR_WILAYAH = 0;
                edit.HR_PERUBAHAN_WILAYAH = 0;
                edit.HR_PERGERAKAN_EWIL = 0;

                edit.HR_KRITIKAL = 0;
                edit.HR_PERUBAHAN_KRITIKAL = 0;
                edit.HR_PERGERAKAN_EKAL = 0;

                edit.HR_JUMLAH_PERUBAHAN = 0;
                edit.HR_GAJI_BARU = 0;

                var jawatan_ind = JawatanInd(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                int? gred = null;
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == item2.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item2.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item2.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item2.HR_KEW8_ID).ToList();
                foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in kew8Detail)
                {
                    if (detail.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(detail.HR_GRED);
                    }

                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }

                    int? peringkat = CariPeringkat(detail.HR_MATRIKS_GAJI);

                    //var pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gaji_maxsimum = 0;
                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == detail.HR_GAJI_BARU).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    edit.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    edit.HR_GAJI_MAX = matriks.HR_GAJI_MAX;

                    edit.HR_JENIS_PERGERAKAN = detail.HR_JENIS_PERGERAKAN;
                    edit.HR_GAJI_BARU = detail.HR_GAJI_BARU;
                    edit.HR_MATRIKS_GAJI = detail.HR_MATRIKS_GAJI;
                    edit.HR_GRED = sGred.SHORT_DESCRIPTION;

                    HR_ELAUN elaun = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                    if (elaun != null)
                    {
                        PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                        edit.HR_WILAYAH = Convert.ToSingle(elaunPotongan.HR_WILAYAH);
                        edit.HR_KOD_WILAYAH = detail.HR_KOD_PELARASAN;
                        edit.HR_PERUBAHAN_WILAYAH = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_PERGERAKAN_EWIL = detail.HR_PERGERAKAN_EWIL;
                    }

                    HR_ELAUN awam12 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                    if (awam12 != null)
                    {
                        PergerakanGajiModels elaunPotongan = mElaunPotongan(detail.HR_NO_PEKERJA, jawatan_ind);
                        edit.HR_KRITIKAL = Convert.ToSingle(elaunPotongan.HR_KRITIKAL);
                        edit.HR_KOD_KRITIKAL = detail.HR_KOD_PELARASAN;
                        edit.HR_PERUBAHAN_KRITIKAL = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_PERGERAKAN_EKAL = detail.HR_PERGERAKAN_EKAL;
                    }

                    HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN);
                    if (tggkk != null)
                    {
                        var tunggakan = detail.HR_GAJI_BARU - detail.HR_GAJI_LAMA;
                        edit.HR_KOD_PELARASAN = detail.HR_KOD_PELARASAN;
                        edit.HR_JUMLAH_PERUBAHAN = detail.HR_JUMLAH_PERUBAHAN;
                        edit.HR_KENAIKAN = tunggakan;
                        //edit.HR_JUMLAH_PERUBAHAN = tunggakan;
                    }
                }

                edit.HR_NO_PEKERJA = item2.HR_NO_PEKERJA;
                edit.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;
                edit.HR_NO_KPBARU = peribadi.HR_NO_KPBARU;
                edit.HR_KOD_PERUBAHAN = item2.HR_KOD_PERUBAHAN;
                edit.HR_TARIKH_MULA = item2.HR_TARIKH_MULA;
                edit.HR_KEW8_ID = item2.HR_KEW8_ID;

                edit.HR_GRED_LAMA = item2.HR_GRED_LAMA;
                edit.HR_GAJI_LAMA = item2.HR_GAJI_LAMA;
                edit.HR_MATRIKS_GAJI_LAMA = item2.HR_MATRIKS_GAJI_LAMA;

                edit.HR_BUTIR_PERUBAHAN = item2.HR_BUTIR_PERUBAHAN;
                edit.HR_NO_SURAT_KEBENARAN = item2.HR_NO_SURAT_KEBENARAN;
                edit.HR_NP_FINALISED_HR = item2.HR_NP_FINALISED_HR;
                edit.HR_FINALISED_IND_HR = item2.HR_FINALISED_IND_HR;


                HR_MAKLUMAT_PERIBADI pgw = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == item2.HR_NP_FINALISED_HR);
                if (pgw == null)
                {
                    pgw = new HR_MAKLUMAT_PERIBADI();
                }
                ViewBag.HR_NAMA_PEGAWAI2 = pgw.HR_NAMA_PEKERJA;
                editKew8.Add(edit);
            }
            ViewBag.EditKew8 = editKew8;
            ViewBag.JumEditKew8 = editKew8.GroupBy(s => new { s.HR_KOD_PERUBAHAN, s.HR_KEW8_ID, s.HR_TARIKH_MULA }).Select(s => s.FirstOrDefault()).ToList();
            ViewBag.countEditKew8 = editKew8.Count();
            ViewBag.belumMuktamad = belumMuktamad;

            //tambah
            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                if (editKew8.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001").Count() <= 0)
                {
                    int? gred = null;
                    if (item.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                    }

                    GE_PARAMTABLE sGred = cariGred(gred, null);

                    int? peringkat = CariPeringkat(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI);
                    string pkt = "P" + peringkat;
                    decimal? kenaikan = 0;
                    decimal? kenaikan2 = 0;
                    decimal? gajiPokokBaru = 0;
                    decimal? gajiPokokBaru2 = 0;
                    decimal? gaji_maxsimum = 0;

                    //decimal? tunggakan = 0;
                    decimal? jumPerubahan = 0;

                    HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    if (jadualGaji != null)
                    {
                        kenaikan = jadualGaji.HR_RM_KENAIKAN;
                        gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    }

                    gajiPokokBaru = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                    if (gajiPokokBaru > gaji_maxsimum)
                    {
                        gajiPokokBaru2 = gaji_maxsimum;
                    }
                    else
                    {
                        gajiPokokBaru2 = gajiPokokBaru;
                    }

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }
                    var jumBulan = (DateTime.Now.Month - Convert.ToDateTime(item.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month);
                    kenaikan2 = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    jumPerubahan = jumBulan * kenaikan2;
                    

                    var jawatan_ind = JawatanInd(item.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND, item.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);

                    PergerakanGajiModels elaun = mElaunPotongan(item.HR_NO_PEKERJA, jawatan_ind);

                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();

                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
                    pergerakanGaji.HR_NO_KPBARU = item.HR_NO_KPBARU;
                    pergerakanGaji.HR_GAJI_POKOK = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerakanGaji.HR_JENIS_PERGERAKAN = "D";

                    pergerakanGaji.HR_GRED_LAMA = sGred.SHORT_DESCRIPTION;
                    pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;

                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;

                    pergerakanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    pergerakanGaji.HR_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = jumPerubahan;
                    pergerakanGaji.HR_KENAIKAN = kenaikan2;

                    pergerakanGaji.HR_MATRIKS_GAJI = "P" + matriks.HR_PERINGKAT + "T" + Convert.ToDouble(matriks.HR_TAHAP);
                    pergerakanGaji.HR_MATRIKS_GAJI_LAMA = item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    pergerakanGaji.HR_KRITIKAL = elaun.HR_KRITIKAL;
                    pergerakanGaji.HR_KOD_KRITIKAL = elaun.HR_KOD_KRITIKAL;
                    pergerakanGaji.HR_PERUBAHAN_KRITIKAL = 0;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = 0;

                    pergerakanGaji.HR_WILAYAH = elaun.HR_WILAYAH;
                    pergerakanGaji.HR_KOD_WILAYAH = elaun.HR_KOD_WILAYAH;
                    pergerakanGaji.HR_PERUBAHAN_WILAYAH = 0;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = 0;

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        Decimal? HR_PERUBAHAN_WILAYAH = pergerakanGaji.HR_KENAIKAN * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        pergerakanGaji.HR_PERUBAHAN_WILAYAH = Math.Abs(Convert.ToDecimal(HR_PERUBAHAN_WILAYAH));
                    }

                    if (pergerakanGaji.HR_WILAYAH > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        Decimal? HR_PERGERAKAN_EWIL_BARU = pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        Decimal? HR_PERGERAKAN_EWIL_LAMA = pergerakanGaji.HR_GAJI_LAMA * (Convert.ToDecimal(pergerakanGaji.HR_WILAYAH) / 100);
                        Decimal? HR_PERGERAKAN_EWIL = (HR_PERGERAKAN_EWIL_LAMA - HR_PERGERAKAN_EWIL_BARU) * jumBulan;
                        pergerakanGaji.HR_PERGERAKAN_EWIL = Math.Abs(Convert.ToDecimal(HR_PERGERAKAN_EWIL));
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_JUMLAH_PERUBAHAN > 0)
                    {
                        Decimal? HR_PERUBAHAN_KRITIKAL = pergerakanGaji.HR_KENAIKAN * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        pergerakanGaji.HR_PERUBAHAN_KRITIKAL = Math.Abs(Convert.ToDecimal(HR_PERUBAHAN_KRITIKAL));
                    }

                    if (pergerakanGaji.HR_KRITIKAL > 0 && pergerakanGaji.HR_GAJI_BARU > 0)
                    {
                        Decimal? HR_PERGERAKAN_EKAL_BARU = pergerakanGaji.HR_GAJI_BARU * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        Decimal? HR_PERGERAKAN_EKAL_LAMA = pergerakanGaji.HR_GAJI_LAMA * (Convert.ToDecimal(pergerakanGaji.HR_KRITIKAL) / 100);
                        Decimal? HR_PERGERAKAN_EKAL = (HR_PERGERAKAN_EKAL_BARU - HR_PERGERAKAN_EKAL_LAMA) * jumBulan;
                        pergerakanGaji.HR_PERGERAKAN_EKAL = Convert.ToDecimal(HR_PERGERAKAN_EKAL);
                    }
                    model.Add(pergerakanGaji);
                }
            }
            //ViewBag.countKew8 = model.Where(s => s.CHECKED == 1).Count();
            ViewBag.countKew8 = model.Count();
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
            ViewBag.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PergerakanGaji_bak(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, string btnSub, int? bulan2)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T" && s.HR_AKTIF_IND != "P").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var incrementID = lastID.HR_KEW8_ID + 1;

                List<HR_MAKLUMAT_KEWANGAN8> SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

                foreach (var kew8 in pergerakanGaji)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA).SingleOrDefault();

                    HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA).Count() > 0);
                    if (gajiUpah == null)
                    {
                        gajiUpah = new HR_GAJI_UPAHAN();
                    }

                    HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                    if (potongan2 == null)
                    {
                        potongan2 = new HR_POTONGAN();
                    }
                    ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

                    var jawatan_ind = "";
                    string kodTGGAJ = null;
                    if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                    {
                        if(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T")
                        {
                            kodTGGAJ = "TGGKT";
                        }
                        else
                        {
                            kodTGGAJ = "TGGKK";
                        }
                        jawatan_ind = "K" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                    }
                    else if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                    {
                        if(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN == "T")
                        {
                            kodTGGAJ = "TGGPT";
                        }
                        else
                        {
                            kodTGGAJ = "TGGPK";
                        }
                        jawatan_ind = "P" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                    }

                    HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_KOD_UPAH == kodTGGAJ && s.HR_SINGKATAN == "TGGAJ");
                    if (gajiUpah == null)
                    {
                        tggkk = new HR_GAJI_UPAHAN();
                    }
                    ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    //var peringkat = "";
                    //var tahap = "";

                    GE_PARAMTABLE sGred = cariGred(null, kew8.HR_GRED);

                    //if (kew8.HR_JENIS_PERGERAKAN == "D" || kew8.HR_JENIS_PERGERAKAN == "S")
                    //{
                    //    if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                    //    {
                    //        peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                    //        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                    //        {

                    //            var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    //            tahap = t;
                    //        }
                    //    }
                    //}
                    //else if (kew8.HR_JENIS_PERGERAKAN == "J")
                    //{
                    //    //HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == kew8.HR_GRED && s.HR_GAJI_POKOK == kew8.HR_GAJI_BARU).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).FirstOrDefault();
                    //    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == kew8.HR_GAJI_MIN && s.HR_GAJI_MAX == kew8.HR_GAJI_MAX && s.HR_GRED_GAJI == kew8.HR_GRED && s.HR_GAJI_POKOK == kew8.HR_GAJI_BARU).SingleOrDefault();
                    //    if (matriks == null)
                    //    {
                    //        matriks = new HR_MATRIKS_GAJI();
                    //    }
                    //    peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                    //    tahap = Convert.ToString(matriks.HR_TAHAP);
                    //}

                    //var pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gaji_maxsimum = 0;

                    //if(kew8.HR_GRED != null)
                    //{

                    //}

                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    //var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred);

                    HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID);
                    //HR_MAKLUMAT_KEWANGAN8 semakModel = model;
                    if (btnSub == "Tambah" || btnSub == "Edit")
                    {
                        if (btnSub == "Tambah")
                        {
                            model = new HR_MAKLUMAT_KEWANGAN8();
                            model.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                            model.HR_KOD_PERUBAHAN = pergerakanGajiDetail.HR_KOD_PERUBAHAN;
                            model.HR_TARIKH_MULA = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                            model.HR_KEW8_ID = incrementID;
                            model.HR_TARIKH_KEYIN = DateTime.Now;
                            model.HR_TARIKH_AKHIR = null;
                            model.HR_BULAN = DateTime.Now.Month;
                            model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                            incrementID++;
                        }

                        model.HR_BUTIR_PERUBAHAN = pergerakanGajiDetail.HR_BUTIR_PERUBAHAN;
                        model.HR_CATATAN = pergerakanGajiDetail.HR_CATATAN;
                        model.HR_NO_SURAT_KEBENARAN = pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN;
                        model.HR_AKTIF_IND = kew8.HR_AKTIF_IND;
                        model.HR_NP_UBAH_HR = peribadi2.HR_NO_PEKERJA;
                        model.HR_TARIKH_UBAH_HR = DateTime.Now;
                        model.HR_NP_FINALISED_HR = pergerakanGajiDetail.HR_NP_FINALISED_HR;
                        model.HR_TARIKH_FINALISED_HR = DateTime.Now;
                        model.HR_FINALISED_IND_HR = pergerakanGajiDetail.HR_FINALISED_IND_HR;
                        model.HR_NP_UBAH_PA = pergerakanGajiDetail.HR_NP_UBAH_PA;
                        model.HR_TARIKH_UBAH_PA = pergerakanGajiDetail.HR_TARIKH_UBAH_PA;
                        model.HR_NP_FINALISED_PA = pergerakanGajiDetail.HR_NP_FINALISED_PA;
                        model.HR_TARIKH_FINALISED_PA = pergerakanGajiDetail.HR_TARIKH_FINALISED_PA;
                        model.HR_FINALISED_IND_PA = pergerakanGajiDetail.HR_FINALISED_IND_PA;
                        model.HR_EKA = 0;
                        model.HR_ITP = 0;
                        model.HR_KEW8_IND = pergerakanGajiDetail.HR_KEW8_IND;
                        model.HR_BIL = pergerakanGajiDetail.HR_BIL;
                        model.HR_KOD_JAWATAN = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

                        model.HR_LANTIKAN_IND = pergerakanGajiDetail.HR_LANTIKAN_IND;
                        model.HR_TARIKH_SP = pergerakanGajiDetail.HR_TARIKH_SP;
                        model.HR_SP_IND = pergerakanGajiDetail.HR_SP_IND;
                        model.HR_JUMLAH_BULAN = pergerakanGajiDetail.HR_JUMLAH_BULAN;
                        model.HR_NILAI_EPF = pergerakanGajiDetail.HR_NILAI_EPF;
                        model.HR_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        model.HR_MATRIKS_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        model.HR_GRED_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED;

                        model.HR_UBAH_IND = "1";

                        SMK8.Add(model);
                        if (btnSub == "Tambah")
                        {
                            db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                        }
                        else
                        {
                            db.Entry(model).State = EntityState.Modified;
                        }
                    }
                    if (btnSub == "Padam")
                    {
                        db.HR_MAKLUMAT_KEWANGAN8.Remove(model);
                    }
                    db.SaveChanges();

                    if(kew8.HR_KRITIKAL == null)
                    {
                        kew8.HR_KRITIKAL = 0;
                    }
                    if (kew8.HR_WILAYAH == null)
                    {
                        kew8.HR_WILAYAH = 0;
                    }

                    //decimal? tunggakan = (bulan2 - Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month) * kenaikan;

                    List<PergerakanGajiModels> sElaun = new List<PergerakanGajiModels>
                    {
                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = 0,
                            HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_JUMLAH_PERUBAHAN,
                            //HR_JUMLAH_PERUBAHAN = tunggakan,
                            HR_PERGERAKAN_EKAL = kew8.HR_PERGERAKAN_EKAL,
                            HR_PERGERAKAN_EWIL = kew8.HR_PERGERAKAN_EWIL
                        },

                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = Convert.ToDecimal(kew8.HR_KRITIKAL),
                            HR_KOD_PELARASAN = kew8.HR_KOD_KRITIKAL,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_PERUBAHAN_KRITIKAL,
                            HR_PERGERAKAN_EKAL = kew8.HR_PERGERAKAN_EKAL,
                            HR_PERGERAKAN_EWIL = 0
                        },

                        new PergerakanGajiModels
                        {
                            HR_JUMLAH_PERUBAHAN_ELAUN = Convert.ToDecimal(kew8.HR_WILAYAH),
                            HR_KOD_PELARASAN = kew8.HR_KOD_WILAYAH,
                            HR_JUMLAH_PERUBAHAN = kew8.HR_PERUBAHAN_WILAYAH,
                            HR_PERGERAKAN_EKAL = 0,
                            HR_PERGERAKAN_EWIL = kew8.HR_PERGERAKAN_EWIL
                        }
                    };

                    if (btnSub == "Tambah" || btnSub == "Edit")
                    {
                        HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        List<HR_POTONGAN> potongan = new List<HR_POTONGAN>();
                        if (sElaun.Count() > 0)
                        {
                            foreach (PergerakanGajiModels elaun in sElaun)
                            {
                                if (elaun.HR_KOD_PELARASAN != null)
                                {
                                    modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID && s.HR_KOD_PELARASAN == elaun.HR_KOD_PELARASAN);
                                    //HR_MAKLUMAT_KEWANGAN8_DETAIL semakModelDetail = modelDetail;
                                    
                                    if (btnSub == "Tambah")
                                    {
                                        modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                        modelDetail.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                                        modelDetail.HR_KOD_PERUBAHAN = pergerakanGajiDetail.HR_KOD_PERUBAHAN;
                                        modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI);
                                        modelDetail.HR_KOD_PELARASAN = elaun.HR_KOD_PELARASAN;
                                        modelDetail.HR_KEW8_ID = model.HR_KEW8_ID;
                                    }

                                    modelDetail.HR_MATRIKS_GAJI = kew8.HR_MATRIKS_GAJI;
                                    modelDetail.HR_GRED = Convert.ToString(sGred.ORDINAL);
                                    modelDetail.HR_JUMLAH_PERUBAHAN = elaun.HR_JUMLAH_PERUBAHAN;
                                    modelDetail.HR_GAJI_BARU = kew8.HR_GAJI_BARU;
                                    modelDetail.HR_JENIS_PERGERAKAN = kew8.HR_JENIS_PERGERAKAN;
                                    modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = elaun.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    modelDetail.HR_STATUS_IND = kew8.HR_KEW8_IND;
                                    modelDetail.HR_ELAUN_KRITIKAL_BARU = kew8.HR_ELAUN_KRITIKAL_BARU;

                                    modelDetail.HR_NO_PEKERJA_PT = kew8.HR_NO_PEKERJA_PT;
                                    modelDetail.HR_PERGERAKAN_EKAL = elaun.HR_PERGERAKAN_EKAL;
                                    modelDetail.HR_PERGERAKAN_EWIL = elaun.HR_PERGERAKAN_EWIL;
                                    modelDetail.HR_GAJI_LAMA = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                    SMK8D.Add(modelDetail);
                                    //db.Entry(modelDetail).State = EntityState.Modified;
                                    if (btnSub == "Tambah")
                                    {
                                        //HR_MAKLUMAT_KEWANGAN8_DETAIL padamDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == modelDetail.HR_NO_PEKERJA && s.HR_TARIKH_MULA == modelDetail.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == modelDetail.HR_KOD_PERUBAHAN && s.HR_KOD_PELARASAN == modelDetail.HR_KOD_PELARASAN);
                                        //db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(padamDetail);
                                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                                    }
                                    else
                                    {
                                        db.Entry(modelDetail).State = EntityState.Modified;
                                    }

                                    //HR_MAKLUMAT_ELAUN_POTONGAN editElaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == modelDetail.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == elaun.HR_KOD_PELARASAN && s.HR_ELAUN_POTONGAN_IND == "E");
                                    //if(editElaunPotongan != null)
                                    //{
                                    //    editElaunPotongan.HR_JUMLAH = elaun.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    //    db.Entry(editElaunPotongan).State = EntityState.Modified;
                                    //}

                                    db.SaveChanges();

                                    //if (pergerakanGajiDetail.HR_FINALISED_IND_HR == "Y")
                                    //{
                                        
                                    //    if (modelDetail.HR_JUMLAH_PERUBAHAN != 0)
                                    //    {
                                    //        HR_POTONGAN pelarasan = new HR_POTONGAN();
                                    //        pelarasan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                    //        pelarasan.HR_NILAI = modelDetail.HR_JUMLAH_PERUBAHAN;
                                    //        potongan.Add(pelarasan);
                                    //    }
                                        
                                        
                                    //}
                                }

                            }
                        }
                        if (pergerakanGajiDetail.HR_FINALISED_IND_HR == "Y")
                        {
                            //Muktamad(model, modelDetail, null, "00001", potongan, gajiUpah, potongan2, tggkk, mPeribadi, null, model.HR_TARIKH_MULA, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                            Muktamad2(SMK8, SMK8D, "00001", gajiUpah, tggkk, potongan2);
                        }     
                    }
                    
                    if (btnSub == "Padam")
                    {
                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == pergerakanGajiDetail.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID).ToList();
                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(modelDetail2);
                        db.SaveChanges();
                    }
                    

                    //incrementID++;
                }
                return RedirectToAction("PergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
            ViewBag.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            return RedirectToAction("PergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
        }

        public ActionResult TestPergerakanGaji(string key, string value, int? bulan, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.Error ? "Data Tidak Berjaya Dimasukkan."
            : message == ManageMessageId.Success ? "Data Berjaya Dimasukkan."
            : "";
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            
            sPeribadi = CariPekerja(key, value, bulan, "00001");

            ViewBag.gambar = new List<HR_GAMBAR_PENGGUNA>();
            if (sPeribadi.Count() > 0)
            {
                ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            }

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                int? gred = null;
                if(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred );
                if(sGred == null)
                {
                    sGred = new GE_PARAMTABLE();
                }
                int? peringkat = null;
                //decimal? tahap = null;
                if (item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                {
                    item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI = item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Trim();
                    if (item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                    {
                        peringkat = Convert.ToInt32(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1));
                    }
                }
                
                //if(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(2,1) == "T" && item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                //{
                //    tahap = Convert.ToDecimal(item.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3));
                //}
                string pkt = "P" + peringkat;
                decimal? kenaikan = 0;
                decimal? gajiPokokBaru = 0;
                decimal? gajiPokokBaru2 = 0;
                decimal? gaji_maxsimum = 0;
                decimal? tunggakan = 0;
                HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                if(jadualGaji != null)
                {
                    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                }

                gajiPokokBaru = item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                if(gajiPokokBaru > gaji_maxsimum)
                {
                    gajiPokokBaru2 = gaji_maxsimum;
                }
                else
                {
                    gajiPokokBaru2 = gajiPokokBaru;
                }

                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                if (matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                    matriks.HR_GAJI_MIN = 0;
                    matriks.HR_GAJI_MAX = 0;
                    matriks.HR_GAJI_POKOK = 0;
                }

                tunggakan = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;
                pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                pergerakanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                pergerakanGaji.HR_JUMLAH_PERUBAHAN = tunggakan;
                pergerakanGaji.CHECKED = 0;
                var countKew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(k => k.HR_NO_PEKERJA == item.HR_NO_PEKERJA && k.HR_FINALISED_IND_HR == "P").Count();
                
                if (countKew8 <= 0)
                {
                    pergerakanGaji.CHECKED = 1;
                }
                model.Add(pergerakanGaji);

            }
            ViewBag.countKew8 = model.Where(s => s.CHECKED == 1).Count();
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestPergerakanGaji(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, int? bulan2)
        {
            if(ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                foreach (var peribadi in pergerakanGaji)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";

                    string gred = null;

                    GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == peribadi.HR_GRED).SingleOrDefault();
                    if(gredList != null)
                    {
                        gred = Convert.ToString(gredList.ORDINAL);
                    }

                    if (peribadi.HR_JENIS_PERGERAKAN == "D" || peribadi.HR_JENIS_PERGERAKAN == "S")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                        {
                            peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                            if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                            {

                                var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                                tahap = t;
                            }
                        }
                    }
                    else if (peribadi.HR_JENIS_PERGERAKAN == "J")
                    {
                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == peribadi.HR_GAJI_MIN && s.HR_GAJI_MAX == peribadi.HR_GAJI_MAX && s.HR_GRED_GAJI == peribadi.HR_GRED && s.HR_GAJI_POKOK == peribadi.HR_GAJI_BARU).SingleOrDefault();
                        if(matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                        }
                        peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                        tahap = Convert.ToString(matriks.HR_TAHAP);
                    }
                    var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred);
                }


               
                return RedirectToAction("TestPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            return RedirectToAction("TestPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
        }

        public ActionResult PengesahanPergerakanGaji(string key, string value, int? bulan, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.Error ? "Data Tidak Berjaya Dimasukkan."
            : message == ManageMessageId.Success ? "Data Berjaya Dimasukkan."
            : "";
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();

            sPeribadi = CariPekerja(key, value, bulan, "00001");

            ViewBag.gambar = new List<HR_GAMBAR_PENGGUNA>();
            if (sPeribadi.Count() > 0)
            {
                ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            }

            foreach (HR_MAKLUMAT_PERIBADI item in sPeribadi)
            {
                PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                HR_MAKLUMAT_KEWANGAN8 kew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (s.HR_FINALISED_IND_HR.Contains("T") || s.HR_FINALISED_IND_HR.Contains("P"))).OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                if (kew8 != null)
                {
                    HR_MAKLUMAT_KEWANGAN8_DETAIL kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kew8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA && s.HR_KEW8_ID == kew8.HR_KEW8_ID);
                    if (kew8Detail == null)
                    {
                        kew8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }

                    int? gred = null;
                    if (kew8Detail.HR_GRED != null)
                    {
                        gred = Convert.ToInt32(kew8Detail.HR_GRED);
                    }
                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }
                    int? peringkat = null;
                    decimal? tahap = null;
                    //decimal? tahap = null;
                    if (kew8Detail.HR_MATRIKS_GAJI != null)
                    {
                        kew8Detail.HR_MATRIKS_GAJI = kew8Detail.HR_MATRIKS_GAJI.Trim();
                        if (kew8Detail.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                        {
                            peringkat = Convert.ToInt32(kew8Detail.HR_MATRIKS_GAJI.Substring(1, 1));
                        }
                        if (kew8Detail.HR_MATRIKS_GAJI.Substring(2, 1) == "T" && kew8Detail.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                        {
                            tahap = Convert.ToDecimal(kew8Detail.HR_MATRIKS_GAJI.Substring(3));
                        }
                    }
                    
                    //string pkt = "P" + peringkat;
                    //decimal? kenaikan = 0;
                    //decimal? gajiPokokBaru = 0;
                    //decimal? gajiPokokBaru2 = 0;
                    //decimal? gaji_maxsimum = 0;
                    decimal? tunggakan = 0;
                    //HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                    //if (jadualGaji != null)
                    //{
                    //    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    //    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                    //}

                    //gajiPokokBaru = kew8Detail.HR_GAJI_BARU;

                    //if (gajiPokokBaru > gaji_maxsimum)
                    //{
                    //    gajiPokokBaru2 = gaji_maxsimum;
                    //}
                    //else
                    //{
                    //    gajiPokokBaru2 = gajiPokokBaru;
                    //}

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    tunggakan = matriks.HR_GAJI_POKOK - item.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_GRED = sGred.SHORT_DESCRIPTION;
                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    pergerakanGaji.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = tunggakan;
                    pergerakanGaji.HR_KEW8_ID = kew8.HR_KEW8_ID;
                    pergerakanGaji.HR_TARIKH_MULA = kew8.HR_TARIKH_MULA;
                    pergerakanGaji.HR_KOD_PELARASAN = kew8Detail.HR_KOD_PELARASAN;
                    model.Add(pergerakanGaji);

                    ViewBag.HR_NO_SURAT_KEBENARAN = kew8.HR_NO_SURAT_KEBENARAN;
                    ViewBag.HR_BUTIR_PERUBAHAN = kew8.HR_BUTIR_PERUBAHAN;

                    HR_MAKLUMAT_PERIBADI peribadiPegawai = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NP_FINALISED_HR);
                    if(peribadiPegawai == null)
                    {
                        peribadiPegawai = new HR_MAKLUMAT_PERIBADI();
                    }

                    ViewBag.HR_NAMA_PEGAWAI = peribadiPegawai.HR_NAMA_PEKERJA;
                    ViewBag.HR_NP_FINALISED_HR = kew8.HR_NP_FINALISED_HR;
                    ViewBag.HR_FINALISED_IND_HR = kew8.HR_FINALISED_IND_HR;
                    ViewBag.HR_CATATAN = kew8.HR_CATATAN;

                    List<SelectListItem> pengesahan = new List<SelectListItem>();
                    pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamat" });
                    pengesahan.Add(new SelectListItem { Value = "T", Text = "Tolak" });
                    pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
                    ViewBag.pengesahan = pengesahan;
                }
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;
            ViewBag.peribadi = sPeribadi;
            ViewBag.sPegawai = sPegawai;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PengesahanPergerakanGaji(IEnumerable<PergerakanGajiModels> pergerakanGaji, PergerakanGajiModels pergerakanGajiDetail, string key2, string value2, int? bulan2)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_AKTIF_IND != "T").OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
                if (peribadi2 == null)
                {
                    peribadi2 = new HR_MAKLUMAT_PERIBADI();
                }

                foreach (var peribadi in pergerakanGaji)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
                    if(mPeribadi == null)
                    {
                        mPeribadi = new HR_MAKLUMAT_PERIBADI();
                    }
                    var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";

                    string gred = null;

                    GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == peribadi.HR_GRED).SingleOrDefault();
                    if (gredList != null)
                    {
                        gred = Convert.ToString(gredList.ORDINAL);
                    }

                    if (peribadi.HR_JENIS_PERGERAKAN == "D" || peribadi.HR_JENIS_PERGERAKAN == "S")
                    {
                        if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                        {
                            peringkat = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1);
                            if (mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                            {

                                var t = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                                tahap = t;
                            }
                        }
                    }

                    else if (peribadi.HR_JENIS_PERGERAKAN == "J")
                    {
                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == peribadi.HR_GAJI_MIN && s.HR_GAJI_MAX == peribadi.HR_GAJI_MAX && s.HR_GRED_GAJI == peribadi.HR_GRED && s.HR_GAJI_POKOK == peribadi.HR_GAJI_BARU).SingleOrDefault();
                        if (matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                        }
                        peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                        tahap = Convert.ToString(matriks.HR_TAHAP);
                    }
                    var pergerakan = db2.ZATUL_UPDATE_KEW_GERAK_GAJI2(bulan2, tahun, peribadi.HR_NO_PEKERJA, pergerakanGajiDetail.HR_BUTIR_PERUBAHAN, peribadi.HR_JENIS_PERGERAKAN, peringkat, tahap, pergerakanGajiDetail.HR_NP_FINALISED_HR, pergerakanGajiDetail.HR_FINALISED_IND_HR, pergerakanGajiDetail.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred, peribadi.HR_KEW8_ID, peribadi.HR_TARIKH_MULA, peribadi.HR_KOD_PELARASAN);

                }

                return RedirectToAction("PengesahanPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
            }
            ViewBag.key = key2;
            ViewBag.value = value2;
            ViewBag.bulan = bulan2;

            return RedirectToAction("PengesahanPergerakanGaji", new { key = key2, value = value2, bulan = bulan2 });
        }

        public PartialViewResult TablePergerakanGaji(string id, string key, string value, int? bulan)
        {
            ViewBag.id = id;
            List<PergerakanGajiModels> model = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERUBAHAN == "00001").ToList<HR_MAKLUMAT_KEWANGAN8>();

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            foreach (var item in kewangan8)
            {
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kewangan8Ditail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERUBAHAN == "00001" && s.HR_KEW8_ID == item.HR_KEW8_ID && s.HR_TARIKH_MULA == item.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
                foreach (var itemDetail in kewangan8Ditail)
                {
                    PergerakanGajiModels pergerakanGaji = new PergerakanGajiModels();
                    pergerakanGaji.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    pergerakanGaji.HR_KOD_PERUBAHAN = item.HR_KOD_PERUBAHAN;
                    pergerakanGaji.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                    pergerakanGaji.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                    pergerakanGaji.HR_BULAN = item.HR_BULAN;
                    pergerakanGaji.HR_TAHUN = item.HR_TAHUN;
                    pergerakanGaji.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                    pergerakanGaji.HR_BUTIR_PERUBAHAN = item.HR_BUTIR_PERUBAHAN;
                    pergerakanGaji.HR_CATATAN = item.HR_CATATAN;
                    pergerakanGaji.HR_NO_SURAT_KEBENARAN = item.HR_NO_SURAT_KEBENARAN;
                    pergerakanGaji.HR_AKTIF_IND = item.HR_AKTIF_IND;
                    pergerakanGaji.HR_NP_UBAH_HR = item.HR_NP_UBAH_HR;
                    pergerakanGaji.HR_TARIKH_UBAH_HR = item.HR_TARIKH_UBAH_HR;
                    pergerakanGaji.HR_NP_FINALISED_HR = item.HR_NP_FINALISED_HR;
                    pergerakanGaji.HR_TARIKH_FINALISED_HR = item.HR_TARIKH_FINALISED_HR;
                    pergerakanGaji.HR_FINALISED_IND_HR = item.HR_FINALISED_IND_HR;
                    pergerakanGaji.HR_NP_UBAH_PA = item.HR_NP_UBAH_PA;
                    pergerakanGaji.HR_TARIKH_UBAH_PA = item.HR_TARIKH_UBAH_PA;
                    pergerakanGaji.HR_NP_FINALISED_PA = item.HR_NP_FINALISED_PA;
                    pergerakanGaji.HR_TARIKH_FINALISED_PA = item.HR_TARIKH_FINALISED_PA;
                    pergerakanGaji.HR_FINALISED_IND_PA = item.HR_FINALISED_IND_PA;
                    pergerakanGaji.HR_EKA = item.HR_EKA;
                    pergerakanGaji.HR_ITP = item.HR_ITP;
                    pergerakanGaji.HR_KEW8_IND = item.HR_KEW8_IND;
                    pergerakanGaji.HR_BIL = item.HR_BIL;
                    pergerakanGaji.HR_KOD_JAWATAN = item.HR_KOD_JAWATAN;
                    pergerakanGaji.HR_KEW8_ID = item.HR_KEW8_ID;
                    pergerakanGaji.HR_LANTIKAN_IND = item.HR_LANTIKAN_IND;
                    pergerakanGaji.HR_TARIKH_SP = item.HR_TARIKH_SP;
                    pergerakanGaji.HR_SP_IND = item.HR_SP_IND;
                    pergerakanGaji.HR_JUMLAH_BULAN = item.HR_JUMLAH_BULAN;
                    pergerakanGaji.HR_NILAI_EPF = item.HR_NILAI_EPF;

                    pergerakanGaji.HR_KOD_PELARASAN = itemDetail.HR_KOD_PELARASAN;
                    pergerakanGaji.HR_MATRIKS_GAJI = itemDetail.HR_MATRIKS_GAJI;
                    pergerakanGaji.HR_GRED = itemDetail.HR_GRED;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN = itemDetail.HR_JUMLAH_PERUBAHAN;
                    pergerakanGaji.HR_GAJI_BARU = itemDetail.HR_GAJI_BARU;
                    pergerakanGaji.HR_JENIS_PERGERAKAN = itemDetail.HR_JENIS_PERGERAKAN;
                    pergerakanGaji.HR_JUMLAH_PERUBAHAN_ELAUN = itemDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                    pergerakanGaji.HR_STATUS_IND = itemDetail.HR_STATUS_IND;
                    pergerakanGaji.HR_ELAUN_KRITIKAL_BARU = itemDetail.HR_ELAUN_KRITIKAL_BARU;
                    pergerakanGaji.HR_NO_PEKERJA_PT = itemDetail.HR_NO_PEKERJA_PT;
                    pergerakanGaji.HR_PERGERAKAN_EKAL = itemDetail.HR_PERGERAKAN_EKAL;
                    pergerakanGaji.HR_PERGERAKAN_EWIL = itemDetail.HR_PERGERAKAN_EWIL;
                    pergerakanGaji.HR_GAJI_LAMA = itemDetail.HR_GAJI_LAMA;

                    GE_PARAMTABLE gred = new GE_PARAMTABLE();

                    if (itemDetail.HR_GRED != null)
                    {
                        var grd = Convert.ToInt32(itemDetail.HR_GRED);
                        gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                        if(gred == null)
                        {
                            gred = new GE_PARAMTABLE();
                        }
                    }


                    var peringkat = "";
                    decimal tahap = 0;
                    if (itemDetail.HR_MATRIKS_GAJI != null)
                    {
                        peringkat = itemDetail.HR_MATRIKS_GAJI.Substring(0, 2);
                        if(itemDetail.HR_MATRIKS_GAJI.Contains('T'))
                        {
                            
                            var t = itemDetail.HR_MATRIKS_GAJI.Substring(3);
                            tahap = Convert.ToDecimal(t);
                        }
                        
                    }

                    HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
                    if(jadual == null)
                    {
                        jadual = new HR_JADUAL_GAJI();
                    }
                    var jadualPeringkat = "";
                    var jp = 0;
                    if(jadual.HR_PERINGKAT != null)
                    {
                        jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                        jp = Convert.ToInt32(jadualPeringkat);
                    }

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
                    if(matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                    }
                    pergerakanGaji.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    pergerakanGaji.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    pergerakanGaji.HR_NAMA_GRED = matriks.HR_GRED_GAJI;
                    model.Add(pergerakanGaji);
                }
            }
            return PartialView("_TablePergerakanGaji", model);
        }

        [HttpGet]
        public ActionResult TambahPergerakanGaji(PergerakanGajiModels model, List<HR_MAKLUMAT_PERIBADI> sPeribadi,  ManageMessageId? message)
        {
            List<HR_MAKLUMAT_PERIBADI> sPekerja = new List<HR_MAKLUMAT_PERIBADI>();

            //List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            /*if (key2 == "1")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value2).ToList();

            }
            else if (key2 == "2")
            {
                value2 = value2.ToUpper();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value2.ToUpper())).ToList();
            }
            else if (key2 == "3")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value2)).ToList();
            }

            else if (key2 == "4")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan2).ToList();
            }*/

            if(sPeribadi.Count() > 0)
            {
                foreach (var peribadi in sPeribadi)
                {
                    // HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA).SingleOrDefault();
                    var bulan = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                    var tahun = DateTime.Now.Year;
                    var peringkat = "";
                    var tahap = "";
                    if (model.HR_MATRIKS_GAJI != null)
                    {
                        peringkat = model.HR_MATRIKS_GAJI.Substring(0, 2);
                        if (model.HR_MATRIKS_GAJI.Contains('T'))
                        {
                            var t = model.HR_MATRIKS_GAJI.Substring(3);
                            tahap = t;
                        }
                    }
                    sPekerja.Add(peribadi);
                }
            }

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();

            ViewBag.sPekerja = sPekerja;
            ViewBag.sPegawai = sPegawai;
            ViewBag.jabatan = mc.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            return PartialView("_TambahPergerakanGaji", model);
        }

        public ActionResult InfoPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if (mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if (mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {

                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if (HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if (HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_InfoPergerakanGaji", model);
        }

        public ActionResult EditPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if(id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if(mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if(mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if(HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if(HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_EditPergerakanGaji", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPergerakanGaji(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail, PergerakanGajiModels model, string key, string value, int? bulan)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);

                //var Gred = Convert.ToInt32(model.HR_GRED);
                //GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == Gred).ToList().OrderBy(s => s.SHORT_DESCRIPTION).SingleOrDefault();

                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == model.HR_GRED && s.HR_GAJI_MIN == model.HR_GAJI_MIN && s.HR_GAJI_MAX == model.HR_GAJI_MAX && s.HR_GAJI_POKOK == model.HR_GAJI_BARU).OrderByDescending(s => s.HR_GAJI_MAX).FirstOrDefault();
                if (matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                }

                var peringkat = Convert.ToString(matriks.HR_PERINGKAT);
                var tahap = Convert.ToString(matriks.HR_TAHAP);

                HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);

                var bulan2 = Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
                var tahun2 = DateTime.Now.Year;

                string gred = null;

                GE_PARAMTABLE gredList = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == model.HR_GRED).SingleOrDefault();
                if (gredList != null)
                {
                    gred = Convert.ToString(gredList.ORDINAL);
                }

                var pergerakan = db2.ZATUL_UPDATE_KEW_GERAK_GAJI2(bulan2, tahun2, peribadi.HR_NO_PEKERJA, model.HR_BUTIR_PERUBAHAN, model.HR_JENIS_PERGERAKAN, peringkat, tahap, model.HR_NP_FINALISED_HR, model.HR_FINALISED_IND_HR, model.HR_NO_SURAT_KEBENARAN, peribadi2.HR_NO_PEKERJA, gred, model.HR_KEW8_ID, model.HR_TARIKH_MULA, model.HR_KOD_PELARASAN);
                return RedirectToAction("PergerakanGaji", new { key = key, value = value, bulan = bulan } );
            }

            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            return View();
        }

        public ActionResult PadamPergerakanGaji(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string HR_KOD_PELARASAN, string key, string value, int? bulan)
        {
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null || HR_KOD_PELARASAN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime tarikh = Convert.ToDateTime(HR_TARIKH_MULA);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            HR_MAKLUMAT_KEWANGAN8 mKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh);
            HR_MAKLUMAT_KEWANGAN8_DETAIL mKewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tarikh && s.HR_KOD_PELARASAN == HR_KOD_PELARASAN);
            PergerakanGajiModels model = new PergerakanGajiModels();
            if (mKewangan8 == null || mKewangan8Detail == null || peribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.bulan = bulan;

            model.HR_NO_PEKERJA = mKewangan8.HR_NO_PEKERJA;
            model.HR_KOD_PERUBAHAN = mKewangan8.HR_KOD_PERUBAHAN;
            model.HR_TARIKH_MULA = mKewangan8.HR_TARIKH_MULA;
            model.HR_TARIKH_AKHIR = mKewangan8.HR_TARIKH_AKHIR;
            model.HR_BULAN = mKewangan8.HR_BULAN;
            model.HR_TAHUN = mKewangan8.HR_TAHUN;
            model.HR_TARIKH_KEYIN = mKewangan8.HR_TARIKH_KEYIN;
            model.HR_BUTIR_PERUBAHAN = mKewangan8.HR_BUTIR_PERUBAHAN;
            model.HR_CATATAN = mKewangan8.HR_CATATAN;
            model.HR_NO_SURAT_KEBENARAN = mKewangan8.HR_NO_SURAT_KEBENARAN;
            model.HR_AKTIF_IND = mKewangan8.HR_AKTIF_IND;
            model.HR_NP_UBAH_HR = mKewangan8.HR_NP_UBAH_HR;
            model.HR_TARIKH_UBAH_HR = mKewangan8.HR_TARIKH_UBAH_HR;
            model.HR_NP_FINALISED_HR = mKewangan8.HR_NP_FINALISED_HR;
            model.HR_TARIKH_FINALISED_HR = mKewangan8.HR_TARIKH_FINALISED_HR;
            model.HR_FINALISED_IND_HR = mKewangan8.HR_FINALISED_IND_HR;
            model.HR_NP_UBAH_PA = mKewangan8.HR_NP_UBAH_PA;
            model.HR_TARIKH_UBAH_PA = mKewangan8.HR_TARIKH_UBAH_PA;
            model.HR_NP_FINALISED_PA = mKewangan8.HR_NP_FINALISED_PA;
            model.HR_TARIKH_FINALISED_PA = mKewangan8.HR_TARIKH_FINALISED_PA;
            model.HR_FINALISED_IND_PA = mKewangan8.HR_FINALISED_IND_PA;
            model.HR_EKA = mKewangan8.HR_EKA;
            model.HR_ITP = mKewangan8.HR_ITP;
            model.HR_KEW8_IND = mKewangan8.HR_KEW8_IND;
            model.HR_BIL = mKewangan8.HR_BIL;
            model.HR_KOD_JAWATAN = mKewangan8.HR_KOD_JAWATAN;

            model.HR_KEW8_ID = mKewangan8.HR_KEW8_ID;
            model.HR_LANTIKAN_IND = mKewangan8.HR_LANTIKAN_IND;
            model.HR_TARIKH_SP = mKewangan8.HR_TARIKH_SP;
            model.HR_SP_IND = mKewangan8.HR_SP_IND;
            model.HR_JUMLAH_BULAN = mKewangan8.HR_JUMLAH_BULAN;
            model.HR_NILAI_EPF = mKewangan8.HR_NILAI_EPF;

            model.HR_KOD_PELARASAN = mKewangan8Detail.HR_KOD_PELARASAN;
            model.HR_MATRIKS_GAJI = mKewangan8Detail.HR_MATRIKS_GAJI;

            var grd2 = 0;
            if (mKewangan8Detail.HR_GRED != null)
            {
                grd2 = Convert.ToInt32(mKewangan8Detail.HR_GRED.Trim());
            }

            GE_PARAMTABLE gred2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd2).SingleOrDefault();

            model.HR_GRED = gred2.SHORT_DESCRIPTION;

            model.HR_JUMLAH_PERUBAHAN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN;
            model.HR_GAJI_BARU = mKewangan8Detail.HR_GAJI_BARU;
            model.HR_JENIS_PERGERAKAN = mKewangan8Detail.HR_JENIS_PERGERAKAN;
            model.HR_JUMLAH_PERUBAHAN_ELAUN = mKewangan8Detail.HR_JUMLAH_PERUBAHAN_ELAUN;
            model.HR_STATUS_IND = mKewangan8Detail.HR_STATUS_IND;
            model.HR_ELAUN_KRITIKAL_BARU = mKewangan8Detail.HR_ELAUN_KRITIKAL_BARU;
            model.HR_NO_PEKERJA_PT = mKewangan8Detail.HR_NO_PEKERJA_PT;
            model.HR_PERGERAKAN_EKAL = mKewangan8Detail.HR_PERGERAKAN_EKAL;
            model.HR_PERGERAKAN_EWIL = mKewangan8Detail.HR_PERGERAKAN_EWIL;
            model.HR_GAJI_LAMA = mKewangan8Detail.HR_GAJI_LAMA;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            GE_PARAMTABLE gred = new GE_PARAMTABLE();

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                var grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();
                if (gred == null)
                {
                    gred = new GE_PARAMTABLE();
                }
            }


            var peringkat2 = "";
            decimal tahap2 = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat2 = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(0, 2);
                if (mKewangan8Detail.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = mKewangan8Detail.HR_MATRIKS_GAJI.Substring(3);
                    tahap2 = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual2 = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred2.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat2).SingleOrDefault();
            if (jadual2 == null)
            {
                jadual2 = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat2 = "";
            var jp2 = 0;
            if (jadual2.HR_PERINGKAT != null)
            {
                jadualPeringkat2 = jadual2.HR_PERINGKAT.Substring(1);
                jp2 = Convert.ToInt32(jadualPeringkat2);
            }

            HR_MATRIKS_GAJI matriks2 = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual2.HR_GRED_GAJI && s.HR_PERINGKAT == jp2 && s.HR_TAHAP == tahap2).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks2 == null)
            {
                matriks2 = new HR_MATRIKS_GAJI();
            }

            model.HR_GAJI_MIN = matriks2.HR_GAJI_MIN;
            model.HR_GAJI_MAX = matriks2.HR_GAJI_MAX;

            var peringkat = "";
            decimal tahap = 0;
            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
            {
                peringkat = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 2);
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Contains('T'))
                {
                    var t = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3);
                    tahap = Convert.ToDecimal(t);
                }

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat).SingleOrDefault();
            if (jadual == null)
            {
                jadual = new HR_JADUAL_GAJI();
            }
            var jadualPeringkat = "";
            var jp = 0;
            if (jadual.HR_PERINGKAT != null)
            {
                jadualPeringkat = jadual.HR_PERINGKAT.Substring(1);
                jp = Convert.ToInt32(jadualPeringkat);
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jp && s.HR_TAHAP == tahap).OrderByDescending(s => s.HR_GAJI_MIN).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
            }


            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            float? HR_PERUBAHAN_KRITIKAL = 0;
            float? HR_PERGERAKAN_KRITIKAL = 0;

            float? HR_PERUBAHAN_WILAYAH = 0;
            float? HR_PERGERAKAN_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {

                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }
                }
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_WILAYAH = HR_WILAYAH;
            }
            if (HR_KRITIKAL > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_KRITIKAL = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_KRITIKAL;
            }
            if (HR_KRITIKAL > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_KRITIKAL = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_KRITIKAL;
            }


            if (HR_WILAYAH > 0 && mKewangan8Detail.HR_JUMLAH_PERUBAHAN > 0)
            {
                HR_PERUBAHAN_WILAYAH = Convert.ToSingle(mKewangan8Detail.HR_JUMLAH_PERUBAHAN) / HR_WILAYAH;
            }
            if (HR_WILAYAH > 0 && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK > 0)
            {
                HR_PERGERAKAN_WILAYAH = Convert.ToSingle(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) / HR_WILAYAH;
            }

            ViewBag.HR_PERUBAHAN_KRITIKAL = HR_PERUBAHAN_KRITIKAL;
            ViewBag.HR_PERGERAKAN_KRITIKAL = HR_PERGERAKAN_KRITIKAL;
            ViewBag.HR_PERUBAHAN_WILAYAH = HR_PERUBAHAN_WILAYAH;
            ViewBag.HR_PERGERAKAN_WILAYAH = HR_PERGERAKAN_WILAYAH;

            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == mKewangan8.HR_KOD_PERUBAHAN);

            ViewBag.HR_GAJI_POKOK = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GAJI_MIN_P = matriks.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_P = matriks.HR_GAJI_MAX;

            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_KOD_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            ViewBag.HR_SISTEM = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
            ViewBag.HR_JENIS_PERUBAHAN = kew8.HR_PENERANGAN;
            ViewBag.gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList().OrderBy(s => s.SHORT_DESCRIPTION);

            List<HR_MAKLUMAT_PERIBADI> sPegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            ViewBag.pegawai = sPegawai;

            HR_MAKLUMAT_PERIBADI Pegawai = sPegawai.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (Pegawai == null)
            {
                Pegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = Pegawai.HR_NAMA_PEKERJA;

            return PartialView("_PadamPergerakanGaji", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamPergerakanGaji(HR_MAKLUMAT_KEWANGAN8 kewangan8, HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail, PergerakanGajiModels model, string key, string value, int? bulan)
        {
            HR_MAKLUMAT_KEWANGAN8 kew8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN);
            db.HR_MAKLUMAT_KEWANGAN8.Remove(kew8);
            HR_MAKLUMAT_KEWANGAN8_DETAIL kew8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8Detail.HR_NO_PEKERJA && s.HR_KEW8_ID == kewangan8Detail.HR_KEW8_ID && s.HR_TARIKH_MULA == kewangan8Detail.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == kewangan8Detail.HR_KOD_PERUBAHAN && s.HR_KOD_PELARASAN == kewangan8Detail.HR_KOD_PELARASAN);
            db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Remove(kew8Detail);
            db.SaveChanges();
            return RedirectToAction("PergerakanGaji", new { key = key, value = value, bulan = bulan });
        }

        //[HttpPost, ActionName("TambahPergerakanGaji")]
        //[ValidateAntiForgeryToken]
        //public ActionResult TambahPergerakanGaji2(PergerakanGajiModels model,  string key2, string value2, int? bulan2, ManageMessageId? message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        List<HR_MAKLUMAT_PERIBADI> sPekerja = new List<HR_MAKLUMAT_PERIBADI>();
        //        //List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

        //        /*if (key2 == "1")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value2).ToList();

        //        }
        //        else if (key2 == "2")
        //        {
        //            value2 = value2.ToUpper();
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value2.ToUpper())).ToList();
        //        }
        //        else if (key2 == "3")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value2)).ToList();
        //        }

        //        else if (key2 == "4")
        //        {
        //            sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan2).ToList();
        //        }*/
        //        if(model.HR_MAKLUMAT_PERIBADI.Count() > 0)
        //        {
        //            foreach (var peribadi in model.HR_MAKLUMAT_PERIBADI)
        //            {
        //                HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA).SingleOrDefault();
        //                var bulan = Convert.ToDateTime(mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month;
        //                var tahun = DateTime.Now.Year;
        //                var peringkat = "";
        //                var tahap = "";
        //                if (model.HR_MATRIKS_GAJI != null)
        //                {
        //                    peringkat = model.HR_MATRIKS_GAJI.Substring(0, 2);
        //                    if (model.HR_MATRIKS_GAJI.Contains('T'))
        //                    {

        //                        var t = model.HR_MATRIKS_GAJI.Substring(3);
        //                        tahap = t;
        //                    }
        //                }
        //                var pergerakan = db2.ZATUL_INSERT_KEW_GERAK_GAJI2(bulan, tahun, peribadi.HR_NO_PEKERJA, model.HR_BUTIR_PERUBAHAN, model.HR_JENIS_PERGERAKAN, peringkat, tahap, model.HR_NP_FINALISED_HR, model.HR_NO_SURAT_KEBENARAN);
        //            }
        //        }

        //        return RedirectToAction("PergerakanGaji2", new { key = key2, value = value2, bulan = bulan2, Message = ManageMessageId.Success });
        //    }
        //    ViewBag.key2 = key2;
        //    ViewBag.value2 = value2;
        //    ViewBag.bulan2 = bulan2;
        //    return PartialView("_TambahPergerakanGaji", new { key = key2, value = value2, bulan = bulan2, Message = ManageMessageId.Error });
        //}

        public ActionResult BayarSemulaGaji()
        {
            return View();
        }

        public PartialViewResult TableKew8(string key, string value, string kod)
        {
            //ViewBag.jawatan = "";
            //ViewBag.gred = "";
            //ViewBag.kodGaji = "";
            //ViewBag.gaji = 0.00;
            //ViewBag.itp = 0.00;
            //ViewBag.awam = 0.00;
            

            //ViewBag.key = key;
            //ViewBag.value = value;
            ViewBag.kod = kod;

            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            var pengesahan = mPeribadi.FirstOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if(pengesahan == null)
            {
                pengesahan = new HR_MAKLUMAT_PERIBADI();
                pengesahan.HR_MAKLUMAT_PEKERJAAN = new HR_MAKLUMAT_PEKERJAAN();
            }
            ViewBag.pengesahan = pengesahan.HR_NO_PEKERJA;
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            if (key == "3")
            {
                peribadi = mPeribadi.FirstOrDefault(s => s.HR_NO_KPBARU == value);
            }
            else
            {
                peribadi = mPeribadi.FirstOrDefault(s => s.HR_NO_PEKERJA == value);
            }

            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
                peribadi.HR_MAKLUMAT_PEKERJAAN = new HR_MAKLUMAT_PEKERJAAN();
            }

            ViewBag.HR_NO_PEKERJA = peribadi.HR_NO_PEKERJA;

            ViewBag.HR_AKTIF_IND = peribadi.HR_AKTIF_IND;
            ViewBag.HR_GAJI_IND = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND;
            ViewBag.HR_TANGGUH_GERAKGAJI_IND = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TANGGUH_GERAKGAJI_IND;

            if (kod == "kew8")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045") && db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(q => q.HR_NO_PEKERJA == s.HR_NO_PEKERJA && q.HR_KOD_PERUBAHAN == s.HR_KOD_PERUBAHAN && q.HR_TARIKH_MULA == s.HR_TARIKH_MULA && q.HR_KEW8_ID == s.HR_KEW8_ID).Count() <= 0).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "TP")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00011" || s.HR_KOD_PERUBAHAN == "00014" || s.HR_KOD_PERUBAHAN == "00016" || s.HR_KOD_PERUBAHAN == "00020" || s.HR_KOD_PERUBAHAN == "00035" || s.HR_KOD_PERUBAHAN == "00044")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "CUTI")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "LNTKN")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00027")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (kod == "TMK")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00032")).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == kod).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }

            //HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            //if (jawatan == null)
            //{
            //    jawatan = new HR_JAWATAN();
            //}
            //ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //var kodGred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);

            //GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            //if (gred != null)
            //{
            //    ViewBag.gred = gred.SHORT_DESCRIPTION;
            //}
            //ViewBag.kodGaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
            //ViewBag.gaji = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

            //var jawatan_ind = "";

            //if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
            //{
            //    jawatan_ind = "K" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            //}
            //else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
            //{
            //    jawatan_ind = "P" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            //}

            //List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();

            //List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value).ToList();
            //if (elaunPotongan.Count() > 0)
            //{
            //    decimal? jumElaun = 0;
            //    decimal? jumAwam = 0;
            //    foreach (var item in elaunPotongan)
            //    {
            //        HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //        if (elaun != null)
            //        {
            //            jumElaun = item.HR_JUMLAH;
            //        }
            //        HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //        if (awam != null)
            //        {
            //            jumAwam = item.HR_JUMLAH;
            //        }

            //        if(item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_TARIKH_AKHIR >= DateTime.Now)
            //        {
            //            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
            //            elaun3.Add(elaun4);
            //        }
            //    }

            //    ViewBag.elaun3 = elaun3;

            //    ViewBag.itp = jumElaun;
            //    ViewBag.awam = jumAwam;
            //}

            //ViewBag.detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            ViewBag.Ansuran = model;
            return PartialView("_TableKew8", model.OrderByDescending(s => s.HR_TARIKH_MULA).GroupBy(s => s.HR_ANSURAN_ID).Select(s => s.FirstOrDefault()));
        }

        static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        [HttpPost]
        public ActionResult SuratKewangan8(PergerakanGajiModels model, string Kod, string TARIKH_BERSIDANG)
        {
            List<PergerakanGajiModels> item = new List<PergerakanGajiModels>();
            item.Add(model);

            return SenaraiSuratKewangan8(item, TARIKH_BERSIDANG);
        }

        public ActionResult SenaraiSuratKewangan8(IEnumerable<PergerakanGajiModels> model, string TARIKH_BERSIDANG)
        {
            DateTime tarikhBersidang = Convert.ToDateTime(TARIKH_BERSIDANG);

            string path_file = Server.MapPath("~/Content/template/");
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            object missing = System.Reflection.Missing.Value;
            string[] filePaths = Directory.GetFiles(path_file + "SuratPergerakanGaji/");
            foreach (string filePath in filePaths)
                System.IO.File.Delete(filePath);

            Microsoft.Office.Interop.Word.Document Doc = WordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            object start = 0; object end = 0;
            Microsoft.Office.Interop.Word.Range rng = Doc.Range(ref start, ref missing);
            foreach (PergerakanGajiModels pergerakanGaji in model)
            {
                HR_MAKLUMAT_KEWANGAN8 sKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_KOD_PERUBAHAN == "00001" && s.HR_KEW8_ID == pergerakanGaji.HR_KEW8_ID && s.HR_NO_PEKERJA == pergerakanGaji.HR_NO_PEKERJA);
                if (sKewangan8 == null)
                {
                    sKewangan8 = new HR_MAKLUMAT_KEWANGAN8();
                }

                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == sKewangan8.HR_NO_PEKERJA);
                if (peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }

                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                if (jawatan == null)
                {
                    jawatan = new HR_JAWATAN();
                }

                var grd = 0;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED.Trim();
                    grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();

                GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                if (jabatan == null)
                {
                    jabatan = new GE_JABATAN();
                }

                GE_BAHAGIAN bahagian = mc.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
                if (bahagian == null)
                {
                    bahagian = new GE_BAHAGIAN();
                }

                GE_UNIT unit = mc.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_UNIT == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT);
                if (unit == null)
                {
                    unit = new GE_UNIT();
                }

                HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == sKewangan8.HR_NO_PEKERJA).Count() > 0);
                if (gajiUpah == null)
                {
                    gajiUpah = new HR_GAJI_UPAHAN();
                }
                HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                if (potongan2 == null)
                {
                    potongan2 = new HR_POTONGAN();
                }

                var jawatan_ind = "";

                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    jawatan_ind = "K" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }
                else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    jawatan_ind = "P" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }

                HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
                if (gajiUpah == null)
                {
                    tggkk = new HR_GAJI_UPAHAN();
                }

                GE_PARAMTABLE tarafJawatan = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                if (tarafJawatan == null)
                {
                    tarafJawatan = new GE_PARAMTABLE();
                }

                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "April" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "September" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };

                var Bulan = "";
                var Bulan3 = "";
                foreach (var m in associativeArray)
                {
                    if (DateTime.Now.Month == m.Key)
                    {
                        Bulan = m.Value;
                    }

                    if (tarikhBersidang.Month == m.Key)
                    {
                        Bulan3 = m.Value;
                    }
                }
                var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
                templateEngine.Process(
                    source: path_file + "Template Surat Pergerakan Gaji.doc",
                    destination: path_file + "SuratPergerakanGaji/SuratPergerakanGaji(" + peribadi.HR_NO_PEKERJA + ").doc",
                    data: new
                    {
                        nama = peribadi.HR_NAMA_PEKERJA,
                        jawatan = (string.IsNullOrEmpty(jawatan.HR_NAMA_JAWATAN)) ? "" : UppercaseWords(jawatan.HR_NAMA_JAWATAN.Replace("&", "&amp;").Replace(", ", ",").Replace(",", ", ").ToLower()),
                        taraf = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN,
                        gred = gred.SHORT_DESCRIPTION,
                        jabatan = (string.IsNullOrEmpty(jabatan.GE_KETERANGAN_JABATAN)) ? "" : UppercaseWords(jabatan.GE_KETERANGAN_JABATAN.Replace("&", "&amp;").Replace(", ", ",").Replace(",", ", ").ToLower()),
                        bahagian = (string.IsNullOrEmpty(bahagian.GE_KETERANGAN)) ? "" : UppercaseWords(bahagian.GE_KETERANGAN.Replace("&", "&amp;").Replace(", ", ",").Replace(",", ", ").ToLower()),
                        //unit = unit.GE_KETERANGAN,
                        tahun = sKewangan8.HR_TAHUN.ToString(),
                        tarikh_cetak = string.Format("{0:dd}", DateTime.Now) + " " + Bulan.ToUpper() + " " + string.Format("{0:yyyy}", DateTime.Now),
                        tarikh_bersidang = string.Format("{0:dd}", tarikhBersidang) + " " + Bulan3 + " " + string.Format("{0:yyyy}", tarikhBersidang)
                    });

                rng.InsertFile(path_file + "SuratPergerakanGaji/SuratPergerakanGaji(" + peribadi.HR_NO_PEKERJA + ").doc", ref missing, ref missing, ref missing, ref missing);

                //Object Type = new object();
                //Type = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;
                //WordApp.ActiveDocument.Content.Words.Last.InsertBreak(ref Type);

                // now make start to point to the end of the content of the first document
                start = WordApp.ActiveDocument.Content.End - 1;
                rng = Doc.Range(ref start, ref missing);
            }

            foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Doc.Paragraphs)
            {
                paragraph.Range.Font.Name = "Times New Roman";
                paragraph.Range.Font.Size = 11;
                paragraph.Format.SpaceAfter = 0.1f;
            }

            Doc.SaveAs2(path_file + "Surat_Pergerakan_Gaji.doc");
            Doc.Application.Quit();
            WordApp.Quit();
            //Microsoft.Office.Interop.Word.Application WordApp2 = new Microsoft.Office.Interop.Word.Application();
            //Microsoft.Office.Interop.Word.Document adoc2 = WordApp2.Documents.Open(path_file + "Surat_Pergerakan_Gaji.docx");
            ////adoc2.ExportAsFixedFormat(path_file + "Surat_Pergerakan_Gaji.pdf", Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
            ////adoc.SaveAs2(path_file + "LIST_TEST.docx");
            //WordApp2.Quit();

            System.IO.FileInfo file = new System.IO.FileInfo(path_file + "Surat_Pergerakan_Gaji.doc");

            Response.Clear();
            Response.AddHeader("content-length", file.Length.ToString());
            Response.AddHeader("content-disposition", "attachment; filename = Surat_Pergerakan_Gaji.doc");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.TransmitFile(path_file + "Surat_Pergerakan_Gaji.doc");
            Response.Flush();
            Response.Close();

            //return View();
            return File(path_file + "Surat_Pergerakan_Gaji.doc", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        }

        //[HttpPost]
        public FileStreamResult SlipKewangan8(PergerakanGajiModels model, string Kod)
        {
            HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if(kew8 == null)
            {
                kew8 = new HR_KEWANGAN8();
            }

            HR_MAKLUMAT_KEWANGAN8 ansuranID = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == model.HR_KEW8_ID).OrderBy(s => s.HR_TARIKH_MULA).FirstOrDefault();
            List<HR_MAKLUMAT_KEWANGAN8> sKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == ansuranID.HR_ANSURAN_ID).OrderBy(s => s.HR_TARIKH_MULA).ToList();
            if(sKewangan8.Count() <= 0)
            {
                sKewangan8 = new List<HR_MAKLUMAT_KEWANGAN8>();
            }
            //HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID);
            //if (kewangan8Detail == null)
            //{
            //    kewangan8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
            //}
            List<HR_MAKLUMAT_KEWANGAN8> sAnsuran = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == model.HR_KEW8_ID).OrderBy(s => s.HR_TARIKH_MULA).ToList();

            //&& s.HR_TARIKH_AKHIR >= DateTime.Now
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E" && s.HR_AKTIF_IND == "Y").ToList();
            if(elaunPotongan == null)
            {
                elaunPotongan = new List<HR_MAKLUMAT_ELAUN_POTONGAN>();
            }

            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if(jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            var grd = 0;
            if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
            {
                peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED.Trim();
                grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            }
            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();

            GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if(jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }

            //GE_UNIT unit = mc.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
            //if(unit == null)
            //{
            //    unit = new GE_UNIT();
            //}

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            if(gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }
            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            var kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }
            else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (gajiUpah == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }
            var kodTGaji = tggkk.HR_KOD_UPAH;

            GE_PARAMTABLE tarafJawatan = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
            if(tarafJawatan == null)
            {
                tarafJawatan = new GE_PARAMTABLE();
            }

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4, 40, 40, 50, 50);
            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();
            
            foreach (HR_MAKLUMAT_KEWANGAN8 kewangan8 in sKewangan8)
            {
                HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
                if (kewangan8Detail == null)
                {
                    kewangan8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                document.NewPage();

                double? gaji = 0.00;
                double? jum = 0.00;
                double? peratusEPF = 0.00;
                if (kewangan8 != null && kewangan8Detail != null && Kod == "00031")
                {
                    HR_PERATUS_KWSP peratusKWSP = db.HR_PERATUS_KWSP.FirstOrDefault();
                    if (peratusKWSP == null)
                    {
                        peratusKWSP.HR_NILAI_PERATUS = 0;
                    }

                    peratusEPF = Convert.ToDouble(peratusKWSP.HR_NILAI_PERATUS);

                    //var EPF = Convert.ToDouble(kewangan8.HR_NILAI_EPF);
                    //var bulan = ((peratusEPF/100) * kewangan8.HR_JUMLAH_BULAN);
                    //jum = (Convert.ToDouble(kewangan8Detail.HR_JUMLAH_PERUBAHAN) + EPF);
                    //gaji = jum / bulan;

                    gaji = Convert.ToDouble(kewangan8Detail.HR_GAJI_LAMA);
                    jum = gaji * (peratusEPF / 100) * kewangan8.HR_JUMLAH_BULAN;

                }

                var html = "<html><head>";
                var font_size = "86%";
                var height = "45";
                html += "<title>Slip</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
                html += "<body>";
                if (Kod == "00031")
                {
                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    //html += "<thead>";
                    html += "<tr>";
                    html += "<td valign='top' rowspan='2' width='34%' style='font-size: " + font_size + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td width='67%' style='font-size: " + font_size + "'><u><strong>GANJARAN YANG DITERIMA :</strong></u></td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td width='67%' style='font-size: " + font_size + "'>[&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", gaji) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + kewangan8.HR_JUMLAH_BULAN + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + peratusEPF + "%&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;]&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF) + "<br />=&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", jum) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF) + "<br />=&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0:#,0.00}", kewangan8Detail.HR_JUMLAH_PERUBAHAN) + "<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;========</div></td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td width='34%' style='font-size: " + font_size + "'><u><strong>CATATAN :</strong></u></td>";
                    html += "<td width='67%' style='font-size: " + font_size + "'></td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td width='34%' style='font-size: " + font_size + "'>" + kewangan8.HR_CATATAN + "</td>";
                    html += "<td width='67%' style='font-size: " + font_size + "'></td>";
                    html += "</tr>";

                    html += "</table>";
                }
                if (Kod == "00025")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                    string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }
                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='34%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top'  align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='11%' style='font-size: " + font_size + "' height='" + height + "'><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + "</p><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</p></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='22%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_CATATAN + "</td>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='21%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_NO_SURAT_KEBENARAN + "</td>";
                    html += "</tr>";

                    foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                    {
                        HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                        html += "<tr>";
                        html += "<td width='13%' style='font-size: " + font_size + "' height='" + height + "'><u><strong>" + elaun2.HR_SINGKATAN + "</strong></u><br />RM " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "</td>";
                        html += "</tr>";
                    }

                    html += "</table>";
                }

                if (Kod == "kew8" || Kod == "00022" || Kod == "00037")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                    string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }
                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='34%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top'  align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='11%' style='font-size: " + font_size + "' height='" + height + "'><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + "</p><p>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</p></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='22%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_CATATAN + "</td>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='21%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_NO_SURAT_KEBENARAN + "</td>";
                    html += "</tr>";

                    foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                    {
                        HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                        html += "<tr>";
                        html += "<td width='13%' style='font-size: " + font_size + "' height='" + height + "'><u><strong>" + elaun2.HR_SINGKATAN + "</strong></u><br />RM / (%) " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "</td>";
                        html += "</tr>";
                    }

                    html += "</table>";
                }

                if (Kod == "00030" || Kod == "TP" || Kod == "CUTI" || Kod == "00015" || Kod == "LNTKN" || Kod == "00026")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                    string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }

                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID && (s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN || s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH));
                    if (kewangan8Detail2 == null)
                    {
                        kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }

                    string catatan = null;
                    if (kewangan8.HR_CATATAN == null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN == null)
                    {
                        catatan = kewangan8.HR_CATATAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN + "<br />" + kewangan8.HR_CATATAN;
                    }

                    decimal? jumElaun = 0;
                    decimal? jumPotongan = 0;
                    var CountElaun = elaunPotongan.Count();
                    if (Kod == "00015")
                    {
                        CountElaun = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID).Count();
                        if (CountElaun > 1)
                        {
                            CountElaun++;
                        }
                    }

                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='" + (CountElaun + 1) + "' width='34%' style='font-size: " + font_size + ";' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top' align='center' rowspan='" + (CountElaun + 1) + "' width='11%' style='font-size: " + font_size + ";' height='" + height + "'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + ((kewangan8.HR_TARIKH_AKHIR != null) ? "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) : "") + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + ";' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + ";' height='" + height + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN) + "</td>";
                    html += "<td valign='top' rowspan='" + (CountElaun) + "' width='21%' style='font-size: " + font_size + ";' height='" + height + "'>" + catatan + "</td>";
                    html += "</tr>";

                    foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                    {

                        HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                        HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail3 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID && (s.HR_KOD_PELARASAN == elaun2.HR_KOD_POTONGAN || s.HR_KOD_PELARASAN == elaun2.HR_KOD_TUNGGAKAN));
                        if (kewangan8Detail3 == null)
                        {
                            kewangan8Detail3 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        }
                        if (kewangan8Detail3.HR_JUMLAH_PERUBAHAN == null)
                        {
                            kewangan8Detail3.HR_JUMLAH_PERUBAHAN = 0;
                        }

                        if(Kod != "00015" || ((kewangan8Detail3.HR_KOD_PELARASAN == elaun2.HR_KOD_POTONGAN || kewangan8Detail3.HR_KOD_PELARASAN == elaun2.HR_KOD_TUNGGAKAN) && Kod == "00015"))
                        {
                            jumElaun += Math.Abs(Convert.ToDecimal(kewangan8Detail3.HR_JUMLAH_PERUBAHAN));

                            html += "<tr>";
                            html += "<td valign='top' width='13%' style='font-size: " + font_size + ";' height='" + height + "'>RM " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "<br />( " + elaun2.HR_SINGKATAN + " )</td>";
                            html += "<td valign='top' width='22%' style='font-size: " + font_size + ";' height='" + height + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail3.HR_JUMLAH_PERUBAHAN) + "</td>";
                            html += "</tr>";
                        }
                        
                    }
                    string txtJenis = "Potongan";
                    jumPotongan = Math.Abs(Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN)) + jumElaun;
                    if (kewangan8.HR_LANTIKAN_IND == "T" || kewangan8.HR_KEW8_IND == "T")
                    {
                        txtJenis = "Tunggakan";
                        jumPotongan = Math.Abs(Convert.ToDecimal(jumPotongan));
                    }
                    else
                    {
                        jumPotongan = -Math.Abs(Convert.ToDecimal(jumPotongan));
                    }

                    html += "<tr>";
                    html += "<td valign='top' width='34%' style='font-size: " + font_size + ";'></td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + ";'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + ";'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + ";'>Jumlah " + ((jumPotongan != 0)? txtJenis:"") + " =</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + ";'>RM " + string.Format("{0:#,0.00}", jumPotongan) + "</td>";
                    html += "</tr>";

                    html += "</table>";
                }

                if (Kod == "00036")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_LAMA);
                    string matriks2 = kewangan8.HR_MATRIKS_GAJI_LAMA;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }

                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
                    if (kewangan8Detail2 == null)
                    {
                        kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }
                    string catatan = null;
                    if (kewangan8.HR_CATATAN == null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN;
                    }
                    else if(kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN == null)
                    {
                        catatan = kewangan8.HR_CATATAN;
                    }
                    else if(kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN + "<br />" + kewangan8.HR_CATATAN;
                    }

                    decimal? jumElaun = 0;
                    decimal? jumDibayar = 0;
                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 3) + "' width='34%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top' align='center' rowspan='" + (elaunPotongan.Count() + 1) + "' width='11%' style='font-size: " + font_size + "' height='" + height + "'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + ((kewangan8.HR_TARIKH_AKHIR != null) ? "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) : "") + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_GAJI_BARU) + "</td>";
                    html += "<td valign='top' rowspan='" + (elaunPotongan.Count() + 1) + "' width='21%' style='font-size: " + font_size + "' height='" + height + "'>" + catatan + "</td>";
                    html += "</tr>";

                    foreach (HR_MAKLUMAT_ELAUN_POTONGAN elaun in elaunPotongan)
                    {

                        HR_ELAUN elaun2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun.HR_KOD_ELAUN_POTONGAN);
                        HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail3 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID && s.HR_KOD_PELARASAN == elaun2.HR_KOD_POTONGAN);
                        if (kewangan8Detail3 == null)
                        {
                            kewangan8Detail3 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        }
                        if (kewangan8Detail3.HR_JUMLAH_PERUBAHAN == null)
                        {
                            kewangan8Detail3.HR_JUMLAH_PERUBAHAN = 0;
                        }

                        jumElaun += Math.Abs(Convert.ToDecimal(kewangan8Detail3.HR_JUMLAH_PERUBAHAN));

                        html += "<tr>";
                        html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", elaun.HR_JUMLAH) + "<br />( " + elaun2.HR_SINGKATAN + " )</td>";
                        html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail3.HR_JUMLAH_PERUBAHAN) + "</td>";
                        html += "</tr>";
                    }

                    if(kewangan8Detail2.HR_GAJI_BARU == null)
                    {
                        kewangan8Detail2.HR_GAJI_BARU = 0;
                    }

                    if(kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }

                    jumDibayar = Convert.ToDecimal(kewangan8Detail2.HR_GAJI_BARU) - Math.Abs(Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN));

                    html += "<tr>";
                    //html += "<td valign='top' width='34%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Jumlah Potongan</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= RM (" + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN))) + ")</td>";
                    html += "</tr>";

                    html += "<tr>";
                    //html += "<td valign='top' width='34%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Jumlah Dibayar</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= RM " + string.Format("{0:#,0.00}", Convert.ToDecimal(jumDibayar)) + "</td>";
                    html += "</tr>";

                    html += "</table>";
                }

                if (Kod == "TMK" || Kod == "00032" || Kod == "00004")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_LAMA);
                    string matriks2 = kewangan8.HR_MATRIKS_GAJI_LAMA;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }

                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
                    if (kewangan8Detail2 == null)
                    {
                        kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }
                    string catatan = null;
                    if (kewangan8.HR_CATATAN == null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN == null)
                    {
                        catatan = kewangan8.HR_CATATAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN + "<br />" + kewangan8.HR_CATATAN;
                    }

                    int gred4 = 0;
                    if (kewangan8Detail2.HR_GRED != null)
                    {
                        gred4 = Convert.ToInt32(kewangan8Detail2.HR_GRED);
                    }
                    string sGred4 = cariGred(gred4, null).SHORT_DESCRIPTION;

                    HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == sGred4).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_GAJI_MIN).FirstOrDefault();
                    if (jGaji == null)
                    {
                        jGaji = new HR_JADUAL_GAJI();
                    }

                    string jenis = "Tunggakan";

                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='2' width='34%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + ((kewangan8.HR_TARIKH_AKHIR != null) ? "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) : "") + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";

                    if (kewangan8.HR_KOD_PERUBAHAN == "00032")
                    {
                        html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>GRED " + sGred4 + " = RM " + string.Format("{0:#,0.00}", kewangan8.HR_GAJI_MIN_BARU) + "<br />RM " + string.Format("{0:#,0.00}", kewangan8.HR_GAJI_MIN_BARU) + " X 25% = " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN) + "</td>";
                    }
                    else
                    {
                        html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    }

                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>" + catatan + "</td>";
                    html += "</tr>";

                    DateTime xKeyInDate = new DateTime(Convert.ToDateTime(model.HR_TARIKH_MULA).Year, Convert.ToDateTime(kewangan8.HR_TARIKH_KEYIN).Month, 1);
                    if (kewangan8.HR_TARIKH_MULA > xKeyInDate)
                    {
                        jenis = "Potongan";
                    }

                    HR_ELAUN txt = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_ELAUN == kewangan8Detail2.HR_KOD_PELARASAN);
                    if (txt == null)
                    {
                        txt = new HR_ELAUN();
                    }
                    
                    html += "<tr>";
                    //html += "<td valign='top' width='34%' style='font-size: " + font_size + ";border: 1px solid black;'></td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Jumlah Elaun</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= RM " + string.Format("{0:#,0.00}", Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN)) + "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='34%' style='font-size: " + font_size + "' height='" + height + "'><strong><u>CATATAN :</u></strong><br />" + txt.HR_PENERANGAN_ELAUN + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Jumlah " + jenis + "</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= RM " + string.Format("{0:#,0.00}", Convert.ToDecimal(kewangan8Detail2.HR_JUMLAH_PERUBAHAN)) + "</td>";
                    html += "</tr>";

                    html += "</table>";
                }

                if (Kod == "00024" || Kod == "00039")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                    string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                    if (kewangan8Detail.HR_GAJI_LAMA == null && kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }

                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == kewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == kewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
                    if (kewangan8Detail2 == null)
                    {
                        kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }

                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                    }

                    string catatan = null;
                    if (kewangan8.HR_CATATAN == null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN == null)
                    {
                        catatan = kewangan8.HR_CATATAN;
                    }
                    else if (kewangan8.HR_CATATAN != null && kewangan8.HR_NO_SURAT_KEBENARAN != null)
                    {
                        catatan = kewangan8.HR_NO_SURAT_KEBENARAN + "<br />" + kewangan8.HR_CATATAN;
                    }

                    string pelarasanTxt = null;
                    var jenis = "";
                    var xJenis = "Tunggakan";
                    if(Kod == "00024")
                    {
                        jenis = "Elaun";
                        HR_ELAUN txt = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_ELAUN == kewangan8Detail2.HR_KOD_PELARASAN);
                        if(txt != null)
                        {
                            pelarasanTxt = txt.HR_PENERANGAN_ELAUN;
                        }
                    }
                    else
                    {
                        jenis = "Potongan";
                        HR_POTONGAN txt = db.HR_POTONGAN.FirstOrDefault(s => s.HR_KOD_POTONGAN == kewangan8Detail2.HR_KOD_PELARASAN);
                        if (txt != null)
                        {
                            pelarasanTxt = txt.HR_PENERANGAN_POTONGAN;
                        }
                    }

                    var sHari = kewangan8.HR_TARIKH_MULA.ToString("dd");
                    var iHari = Convert.ToInt32(sHari);

                    if (kewangan8.HR_KEW8_IND == "P" || (kewangan8.HR_KEW8_IND == "E" && iHari > 1))
                    {
                        xJenis = "Potongan";
                    }

                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' width='34%' style='font-size: " + font_size + "' height='" + height + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR) + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>" + catatan + "</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'></td>";

                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' rowspan='2' width='34%' style='font-size: " + font_size + "' height='" + height + "'><strong><u>CATATAN :</u></strong><br />" + pelarasanTxt + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Nilai " + jenis +"</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= <b>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN_ELAUN) + "</b></td>";
                    html += "</tr>";

                    html += "<tr>";
                    //html += "<td valign='top' width='35%' style='font-size: " + font_size + "'><strong><u>CATATAN :</u></strong><br />" + pelarasanTxt + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "' height='" + height + "'></td>";
                    html += "<td valign='top' width='22%' style='font-size: " + font_size + "' height='" + height + "'>Jumlah " + xJenis + " (Pelarasan)</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "' height='" + height + "'>= <b>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN) + "</b></td>";
                    html += "</tr>";

                    html += "</table>";
                }

                if (Kod == "00001")
                {
                    double? gaji2 = Convert.ToDouble(kewangan8Detail.HR_GAJI_BARU);
                    double? gaji3 = Convert.ToDouble(kewangan8Detail.HR_GAJI_LAMA);
                    string matriks2 = kewangan8Detail.HR_MATRIKS_GAJI;
                    string matriks3 = kewangan8.HR_MATRIKS_GAJI_LAMA;
                    if (kewangan8Detail.HR_GAJI_BARU == null)
                    {
                        gaji2 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                        matriks2 = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    }

                    if (kewangan8Detail.HR_GAJI_LAMA == null)
                    {
                        gaji3 = Convert.ToDouble(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                    }

                    HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH);
                    if (kewangan8Detail2 == null)
                    {
                        kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }
                    if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                    {
                        kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                    }
                    int? gred5 = null;
                    if (kewangan8Detail2.HR_GRED != null)
                    {
                        gred5 = Convert.ToInt32(kewangan8Detail2.HR_GRED);
                    }

                    GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred5);
                    if (sGred == null)
                    {
                        sGred = new GE_PARAMTABLE();
                    }

                    int? peringkat = null;
                    //decimal? tahap = null;
                    if (kewangan8Detail2.HR_MATRIKS_GAJI != null)
                    {
                        kewangan8Detail2.HR_MATRIKS_GAJI = kewangan8Detail2.HR_MATRIKS_GAJI.Trim();
                        if (kewangan8Detail2.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                        {
                            peringkat = Convert.ToInt32(kewangan8Detail2.HR_MATRIKS_GAJI.Substring(1, 1));
                        }
                    }

                    HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == kewangan8Detail2.HR_GAJI_BARU).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                    if (matriks == null)
                    {
                        matriks = new HR_MATRIKS_GAJI();
                        matriks.HR_GAJI_MIN = 0;
                        matriks.HR_GAJI_MAX = 0;
                        matriks.HR_GAJI_POKOK = 0;
                    }

                    Decimal peratusWil = 0;
                    Decimal peratusKal = 0;
                    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kewangan8Detail3 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                    foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL Detail3 in kewangan8Detail3)
                    {
                        HR_ELAUN elaunWil = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == Detail3.HR_KOD_PELARASAN);
                        if (elaunWil != null)
                        {
                            peratusWil = Convert.ToDecimal(Detail3.HR_JUMLAH_PERUBAHAN_ELAUN / 100);
                        }

                        HR_ELAUN elaunKal = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == Detail3.HR_KOD_PELARASAN);
                        if (elaunKal != null)
                        {
                            peratusKal = Convert.ToDecimal(Detail3.HR_JUMLAH_PERUBAHAN_ELAUN / 100);
                        }
                    }

                    var totalAll = kewangan8Detail2.HR_JUMLAH_PERUBAHAN + kewangan8Detail2.HR_PERGERAKAN_EKAL + kewangan8Detail2.HR_PERGERAKAN_EWIL;

                    html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                    html += "<tr>";
                    html += "<td valign='top' colspan='2' width='34%' style='font-size: " + font_size + "'>" + kewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                    html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "'>" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_MULA) + ((kewangan8.HR_TARIKH_AKHIR != null) ? "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", kewangan8.HR_TARIKH_AKHIR): "") + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", gaji2) + "</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>Dibayar</u><br />RM " + string.Format("{0:#,0.00}", gaji3) + "<br />(" + matriks3 + ")</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>Baru</u><br />RM " + string.Format("{0:#,0.00}", gaji2) + "<br />(" + matriks2 + ")</td>";
                    html += "<td valign='top' rowspan='3' width='21%' style='font-size: " + font_size + "'><u>Beza</u><br />RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN) + "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'><u>GAJI MIN</u></td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'><u>GAJI MAX</u></td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>KGT</u><br /></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'><u>Elaun Kritikal</u></td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", matriks.HR_GAJI_MIN) + "</td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", matriks.HR_GAJI_MAX) + "</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", (gaji2 - gaji3)) + "</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gaji3) * peratusKal)) + "</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gaji2) * peratusKal)) + "</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_PERGERAKAN_EKAL) + "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'><u>Elaun Wilayah</u></td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gaji3) * peratusWil)) + "</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gaji2) * peratusWil)) + "</td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_PERGERAKAN_EWIL) + "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'>&nbsp;</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td valign='top' colspan='5' width='63%' style='font-size: " + font_size + "'></td>";
                    html += "<td valign='top' align='right' width='11%' style='font-size: " + font_size + "'><strong>Jumlah = </strong></td>";
                    html += "<td valign='top' width='21%' style='font-size: " + font_size + "'><strong>RM " + string.Format("{0:#,0.00}", totalAll) + "</strong></td>";
                    html += "</tr>";

                    html += "</table>";
                }

                html += "<br /><br />";

                html += "</body></html>";

                string exportData = string.Format(html);
                var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
                var input = new MemoryStream(bytes);
                
                    
                    

                    var xmlWorker = XMLWorkerHelper.GetInstance();
                    //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

                    var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "April" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "September" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };

                    var Bulan = "";
                    foreach (var m in associativeArray)
                    {
                        if (DateTime.Now.Month == m.Key)
                        {
                            Bulan = m.Value;
                        }

                    }

                    var KPLama = "";
                    if (peribadi.HR_NO_KPLAMA != null)
                    {
                        KPLama = " / " + peribadi.HR_NO_KPLAMA.ToUpper();
                    }

                    //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                    iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font contentFont4 = iTextSharp.text.FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Paragraph noPekerja = new iTextSharp.text.Paragraph(peribadi.HR_NAMA_PEKERJA + "      " + peribadi.HR_NO_PEKERJA, contentFont2);
                    //noPekerja.Alignment = Element.ALIGN_JUSTIFIED;
                    noPekerja.IndentationLeft = 57f;

                    iTextSharp.text.Paragraph IC = new iTextSharp.text.Paragraph(peribadi.HR_NO_KPBARU + KPLama, contentFont2);
                    //IC.Alignment = Element.ALIGN_JUSTIFIED;

                    iTextSharp.text.Paragraph jwt = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                    //jwt.Alignment = Element.ALIGN_JUSTIFIED;
                    jwt.IndentationLeft = 35f;

                    iTextSharp.text.Paragraph jawatan2 = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                    //jawatan2.Alignment = Element.ALIGN_JUSTIFIED;


                    iTextSharp.text.Paragraph gred2 = new iTextSharp.text.Paragraph(gred.SHORT_DESCRIPTION, contentFont2);
                    //gred2.Alignment = Element.ALIGN_JUSTIFIED;


                    iTextSharp.text.Paragraph tJawatan = new iTextSharp.text.Paragraph(tarafJawatan.SHORT_DESCRIPTION, contentFont2);
                    //tJawatan.Alignment = Element.ALIGN_JUSTIFIED;

                    iTextSharp.text.Paragraph jabatan2 = new iTextSharp.text.Paragraph(jabatan.GE_KETERANGAN_JABATAN, contentFont2);
                    //jabatan2.Alignment = Element.ALIGN_JUSTIFIED;
                    jabatan2.IndentationLeft = 100f;

                    iTextSharp.text.Paragraph votGaji = new iTextSharp.text.Paragraph("11-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + gajiUpah.HR_VOT_UPAH, contentFont2);
                    //votGaji.Alignment = Element.ALIGN_JUSTIFIED;

                    //pic.ScaleToFit(100f, 80f);
                    //pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                    //pic.IndentationRight = 10f;

                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    //document.Add(pic);
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    //document.Add(new iTextSharp.text.Paragraph("\n"));

                    PdfPTable table = new PdfPTable(3);
                    table.TotalWidth = 510f;
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.LockedWidth = true;
                    float[] widths = new float[] { 7f, 1f, 2f };

                    table.SetWidths(widths);

                    table.HorizontalAlignment = 1;

                    //leave a gap before and after the table

                    //table.SpacingBefore = 20f;

                    //table.SpacingAfter = 30f;

                    PdfPCell cell = new PdfPCell();
                    cell.AddElement(noPekerja);
                    cell.PaddingTop = 8f;
                    cell.BorderWidthBottom = 0f;
                    cell.BorderWidthLeft = 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = 0f;

                    PdfPCell cell2 = new PdfPCell();
                    cell2.AddElement(IC);
                    cell2.PaddingTop = 8f;
                    cell2.Colspan = 4;
                    cell2.BorderWidthBottom = 0f;
                    cell2.BorderWidthLeft = 0f;
                    cell2.BorderWidthTop = 0f;
                    cell2.BorderWidthRight = 0f;

                    PdfPCell cell3 = new PdfPCell();
                    cell3.AddElement(jwt);
                    cell3.PaddingTop = 4f;
                    cell3.BorderWidthBottom = 0f;
                    cell3.BorderWidthLeft = 0f;
                    cell3.BorderWidthTop = 0f;
                    cell3.BorderWidthRight = 0f;

                    PdfPCell cell4 = new PdfPCell();
                    cell4.AddElement(gred2);
                    cell4.PaddingTop = 4f;
                    cell4.BorderWidthBottom = 0f;
                    cell4.BorderWidthLeft = 0f;
                    cell4.BorderWidthTop = 0f;
                    cell4.BorderWidthRight = 0f;

                    PdfPCell cell5 = new PdfPCell();
                    cell5.AddElement(tJawatan);
                    cell5.PaddingTop = 4f;
                    cell5.BorderWidthBottom = 0f;
                    cell5.BorderWidthLeft = 0f;
                    cell5.BorderWidthTop = 0f;
                    cell5.BorderWidthRight = 0f;

                    PdfPCell cell6 = new PdfPCell();
                    cell6.AddElement(jabatan2);
                    cell6.PaddingTop = 4f;
                    cell6.BorderWidthBottom = 0f;
                    cell6.BorderWidthLeft = 0f;
                    cell6.BorderWidthTop = 0f;
                    cell6.BorderWidthRight = 0f;

                    PdfPCell cell7 = new PdfPCell();
                    cell7.AddElement(new iTextSharp.text.Paragraph("Vot Gaji :", contentFont2));
                    cell7.PaddingTop = 4f;
                    cell7.BorderWidthBottom = 0f;
                    cell7.BorderWidthLeft = 0f;
                    cell7.BorderWidthTop = 0f;
                    cell7.BorderWidthRight = 0f;

                    PdfPCell cell8 = new PdfPCell();
                    cell8.AddElement(votGaji);
                    cell8.PaddingTop = 4f;
                    cell8.BorderWidthBottom = 0f;
                    cell8.BorderWidthLeft = 0f;
                    cell8.BorderWidthTop = 0f;
                    cell8.BorderWidthRight = 0f;



                    //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    //table.AddCell(cell);
                    //table.AddCell(new iTextSharp.text.Paragraph("NAMA", contentFont));
                    //table.AddCell(new iTextSharp.text.Paragraph(" : ", contentFont));
                    table.AddCell(cell);
                    table.AddCell(cell2);

                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);

                    table.AddCell(cell6);
                    table.AddCell(cell7);
                    table.AddCell(cell8);

                    document.Add(table);

                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    document.Add(new iTextSharp.text.Paragraph("\n"));

                    //PdfPTable table2 = new PdfPTable(2);
                    //table2.TotalWidth = 600f;
                    //table2.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //table2.LockedWidth = true;
                    //float[] widths2 = new float[] { 2f, 2f };

                    //table2.SetWidths(widths2);

                    //table2.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right

                    ////leave a gap before and after the table

                    ////table.SpacingBefore = 20f;

                    ////table.SpacingAfter = 30f;



                    //Chunk c2 = new Chunk("GANJARAN YANG DITERIMA :", contentFont);
                    //c2.SetUnderline(0.5f, -1.5f);
                    //Chunk c = new Chunk("BUTIRAN :", contentFont);
                    //c.SetUnderline(0.5f, -1.5f);
                    //Chunk c3 = new Chunk("CATATAN :", contentFont);
                    //c3.SetUnderline(0.5f, -1.5f);

                    //Chunk total = new Chunk(string.Format("{0:#,0.00}", kewangan8Detail.HR_JUMLAH_PERUBAHAN), contentFont);
                    //total.SetUnderline(0.5f, -1.5f); ;

                    //table2.AddCell(new iTextSharp.text.Paragraph(c));
                    //table2.AddCell(new iTextSharp.text.Paragraph(c2));
                    //table2.AddCell(new iTextSharp.text.Paragraph(kewangan8.HR_BUTIR_PERUBAHAN, contentFont2));
                    //table2.AddCell(new iTextSharp.text.Paragraph("[     RM     "+ string.Format("{0:#,0.00}", gaji) + "     X     "+ kewangan8.HR_JUMLAH_BULAN +"     X     5.5%     ]     -     RM     "+ string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF)+"\n=     RM     "+ string.Format("{0:#,0.00}", jum) +"     -     RM     "+ string.Format("{0:#,0.00}", kewangan8.HR_NILAI_EPF)+"\n=     RM     "+ total, contentFont2));

                    //table2.AddCell(new iTextSharp.text.Paragraph(c3));
                    //table2.AddCell(new iTextSharp.text.Paragraph(""));
                    //table2.AddCell(new iTextSharp.text.Paragraph(kewangan8.HR_CATATAN, contentFont2));
                    //table2.AddCell(new iTextSharp.text.Paragraph(""));

                    //document.Add(table2);

                    //document.Add(new iTextSharp.text.Paragraph("\n"));
                    //document.Add(new iTextSharp.text.Paragraph("\n"));

                    xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                    //iTextSharp.text.Font contentFont3 = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL);



                    iTextSharp.text.Paragraph paragraph5 = new iTextSharp.text.Paragraph(DateTime.Now.Day + "-" + Bulan + "-" + DateTime.Now.Year + "\nKod Kew. 8 : " + kewangan8.HR_KEW8_ID, contentFont2);
                    PdfContentByte cb = writer.DirectContent;
                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(70f, 395f, 395f, 150f);
                    ct.SetText(paragraph5);
                    ct.Go();
            }
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public int? CariPeringkat(string matriks)
        {
            int? peringkat = null;
            if (matriks != null)
            {
                matriks = matriks.Trim();
                if (matriks.Substring(0, 1) == "P")
                {
                    peringkat = Convert.ToInt32(matriks.Substring(1, 1));
                }
            }
            
            return peringkat;
        }

        [HttpPost]
        public FileStreamResult SenaraiSlipKewangan8(IEnumerable<PergerakanGajiModels> model)
        {
            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4, 40, 40, 50, 50);
            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();

            foreach (PergerakanGajiModels kewangan8 in model)
            {
                HR_KEWANGAN8 kew8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == "00001");
                if (kew8 == null)
                {
                    kew8 = new HR_KEWANGAN8();
                }
                HR_MAKLUMAT_KEWANGAN8 sKewangan8 = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001" && s.HR_KEW8_ID == kewangan8.HR_KEW8_ID);
                if (sKewangan8 == null)
                {
                    sKewangan8 = new HR_MAKLUMAT_KEWANGAN8();
                }
                HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == sKewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001" && s.HR_TARIKH_MULA == sKewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == sKewangan8.HR_KEW8_ID);
                if (kewangan8Detail == null)
                {
                    kewangan8Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }
                
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA);
                if (peribadi == null)
                {
                    peribadi = new HR_MAKLUMAT_PERIBADI();
                }

                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                if (jawatan == null)
                {
                    jawatan = new HR_JAWATAN();
                }
                var grd = 0;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED.Trim();
                    grd = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.ORDINAL == grd).SingleOrDefault();

                GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                if (jabatan == null)
                {
                    jabatan = new GE_JABATAN();
                }

                HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == kewangan8.HR_NO_PEKERJA).Count() > 0);
                if (gajiUpah == null)
                {
                    gajiUpah = new HR_GAJI_UPAHAN();
                }
                HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                if (potongan2 == null)
                {
                    potongan2 = new HR_POTONGAN();
                }

                var jawatan_ind = "";

                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    jawatan_ind = "K" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }
                else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    jawatan_ind = "P" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }

                HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
                if (gajiUpah == null)
                {
                    tggkk = new HR_GAJI_UPAHAN();
                }

                GE_PARAMTABLE tarafJawatan = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN);
                if (tarafJawatan == null)
                {
                    tarafJawatan = new GE_PARAMTABLE();
                }

                document.NewPage();

                var html = "<html><head>";
                var font_size = "86%";
                html += "<title>Slip</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
                html += "<body>";
                
                decimal? gajiDibayar = Convert.ToDecimal(kewangan8Detail.HR_GAJI_LAMA);
                decimal? gajiBaru = Convert.ToDecimal(kewangan8Detail.HR_GAJI_BARU);
                decimal? bulan = Convert.ToDecimal(sKewangan8.HR_BULAN);
                string matriksDibayar = sKewangan8.HR_MATRIKS_GAJI_LAMA;
                string matriksBaru = kewangan8Detail.HR_MATRIKS_GAJI;

                if (kewangan8Detail.HR_GAJI_BARU == null)
                {
                    gajiBaru = Convert.ToDecimal(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                    matriksDibayar = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                }

                if (kewangan8Detail.HR_GAJI_LAMA == null)
                {
                    gajiDibayar = Convert.ToDecimal(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
                }

                HR_MAKLUMAT_KEWANGAN8_DETAIL kewangan8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == sKewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == sKewangan8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == sKewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == sKewangan8.HR_KEW8_ID && s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH);
                if (kewangan8Detail2 == null)
                {
                    kewangan8Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }
                if (kewangan8Detail2.HR_JUMLAH_PERUBAHAN == null)
                {
                    kewangan8Detail2.HR_JUMLAH_PERUBAHAN = 0;
                }
                int? gred5 = null;
                if (kewangan8Detail2.HR_GRED != null)
                {
                    gred5 = Convert.ToInt32(kewangan8Detail2.HR_GRED);
                }

                int? gredDibayar = null;
                if (sKewangan8.HR_GRED_LAMA != null)
                {
                    gredDibayar = Convert.ToInt32(sKewangan8.HR_GRED_LAMA);
                }

                GE_PARAMTABLE sGred = cariGred(gred5, null);
                GE_PARAMTABLE sGred2 = cariGred(gredDibayar, null);

                int? peringkat = CariPeringkat(kewangan8Detail2.HR_MATRIKS_GAJI);

                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == kewangan8Detail2.HR_GAJI_BARU).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                if (matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                    matriks.HR_GAJI_MIN = 0;
                    matriks.HR_GAJI_MAX = 0;
                    matriks.HR_GAJI_POKOK = 0;
                }

                Decimal peratusWil = 0;
                Decimal peratusKal = 0;
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kewangan8Detail3 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == sKewangan8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == "00001" && s.HR_TARIKH_MULA == sKewangan8.HR_TARIKH_MULA && s.HR_KEW8_ID == sKewangan8.HR_KEW8_ID).ToList();
                foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL Detail3 in kewangan8Detail3)
                {
                    HR_ELAUN elaunWil = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == Detail3.HR_KOD_PELARASAN);
                    if (elaunWil != null)
                    {
                        peratusWil = Convert.ToDecimal(Detail3.HR_JUMLAH_PERUBAHAN_ELAUN / 100);
                    }

                    HR_ELAUN elaunKal = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == Detail3.HR_KOD_PELARASAN);
                    if (elaunKal != null)
                    {
                        peratusKal = Convert.ToDecimal(Detail3.HR_JUMLAH_PERUBAHAN_ELAUN / 100);
                    }
                }

                var totalAll = kewangan8Detail2.HR_JUMLAH_PERUBAHAN + kewangan8Detail2.HR_PERGERAKAN_EKAL + kewangan8Detail2.HR_PERGERAKAN_EWIL;

                html += "<table width='105%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' colspan='2' width='34%' style='font-size: " + font_size + "'>" + sKewangan8.HR_BUTIR_PERUBAHAN + "</td>";
                html += "<td valign='top' align='center' width='11%' style='font-size: " + font_size + "'>" + string.Format("{0:dd/MM/yyyy}", sKewangan8.HR_TARIKH_MULA) + ((sKewangan8.HR_TARIKH_AKHIR != null) ? "<br />-<br />" + string.Format("{0:dd/MM/yyyy}", sKewangan8.HR_TARIKH_AKHIR) : "") + "</td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", gajiBaru) + "</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>Dibayar</u><br />RM " + string.Format("{0:#,0.00}", gajiDibayar) + "<br />(" + matriksDibayar + ")</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>Baru</u><br />RM " + string.Format("{0:#,0.00}", gajiBaru) + "<br />(" + matriksBaru + ")</td>";
                html += "<td valign='top' width='21%' style='font-size: " + font_size + "'><u>Beza</u><br />RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_JUMLAH_PERUBAHAN) + "</td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'><u>GAJI MIN</u></td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'><u>GAJI MAX</u></td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'><u>KGT</u><br /></td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'><u>Elaun Kritikal</u></td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", matriks.HR_GAJI_MIN) + "</td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", matriks.HR_GAJI_MAX) + "</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>" + string.Format("{0:#,0.00}", (gajiBaru - gajiDibayar)) + "</td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gajiDibayar * peratusKal))) + "</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gajiBaru * peratusKal))) + "</td>";
                html += "<td valign='top' width='21%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_PERGERAKAN_EKAL) + "</td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'><u>Elaun Wilayah</u></td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gajiDibayar * peratusWil))) + "</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", Math.Abs(Convert.ToDecimal(gajiBaru * peratusWil))) + "</td>";
                html += "<td valign='top' width='21%' style='font-size: " + font_size + "'>RM " + string.Format("{0:#,0.00}", kewangan8Detail2.HR_PERGERAKAN_EWIL) + "</td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='17%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='11%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' width='13%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "<td valign='top' colspan='3' width='43%' style='font-size: " + font_size + "'>&nbsp;</td>";
                html += "</tr>";

                html += "<tr>";
                html += "<td valign='top' colspan='5' width='63%' style='font-size: " + font_size + "'></td>";
                html += "<td valign='top' align='right' width='11%' style='font-size: " + font_size + "'><strong>Jumlah = </strong></td>";
                html += "<td valign='top' width='21%' style='font-size: " + font_size + "'><strong>RM " + string.Format("{0:#,0.00}", totalAll) + "</strong></td>";
                html += "</tr>";

                html += "</table>";
                

                html += "<br /><br />";

                html += "</body></html>";

                string exportData = string.Format(html);
                var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
                var input = new MemoryStream(bytes);




                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

                var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "April" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "September" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };

                var Bulan = "";
                foreach (var m in associativeArray)
                {
                    if (DateTime.Now.Month == m.Key)
                    {
                        Bulan = m.Value;
                    }

                }

                var KPLama = "";
                if (peribadi.HR_NO_KPLAMA != null)
                {
                    KPLama = " / " + peribadi.HR_NO_KPLAMA.ToUpper();
                }

                //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentFont4 = iTextSharp.text.FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Paragraph noPekerja = new iTextSharp.text.Paragraph(peribadi.HR_NAMA_PEKERJA + "      " + peribadi.HR_NO_PEKERJA, contentFont2);
                //noPekerja.Alignment = Element.ALIGN_JUSTIFIED;
                noPekerja.IndentationLeft = 57f;

                iTextSharp.text.Paragraph IC = new iTextSharp.text.Paragraph(peribadi.HR_NO_KPBARU + KPLama, contentFont2);
                //IC.Alignment = Element.ALIGN_JUSTIFIED;

                iTextSharp.text.Paragraph jwt = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                //jwt.Alignment = Element.ALIGN_JUSTIFIED;
                jwt.IndentationLeft = 35f;

                iTextSharp.text.Paragraph jawatan2 = new iTextSharp.text.Paragraph(jawatan.HR_NAMA_JAWATAN, contentFont2);
                //jawatan2.Alignment = Element.ALIGN_JUSTIFIED;


                iTextSharp.text.Paragraph gred2 = new iTextSharp.text.Paragraph(gred.SHORT_DESCRIPTION, contentFont2);
                //gred2.Alignment = Element.ALIGN_JUSTIFIED;


                iTextSharp.text.Paragraph tJawatan = new iTextSharp.text.Paragraph(tarafJawatan.SHORT_DESCRIPTION, contentFont2);
                tJawatan.Alignment = Element.ALIGN_JUSTIFIED;

                iTextSharp.text.Paragraph jabatan2 = new iTextSharp.text.Paragraph(jabatan.GE_KETERANGAN_JABATAN, contentFont2);
                //jabatan2.Alignment = Element.ALIGN_JUSTIFIED;
                jabatan2.IndentationLeft = 100f;

                iTextSharp.text.Paragraph votGaji = new iTextSharp.text.Paragraph("11-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + gajiUpah.HR_VOT_UPAH, contentFont2);
                //votGaji.Alignment = Element.ALIGN_JUSTIFIED;

                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));
                //document.Add(new iTextSharp.text.Paragraph("\n"));

                PdfPTable table = new PdfPTable(3);
                table.TotalWidth = 510f;
                table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.LockedWidth = true;
                float[] widths = new float[] { 7f, 1f, 2f };

                table.SetWidths(widths);

                table.HorizontalAlignment = 1;

                PdfPCell cell = new PdfPCell();
                cell.AddElement(noPekerja);
                cell.PaddingTop = 8f;
                cell.BorderWidthBottom = 0f;
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthRight = 0f;

                PdfPCell cell2 = new PdfPCell();
                cell2.AddElement(IC);
                cell2.PaddingTop = 8f;
                cell2.Colspan = 4;
                cell2.BorderWidthBottom = 0f;
                cell2.BorderWidthLeft = 0f;
                cell2.BorderWidthTop = 0f;
                cell2.BorderWidthRight = 0f;

                PdfPCell cell3 = new PdfPCell();
                cell3.AddElement(jwt);
                cell3.PaddingTop = 4f;
                cell3.BorderWidthBottom = 0f;
                cell3.BorderWidthLeft = 0f;
                cell3.BorderWidthTop = 0f;
                cell3.BorderWidthRight = 0f;

                PdfPCell cell4 = new PdfPCell();
                cell4.AddElement(gred2);
                cell4.PaddingTop = 4f;
                cell4.BorderWidthBottom = 0f;
                cell4.BorderWidthLeft = 0f;
                cell4.BorderWidthTop = 0f;
                cell4.BorderWidthRight = 0f;

                PdfPCell cell5 = new PdfPCell();
                cell5.AddElement(tJawatan);
                cell5.PaddingTop = 4f;
                cell5.BorderWidthBottom = 0f;
                cell5.BorderWidthLeft = 0f;
                cell5.BorderWidthTop = 0f;
                cell5.BorderWidthRight = 0f;

                PdfPCell cell6 = new PdfPCell();
                cell6.AddElement(jabatan2);
                cell6.PaddingTop = 4f;
                cell6.BorderWidthBottom = 0f;
                cell6.BorderWidthLeft = 0f;
                cell6.BorderWidthTop = 0f;
                cell6.BorderWidthRight = 0f;

                PdfPCell cell7 = new PdfPCell();
                cell7.AddElement(new iTextSharp.text.Paragraph("Vot Gaji :", contentFont2));
                cell7.PaddingTop = 4f;
                cell7.BorderWidthBottom = 0f;
                cell7.BorderWidthLeft = 0f;
                cell7.BorderWidthTop = 0f;
                cell7.BorderWidthRight = 0f;

                PdfPCell cell8 = new PdfPCell();
                cell8.AddElement(votGaji);
                cell8.PaddingTop = 4f;
                cell8.BorderWidthBottom = 0f;
                cell8.BorderWidthLeft = 0f;
                cell8.BorderWidthTop = 0f;
                cell8.BorderWidthRight = 0f;

                table.AddCell(cell);
                table.AddCell(cell2);

                table.AddCell(cell3);
                table.AddCell(cell4);
                table.AddCell(cell5);

                table.AddCell(cell6);
                table.AddCell(cell7);
                table.AddCell(cell8);

                document.Add(table);

                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                iTextSharp.text.Paragraph paragraph5 = new iTextSharp.text.Paragraph(DateTime.Now.Day + "-" + Bulan + "-" + DateTime.Now.Year + "\nKod Kew. 8 : " + kewangan8.HR_KEW8_ID, contentFont2);
                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);
                ct.SetSimpleColumn(70f, 395f, 395f, 150f);
                ct.SetText(paragraph5);
                ct.Go();
                
            }
            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public GE_PARAMTABLE cariGred(int? kodGred, string kodGred2)
        {
            GE_PARAMTABLE gred = new GE_PARAMTABLE();
            if (kodGred != null)
            {
                gred = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            }
            else
            {
                gred = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == kodGred2);
            }
            
            if(gred == null)
            {
                gred = new GE_PARAMTABLE();
            }
            return gred;
        }

        public HR_MATRIKS_GAJI cariMatriks(string gred, string matrik, decimal? gaji)
        {
            int? peringkat = null;
            if (matrik != null)
            {
                matrik = matrik.Trim();

                if (matrik.Substring(0, 1) == "P" && matrik.Length >= 2)
                {
                    peringkat = Convert.ToInt32(matrik.Substring(1, 1));
                }
            }

            HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == gred && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gaji).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
            if (matriks == null)
            {
                matriks = new HR_MATRIKS_GAJI();
                matriks.HR_GAJI_MIN = 0;
                matriks.HR_GAJI_MAX = 0;
                matriks.HR_GAJI_POKOK = 0;
                matriks.HR_RM_KENAIKAN = 0;
            }

            return matriks;
        }

        private ActionResult Muktamad(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, HR_MAKLUMAT_KEWANGAN8[][] sAnsuran, string Kod, IEnumerable<HR_POTONGAN> sPotongan, HR_GAJI_UPAHAN gajiUpah, HR_POTONGAN potongan2, HR_GAJI_UPAHAN tggkk, HR_MAKLUMAT_PERIBADI pekerja, HR_MAKLUMAT_PERIBADI userKeyin, DateTime tMula, int? PA_BULAN, short? PA_TAHUN)
        {
            HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(model.HR_NO_PEKERJA);

            List<PA_PELARASAN> padamPelarasan = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
            spg.PA_PELARASAN.RemoveRange(padamPelarasan);
            spg.SaveChanges();

            if (sAnsuran == null)
            {
                sAnsuran = new HR_MAKLUMAT_KEWANGAN8[1][];
                sAnsuran[0] = null;
            }

            if (sAnsuran.ElementAt(0) != null && (Kod == "00030" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")))
            {

                foreach (HR_MAKLUMAT_KEWANGAN8 ansuran in sAnsuran.ElementAt(0))
                {
                    HR_MAKLUMAT_KEWANGAN8 editModel = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_TARIKH_MULA == ansuran.HR_TARIKH_MULA);
                    editModel.HR_TARIKH_FINALISED_HR = DateTime.Now;
                    db.Entry(editModel).State = EntityState.Modified;
                    db.SaveChanges();

                    var no = 0;
                    foreach (HR_POTONGAN potongan in sPotongan)
                    {
                        if (potongan.HR_KOD_POTONGAN != null)
                        {
                            if (potongan.HR_NILAI == null)
                            {
                                potongan.HR_NILAI = 0;
                            }
                            for (var a = 0; a < sAnsuran.Count(); a++)
                            {
                                for (var b = 0; b < sAnsuran.ElementAt(a).Count(); b++)
                                {
                                    if (sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_MULA == ansuran.HR_TARIKH_MULA && sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_AKHIR == ansuran.HR_TARIKH_AKHIR && a == no)
                                    {
                                        potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));

                                        //if (no == 0)
                                        //{
                                        //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                                        //    if (potongan.HR_NILAI <= 0)
                                        //    {
                                        //        //potongan
                                        //        potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                                        //    }
                                        //    else
                                        //    {
                                        //        //tunggakan
                                        //        potongan.HR_KOD_POTONGAN = tggkk.HR_KOD_UPAH;
                                        //    }
                                        //}

                                        string jenis = null;
                                        string vot = null;
                                        string singkatan = null;
                                        string laporan = null;

                                        HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                                        if (salary != null)
                                        {
                                            jenis = "G";
                                            laporan = "G";
                                            singkatan = salary.HR_SINGKATAN;
                                            vot = salary.HR_VOT_UPAH;
                                        }
                                        else
                                        {
                                            HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                                            if (ellowance2 != null)
                                            {
                                                jenis = "E";

                                                singkatan = ellowance2.HR_SINGKATAN;
                                                string ekor = ellowance2.HR_VOT_ELAUN;
                                                if (ekor == null)
                                                {
                                                    ekor = "XXXXX";
                                                }
                                                ekor = ekor.PadRight(5, 'X');
                                                vot = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + ekor;
                                            }

                                            HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                                            if (deduction != null)
                                            {
                                                jenis = "P";
                                                singkatan = deduction.HR_SINGKATAN;
                                                if (pekerjaan.HR_KAKITANGAN_IND == "Y")
                                                {
                                                    vot = deduction.HR_VOT_POTONGAN;
                                                }
                                                else
                                                {
                                                    vot = deduction.HR_VOT_POTONGAN_P;
                                                }


                                                string kepala = "11";
                                                string ekor = "XXXXX";

                                                vot = vot.Replace("-", "");

                                                if (vot.ToCharArray().Count() > 2)
                                                {
                                                    ekor = vot.Substring(vot.Length - 5);
                                                }

                                                if (vot.ToCharArray().Count() >= 13 || vot.ToCharArray().Count() == 2)
                                                {
                                                    kepala = vot.Substring(0, 2);
                                                }

                                                if (kepala != "41" && kepala != "12")
                                                {
                                                    kepala = "11";
                                                }

                                                ekor = ekor.PadRight(5, 'X');

                                                vot = kepala + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + ekor;
                                            }

                                            List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN || s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN).ToList();
                                            if (ellowance.Count() > 0)
                                            {
                                                laporan = "E";
                                            }
                                            else
                                            {
                                                if (gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN)
                                                {
                                                    laporan = "P";
                                                }
                                                else
                                                {
                                                    laporan = "G";
                                                }
                                            }

                                        }

                                        //List<PA_PELARASAN> padamPelarasan = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == DateTime.Now && s.PA_BULAN == DateTime.Now.Month && s.PA_TAHUN == DateTime.Now.Year && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                                        //spg.PA_PELARASAN.RemoveRange(padamPelarasan);
                                        //spg.SaveChanges();

                                        PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(ansuran.HR_TARIKH_MULA.Month) && s.PA_TAHUN == Convert.ToInt16(ansuran.HR_TARIKH_MULA.Year) && s.PA_KOD_PELARASAN == potongan.HR_KOD_POTONGAN);
                                        PA_PELARASAN semakPelarasan = pelarasan;
                                        if (semakPelarasan == null)
                                        {
                                            pelarasan = new PA_PELARASAN();
                                            pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                                            pelarasan.PA_BULAN = Convert.ToInt32(ansuran.HR_TARIKH_MULA.Month);
                                            pelarasan.PA_TAHUN = Convert.ToInt16(ansuran.HR_TARIKH_MULA.Year);
                                            pelarasan.PA_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                        }

                                        pelarasan.PA_PERATUS = 0;
                                        if(Kod == "00030")
                                        {
                                            pelarasan.PA_NILAI = potongan.HR_NILAI / sAnsuran.ElementAt(0).Count();
                                        }
                                        else
                                        {
                                            pelarasan.PA_NILAI = sAnsuran.ElementAt(a).ElementAt(b).HR_BIL;
                                        }
                                        
                                        pelarasan.PA_NILAI_MAKSIMUM = 0;
                                        pelarasan.PA_NILAI_MINIMUM = 0;
                                        pelarasan.PA_JENIS_PELARASAN = jenis;

                                        DateTime tKeyIn = Convert.ToDateTime(ansuran.HR_TARIKH_MULA);

                                        pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                                        pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month, 1).AddMonths(1).AddDays(-1);

                                        pelarasan.PA_PROSES_IND = "T";

                                        pelarasan.PA_VOT_PELARASAN = vot;
                                        pelarasan.PA_SINGKATAN = singkatan;
                                        pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                                        pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                                        pelarasan.PA_LAPORAN_IND = laporan;
                                        pelarasan.HR_KEW8_ID = model.HR_KEW8_ID;

                                        if (semakPelarasan == null)
                                        {
                                            spg.PA_PELARASAN.Add(pelarasan);
                                        }
                                        else
                                        {
                                            spg.Entry(pelarasan).State = EntityState.Modified;
                                        }
                                        spg.SaveChanges();
                                    }
                                }
                            }
                        }
                        no++;
                    }
                }
            }
            else
            {
                HR_MAKLUMAT_KEWANGAN8 editModel = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA);
                model = new HR_MAKLUMAT_KEWANGAN8();
                model = editModel;
                model.HR_TARIKH_FINALISED_HR = DateTime.Now;
                model.HR_FINALISED_IND_HR = "Y";
                if (Kod == "00026")
                {
                    pekerjaan.HR_GAJI_IND = "Y";
                    if (pekerjaan.HR_STATUS_PENCEN != "Y")
                    {
                        pekerjaan.HR_STATUS_KWSP = "Y";
                    }
                    db.Entry(pekerjaan).State = EntityState.Modified;
                }

                if (Kod == "00025")
                {
                    pekerjaan.HR_GAJI_IND = "N";
                    db.Entry(pekerjaan).State = EntityState.Modified;
                }

                if (Kod == "TP")
                {
                    pekerjaan.HR_GAJI_IND = "T";
                    db.Entry(pekerjaan).State = EntityState.Modified;

                    HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Find(model.HR_NO_PEKERJA);
                    peribadi2.HR_AKTIF_IND = "T";
                    db.Entry(peribadi2).State = EntityState.Modified;
                }

                if (Kod == "00022")
                {
                    pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "Y";
                    db.Entry(pekerjaan).State = EntityState.Modified;
                }

                if (Kod == "00037")
                {
                    pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "T";
                    db.Entry(pekerjaan).State = EntityState.Modified;
                }

                if (Kod == "00036")
                {
                    pekerjaan.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                    pekerjaan.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                    pekerjaan.HR_GAJI_POKOK = modelDetail.HR_GAJI_BARU;
                    pekerjaan.HR_KOD_GAJI = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_KOD_GAJI;
                    pekerjaan.HR_SISTEM = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_SISTEM_SARAAN;

                    db.Entry(pekerjaan).State = EntityState.Modified;
                }

                if (Kod == "CUTI")
                {
                    if (model.HR_KOD_PERUBAHAN == "00018")
                    {
                        pekerjaan.HR_GAJI_IND = "T";
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                }

                if (Kod == "00024" || Kod == "00039")
                {
                    model.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_HR;
                    model.HR_TARIKH_FINALISED_PA = DateTime.Now;
                    model.HR_FINALISED_IND_PA = "Y";
                }

                if (Kod == "00001")
                {
                    DateTime? tarikhKenaikan = Convert.ToDateTime("01/" + model.HR_TARIKH_MULA.Month + "/" + DateTime.Now.Year);
                    pekerjaan.HR_BULAN_KENAIKAN_GAJI = tarikhKenaikan;
                    pekerjaan.HR_GRED = modelDetail.HR_GRED;
                    pekerjaan.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                    pekerjaan.HR_GAJI_POKOK = modelDetail.HR_GAJI_BARU;
                    db.Entry(pekerjaan).State = EntityState.Modified;
                    model.HR_UBAH_IND = "0";
                }

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039" || Kod == "00001" || Kod == "LNTKN" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                {
                    List<PA_PELARASAN> pelarasan2 = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == tMula && s.PA_BULAN == PA_BULAN && s.PA_TAHUN == PA_TAHUN && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                    spg.PA_PELARASAN.RemoveRange(pelarasan2);
                    spg.SaveChanges();

                    var no = 0;
                    foreach (HR_POTONGAN potongan in sPotongan)
                    {
                        if (potongan.HR_NILAI == null)
                        {
                            potongan.HR_NILAI = 0;
                        }
                        var potongan4 = potongan.HR_KOD_POTONGAN;
                        if (Kod == "00031")
                        {
                            potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                            potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                        }
                        else if (Kod == "00039" || Kod == "00024")
                        {
                            if (Kod == "00024")
                            {
                                HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                                if (cariElaun == null)
                                {
                                    cariElaun = new HR_ELAUN();
                                }
                                if (model.HR_KEW8_IND == "E")
                                {

                                    if (cariElaun.HR_KOD_TUNGGAKAN != null)
                                    {
                                        potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_TUNGGAKAN;
                                    }
                                    else
                                    {
                                        potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                    }
                                }
                                else
                                {
                                    if (cariElaun.HR_KOD_POTONGAN != null)
                                    {
                                        potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_POTONGAN;
                                    }
                                    else
                                    {
                                        potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                    }
                                }
                            }
                            else
                            {
                                HR_POTONGAN cariPotongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == modelDetail.HR_KOD_PELARASAN);
                                if (cariPotongan == null)
                                {
                                    cariPotongan = new HR_POTONGAN();
                                }
                                potongan.HR_KOD_POTONGAN = cariPotongan.HR_KOD_POTONGAN;
                            }


                            potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                        }
                        else
                        {
                            HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                            if (cariElaun == null)
                            {
                                cariElaun = new HR_ELAUN();
                            }
                            DateTime xKeyInDate = new DateTime(Convert.ToDateTime(model.HR_TARIKH_MULA).Year, Convert.ToDateTime(model.HR_TARIKH_KEYIN).Month, 1);
                            if (Kod == "00036" || (Kod == "00026" && model.HR_KEW8_IND == "T") || Kod == "00001" || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T") || sPotongan.ElementAt(0).HR_KOD_POTONGAN == tggkk.HR_KOD_UPAH || ((Kod == "TMK" || Kod == "00032" || Kod == "00004") && model.HR_TARIKH_MULA <= xKeyInDate))
                            {
                                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                if (cariElaun.HR_KOD_TUNGGAKAN != null)
                                {
                                    potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_TUNGGAKAN;
                                }
                                //else
                                //{
                                //    potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                //}
                            }
                            else
                            {
                                potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                if (cariElaun.HR_KOD_POTONGAN != null)
                                {
                                    potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_POTONGAN;
                                }
                                //else
                                //{
                                //    potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                //}
                            }
                        }

                        //if (no == 0 && Kod != "00031")
                        //{
                        //    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                        //}

                        // start 24/7/2018
                        //if (no == 0 && (Kod != "00031" && Kod != "00039" && Kod != "00024"))
                        //{
                        //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                        //    if (potongan.HR_NILAI <= 0)
                        //    {
                        //        //potongan
                        //        potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                        //    }
                        //    else
                        //    {
                        //        //tunggakan
                        //        potongan.HR_KOD_POTONGAN = tggkk.HR_KOD_UPAH;
                        //    }
                        //}
                        // end 24/7/2018

                        //HR_POTONGAN pot = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                        //if (pot == null)
                        //{
                        //    pot = new HR_POTONGAN();
                        //}

                        //HR_GAJI_UPAHAN gUpah = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                        //if(gUpah == null)
                        //{
                        //    gUpah = new HR_GAJI_UPAHAN();
                        //}



                        string jenis = null;
                        string vot = null;
                        string singkatan = null;
                        string laporan = null;
                        //HR_MAKLUMAT_ELAUN_POTONGAN potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E" && db.HR_ELAUN.Where(e => e.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN && e.HR_KOD_ELAUN == s.HR_KOD_ELAUN_POTONGAN).Count() > 0);
                        //if (potonganInd == null)
                        //{
                        //    potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                        //    potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G" && db.HR_GAJI_UPAHAN.Where(e => e.HR_KOD_UPAH == s.HR_KOD_ELAUN_POTONGAN && e.HR_VOT_UPAH == pot.HR_VOT_POTONGAN).Count() > 0);
                        //    if (potonganInd == null)
                        //    {
                        //        potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                        //    }
                        //}

                        HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                        if (salary != null)
                        {
                            jenis = "G";
                            laporan = "G";
                            singkatan = salary.HR_SINGKATAN;
                            vot = salary.HR_VOT_UPAH;
                        }
                        else
                        {
                            HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                            if (ellowance2 != null)
                            {
                                jenis = "E";

                                singkatan = ellowance2.HR_SINGKATAN;
                                string ekor = ellowance2.HR_VOT_ELAUN;
                                if (ekor == null)
                                {
                                    ekor = "XXXXX";
                                }
                                ekor = ekor.PadRight(5, 'X');
                                vot = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + ekor;
                            }

                            HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                            if (deduction != null)
                            {
                                jenis = "P";
                                singkatan = deduction.HR_SINGKATAN;

                                if (pekerjaan.HR_KAKITANGAN_IND == "Y")
                                {
                                    vot = deduction.HR_VOT_POTONGAN;
                                }
                                else
                                {
                                    vot = deduction.HR_VOT_POTONGAN_P;
                                }


                                string kepala = "11";
                                string ekor = "XXXXX";
                                if (vot != null)
                                {
                                    vot = vot.Replace("-", "");

                                    if (vot.ToCharArray().Count() > 2)
                                    {
                                        ekor = vot.Substring(vot.Length - 5);
                                    }

                                    if (vot.ToCharArray().Count() >= 13 || vot.ToCharArray().Count() == 2)
                                    {
                                        kepala = vot.Substring(0, 2);
                                    }
                                }

                                if (kepala != "41" && kepala != "12")
                                {
                                    kepala = "11";
                                }

                                ekor = ekor.PadRight(5, 'X');

                                vot = kepala + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + ekor;

                            }

                            List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN || s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN).ToList();
                            if (ellowance.Count() > 0)
                            {
                                laporan = "E";
                            }
                            else
                            {
                                if (gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN || Kod == "00036")
                                {
                                    laporan = "P";
                                }
                                else
                                {
                                    laporan = "G";
                                }
                            }

                        }


                        //if (Kod == "00031")
                        //{
                        //    jenis = "E";

                        //    singkatan = ellowance.HR_SINGKATAN;
                        //}
                        //else
                        //{  
                        //    pelarasan.PA_JENIS_PELARASAN = "P";
                        //}

                        if (Kod == "00039" || Kod == "00024" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                        {
                            var KodPelarasan4 = modelDetail.HR_KOD_PELARASAN;
                            if (Kod == "TMK" || Kod == "00032" || Kod == "00004")
                            {
                                KodPelarasan4 = potongan4;
                            }
                            HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan4 = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == KodPelarasan4);
                            HR_MAKLUMAT_ELAUN_POTONGAN semakElaunPotongan4 = ElaunPotongan4;
                            var aktifind = "Y";
                            //if (model.HR_KEW8_IND == "E" && model.HR_TARIKH_MULA <= DateTime.Now && model.HR_TARIKH_AKHIR >= DateTime.Now)
                            //{
                            //    aktifind = "Y";
                            //}
                            if ((Kod == "00039" || Kod == "00024") && model.HR_KEW8_IND == "P")
                            {
                                aktifind = "T";
                            }

                            if (semakElaunPotongan4 == null)
                            {
                                ElaunPotongan4 = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                ElaunPotongan4.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                                ElaunPotongan4.HR_KOD_ELAUN_POTONGAN = KodPelarasan4;
                                ElaunPotongan4.HR_UBAH_IND = "B";
                                ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                                ElaunPotongan4.HR_NP_KEYIN = userKeyin.HR_NO_PEKERJA;
                            }

                            ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                            if (Kod == "00024" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                            {
                                ElaunPotongan4.HR_ELAUN_POTONGAN_IND = "E";
                            }
                            else
                            {
                                ElaunPotongan4.HR_ELAUN_POTONGAN_IND = "P";
                            }
                            ElaunPotongan4.HR_MOD_BAYARAN = "1";
                            ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                            ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                            ElaunPotongan4.HR_AKTIF_IND = aktifind;
                            ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                            ElaunPotongan4.HR_NP_UBAH = userKeyin.HR_NO_PEKERJA;
                            ElaunPotongan4.HR_AUTO_IND = "T";

                            if (semakElaunPotongan4 == null)
                            {
                                db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(ElaunPotongan4);
                            }
                            else
                            {
                                ElaunPotongan4.HR_UBAH_IND = "K";
                                db.Entry(ElaunPotongan4).State = EntityState.Modified;
                            }
                            db.SaveChanges();

                            //SejarahElaunPotongan(ElaunPotongan4, model);
                        }
                        PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(model.HR_BULAN) && s.PA_TAHUN == Convert.ToInt16(model.HR_TAHUN) && s.PA_KOD_PELARASAN == potongan.HR_KOD_POTONGAN);
                        PA_PELARASAN semakPelarasan = pelarasan;
                        if (semakPelarasan == null)
                        {
                            pelarasan = new PA_PELARASAN();
                            pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                            pelarasan.PA_BULAN = Convert.ToInt32(model.HR_BULAN);
                            pelarasan.PA_TAHUN = Convert.ToInt16(model.HR_TAHUN);
                            pelarasan.PA_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                        }

                        pelarasan.PA_PERATUS = 0;
                        pelarasan.PA_NILAI = potongan.HR_NILAI;
                        pelarasan.PA_NILAI_MAKSIMUM = 0;
                        pelarasan.PA_NILAI_MINIMUM = 0;
                        pelarasan.PA_JENIS_PELARASAN = jenis;

                        DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                        pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                        pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month, 1).AddMonths(1).AddDays(-1);

                        pelarasan.PA_PROSES_IND = "T";

                        pelarasan.PA_VOT_PELARASAN = vot;
                        pelarasan.PA_SINGKATAN = singkatan;
                        pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                        pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                        pelarasan.PA_LAPORAN_IND = laporan;
                        pelarasan.HR_KEW8_ID = model.HR_KEW8_ID;

                        if (semakPelarasan == null)
                        {
                            spg.PA_PELARASAN.Add(pelarasan);
                        }
                        else
                        {
                            spg.Entry(pelarasan).State = EntityState.Modified;
                        }
                        spg.SaveChanges();
                        no++;

                    }

                }
            }

            return null;
        }

        private ActionResult Muktamad2(List<HR_MAKLUMAT_KEWANGAN8> SMK8, List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D, string Kod, HR_GAJI_UPAHAN gajiUpah, HR_GAJI_UPAHAN tggkk, HR_POTONGAN ptgn)
        {
            DateTime crrnDate = DateTime.Now;
            
            foreach (HR_MAKLUMAT_KEWANGAN8 model in SMK8)
            {
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(model.HR_NO_PEKERJA);
                List<PA_PELARASAN> padamPelarasan = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == DateTime.Now && s.PA_BULAN == DateTime.Now.Month && s.PA_TAHUN == DateTime.Now.Year && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                spg.PA_PELARASAN.RemoveRange(padamPelarasan);
                spg.SaveChanges();

                var endOfDayInMonth = new DateTime(model.HR_TARIKH_MULA.Year, model.HR_TARIKH_MULA.Month, 1).AddMonths(1).AddDays(-1);

                //HR_MAKLUMAT_KEWANGAN8 editModel = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_TARIKH_MULA == ansuran.HR_TARIKH_MULA);
                //editModel.HR_TARIKH_FINALISED_HR = DateTime.Now;
                //db.Entry(editModel).State = EntityState.Modified;
                //db.SaveChanges();

                HR_MAKLUMAT_KEWANGAN8 Q8 = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID);
                if(Q8 != null)
                {
                    HR_MAKLUMAT_KEWANGAN8_DETAIL Q8D = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID);
                    if(Q8D == null)
                    {
                        Q8D = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                    }

                    string gred = null;
                    if(Q8D.HR_GRED != null)
                    {
                        gred = Q8D.HR_GRED.Trim();
                    }

                    Q8.HR_TARIKH_FINALISED_HR = DateTime.Now;
                    Q8.HR_FINALISED_IND_HR = "Y";

                    if (Kod == "00026")
                    {
                        pekerjaan.HR_GAJI_IND = "Y";
                        if (pekerjaan.HR_STATUS_PENCEN != "Y")
                        {
                            pekerjaan.HR_STATUS_KWSP = "Y";
                        }

                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00025")
                    {
                        pekerjaan.HR_GAJI_IND = "N";
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "TP")
                    {
                        if (crrnDate >= endOfDayInMonth)
                        {
                            pekerjaan.HR_GAJI_IND = "T";
                        }
                            
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        if(pekerjaan.HR_TARIKH_TAMAT != null)
                        {
                            pekerjaan.HR_TARIKH_TAMAT = model.HR_TARIKH_MULA;
                        }
                        else
                        {
                            pekerjaan.HR_TARIKH_TAMAT_KONTRAK = model.HR_TARIKH_MULA;
                        }
                        db.Entry(pekerjaan).State = EntityState.Modified;

                        HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Find(model.HR_NO_PEKERJA);
                        if (crrnDate >= endOfDayInMonth)
                        {
                            peribadi2.HR_AKTIF_IND = "T";
                        }
                            
                        peribadi2.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        peribadi2.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        db.Entry(peribadi2).State = EntityState.Modified;
                    }

                    if (Kod == "00022")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "Y";
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_BULAN_KENAIKAN_GAJI = model.HR_TARIKH_AKHIR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00037")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "T";
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "CUTI")
                    {
                        if (model.HR_KOD_PERUBAHAN == "00018")
                        {
                            pekerjaan.HR_GAJI_IND = "T";
                            pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                            pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                            db.Entry(pekerjaan).State = EntityState.Modified;
                        }

                    }

                    if (Kod == "00024" || Kod == "00039")
                    {
                        Q8.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_HR;
                        Q8.HR_TARIKH_FINALISED_PA = DateTime.Now;
                        Q8.HR_FINALISED_IND_PA = "Y";
                    }
                    if (SMK8D.Count() > 0)
                    {
                        if (Kod == "00036")
                        {
                            pekerjaan.HR_GRED = gred;
                            pekerjaan.HR_MATRIKS_GAJI = Q8D.HR_MATRIKS_GAJI;
                            pekerjaan.HR_GAJI_POKOK = Q8D.HR_GAJI_BARU;
                            pekerjaan.HR_KOD_GAJI = cariMatriks(Q8D.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_KOD_GAJI;
                            pekerjaan.HR_SISTEM = cariMatriks(Q8D.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_SISTEM_SARAAN;
                            pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                            pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                            db.Entry(pekerjaan).State = EntityState.Modified;
                        }

                        if (Kod == "00001")
                        {
                            DateTime? tarikhKenaikan = Convert.ToDateTime("01/" + model.HR_TARIKH_MULA.Month + "/" + DateTime.Now.AddYears(1).Year);
                            pekerjaan.HR_BULAN_KENAIKAN_GAJI = Convert.ToDateTime(tarikhKenaikan);
                            pekerjaan.HR_GRED = gred;
                            pekerjaan.HR_MATRIKS_GAJI = Q8D.HR_MATRIKS_GAJI;
                            pekerjaan.HR_GAJI_POKOK = Q8D.HR_GAJI_BARU;
                            pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                            pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                            db.Entry(pekerjaan).State = EntityState.Modified;
                            db.SaveChanges();
                            Q8.HR_UBAH_IND = "0";
                        }
                    }

                    db.Entry(Q8).State = EntityState.Modified;
                    db.SaveChanges();

                    if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039" || Kod == "00001" || Kod == "LNTKN" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                    {

                        HR_MAKLUMAT_KEWANGAN8_DETAIL CariKod = SMK8D.FirstOrDefault(s => s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH || s.HR_KOD_PELARASAN == ptgn.HR_KOD_POTONGAN);
                        if (CariKod == null)
                        {
                            CariKod = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                        }
 
                        if (Kod == "TP" || Kod == "00015")
                        {
                            List<HR_MAKLUMAT_ELAUN_POTONGAN> cariElaun2 = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => (Kod != "00015" && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA) || (Kod == "00015" && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_AKTIF_IND == "Y")).ToList();
                            foreach (HR_MAKLUMAT_ELAUN_POTONGAN el in cariElaun2)
                            {
                                el.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;

                                if (crrnDate >= endOfDayInMonth && Kod != "00015")
                                {
                                    el.HR_AKTIF_IND = "T";
                                }

                                el.HR_TARIKH_UBAH = DateTime.Now;
                                el.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                                db.Entry(el).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        

                        var KodPelarasanGaji = CariKod.HR_KOD_PELARASAN;
                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> HRMK8D = SMK8D.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                        foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail in HRMK8D)
                        {
                            var KodPelarasan = modelDetail.HR_KOD_PELARASAN;
                            //if (Kod == "00039" || Kod == "00024")
                            //{
                            //    if (Kod == "00024")
                            //    {
                            //        HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                            //        if (cariElaun == null)
                            //        {
                            //            cariElaun = new HR_ELAUN();
                            //        }
                            //        if (model.HR_KEW8_IND == "E")
                            //        {
                            //            if (cariElaun.HR_KOD_TUNGGAKAN != null)
                            //            {
                            //                KodPelarasan = cariElaun.HR_KOD_TUNGGAKAN;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            if (cariElaun.HR_KOD_POTONGAN != null)
                            //            {
                            //                KodPelarasan = cariElaun.HR_KOD_POTONGAN;
                            //            }

                            //        }
                            //    }
                            //    else
                            //    {
                            //        HR_POTONGAN cariPotongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == modelDetail.HR_KOD_PELARASAN);
                            //        if (cariPotongan == null)
                            //        {
                            //            cariPotongan = new HR_POTONGAN();
                            //        }
                            //        KodPelarasan = cariPotongan.HR_KOD_POTONGAN;
                            //    }

                            //}
                            //else
                            //{
                            //    HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                            //    if (cariElaun == null)
                            //    {
                            //        cariElaun = new HR_ELAUN();
                            //    }
                            //    DateTime xKeyInDate = new DateTime(Convert.ToDateTime(model.HR_TARIKH_MULA).Year, Convert.ToDateTime(model.HR_TARIKH_KEYIN).Month, 1);
                            //    if (Kod == "00036" || Kod == "00031" || (Kod == "00026" && model.HR_KEW8_IND == "T") || Kod == "00001" || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T") || KodPelarasanGaji == tggkk.HR_KOD_UPAH || ((Kod == "TMK" || Kod == "00032" || Kod == "00004") && model.HR_TARIKH_MULA <= xKeyInDate))
                            //    {
                            //        if (cariElaun.HR_KOD_TUNGGAKAN != null)
                            //        {
                            //            KodPelarasan = cariElaun.HR_KOD_TUNGGAKAN;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (cariElaun.HR_KOD_POTONGAN != null)
                            //        {
                            //            KodPelarasan = cariElaun.HR_KOD_POTONGAN;
                            //        }
                            //    }
                            //}

                            string jenis = null;
                            string vot = null;
                            string singkatan = null;
                            string laporan = null;

                            DateTime tKeyIn = DateTime.Now;
                            DateTime xKeyInDate = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);

                            HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == KodPelarasan);
                            if (salary != null)
                            {
                                jenis = "G";
                                laporan = "G";
                                singkatan = salary.HR_SINGKATAN;
                                vot = salary.HR_VOT_UPAH;
                            }
                            else
                            {
                                HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == KodPelarasan);
                                if (ellowance2 != null)
                                {
                                    jenis = "E";
                                    singkatan = ellowance2.HR_SINGKATAN;

                                    if (Kod == "00036" || (Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T") || modelDetail.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH || ((Kod == "TMK" || Kod == "00032" || Kod == "00004") && model.HR_TARIKH_MULA <= xKeyInDate))
                                    {
                                        var kodTunggakan = ellowance2.HR_KOD_TUNGGAKAN;
                                        if(kodTunggakan != null)
                                        {
                                            HR_ELAUN ellowance3 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == kodTunggakan);
                                            if (ellowance3 != null)
                                            {
                                                KodPelarasan = kodTunggakan;
                                                singkatan = ellowance3.HR_SINGKATAN;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var kodPotongan = ellowance2.HR_KOD_POTONGAN;
                                        if (kodPotongan != null)
                                        {
                                            HR_ELAUN ellowance3 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == kodPotongan);
                                            if (ellowance3 != null)
                                            {
                                                KodPelarasan = kodPotongan;
                                                singkatan = ellowance3.HR_SINGKATAN;
                                            }
                                        }
                                    }

                                    string ekor = ellowance2.HR_VOT_ELAUN;
                                    if (ekor == null)
                                    {
                                        ekor = "XXXXX";
                                    }
                                    ekor = ekor.PadRight(5, 'X');
                                    vot = "11-" + pekerjaan.HR_JABATAN + "-" + pekerjaan.HR_BAHAGIAN + "-" + pekerjaan.HR_UNIT + "-" + ekor;
                                }

                                HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == KodPelarasan);
                                if (deduction != null)
                                {
                                    jenis = "P";
                                    singkatan = deduction.HR_SINGKATAN;

                                    if (pekerjaan.HR_KAKITANGAN_IND == "Y")
                                    {
                                        vot = deduction.HR_VOT_POTONGAN;
                                    }
                                    else
                                    {
                                        vot = deduction.HR_VOT_POTONGAN_P;
                                    }


                                    string kepala = "11";
                                    string ekor = "XXXXX";
                                    if(vot != null)
                                    {
                                        vot = vot.Replace("-", "");

                                        if (vot.ToCharArray().Count() > 2)
                                        {
                                            ekor = vot.Substring(vot.Length - 5);
                                        }

                                        if (vot.ToCharArray().Count() >= 13 || vot.ToCharArray().Count() == 2)
                                        {
                                            kepala = vot.Substring(0, 2);
                                        }
                                    }

                                    if (kepala != "41" && kepala != "12")
                                    {
                                        kepala = "11";
                                    }

                                    ekor = ekor.PadRight(5, 'X');

                                    vot = kepala + "-" + pekerjaan.HR_JABATAN + "-" + pekerjaan.HR_BAHAGIAN + "-" + pekerjaan.HR_UNIT + "-" + ekor;

                                }

                                List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == KodPelarasan || s.HR_KOD_POTONGAN == KodPelarasan).ToList();
                                if (ellowance.Count() > 0)
                                {
                                    laporan = "E";
                                }
                                else
                                {
                                    if (gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN || Kod == "00036")
                                    {
                                        laporan = "P";
                                    }
                                    else
                                    {
                                        laporan = "G";
                                    }
                                }

                            }

                            //List<PA_PELARASAN> padamPelarasan = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == DateTime.Now && s.PA_BULAN == DateTime.Now.Month && s.PA_TAHUN == DateTime.Now.Year && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                            //spg.PA_PELARASAN.RemoveRange(padamPelarasan);
                            //spg.SaveChanges();

                            if (Kod == "00039" || Kod == "00024" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                            {
                                var KodElaun = modelDetail.HR_KOD_PELARASAN;

                                HR_POTONGAN cariPGaji = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_KOD_POTONGAN == KodElaun);
                                if (cariPGaji == null || Kod == "00015")
                                {
                                    if(cariPGaji == null)
                                    {
                                        var cariElaun = db.HR_MAKLUMAT_ELAUN_POTONGAN.Join(db.HR_ELAUN, HR_MAKLUMAT_ELAUN_POTONGAN => HR_MAKLUMAT_ELAUN_POTONGAN.HR_KOD_ELAUN_POTONGAN, HR_ELAUN => HR_ELAUN.HR_KOD_ELAUN, (HR_MAKLUMAT_ELAUN_POTONGAN, HR_ELAUN) => new { HR_MAKLUMAT_ELAUN_POTONGAN, HR_ELAUN }).FirstOrDefault(s => s.HR_MAKLUMAT_ELAUN_POTONGAN.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_MAKLUMAT_ELAUN_POTONGAN.HR_AKTIF_IND == "Y" && s.HR_ELAUN.HR_KOD_POTONGAN == modelDetail.HR_KOD_PELARASAN || s.HR_ELAUN.HR_KOD_TUNGGAKAN == modelDetail.HR_KOD_PELARASAN);
                                        if (cariElaun != null)
                                        {
                                            KodElaun = cariElaun.HR_MAKLUMAT_ELAUN_POTONGAN.HR_KOD_ELAUN_POTONGAN;
                                        }
                                    }
                                    else
                                    {
                                        var cariElaun = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G");
                                        if (cariElaun != null)
                                        {
                                            KodElaun = cariElaun.HR_KOD_ELAUN_POTONGAN;
                                        }
                                    }

                                    HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan4 = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == KodElaun);
                                    HR_MAKLUMAT_ELAUN_POTONGAN semakElaunPotongan4 = ElaunPotongan4;
                                    var aktifind = "Y";
                                    //if (model.HR_KEW8_IND == "E" && model.HR_TARIKH_MULA <= DateTime.Now && model.HR_TARIKH_AKHIR >= DateTime.Now)
                                    //{
                                    //    aktifind = "Y";
                                    //}
                                    if ((((Kod == "00039" || Kod == "00024") && model.HR_KEW8_IND == "P") || Kod == "TP") && model.HR_TARIKH_MULA >= crrnDate && model.HR_TARIKH_AKHIR <= crrnDate)
                                    {
                                        aktifind = "T";
                                    }

                                    if (semakElaunPotongan4 == null)
                                    {
                                        ElaunPotongan4 = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                        ElaunPotongan4.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                                        ElaunPotongan4.HR_KOD_ELAUN_POTONGAN = KodElaun;
                                        ElaunPotongan4.HR_UBAH_IND = "B";
                                        ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                                        ElaunPotongan4.HR_NP_KEYIN = model.HR_NP_UBAH_HR;
                                    }

                                    if(Kod != "00015")
                                    {
                                        if(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN == null)
                                        {
                                            modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                                        }
                                        ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    }
                                    
                                    //if (Kod == "00024" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                                    //{
                                    //    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = "E";
                                    //}
                                    //else
                                    //{
                                    //    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = "P";
                                    //}

                                    
                                    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = KodElaun.Substring(0, 1);
                                    ElaunPotongan4.HR_MOD_BAYARAN = "1";
                                    if(Kod != "00015")
                                    {
                                        ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                                    }
                                    
                                    ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                                    ElaunPotongan4.HR_AKTIF_IND = aktifind;
                                    ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                                    ElaunPotongan4.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                                    ElaunPotongan4.HR_AUTO_IND = "T";
                                    

                                    if (semakElaunPotongan4 == null)
                                    {
                                        db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(ElaunPotongan4);
                                    }
                                    else
                                    {
                                        ElaunPotongan4.HR_UBAH_IND = "K";
                                        db.Entry(ElaunPotongan4).State = EntityState.Modified;
                                    }
                                    db.SaveChanges();
                                }
                                

                                //SejarahElaunPotongan(ElaunPotongan4, model);
                            }

                            

                            PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(model.HR_TARIKH_MULA.Month) && s.PA_TAHUN == Convert.ToInt16(model.HR_TARIKH_MULA.Year) && s.PA_KOD_PELARASAN == modelDetail.HR_KOD_PELARASAN);
                            PA_PELARASAN semakPelarasan = pelarasan;

                            if (semakPelarasan == null)
                            {
                                pelarasan = new PA_PELARASAN();
                                pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                                pelarasan.PA_BULAN = Convert.ToInt32(tKeyIn.Month);
                                pelarasan.PA_TAHUN = Convert.ToInt16(tKeyIn.Year);
                                pelarasan.PA_KOD_PELARASAN = KodPelarasan;
                            }

                            pelarasan.PA_PERATUS = 0;

                            HR_ELAUN elaunWil = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                            HR_ELAUN elaunKal = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);

                            if (elaunWil != null && Kod == "00001")
                            {
                                 pelarasan.PA_NILAI = modelDetail.HR_PERGERAKAN_EWIL;
                            }
                            else if (elaunKal != null && Kod == "00001")
                            {
                                pelarasan.PA_NILAI = modelDetail.HR_PERGERAKAN_EKAL;
                            }
                            else
                            {
                                pelarasan.PA_NILAI = modelDetail.HR_JUMLAH_PERUBAHAN;
                            }
                            
                            pelarasan.PA_NILAI_MAKSIMUM = 0;
                            pelarasan.PA_NILAI_MINIMUM = 0;
                            pelarasan.PA_JENIS_PELARASAN = jenis;

                            

                            pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                            pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month, 1).AddMonths(1).AddDays(-1);

                            pelarasan.PA_PROSES_IND = "T";

                            //if(Kod == "TP")
                            //{
                            //    pelarasan.PA_PROSES_IND = "T";
                            //}

                            pelarasan.PA_VOT_PELARASAN = vot;
                            pelarasan.PA_SINGKATAN = singkatan;
                            pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                            pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                            pelarasan.PA_LAPORAN_IND = laporan;
                            pelarasan.HR_KEW8_ID = model.HR_KEW8_ID;

                            if(pelarasan.PA_NILAI > 0 || pelarasan.PA_NILAI < 0)
                            {
                                if (semakPelarasan == null)
                                {
                                    spg.PA_PELARASAN.Add(pelarasan);
                                }
                                else
                                {
                                    spg.Entry(pelarasan).State = EntityState.Modified;
                                }
                                spg.SaveChanges();
                            }
                        }
                    }
                }

                
            }
            return null;
        }

        //public ActionResult SejarahElaunPotongan(HR_MAKLUMAT_ELAUN_POTONGAN model, HR_MAKLUMAT_KEWANGAN8 kew8)
        //{
        //    HR_SEJARAH_ELAUN_POTONGAN sejarah = new HR_SEJARAH_ELAUN_POTONGAN();
        //    sejarah.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
        //    sejarah.HR_KOD_ELAUN_POTONGAN = model.HR_KOD_ELAUN_POTONGAN;
        //    sejarah.HR_PENERANGAN = model.HR_PENERANGAN;
        //    sejarah.HR_NO_FAIL = model.HR_NO_FAIL;
        //    sejarah.HR_JUMLAH = model.HR_JUMLAH;
        //    sejarah.HR_ELAUN_POTONGAN_IND = model.HR_ELAUN_POTONGAN_IND;
        //    sejarah.HR_MOD_BAYARAN = model.HR_MOD_BAYARAN;
        //    sejarah.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
        //    sejarah.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
        //    sejarah.HR_TUNTUTAN_MAKSIMA = model.HR_TUNTUTAN_MAKSIMA;
        //    sejarah.HR_BAKI = model.HR_BAKI;
        //    sejarah.HR_AKTIF_IND = model.HR_AKTIF_IND;
        //    sejarah.HR_HARI_BEKERJA = model.HR_HARI_BEKERJA;
        //    sejarah.HR_NO_PEKERJA_PT = model.HR_NO_PEKERJA_PT;
        //    sejarah.HR_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
        //    sejarah.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH;
        //    sejarah.HR_UBAH_IND = model.HR_UBAH_IND;
        //    sejarah.HR_GRED_PT = model.HR_GRED_PT;
        //    sejarah.HR_MATRIKS_GAJI_PT = model.HR_MATRIKS_GAJI_PT;
        //    sejarah.HR_NP_KEYIN = model.HR_NP_KEYIN;
        //    sejarah.HR_NP_UBAH = model.HR_NP_UBAH;
        //    sejarah.HR_AUTO_IND = model.HR_AUTO_IND;
        //    db.HR_SEJARAH_ELAUN_POTONGAN.Add(sejarah);
        //    db.SaveChanges();

        //    return null;
        //}

        public ActionResult PadamMuktamad(HR_MAKLUMAT_KEWANGAN8 model, List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail, string Kod)
        {
            if(model.HR_FINALISED_IND_HR == "Y")
            {
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(model.HR_NO_PEKERJA);

                HR_SEJARAH_PEKERJAAN sejarahPekerja = db.HR_SEJARAH_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_TINDAKAN != "P").OrderByDescending(s => s.HR_ID_SEJARAH).FirstOrDefault();
                if(Kod == "00001")
                {
                    sejarahPekerja = new HR_SEJARAH_PEKERJAAN();
                    sejarahPekerja = db.HR_SEJARAH_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_TINDAKAN != "P" && s.HR_GAJI_POKOK < pekerjaan.HR_GAJI_POKOK).OrderByDescending(s => s.HR_GAJI_POKOK).ThenByDescending(s => s.HR_ID_SEJARAH).FirstOrDefault();
                    if(sejarahPekerja == null)
                    {
                        sejarahPekerja = new HR_SEJARAH_PEKERJAAN();
                        sejarahPekerja = db.HR_SEJARAH_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_TINDAKAN != "P" && s.HR_GAJI_POKOK <= pekerjaan.HR_GAJI_POKOK).OrderByDescending(s => s.HR_GAJI_POKOK).ThenByDescending(s => s.HR_ID_SEJARAH).FirstOrDefault();
                    }
                }
                if (sejarahPekerja == null)
                {
                    sejarahPekerja = new HR_SEJARAH_PEKERJAAN();
                }

                //HR_SEJARAH_PERIBADI sejarahPeribadi = db.HR_SEJARAH_PERIBADI.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_TINDAKAN != "P").OrderByDescending(s => s.HR_ID_SEJARAH).FirstOrDefault();
                HR_SEJARAH_PERIBADI sejarahPeribadi = null;
                if (sejarahPeribadi == null)
                {
                    sejarahPeribadi = new HR_SEJARAH_PERIBADI();
                }

                if (sejarahPekerja.HR_NO_PEKERJA != null)
                {
                    if (Kod == "00026")
                    {
                        pekerjaan.HR_GAJI_IND = sejarahPekerja.HR_GAJI_IND;
                        pekerjaan.HR_STATUS_PENCEN = sejarahPekerja.HR_STATUS_PENCEN;
                        pekerjaan.HR_STATUS_KWSP = sejarahPekerja.HR_STATUS_KWSP;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00025")
                    {
                        pekerjaan.HR_GAJI_IND = sejarahPekerja.HR_GAJI_IND;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = DateTime.Now;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00022")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = sejarahPekerja.HR_TANGGUH_GERAKGAJI_IND;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        pekerjaan.HR_BULAN_KENAIKAN_GAJI = sejarahPekerja.HR_BULAN_KENAIKAN_GAJI;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00037")
                    {
                        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = sejarahPekerja.HR_TANGGUH_GERAKGAJI_IND;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "00036")
                    {
                        pekerjaan.HR_GRED = sejarahPekerja.HR_GRED;
                        pekerjaan.HR_MATRIKS_GAJI = sejarahPekerja.HR_MATRIKS_GAJI;
                        pekerjaan.HR_GAJI_POKOK = sejarahPekerja.HR_GAJI_POKOK;
                        pekerjaan.HR_KOD_GAJI = cariMatriks(sejarahPekerja.HR_GRED, sejarahPekerja.HR_MATRIKS_GAJI, sejarahPekerja.HR_GAJI_POKOK).HR_KOD_GAJI;
                        pekerjaan.HR_SISTEM = cariMatriks(sejarahPekerja.HR_GRED, sejarahPekerja.HR_MATRIKS_GAJI, sejarahPekerja.HR_GAJI_POKOK).HR_SISTEM_SARAAN;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (Kod == "CUTI")
                    {
                        if (model.HR_KOD_PERUBAHAN == "00018")
                        {
                            pekerjaan.HR_GAJI_IND = sejarahPekerja.HR_GAJI_IND;
                            pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                            pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                            db.Entry(pekerjaan).State = EntityState.Modified;
                        }

                    }

                    if (Kod == "00001")
                    {
                        pekerjaan.HR_BULAN_KENAIKAN_GAJI = sejarahPekerja.HR_BULAN_KENAIKAN_GAJI;
                        pekerjaan.HR_GRED = sejarahPekerja.HR_GRED;
                        pekerjaan.HR_MATRIKS_GAJI = sejarahPekerja.HR_MATRIKS_GAJI;
                        pekerjaan.HR_GAJI_POKOK = sejarahPekerja.HR_GAJI_POKOK;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }
                }

                if (Kod == "TP")
                {
                    if (sejarahPekerja.HR_NO_PEKERJA != null)
                    {
                        pekerjaan.HR_GAJI_IND = sejarahPekerja.HR_GAJI_IND;
                        pekerjaan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        pekerjaan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        if(pekerjaan.HR_TARIKH_TAMAT != null)
                        {
                            pekerjaan.HR_TARIKH_TAMAT = sejarahPekerja.HR_TARIKH_TAMAT;
                        }
                        else
                        {
                            pekerjaan.HR_TARIKH_TAMAT_KONTRAK = sejarahPekerja.HR_TARIKH_TAMAT_KONTRAK;
                        }
                        db.Entry(pekerjaan).State = EntityState.Modified;
                    }

                    if (sejarahPeribadi.HR_NO_PEKERJA != null)
                    {
                        HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Find(model.HR_NO_PEKERJA);
                        peribadi2.HR_AKTIF_IND = sejarahPeribadi.HR_AKTIF_IND;
                        peribadi2.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                        peribadi2.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                        db.Entry(peribadi2).State = EntityState.Modified;
                    }
                }

                //if (Kod == "00024" || Kod == "00039")
                //{
                //    model.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_HR;
                //    model.HR_TARIKH_FINALISED_PA = DateTime.Now;
                //    model.HR_FINALISED_IND_PA = "T";
                //}

                db.SaveChanges();

                List<PA_PELARASAN> padamPelarasan = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                spg.PA_PELARASAN.RemoveRange(padamPelarasan);
                spg.SaveChanges();

                if (Kod == "TP")
                {
                    List<HR_MAKLUMAT_ELAUN_POTONGAN> cariElaun2 = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
                    foreach (HR_MAKLUMAT_ELAUN_POTONGAN el in cariElaun2)
                    {
                        HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == el.HR_KOD_ELAUN_POTONGAN);
                        HR_SEJARAH_ELAUN_POTONGAN sejarahElaun = db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == el.HR_KOD_ELAUN_POTONGAN && s.HR_TINDAKAN != "P").OrderByDescending(s => s.HR_ID_ELAUN_POTONGAN).FirstOrDefault();
                        if(sejarahElaun != null)
                        {
                            HR_SEJARAH_ELAUN_POTONGAN sejarah = sejarahElaun;
                            //ElaunPotongan.HR_NO_PEKERJA = sejarah.HR_NO_PEKERJA;
                            //ElaunPotongan.HR_KOD_ELAUN_POTONGAN = sejarah.HR_KOD_ELAUN_POTONGAN;
                            ElaunPotongan.HR_PENERANGAN = sejarah.HR_PENERANGAN;
                            ElaunPotongan.HR_NO_FAIL = sejarah.HR_NO_FAIL;
                            ElaunPotongan.HR_JUMLAH = sejarah.HR_JUMLAH;
                            ElaunPotongan.HR_ELAUN_POTONGAN_IND = sejarah.HR_ELAUN_POTONGAN_IND;
                            ElaunPotongan.HR_MOD_BAYARAN = sejarah.HR_MOD_BAYARAN;
                            ElaunPotongan.HR_TARIKH_MULA = sejarah.HR_TARIKH_MULA;
                            ElaunPotongan.HR_TARIKH_AKHIR = sejarah.HR_TARIKH_AKHIR;
                            ElaunPotongan.HR_TUNTUTAN_MAKSIMA = sejarah.HR_TUNTUTAN_MAKSIMA;
                            ElaunPotongan.HR_BAKI = sejarah.HR_BAKI;
                            ElaunPotongan.HR_AKTIF_IND = sejarah.HR_AKTIF_IND;
                            ElaunPotongan.HR_HARI_BEKERJA = sejarah.HR_HARI_BEKERJA;
                            ElaunPotongan.HR_NO_PEKERJA_PT = sejarah.HR_NO_PEKERJA_PT;
                            ElaunPotongan.HR_TARIKH_KEYIN = sejarah.HR_TARIKH_KEYIN;
                            ElaunPotongan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                            ElaunPotongan.HR_UBAH_IND = sejarah.HR_UBAH_IND;
                            ElaunPotongan.HR_GRED_PT = sejarah.HR_GRED_PT;
                            ElaunPotongan.HR_MATRIKS_GAJI_PT = sejarah.HR_MATRIKS_GAJI_PT;
                            ElaunPotongan.HR_NP_KEYIN = sejarah.HR_NP_KEYIN;
                            ElaunPotongan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                            ElaunPotongan.HR_AUTO_IND = sejarah.HR_AUTO_IND;
                            db.Entry(ElaunPotongan).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                }

                if (Kod == "00039" || Kod == "00024" || Kod == "TMK" || Kod == "00032" || Kod == "00004" || model.HR_KOD_PERUBAHAN == "00015")
                {
                    foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
                    {
                        HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == item.HR_KOD_ELAUN);
                        if (ElaunPotongan != null)
                        {
                            HR_SEJARAH_ELAUN_POTONGAN sejarahElaun = db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == item.HR_KOD_ELAUN && s.HR_TINDAKAN != "P").OrderByDescending(s => s.HR_ID_ELAUN_POTONGAN).FirstOrDefault();

                            if (sejarahElaun != null)
                            {
                                HR_SEJARAH_ELAUN_POTONGAN cariTambahElaun = sejarahElaun;
                                if (cariTambahElaun.HR_TINDAKAN == "T")
                                {
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Remove(ElaunPotongan);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    HR_SEJARAH_ELAUN_POTONGAN sejarah = sejarahElaun;
                                    //ElaunPotongan.HR_NO_PEKERJA = sejarah.HR_NO_PEKERJA;
                                    //ElaunPotongan.HR_KOD_ELAUN_POTONGAN = sejarah.HR_KOD_ELAUN_POTONGAN;
                                    ElaunPotongan.HR_PENERANGAN = sejarah.HR_PENERANGAN;
                                    ElaunPotongan.HR_NO_FAIL = sejarah.HR_NO_FAIL;
                                    ElaunPotongan.HR_JUMLAH = sejarah.HR_JUMLAH;
                                    ElaunPotongan.HR_ELAUN_POTONGAN_IND = sejarah.HR_ELAUN_POTONGAN_IND;
                                    ElaunPotongan.HR_MOD_BAYARAN = sejarah.HR_MOD_BAYARAN;
                                    ElaunPotongan.HR_TARIKH_MULA = sejarah.HR_TARIKH_MULA;
                                    ElaunPotongan.HR_TARIKH_AKHIR = sejarah.HR_TARIKH_AKHIR;
                                    ElaunPotongan.HR_TUNTUTAN_MAKSIMA = sejarah.HR_TUNTUTAN_MAKSIMA;
                                    ElaunPotongan.HR_BAKI = sejarah.HR_BAKI;
                                    ElaunPotongan.HR_AKTIF_IND = sejarah.HR_AKTIF_IND;
                                    ElaunPotongan.HR_HARI_BEKERJA = sejarah.HR_HARI_BEKERJA;
                                    ElaunPotongan.HR_NO_PEKERJA_PT = sejarah.HR_NO_PEKERJA_PT;
                                    ElaunPotongan.HR_TARIKH_KEYIN = sejarah.HR_TARIKH_KEYIN;
                                    ElaunPotongan.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH_HR;
                                    ElaunPotongan.HR_UBAH_IND = sejarah.HR_UBAH_IND;
                                    ElaunPotongan.HR_GRED_PT = sejarah.HR_GRED_PT;
                                    ElaunPotongan.HR_MATRIKS_GAJI_PT = sejarah.HR_MATRIKS_GAJI_PT;
                                    ElaunPotongan.HR_NP_KEYIN = sejarah.HR_NP_KEYIN;
                                    ElaunPotongan.HR_NP_UBAH = model.HR_NP_UBAH_HR;
                                    ElaunPotongan.HR_AUTO_IND = sejarah.HR_AUTO_IND;
                                    db.Entry(ElaunPotongan).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                //HR_SEJARAH_KEWANGAN8 sejarahKew8 = db.HR_SEJARAH_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == model.HR_TARIKH_MULA && s.HR_TINDAKAN != "P" && s.HR_FINALISED_IND_HR != "Y").OrderByDescending(s => s.HR_ID_SEJARAH).FirstOrDefault();
                HR_SEJARAH_KEWANGAN8 sejarahKew8 = null;

                if (sejarahKew8 != null)
                {
                    List<HR_MAKLUMAT_KEWANGAN8> senaraiQ8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == model.HR_ANSURAN_ID).ToList();
                    foreach(HR_MAKLUMAT_KEWANGAN8 Q8 in senaraiQ8)
                    {
                        HR_MAKLUMAT_KEWANGAN8 model2 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == Q8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == Q8.HR_TARIKH_MULA && s.HR_KEW8_ID == Q8.HR_KEW8_ID).FirstOrDefault();
                        if (model2 != null)
                        {
                            //model2.HR_TARIKH_AKHIR = sejarahKew8.HR_TARIKH_AKHIR;
                            //model2.HR_BULAN = sejarahKew8.HR_BULAN;
                            //model2.HR_TAHUN = sejarahKew8.HR_TAHUN;
                            //model2.HR_TARIKH_KEYIN = sejarahKew8.HR_TARIKH_KEYIN;
                            //model2.HR_BUTIR_PERUBAHAN = sejarahKew8.HR_BUTIR_PERUBAHAN;
                            //model2.HR_CATATAN = sejarahKew8.HR_CATATAN;
                            //model2.HR_NO_SURAT_KEBENARAN = sejarahKew8.HR_NO_SURAT_KEBENARAN;
                            //model2.HR_AKTIF_IND = sejarahKew8.HR_AKTIF_IND;
                            model2.HR_NP_UBAH_HR = model.HR_NP_UBAH_HR;
                            model2.HR_TARIKH_UBAH_HR = model.HR_TARIKH_UBAH_HR;
                            //model2.HR_NP_FINALISED_HR = sejarahKew8.HR_NP_FINALISED_HR;
                            model2.HR_TARIKH_FINALISED_HR = sejarahKew8.HR_TARIKH_FINALISED_HR;
                            model2.HR_FINALISED_IND_HR = sejarahKew8.HR_FINALISED_IND_HR;
                            model2.HR_UBAH_IND = sejarahKew8.HR_UBAH_IND;
                            //model2.HR_NP_UBAH_PA = sejarahKew8.HR_NP_UBAH_PA;
                            //model2.HR_TARIKH_UBAH_PA = sejarahKew8.HR_TARIKH_UBAH_PA;
                            //model2.HR_NP_FINALISED_PA = sejarahKew8.HR_NP_FINALISED_PA;
                            //model2.HR_TARIKH_FINALISED_PA = sejarahKew8.HR_TARIKH_FINALISED_PA;
                            //model2.HR_FINALISED_IND_PA = sejarahKew8.HR_FINALISED_IND_PA;
                            //model2.HR_EKA = sejarahKew8.HR_EKA;
                            //model2.HR_ITP = sejarahKew8.HR_ITP;
                            //model2.HR_KEW8_IND = sejarahKew8.HR_KEW8_IND;
                            //model2.HR_BIL = sejarahKew8.HR_BIL;
                            //model2.HR_KOD_JAWATAN = sejarahKew8.HR_KOD_JAWATAN;
                            //model2.HR_KEW8_ID = Convert.ToInt32(sejarahKew8.HR_KEW8_ID);
                            //model2.HR_ANSURAN_ID = sejarahKew8.HR_ANSURAN_ID;
                            //model2.HR_LANTIKAN_IND = sejarahKew8.HR_LANTIKAN_IND;
                            //model2.HR_TARIKH_SP = sejarahKew8.HR_TARIKH_SP;
                            //model2.HR_SP_IND = sejarahKew8.HR_SP_IND;
                            //model2.HR_JUMLAH_BULAN = sejarahKew8.HR_JUMLAH_BULAN;
                            //model2.HR_NILAI_EPF = sejarahKew8.HR_NILAI_EPF;
                            //model2.HR_GAJI_LAMA = sejarahKew8.HR_GAJI_LAMA;
                            //model2.HR_GRED_LAMA = sejarahKew8.HR_GRED_LAMA;
                            //model2.HR_MATRIKS_GAJI_LAMA = sejarahKew8.HR_MATRIKS_GAJI_LAMA;
                            db.Entry(model2).State = EntityState.Modified;
                        }
                    }
                }
                else
                {
                    List<HR_MAKLUMAT_KEWANGAN8> senaraiQ8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == model.HR_ANSURAN_ID).ToList();
                    foreach (HR_MAKLUMAT_KEWANGAN8 Q8 in senaraiQ8)
                    {
                        HR_MAKLUMAT_KEWANGAN8 model2 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == Q8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == Q8.HR_TARIKH_MULA && s.HR_KEW8_ID == Q8.HR_KEW8_ID).FirstOrDefault();
                        if (model2 != null)
                        {
                            model2.HR_NP_UBAH_HR = model.HR_NP_UBAH_HR;
                            model2.HR_TARIKH_UBAH_HR = model.HR_TARIKH_UBAH_HR;
                            //model2.HR_NP_FINALISED_HR = null;
                            model2.HR_TARIKH_FINALISED_HR = null;
                            model2.HR_FINALISED_IND_HR = "T";
                            model2.HR_UBAH_IND = "1";
                            db.Entry(model2).State = EntityState.Modified;
                        }
                    }
                        
                }

                db.SaveChanges();
            }
            return null;
        }

        public ActionResult TambahKew8(HR_MAKLUMAT_KEWANGAN8 model, string Kod)
        {
            model.HR_TARIKH_KEYIN = DateTime.Now;
            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if(kewangan8 != null)
            {
                model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                //ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }
            
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);

            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;
            ViewBag.HR_MATRIKS_GAJI_LAMA = mPekerjaan.HR_MATRIKS_GAJI;
            ViewBag.HR_SISTEM = mPekerjaan.HR_SISTEM;
            ViewBag.HR_TARIKH_MULA = null;
            ViewBag.HR_TARIKH_AKHIR = null;
            if (Kod == "CUTI")
            {
                ViewBag.HR_TARIKH_MULA = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }
            }

            if (Kod == "TP" || Kod == "00026")
            {
                if(Kod == "TP")
                {
                    ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
                }
                
                ViewBag.HR_TARIKH_TAMAT = null;
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if(mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}",mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }

            }

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);


            HR_MATRIKS_GAJI matriksPekerja = cariMatriks(cariGred(kodGred, null).SHORT_DESCRIPTION, mPekerjaan.HR_MATRIKS_GAJI, mPekerjaan.HR_GAJI_POKOK);

            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerja.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerja.HR_GAJI_MAX;
            ViewBag.HR_PGT_LAMA = matriksPekerja.HR_RM_KENAIKAN;
            ViewBag.kodGaji = matriksPekerja.HR_KOD_GAJI;

            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == kodGred);
            if (gred != null)
            {
                ViewBag.HR_GRED_LAMA = gred.SHORT_DESCRIPTION;
            }
            
            decimal? gaji = 0;
            if (mPekerjaan.HR_GAJI_POKOK != null)
            {
                gaji = mPekerjaan.HR_GAJI_POKOK;
            }
            ViewBag.HR_GAJI_LAMA = gaji;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (mPekerjaan.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + mPekerjaan.HR_TARAF_JAWATAN;
            }
            else if (mPekerjaan.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + mPekerjaan.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (gajiUpah == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }
            ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            float? HR_KRITIKAL = 0;
            float? HR_WILAYAH = 0;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun11 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0007" && s.HR_AKTIF_IND == "Y" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun11 != null)
                    {
                        HR_WILAYAH = Convert.ToSingle(item.HR_JUMLAH);
                    }
                    HR_ELAUN awam12 = db.HR_ELAUN.FirstOrDefault(s => s.HR_KOD_KATEGORI == "K0002" && s.HR_AKTIF_IND == "Y" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam12 != null)
                    {
                        HR_KRITIKAL = Convert.ToSingle(item.HR_JUMLAH);
                    }

                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_JAWATAN_IND == jawatan_ind && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        if (elaun4.HR_PERATUS_IND == "Y")
                        {
                            item.HR_JUMLAH = gaji * (item.HR_JUMLAH / 100);
                        }
                        elaun4.HR_NILAI = item.HR_JUMLAH;
                        elaun3.Add(elaun4);

                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);
                    }
                }

                ViewBag.elaun3 = elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList();

                ViewBag.potongan6 = db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList();

                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
  
            }
            if (Kod == "00001")
            {
                model.HR_TARIKH_MULA = Convert.ToDateTime(mPekerjaan.HR_BULAN_KENAIKAN_GAJI);
                ViewBag.HR_WILAYAH = HR_WILAYAH;
                ViewBag.HR_KRITIKAL = HR_KRITIKAL;
                ViewBag.HR_JENIS_PERGERAKAN = "D";
            }

            HR_MAKLUMAT_ELAUN_POTONGAN kodG = elaunPotongan.FirstOrDefault(s => s.HR_ELAUN_POTONGAN_IND == "G");
            if (kodG == null)
            {
                kodG = new HR_MAKLUMAT_ELAUN_POTONGAN();
            }
            ViewBag.kodG = kodG.HR_KOD_ELAUN_POTONGAN;

            ViewBag.HR_KAKITANGAN_IND = mPekerjaan.HR_KAKITANGAN_IND;

            ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_SINGKATAN == "EBGK").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", mPekerjaan.HR_KAKITANGAN_IND == "Y"? "E0069": "E0070");

            if (Kod == "00039")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            }

            if (Kod == "00024")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            model.HR_NP_FINALISED_HR = namaPegawai.HR_NO_PEKERJA;
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "00022" || Kod == "00037")
            {
                if(mPekerjaan.HR_BULAN_KENAIKAN_GAJI != null)
                {
                    model.HR_TARIKH_MULA = Convert.ToDateTime(mPekerjaan.HR_BULAN_KENAIKAN_GAJI);
                }
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim());
                if(jGaji != null)
                {
                    gredList.Add(sGred);
                }
                
            }
            ViewBag.gredList = gredList.OrderBy(s => s.SHORT_DESCRIPTION).ToList();

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if(transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            HR_PERATUS_KWSP PeratusKWSP = db.HR_PERATUS_KWSP.FirstOrDefault();
            if (PeratusKWSP == null)
            {
                PeratusKWSP = new HR_PERATUS_KWSP();
            }
            ViewBag.HR_KOD_PERATUS = PeratusKWSP.HR_KOD_PERATUS;
            ViewBag.HR_NILAI_PERATUS = PeratusKWSP.HR_NILAI_PERATUS;

            return PartialView("_TambahKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKew8([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND,HR_JUMLAH_BULAN,HR_NILAI_EPF,HR_GAJI_LAMA,HR_MATRIKS_GAJI_LAMA,HR_GRED_LAMA,HR_UBAH_IND,HR_ANSURAN_ID,HR_GAJI_MIN_BARU")] HR_MAKLUMAT_KEWANGAN8 model, [Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_KOD_PELARASAN,HR_MATRIKS_GAJI,HR_GRED,HR_JUMLAH_PERUBAHAN,HR_GAJI_BARU,HR_JENIS_PERGERAKAN,HR_JUMLAH_PERUBAHAN_ELAUN,HR_STATUS_IND,HR_ELAUN_KRITIKAL_BARU,HR_KEW8_ID,HR_NO_PEKERJA_PT,HR_PERGERAKAN_EKAL,HR_PERGERAKAN_EWIL,HR_GAJI_LAMA,HR_JAWATAN_BARU,HR_KOD_ELAUN")] HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, decimal? HR_JUMLAH_POTONGAN, IEnumerable<HR_POTONGAN> sPotongan, string Kod, DateTime? HR_TARIKH_TAMAT, HR_MAKLUMAT_KEWANGAN8[][] sAnsuran, HR_PERATUS_KWSP PeratusKWSP)
        {
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.Where(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y").FirstOrDefault();
            HR_MAKLUMAT_PERIBADI pekerja = mPeribadi.SingleOrDefault(s => s.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA == model.HR_NO_PEKERJA);

            List<HR_MAKLUMAT_KEWANGAN8> SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
            List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            if (ModelState.IsValid)
            {
                var jawatan_ind = "";
                if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    jawatan_ind = "K" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }
                else if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    jawatan_ind = "P" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }

                HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
                if (gajiUpah == null)
                {
                    gajiUpah = new HR_GAJI_UPAHAN();
                }

                HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
                if (gajiUpah == null)
                {
                    tggkk = new HR_GAJI_UPAHAN();
                }

                HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                if(potongan2 == null)
                {
                    potongan2 = new HR_POTONGAN();
                }
                var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                var incrementID = lastID.HR_KEW8_ID + 1;

                //var lastAnsuranID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_ANSURAN_ID).FirstOrDefault();
                var ansuranID = incrementID;

                if (sAnsuran == null)
                {
                    sAnsuran = new HR_MAKLUMAT_KEWANGAN8[1][];
                    sAnsuran[0] = null;
                }

                if (sAnsuran.ElementAt(0) == null)
                {
                    SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                    if (Kod == "00031")
                    {
                        var jumBulan = Convert.ToString(model.HR_JUMLAH_BULAN);
                        var EPF = Convert.ToString(model.HR_NILAI_EPF);
                        var bil = jumBulan + EPF;
                        model.HR_BIL = Convert.ToDecimal(bil);

                        db.Entry(PeratusKWSP).State = EntityState.Modified;
                    }

                    if (Kod == "TP")
                    {
                        model.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                        model.HR_TARIKH_AKHIR = Convert.ToDateTime(HR_TARIKH_TAMAT);
                        modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                    }

                    //model.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                    model.HR_BULAN = DateTime.Now.Month;
                    model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                    model.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    model.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    //HR_JAWATAN jwt4 = db.HR_JAWATAN.SingleOrDefault(s => s.HR_NAMA_JAWATAN == model.HR_KOD_JAWATAN);
                    //if (jwt4 == null)
                    //{
                    //    jwt4 = new HR_JAWATAN();
                    //}
                    //model.HR_KOD_JAWATAN = jwt4.HR_KOD_JAWATAN;

                    //if(model.HR_TARIKH_MULA == null)
                    //{
                    //    model.HR_TARIKH_MULA = DateTime.Now;
                    //}

                    model.HR_TARIKH_KEYIN = DateTime.Now;
                    //if(model.HR_KOD_PERUBAHAN == "00026")
                    //{
                    //    model.HR_TARIKH_AKHIR = model.HR_TARIKH_MULA;
                    //}

                    model.HR_KEW8_ID = incrementID;
                    model.HR_ANSURAN_ID = ansuranID;
                    if (model.HR_FINALISED_IND_HR == null)
                    {
                        model.HR_FINALISED_IND_HR = "T";
                    }
                    model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                    model.HR_TARIKH_UBAH_HR = DateTime.Now;

                    if (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")
                    {
                        model.HR_TARIKH_SP = new DateTime(model.HR_TARIKH_MULA.Year, model.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                        model.HR_SP_IND = "Y";
                    }

                    db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                    SMK8.Add(model);
                    db.SaveChanges();
                }

                if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI") || Kod == "00015" || Kod == "00024" || Kod == "00039" || Kod == "LNTKN" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                {
                    if (sAnsuran.ElementAt(0) != null && (Kod == "00030" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")))
                    {
                        SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                        SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
                        foreach (HR_MAKLUMAT_KEWANGAN8 ansuran in sAnsuran.ElementAt(0))
                        {
                            HR_MAKLUMAT_KEWANGAN8 model3 = new HR_MAKLUMAT_KEWANGAN8();

                            model3.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                            model3.HR_KOD_PERUBAHAN = model.HR_KOD_PERUBAHAN;
                            model3.HR_TARIKH_MULA = ansuran.HR_TARIKH_MULA;
                            model3.HR_TARIKH_AKHIR = ansuran.HR_TARIKH_AKHIR;
                            model3.HR_TAHUN = Convert.ToInt16(ansuran.HR_TARIKH_MULA.Year);
                            model3.HR_BULAN = ansuran.HR_TARIKH_MULA.Month;
                            model3.HR_TARIKH_KEYIN = DateTime.Now;
                            model3.HR_BUTIR_PERUBAHAN = model.HR_BUTIR_PERUBAHAN;
                            model3.HR_CATATAN = model.HR_CATATAN;
                            model3.HR_NO_SURAT_KEBENARAN = model.HR_NO_SURAT_KEBENARAN;
                            model3.HR_AKTIF_IND = model.HR_AKTIF_IND;
                            model3.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                            model3.HR_TARIKH_UBAH_HR = DateTime.Now;
                            model3.HR_NP_FINALISED_HR = model.HR_NP_FINALISED_HR;
                            model3.HR_TARIKH_FINALISED_HR = model.HR_TARIKH_FINALISED_HR;
                            model3.HR_FINALISED_IND_HR = model.HR_FINALISED_IND_HR;
                            model3.HR_NP_UBAH_PA = model.HR_NP_UBAH_PA;
                            model3.HR_TARIKH_UBAH_PA = model.HR_TARIKH_UBAH_PA;
                            model3.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_PA;
                            model3.HR_TARIKH_FINALISED_PA = model.HR_TARIKH_FINALISED_PA;
                            model3.HR_FINALISED_IND_PA = model.HR_FINALISED_IND_PA;
                            model3.HR_EKA = model.HR_EKA;
                            model3.HR_ITP = model.HR_ITP;
                            model3.HR_KEW8_IND = model.HR_KEW8_IND;
                            model3.HR_BIL = model.HR_BIL;
                            model3.HR_KOD_JAWATAN = model.HR_KOD_JAWATAN;
                            model3.HR_KEW8_ID = incrementID;
                            model3.HR_ANSURAN_ID = ansuranID;
                            model3.HR_LANTIKAN_IND = model.HR_LANTIKAN_IND;
                            model3.HR_TARIKH_SP = model.HR_TARIKH_SP;
                            model3.HR_SP_IND = model.HR_SP_IND;
                            model3.HR_JUMLAH_BULAN = model.HR_JUMLAH_BULAN;
                            model3.HR_NILAI_EPF = model.HR_NILAI_EPF;
                            model3.HR_GAJI_LAMA = model.HR_GAJI_LAMA;
                            model3.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                            model3.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                            model3.HR_GAJI_MIN_BARU = model.HR_GAJI_MIN_BARU;

                            if (Kod == "CUTI" && model3.HR_KOD_PERUBAHAN == "00017")
                            {
                                model3.HR_TARIKH_SP = new DateTime(model3.HR_TARIKH_MULA.Year, model3.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                                model3.HR_SP_IND = "Y";
                            }


                            if (model3.HR_FINALISED_IND_HR == null)
                            {
                                model3.HR_FINALISED_IND_HR = "T";
                            }

                            db.HR_MAKLUMAT_KEWANGAN8.Add(model3);
                            SMK8.Add(model3);
                            db.SaveChanges();

                            var no = 0;
                            foreach (HR_POTONGAN potongan in sPotongan)
                            {
                                //if (Kod != "00015" || (potongan.HR_AKTIF_IND == "Y" && Kod == "00015"))
                                //{
                                    if (potongan.HR_NILAI == null)
                                    {
                                        potongan.HR_NILAI = 0;
                                    }

                                    for (var a = 0; a < sAnsuran.Count(); a++)
                                    {
                                        for (var b = 0; b < sAnsuran.ElementAt(a).Count(); b++)
                                        {
                                            if (sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_MULA == ansuran.HR_TARIKH_MULA && sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_AKHIR == ansuran.HR_TARIKH_AKHIR && a == no)
                                            {
                                                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                                modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                                                modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                                                modelDetail2.HR_TARIKH_MULA = ansuran.HR_TARIKH_MULA;
                                                modelDetail2.HR_KOD_ELAUN = potongan.HR_KOD_CARUMAN;
                                                modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                                potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));

                                                //if (no == 0)
                                                //{
                                                //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                                                //    if (potongan.HR_NILAI > 0)
                                                //    {
                                                //        //tunggakan
                                                //        modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                                                //    }
                                                //    else
                                                //    {
                                                //        //potongan
                                                //        modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                                                //    }
                                                //}

                                                modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                                                modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                                modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                                if (Kod == "00030")
                                                {
                                                    modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI / sAnsuran.ElementAt(0).Count();
                                                }
                                                else
                                                {
                                                    modelDetail2.HR_JUMLAH_PERUBAHAN = sAnsuran.ElementAt(a).ElementAt(b).HR_BIL;
                                                }

                                                modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                                                modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                                modelDetail2.HR_STATUS_IND = "T";

                                                modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;

                                                modelDetail2.HR_KEW8_ID = incrementID;
                                                modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                                modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                                                modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                                                modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                                db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);
                                                SMK8D.Add(modelDetail2);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    no++;
                                //}
                            }
                            incrementID++;
                        }
                    }
                    else
                    {
                        SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
                        var no = 0;
                        foreach (HR_POTONGAN potongan in sPotongan)
                        {
                            //if (Kod != "00015" || (potongan.HR_AKTIF_IND == "Y" && Kod == "00015"))
                            //{
                                var xPotongan = potongan.HR_KOD_POTONGAN;
                                if (Kod == "00031" || Kod == "00024" || Kod == "00039")
                                {
                                    potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                                }

                                if (potongan.HR_KOD_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                    //modelDetail.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;

                                    //modelDetail.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;
                                    //modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                                    ////modelDetail.HR_TARIKH_MULA = DateTime.Now;
                                    //modelDetail.HR_KEW8_ID = incrementID;
                                    //modelDetail.HR_STATUS_IND = "T";
                                    //modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                    //modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    //modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;


                                    modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                                    modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                                    modelDetail2.HR_TARIKH_MULA = modelDetail.HR_TARIKH_MULA;
                                    modelDetail2.HR_KOD_ELAUN = potongan.HR_KOD_CARUMAN;

                                    if (potongan.HR_NILAI == null)
                                    {
                                        potongan.HR_NILAI = 0;
                                    }

                                    if (Kod == "00031" || Kod == "00024" || Kod == "00039")
                                    {
                                        modelDetail2.HR_KOD_PELARASAN = modelDetail.HR_KOD_PELARASAN;
                                        potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                        if ((Kod == "00024" && model.HR_KEW8_IND == "P") || (Kod == "00039" && model.HR_KEW8_IND == "P") || modelDetail2.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN || xPotongan == potongan2.HR_KOD_POTONGAN)
                                        {
                                            potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                        }
                                    }
                                    else
                                    {
                                        modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                        DateTime xKeyInDate = new DateTime(Convert.ToDateTime(model.HR_TARIKH_MULA).Year, Convert.ToDateTime(model.HR_TARIKH_KEYIN).Month, 1);
                                        if (Kod == "00036" || (Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T") || sPotongan.ElementAt(0).HR_KOD_POTONGAN == tggkk.HR_KOD_UPAH || ((Kod == "TMK" || Kod == "00032" || Kod == "00004") && model.HR_TARIKH_MULA <= xKeyInDate))
                                        {
                                            potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                        }
                                        else
                                        {
                                            potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                        }
                                    }

                                    //if(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN != null)
                                    //{
                                    //    if(sPotongan.ElementAt(0).HR_KOD_POTONGAN == tggkk.HR_KOD_UPAH || (Kod == "TMK" && model.HR_TARIKH_MULA <= model.HR_TARIKH_KEYIN))
                                    //    {
                                    //        modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = Math.Abs(Convert.ToDecimal(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN));
                                    //    }
                                    //    else
                                    //    {
                                    //        modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = -Math.Abs(Convert.ToDecimal(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN));
                                    //    }
                                    //}

                                    //start 24/7/2018
                                    //if (no == 0 && (Kod != "00031" && Kod != "00024" && Kod != "00039"))
                                    //{
                                    //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                                    //    if (potongan.HR_NILAI > 0)
                                    //    {
                                    //        //tunggakan
                                    //        modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                                    //    }
                                    //    else
                                    //    {
                                    //        //potongan
                                    //        modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                                    //    }
                                    //}
                                    //end 24/7/2018

                                    if (Kod != "00036" && Kod != "TMK" && Kod != "00032" && Kod != "00004")
                                    {
                                        modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                                        modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                        modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    }
                                    else
                                    {
                                        modelDetail2.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                                        modelDetail2.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                                        modelDetail2.HR_GAJI_BARU = modelDetail.HR_GAJI_BARU;
                                    }

                                    modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;

                                    modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                                    modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                    modelDetail2.HR_STATUS_IND = "T";

                                    if (Kod == "00039")
                                    {
                                        if (model.HR_KEW8_IND != "P")
                                        {
                                            modelDetail.HR_JUMLAH_PERUBAHAN = 0;
                                            modelDetail.HR_STATUS_IND = null;
                                        }
                                        else
                                        {
                                            modelDetail.HR_STATUS_IND = "P";
                                        }
                                    }

                                    modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;

                                    modelDetail2.HR_KEW8_ID = incrementID;
                                    modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                    modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                                    modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                                    modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                    modelDetail2.HR_JAWATAN_BARU = modelDetail.HR_JAWATAN_BARU;
                                    modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                    //modelDetail2.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                    //modelDetail2.HR_GRED_LAMA = modelDetail.HR_GRED;

                                    db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);
                                    SMK8D.Add(modelDetail2);
                                    db.SaveChanges();

                                    no++;
                                }
                            //}
                        }
                    }
                }
                ////else if(Kod != "00022" && Kod != "00037" && Kod != "kew8" && Kod != "00025")
                //else if (Kod == "00039" || Kod == "00024")
                //{
                //    //modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                //    //modelDetail.HR_TARIKH_MULA = DateTime.Now;
                //    modelDetail.HR_KEW8_ID = incrementID;
                //    //if(Kod != "00022" && Kod != "00037")
                //    //{
                //        modelDetail.HR_STATUS_IND = "T";
                //    //}

                //    //if(Kod != "00036")
                //    //{
                //    //    modelDetail.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                //    //    modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    //    modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                //    //}
                //    //else
                //    //{
                //    //    //modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    //    modelDetail.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                //    //}

                //    if(Kod == "00039")
                //    {
                //        if(model.HR_KEW8_IND != "P")
                //        {
                //            modelDetail.HR_JUMLAH_PERUBAHAN = 0;
                //            modelDetail.HR_STATUS_IND = null;
                //        }
                //        else
                //        {
                //            modelDetail.HR_STATUS_IND = "P";
                //        }
                //    }

                //    //modelDetail.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                //    //modelDetail.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                //    modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                //    db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail);
                //    db.SaveChanges();
                //}

                if (model.HR_FINALISED_IND_HR == "Y")
                {
                    //Muktamad(model, modelDetail, sAnsuran, Kod, sPotongan, gajiUpah, potongan2, tggkk, pekerja, peribadi, model.HR_TARIKH_MULA, model.HR_TARIKH_MULA.Month, Convert.ToInt16(model.HR_TARIKH_MULA.Year));
                    Muktamad2(SMK8, SMK8D, Kod, gajiUpah, tggkk, potongan2);
                }

                var redirect = RedirectLink(Kod);

                var txtMss = "Data berjaya dimasukkan";
                if (model.HR_FINALISED_IND_HR == "Y")
                {
                    txtMss += " dan dimuktamadkan";
                }

                //return RedirectToAction(redirect, new { key = "1", value = model.HR_NO_PEKERJA });
                return Json(new { error = false, msg = txtMss, location = "../Kewangan8/" + redirect + "?key=1&value=" + model.HR_NO_PEKERJA }, JsonRequestBehavior.AllowGet);

            }
            ViewBag.HR_KOD_JAWATAN = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            decimal? gaji = 0;
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK != null)
            {
                gaji = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            ViewBag.HR_GAJI_LAMA = gaji;

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                foreach (var item in elaunPotongan)
                {
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        if (elaun4.HR_PERATUS_IND == "Y")
                        {
                            item.HR_JUMLAH = gaji * (item.HR_JUMLAH / 100);
                        }
                        elaun3.Add(elaun4);

                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);

                    }
                }

            }
            if(Kod == "00031")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }
            
            if (Kod == "00039")
            {
                if(model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                
            }

            if (Kod == "00024")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                    
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;


            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && (s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI.Trim() == modelDetail.HR_GRED));
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList.OrderBy(s => s.SHORT_DESCRIPTION).ToList();

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if (transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            //return PartialView("_TambahKew8", model);
            return Json(new { error = false, msg = "Data tidak berjaya dimasukkan" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InfoKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            List<HR_MAKLUMAT_KEWANGAN8> tarikhMulaAkhir = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).OrderBy(s => s.HR_TARIKH_MULA).ToList();
            if (model == null)
            {
                return HttpNotFound();
            }

            if (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")
            {
                model.HR_TARIKH_MULA = tarikhMulaAkhir.FirstOrDefault().HR_TARIKH_MULA;
                model.HR_TARIKH_AKHIR = tarikhMulaAkhir.LastOrDefault().HR_TARIKH_AKHIR;
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(Detail2.HR_GRED);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN_ELAUN));
            ViewBag.HR_JUMLAH_PERUBAHAN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN));
            ViewBag.HR_JENIS_PERGERAKAN = Detail2.HR_JENIS_PERGERAKAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, Detail2.HR_MATRIKS_GAJI, Detail2.HR_GAJI_BARU);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            if (Kod == "TMK" || Kod == "00032" || Kod == "00004")
            {
                HR_MAKLUMAT_PERIBADI mPeribadi4 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == Detail2.HR_NO_PEKERJA_PT);
                if (mPeribadi4 == null)
                {
                    mPeribadi4 = new HR_MAKLUMAT_PERIBADI();
                }

                ViewBag.HR_GAJI_MIN_BARU = Detail2.HR_GAJI_BARU;
                ViewBag.HR_NAMA_PEKERJA_PT = mPeribadi4.HR_NAMA_PEKERJA;
                ViewBag.HR_NO_PEKERJA_PT = Detail2.HR_NO_PEKERJA_PT;
                ViewBag.HR_PGT_BARU = matriksDetail2.HR_RM_KENAIKAN;
                ViewBag.HR_JAWATAN_BARU = Detail2.HR_JAWATAN_BARU;
                ViewBag.HR_KOD_PELARASAN = Detail2.HR_KOD_PELARASAN;
            }

            

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(model.HR_GRED_LAMA);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = model.HR_GAJI_LAMA;
            ViewBag.HR_SISTEM = mPekerjaan.HR_SISTEM;
            ViewBag.HR_PGT_LAMA = matriksPekerjaan.HR_RM_KENAIKAN;
            ViewBag.kodGaji = matriksPekerjaan.HR_KOD_GAJI;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == model.HR_KOD_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //int? kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);

            //ViewBag.gred = cariGred(kodGred, null).SHORT_DESCRIPTION;
            //if(cariGred(gredDetail, null).SHORT_DESCRIPTION != null)
            //{
            //    ViewBag.gred = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            //}
            
            //ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;
            //ViewBag.gaji = mPekerjaan.HR_GAJI_POKOK;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (mPekerjaan.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + mPekerjaan.HR_TARAF_JAWATAN;
            }
            else if (mPekerjaan.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + mPekerjaan.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (gajiUpah == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }
            ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

            List<HR_MAKLUMAT_KEWANGAN8> listAnsuran = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).ToList();
            var bilAnsuran = listAnsuran.Count();
            if (listAnsuran.Count() <= 1)
            {
                bilAnsuran = 1;
            }

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if (Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI") || Kod == "00015" || Kod == "LNTKN")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_TUNGGAKAN);
                                }
                                else
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                }

                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                if (elaun4.HR_PERATUS_IND == "Y")
                                {
                                    item.HR_JUMLAH = model.HR_GAJI_LAMA * (item.HR_JUMLAH / 100);
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                elaun3.Add(elaun4);


                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    potongan4.HR_KOD_POTONGAN = elaun4.HR_KOD_TUNGGAKAN;
                                    potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                    var peneranganElaun = "";
                                    HR_ELAUN elaunTunggakan = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun4.HR_KOD_TUNGGAKAN);
                                    if (elaunTunggakan != null)
                                    {
                                        peneranganElaun = elaunTunggakan.HR_PENERANGAN_ELAUN;
                                    }
                                    potongan4.HR_PENERANGAN_POTONGAN = peneranganElaun;
                                }
                                else
                                {
                                    //potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                    HR_POTONGAN potongan6 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_KOD_POTONGAN == Detail.HR_KOD_PELARASAN);
                                    if (potongan6 == null)
                                    {
                                        potongan6 = new HR_POTONGAN();
                                    }
                                    potongan4 = potongan6;
                                }
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH);
                            }
                            else
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            }

                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                        }
                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4.HR_PERATUS_IND == "Y")
                            {
                                item.HR_JUMLAH = model.HR_GAJI_LAMA * (item.HR_JUMLAH / 100);
                            }
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    //ViewBag.nilaiPotongan = model.HR_BIL;
                }

                ViewBag.elaun3 = elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList();
                ViewBag.potongan3 = potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList();
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }

            HR_MAKLUMAT_ELAUN_POTONGAN kodG = elaunPotongan.FirstOrDefault(s => s.HR_ELAUN_POTONGAN_IND == "G");
            if (kodG == null)
            {
                kodG = new HR_MAKLUMAT_ELAUN_POTONGAN();
            }
            ViewBag.kodG = kodG.HR_KOD_ELAUN_POTONGAN;

            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;

            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            {
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList(), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList(), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }

            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            if (Kod == "00026")
            {
                ViewBag.HR_TARIKH_TAMAT = null;
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            //decimal? gaji = 0;
            //if (mPekerjaan.HR_GAJI_POKOK != null)
            //{
            //    gaji = mPekerjaan.HR_GAJI_POKOK;
            //}
            //ViewBag.HR_GAJI_LAMA = gaji;
            //if(Detail2.HR_GAJI_BARU != null)
            //{
            //    ViewBag.gaji = Detail2.HR_GAJI_BARU;
            //}
            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList.OrderBy(s => s.SHORT_DESCRIPTION).ToList();

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if (transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            HR_PERATUS_KWSP PeratusKWSP = db.HR_PERATUS_KWSP.FirstOrDefault();
            if (PeratusKWSP == null)
            {
                PeratusKWSP = new HR_PERATUS_KWSP();
            }
            ViewBag.HR_KOD_PERATUS = PeratusKWSP.HR_KOD_PERATUS;
            ViewBag.HR_NILAI_PERATUS = PeratusKWSP.HR_NILAI_PERATUS;

            return PartialView("_InfoKew8", model);
        }

        public ActionResult EditKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            List<HR_MAKLUMAT_KEWANGAN8> tarikhMulaAkhir = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).OrderBy(s => s.HR_TARIKH_MULA).ToList();

            if (model == null)
            {
                return HttpNotFound();
            }

            if(Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")
            {
                model.HR_TARIKH_MULA = tarikhMulaAkhir.FirstOrDefault().HR_TARIKH_MULA;
                model.HR_TARIKH_AKHIR = tarikhMulaAkhir.LastOrDefault().HR_TARIKH_AKHIR;
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(Detail2.HR_GRED);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN_ELAUN));
            ViewBag.HR_JUMLAH_PERUBAHAN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN));
            ViewBag.HR_JENIS_PERGERAKAN = Detail2.HR_JENIS_PERGERAKAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, Detail2.HR_MATRIKS_GAJI, Detail2.HR_GAJI_BARU);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            if(Kod == "TMK" || Kod == "00032" || Kod == "00004")
            {
                HR_MAKLUMAT_PERIBADI mPeribadi4 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == Detail2.HR_NO_PEKERJA_PT);
                if (mPeribadi4 == null)
                {
                    mPeribadi4 = new HR_MAKLUMAT_PERIBADI();
                }

                ViewBag.HR_GAJI_MIN_BARU = Detail2.HR_GAJI_BARU;
                ViewBag.HR_NAMA_PEKERJA_PT = mPeribadi4.HR_NAMA_PEKERJA;
                ViewBag.HR_NO_PEKERJA_PT = Detail2.HR_NO_PEKERJA_PT;
                ViewBag.HR_PGT_BARU = matriksDetail2.HR_RM_KENAIKAN;
                ViewBag.HR_JAWATAN_BARU = Detail2.HR_JAWATAN_BARU;
                ViewBag.HR_KOD_PELARASAN = Detail2.HR_KOD_PELARASAN;
            }
            

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(mPekerjaan.HR_GRED);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, mPekerjaan.HR_MATRIKS_GAJI, mPekerjaan.HR_GAJI_POKOK);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_MATRIKS_GAJI_LAMA = mPekerjaan.HR_MATRIKS_GAJI;
            ViewBag.HR_SISTEM = mPekerjaan.HR_SISTEM;
            ViewBag.HR_PGT_LAMA = matriksPekerjaan.HR_RM_KENAIKAN;
            ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (mPekerjaan.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + mPekerjaan.HR_TARAF_JAWATAN;
            }
            else if (mPekerjaan.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + mPekerjaan.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (gajiUpah == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }
            ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

            List<HR_MAKLUMAT_KEWANGAN8> listAnsuran = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).ToList();
            var bilAnsuran = listAnsuran.Count();
            if(listAnsuran.Count() <= 1)
            {
                bilAnsuran = 1;
            }

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if(Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI") || Kod == "00015" || Kod == "LNTKN")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                if((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_TUNGGAKAN);
                                }
                                else
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                }
                                
                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                if (elaun4.HR_PERATUS_IND == "Y")
                                {
                                    item.HR_JUMLAH = model.HR_GAJI_LAMA * (item.HR_JUMLAH / 100);
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                
                                if(Kod == "00015")
                                {
                                    elaun4.HR_AKTIF_IND = "T";
                                    if(Detail == null)
                                    {
                                        elaun4.HR_AKTIF_IND = "Y";
                                    }
                                }

                                elaun3.Add(elaun4);


                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                if((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    potongan4.HR_KOD_POTONGAN = elaun4.HR_KOD_TUNGGAKAN;
                                    potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                    var peneranganElaun = "";
                                    HR_ELAUN elaunTunggakan = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun4.HR_KOD_TUNGGAKAN);
                                    if(elaunTunggakan != null)
                                    {
                                        peneranganElaun = elaunTunggakan.HR_PENERANGAN_ELAUN;
                                    }
                                    potongan4.HR_PENERANGAN_POTONGAN = peneranganElaun;
                                }
                                else
                                {
                                    //potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                    HR_POTONGAN potongan6 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_KOD_POTONGAN == Detail.HR_KOD_PELARASAN);
                                    if(potongan6 == null)
                                    {
                                        potongan6 = new HR_POTONGAN();
                                    }
                                    potongan4 = potongan6;
                                }
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH);
                            }
                            else
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            }
                            
                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                        }
                        
                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    //ViewBag.nilaiPotongan += model.HR_BIL;
                }
                
                ViewBag.elaun3 = elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList();
                ViewBag.potongan3 = potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList();
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }

            HR_MAKLUMAT_ELAUN_POTONGAN kodG = elaunPotongan.FirstOrDefault(s => s.HR_ELAUN_POTONGAN_IND == "G");
            if (kodG == null)
            {
                kodG = new HR_MAKLUMAT_ELAUN_POTONGAN();
            }
            ViewBag.kodG = kodG.HR_KOD_ELAUN_POTONGAN;

            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;

            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            { 
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList(), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList(), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }
            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            if(Kod == "00026")
            {
                ViewBag.HR_TARIKH_TAMAT = null;
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList.OrderBy(s => s.SHORT_DESCRIPTION).ToList();

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if (transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            HR_PERATUS_KWSP PeratusKWSP = db.HR_PERATUS_KWSP.FirstOrDefault();
            if(PeratusKWSP == null)
            {
                PeratusKWSP = new HR_PERATUS_KWSP();
            }
            ViewBag.HR_KOD_PERATUS = PeratusKWSP.HR_KOD_PERATUS;
            ViewBag.HR_NILAI_PERATUS = PeratusKWSP.HR_NILAI_PERATUS;

            return PartialView("_EditKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKew8([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_TARIKH_AKHIR,HR_BULAN,HR_TAHUN,HR_TARIKH_KEYIN,HR_BUTIR_PERUBAHAN,HR_CATATAN,HR_NO_SURAT_KEBENARAN,HR_AKTIF_IND,HR_NP_UBAH_HR,HR_TARIKH_UBAH_HR,HR_NP_FINALISED_HR,HR_TARIKH_FINALISED_HR,HR_FINALISED_IND_HR,HR_NP_UBAH_PA,HR_TARIKH_UBAH_PA,HR_NP_FINALISED_PA,HR_TARIKH_FINALISED_PA,HR_FINALISED_IND_PA,HR_EKA,HR_ITP,HR_KEW8_IND,HR_BIL,HR_KOD_JAWATAN,HR_KEW8_ID,HR_LANTIKAN_IND,HR_TARIKH_SP,HR_SP_IND,HR_JUMLAH_BULAN,HR_NILAI_EPF,HR_GAJI_LAMA,HR_MATRIKS_GAJI_LAMA,HR_GRED_LAMA,HR_UBAH_IND,HR_ANSURAN_ID,HR_GAJI_MIN_BARU")] HR_MAKLUMAT_KEWANGAN8 model, [Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERUBAHAN,HR_TARIKH_MULA,HR_KOD_PELARASAN,HR_MATRIKS_GAJI,HR_GRED,HR_JUMLAH_PERUBAHAN,HR_GAJI_BARU,HR_JENIS_PERGERAKAN,HR_JUMLAH_PERUBAHAN_ELAUN,HR_STATUS_IND,HR_ELAUN_KRITIKAL_BARU,HR_KEW8_ID,HR_NO_PEKERJA_PT,HR_PERGERAKAN_EKAL,HR_PERGERAKAN_EWIL,HR_GAJI_LAMA,HR_JAWATAN_BARU,HR_KOD_ELAUN")] HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, decimal? HR_JUMLAH_POTONGAN, IEnumerable<HR_POTONGAN> sPotongan, int? PA_BULAN, short? PA_TAHUN, string Kod, DateTime? HR_TARIKH_TAMAT, HR_MAKLUMAT_KEWANGAN8[][] sAnsuran, HR_PERATUS_KWSP PeratusKWSP)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.Where(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y").FirstOrDefault();
            HR_MAKLUMAT_PERIBADI pekerja = mPeribadi.SingleOrDefault(s => s.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI, pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);
            var kodGred = Convert.ToInt32(pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED);

            ViewBag.HR_KOD_JAWATAN = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(kodGred, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_PGT_LAMA = matriksPekerjaan.HR_RM_KENAIKAN;

            DateTime tMula = model.HR_TARIKH_MULA;

            List<HR_MAKLUMAT_KEWANGAN8> SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
            List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            //HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA).Count() > 0);
            //if (gajiUpah == null)
            //{
            //    gajiUpah = new HR_GAJI_UPAHAN();
            //}

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            var jawatan_ind = "";
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }
            else if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (tggkk == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }

            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (ModelState.IsValid)
            {
                List<HR_MAKLUMAT_KEWANGAN8> padamKew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == model.HR_ANSURAN_ID).OrderBy(s => s.HR_TARIKH_MULA).ToList();
                HR_MAKLUMAT_KEWANGAN8 firstData = padamKew8.FirstOrDefault();
                
                foreach (HR_MAKLUMAT_KEWANGAN8 padam in padamKew8)
                {
                    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == padam.HR_KEW8_ID && s.HR_NO_PEKERJA == padam.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == padam.HR_KOD_PERUBAHAN).ToList();
                    db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(Detail);
                    db.SaveChanges();
                }

                if (padamKew8.Count() > 0)
                {
                    padamKew8.Remove(firstData);
                    db.HR_MAKLUMAT_KEWANGAN8.RemoveRange(padamKew8);
                    db.SaveChanges();
                }

                if (sAnsuran == null)
                {
                    sAnsuran = new HR_MAKLUMAT_KEWANGAN8[1][];
                    sAnsuran[0] = null;
                }

                if (sAnsuran.ElementAt(0) != null && (Kod == "00030" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")))
                {
                    padamKew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_ANSURAN_ID == model.HR_ANSURAN_ID).OrderBy(s => s.HR_TARIKH_MULA).ToList();
                    db.HR_MAKLUMAT_KEWANGAN8.RemoveRange(padamKew8);
                    db.SaveChanges();

                    var lastID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault();
                    var incrementID = lastID.HR_KEW8_ID + 1;

                    //var lastAnsuranID = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_ANSURAN_ID).FirstOrDefault();
                    var ansuranID = incrementID;

                    SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                    SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

                    foreach (HR_MAKLUMAT_KEWANGAN8 ansuran in sAnsuran.ElementAt(0))
                    {
                        HR_MAKLUMAT_KEWANGAN8 model3 = new HR_MAKLUMAT_KEWANGAN8();

                        model3.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                        model3.HR_KOD_PERUBAHAN = model.HR_KOD_PERUBAHAN;
                        model3.HR_TARIKH_MULA = ansuran.HR_TARIKH_MULA;
                        model3.HR_TARIKH_AKHIR = ansuran.HR_TARIKH_AKHIR;
                        model3.HR_TAHUN = Convert.ToInt16(ansuran.HR_TARIKH_MULA.Year);
                        model3.HR_BULAN = ansuran.HR_TARIKH_MULA.Month;
                        model3.HR_TARIKH_KEYIN = DateTime.Now;
                        model3.HR_BUTIR_PERUBAHAN = model.HR_BUTIR_PERUBAHAN;
                        model3.HR_CATATAN = model.HR_CATATAN;
                        model3.HR_NO_SURAT_KEBENARAN = model.HR_NO_SURAT_KEBENARAN;
                        model3.HR_AKTIF_IND = model.HR_AKTIF_IND;
                        model3.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                        model3.HR_TARIKH_UBAH_HR = DateTime.Now;
                        model3.HR_NP_FINALISED_HR = model.HR_NP_FINALISED_HR;
                        model3.HR_TARIKH_FINALISED_HR = model.HR_TARIKH_FINALISED_HR;
                        model3.HR_FINALISED_IND_HR = model.HR_FINALISED_IND_HR;
                        model3.HR_NP_UBAH_PA = model.HR_NP_UBAH_PA;
                        model3.HR_TARIKH_UBAH_PA = model.HR_TARIKH_UBAH_PA;
                        model3.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_PA;
                        model3.HR_TARIKH_FINALISED_PA = model.HR_TARIKH_FINALISED_PA;
                        model3.HR_FINALISED_IND_PA = model.HR_FINALISED_IND_PA;
                        model3.HR_EKA = model.HR_EKA;
                        model3.HR_ITP = model.HR_ITP;
                        model3.HR_KEW8_IND = model.HR_KEW8_IND;
                        model3.HR_BIL = model.HR_BIL;
                        model3.HR_KOD_JAWATAN = model.HR_KOD_JAWATAN;
                        model3.HR_KEW8_ID = incrementID;
                        model3.HR_ANSURAN_ID = ansuranID;
                        model3.HR_LANTIKAN_IND = model.HR_LANTIKAN_IND;
                        model3.HR_TARIKH_SP = model.HR_TARIKH_SP;
                        model3.HR_SP_IND = model.HR_SP_IND;
                        model3.HR_JUMLAH_BULAN = model.HR_JUMLAH_BULAN;
                        model3.HR_NILAI_EPF = model.HR_NILAI_EPF;
                        model3.HR_GAJI_LAMA = model.HR_GAJI_LAMA;
                        model3.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                        model3.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                        model3.HR_GAJI_MIN_BARU = model.HR_GAJI_MIN_BARU;

                        if (Kod == "CUTI" && model3.HR_KOD_PERUBAHAN == "00017")
                        {
                            model3.HR_TARIKH_SP = new DateTime(model3.HR_TARIKH_MULA.Year, model3.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                            model3.HR_SP_IND = "Y";
                        }

                        db.HR_MAKLUMAT_KEWANGAN8.Add(model3);
                        SMK8.Add(model3);
                        db.SaveChanges();

                        var no = 0;
                        foreach (HR_POTONGAN potongan in sPotongan)
                        {
                            if (potongan.HR_KOD_POTONGAN != null)
                            {
                                if (potongan.HR_NILAI == null)
                                {
                                    potongan.HR_NILAI = 0;
                                }

                                for (var a = 0; a < sAnsuran.Count(); a++)
                                {
                                    for (var b = 0; b < sAnsuran.ElementAt(a).Count(); b++)
                                    {
                                        if (sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_MULA == ansuran.HR_TARIKH_MULA && sAnsuran.ElementAt(a).ElementAt(b).HR_TARIKH_AKHIR == ansuran.HR_TARIKH_AKHIR && a == no)
                                        {
                                            HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();

                                            modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                                            modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                                            modelDetail2.HR_TARIKH_MULA = ansuran.HR_TARIKH_MULA;
                                            modelDetail2.HR_KOD_ELAUN = potongan.HR_KOD_CARUMAN;
                                            modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                            potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));

                                            //if (no == 0)
                                            //{
                                            //    if (potongan.HR_NILAI > 0)
                                            //    {
                                            //        //tunggakan
                                            //        modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                                            //    }
                                            //    else
                                            //    {
                                            //        //potongan
                                            //        modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                                            //    }
                                            //}

                                            modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                                            modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                            modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                            if (Kod == "00030")
                                            {
                                                modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI / sAnsuran.ElementAt(0).Count();
                                            }
                                            else
                                            {
                                                modelDetail2.HR_JUMLAH_PERUBAHAN = sAnsuran.ElementAt(a).ElementAt(b).HR_BIL;
                                            }

                                            modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                                            modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                            modelDetail2.HR_STATUS_IND = "E";
                                            modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;
                                            modelDetail2.HR_KEW8_ID = incrementID;
                                            modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                            modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                                            modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                                            modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                                            db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);
                                            SMK8D.Add(modelDetail2);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            no++;
                        }
                        incrementID++;
                    }
                }
                else
                {
                    SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();

                    if (Kod == "00031")
                    {
                        var jumBulan = Convert.ToString(model.HR_JUMLAH_BULAN);
                        var EPF = Convert.ToString(model.HR_NILAI_EPF);
                        var bil = jumBulan + EPF;
                        model.HR_BIL = Convert.ToDecimal(bil);

                        db.Entry(PeratusKWSP).State = EntityState.Modified;
                    }

                    model.HR_BULAN = DateTime.Now.Month;
                    model.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                    model.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                    model.HR_TARIKH_UBAH_HR = DateTime.Now;
                    model.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    model.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    //HR_JAWATAN jwt4 = db.HR_JAWATAN.SingleOrDefault(s => s.HR_NAMA_JAWATAN == model.HR_KOD_JAWATAN);
                    //if (jwt4 == null)
                    //{
                    //    jwt4 = new HR_JAWATAN();
                    //}

                    //model.HR_KOD_JAWATAN = jwt4.HR_KOD_JAWATAN;

                    if (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")
                    {
                        model.HR_TARIKH_SP = new DateTime(model.HR_TARIKH_MULA.Year, model.HR_TARIKH_MULA.AddMonths(1).Month, 1);
                        model.HR_SP_IND = "Y";
                    }

                    if (Kod == "TP")
                    {
                        HR_MAKLUMAT_KEWANGAN8 cariKew8 = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tMula);
                        db.HR_MAKLUMAT_KEWANGAN8.Remove(cariKew8);

                        model.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                        model.HR_TARIKH_AKHIR = Convert.ToDateTime(HR_TARIKH_TAMAT);
                        modelDetail.HR_TARIKH_MULA = Convert.ToDateTime(HR_TARIKH_TAMAT);
                        db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                        SMK8.Add(model);
                    }
                    else
                    {
                        padamKew8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                        if (padamKew8.Count() <= 0)
                        {
                            db.HR_MAKLUMAT_KEWANGAN8.Add(model);
                        }
                        else
                        {
                            firstData.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                            firstData.HR_TAHUN = model.HR_TAHUN;
                            firstData.HR_BULAN = model.HR_BULAN;
                            firstData.HR_BUTIR_PERUBAHAN = model.HR_BUTIR_PERUBAHAN;
                            firstData.HR_CATATAN = model.HR_CATATAN;
                            firstData.HR_NO_SURAT_KEBENARAN = model.HR_NO_SURAT_KEBENARAN;
                            firstData.HR_AKTIF_IND = model.HR_AKTIF_IND;
                            firstData.HR_NP_UBAH_HR = model.HR_NP_UBAH_HR;
                            firstData.HR_TARIKH_UBAH_HR = model.HR_TARIKH_UBAH_HR;
                            firstData.HR_NP_FINALISED_HR = model.HR_NP_FINALISED_HR;
                            firstData.HR_TARIKH_FINALISED_HR = model.HR_TARIKH_FINALISED_HR;
                            firstData.HR_FINALISED_IND_HR = model.HR_FINALISED_IND_HR;
                            firstData.HR_NP_UBAH_PA = model.HR_NP_UBAH_PA;
                            firstData.HR_TARIKH_UBAH_PA = model.HR_TARIKH_UBAH_PA;
                            firstData.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_PA;
                            firstData.HR_TARIKH_FINALISED_PA = model.HR_TARIKH_FINALISED_PA;
                            firstData.HR_FINALISED_IND_PA = model.HR_FINALISED_IND_PA;
                            firstData.HR_EKA = model.HR_EKA;
                            firstData.HR_ITP = model.HR_ITP;
                            firstData.HR_KEW8_IND = model.HR_KEW8_IND;
                            firstData.HR_BIL = model.HR_BIL;
                            firstData.HR_KOD_JAWATAN = model.HR_KOD_JAWATAN;
                            firstData.HR_LANTIKAN_IND = model.HR_LANTIKAN_IND;
                            firstData.HR_TARIKH_SP = model.HR_TARIKH_SP;
                            firstData.HR_SP_IND = model.HR_SP_IND;
                            firstData.HR_JUMLAH_BULAN = model.HR_JUMLAH_BULAN;
                            firstData.HR_NILAI_EPF = model.HR_NILAI_EPF;
                            firstData.HR_GAJI_LAMA = model.HR_GAJI_LAMA;
                            firstData.HR_GRED_LAMA = model.HR_GRED_LAMA;
                            firstData.HR_MATRIKS_GAJI_LAMA = model.HR_MATRIKS_GAJI_LAMA;
                            firstData.HR_GAJI_MIN_BARU = model.HR_GAJI_MIN_BARU;
                            db.Entry(firstData).State = EntityState.Modified;
                            SMK8.Add(firstData);
                            tMula = firstData.HR_TARIKH_MULA;
                        }
                        
                    }
                    db.SaveChanges();

                    if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI") || Kod == "00015" || Kod == "00024" || Kod == "00039" || Kod == "LNTKN" || Kod == "TMK" || Kod == "00032" || Kod == "00004")
                    {
                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == model.HR_KEW8_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == tMula).ToList();
                        db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(Detail);
                        db.SaveChanges();
                        var no = 0;
                        SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
                        foreach (HR_POTONGAN potongan in sPotongan)
                        {
                            var xPotongan = potongan.HR_KOD_POTONGAN;
                            if (Kod == "00031" || Kod == "00024" || Kod == "00039")
                            {
                                potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                            }

                            if (potongan.HR_KOD_POTONGAN != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();

                                modelDetail2.HR_NO_PEKERJA = modelDetail.HR_NO_PEKERJA;
                                modelDetail2.HR_KOD_PERUBAHAN = modelDetail.HR_KOD_PERUBAHAN;
                                modelDetail2.HR_TARIKH_MULA = tMula;
                                modelDetail2.HR_KOD_ELAUN = potongan.HR_KOD_CARUMAN;
                                modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;

                                if (potongan.HR_NILAI == null)
                                {
                                    potongan.HR_NILAI = 0;
                                }

                                if (Kod == "00031" || Kod == "00024" || Kod == "00039")
                                {
                                    modelDetail2.HR_KOD_PELARASAN = modelDetail.HR_KOD_PELARASAN;
                                    potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                    if ((Kod == "00024" && model.HR_KEW8_IND == "P") || (Kod == "00039" && model.HR_KEW8_IND == "P") || modelDetail2.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN || xPotongan == potongan2.HR_KOD_POTONGAN)
                                    {
                                        potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                    }
                                }
                                else
                                {
                                    modelDetail2.HR_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                                    DateTime xKeyInDate = new DateTime(Convert.ToDateTime(model.HR_TARIKH_MULA).Year, Convert.ToDateTime(model.HR_TARIKH_KEYIN).Month, 1);
                                    if (Kod == "00036" || (Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T") || sPotongan.ElementAt(0).HR_KOD_POTONGAN == tggkk.HR_KOD_UPAH || ((Kod == "TMK" || Kod == "00032" || Kod == "00004") && model.HR_TARIKH_MULA <= xKeyInDate))
                                    {
                                        potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                                    }
                                    else
                                    {
                                        potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                                    }
                                }

                                //if (modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN != null)
                                //{
                                //    if (sPotongan.ElementAt(0).HR_KOD_POTONGAN == tggkk.HR_KOD_UPAH || (Kod == "TMK" && model.HR_TARIKH_MULA <= model.HR_TARIKH_KEYIN))
                                //    {
                                //        modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = Math.Abs(Convert.ToDecimal(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN));
                                //    }
                                //    else
                                //    {
                                //        modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN = -Math.Abs(Convert.ToDecimal(modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN));
                                //    }
                                //}

                                //if (no == 0 && (Kod != "00031" && Kod != "00036"))
                                //{
                                //    //potongan.HR_NILAI = -potongan.HR_NILAI;
                                //    modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                                //}

                                //if (no == 0 && (Kod != "00031" && Kod != "00039" && Kod != "00024"))
                                //{
                                //    if (potongan.HR_NILAI > 0)
                                //    {
                                //        //tunggakan
                                //        modelDetail2.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                                //    }
                                //    else
                                //    {
                                //        //potongan
                                //        modelDetail2.HR_KOD_PELARASAN = potongan2.HR_KOD_POTONGAN;
                                //    }
                                //}

                                if (Kod != "00036" && Kod != "TMK" && Kod != "00032" && Kod != "00004")
                                {
                                    modelDetail2.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                                    modelDetail2.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                    modelDetail2.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                }
                                else
                                {
                                    modelDetail2.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                                    modelDetail2.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                                    modelDetail2.HR_GAJI_BARU = modelDetail.HR_GAJI_BARU;
                                }


                                modelDetail2.HR_JUMLAH_PERUBAHAN = potongan.HR_NILAI;

                                modelDetail2.HR_JENIS_PERGERAKAN = modelDetail.HR_JENIS_PERGERAKAN;
                                modelDetail2.HR_JUMLAH_PERUBAHAN_ELAUN = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                                modelDetail2.HR_STATUS_IND = "E";
                                modelDetail2.HR_ELAUN_KRITIKAL_BARU = modelDetail.HR_ELAUN_KRITIKAL_BARU;
                                modelDetail2.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                                modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                modelDetail2.HR_PERGERAKAN_EKAL = modelDetail.HR_PERGERAKAN_EKAL;
                                modelDetail2.HR_PERGERAKAN_EWIL = modelDetail.HR_PERGERAKAN_EWIL;
                                modelDetail2.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                                modelDetail2.HR_JAWATAN_BARU = modelDetail.HR_JAWATAN_BARU;
                                modelDetail2.HR_NO_PEKERJA_PT = modelDetail.HR_NO_PEKERJA_PT;
                                //modelDetail2.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                                //modelDetail2.HR_GRED_LAMA = modelDetail.HR_GRED;

                                db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Add(modelDetail2);
                                SMK8D.Add(modelDetail2);
                                no++;
                                db.SaveChanges();
                            }
                        }

                    }
                    //else if(Kod != "00022" && Kod != "00037" && Kod != "kew8" && Kod != "00025")
                    //else if (Kod == "00039" || Kod == "00024")
                    //{
                    //    modelDetail.HR_KOD_PERUBAHAN = kewangan8.HR_KOD_KEW8;
                    //    //modelDetail.HR_TARIKH_MULA = DateTime.Now;
                    //    //if(Kod != "00022" && Kod != "00037")
                    //    //{
                    //    modelDetail.HR_STATUS_IND = "E";
                    //    //}

                    //    //if (Kod != "00036")
                    //    //{
                    //    //    modelDetail.HR_GRED = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    //    //    modelDetail.HR_MATRIKS_GAJI = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
                    //    //    modelDetail.HR_GAJI_BARU = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    //    //}
                    //    //else
                    //    //{
                    //    //    modelDetail.HR_KOD_PELARASAN = tggkk.HR_KOD_UPAH;
                    //    //}

                    //    modelDetail.HR_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                    //    //modelDetail.HR_GRED_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
                    //    //modelDetail.HR_MATRIKS_GAJI_LAMA = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                    //    db.Entry(modelDetail).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                }
                if (model.HR_FINALISED_IND_HR == "Y")
                {

                    //Muktamad(model, modelDetail, sAnsuran, Kod, sPotongan, gajiUpah, potongan2, tggkk, pekerja, peribadi, tMula, PA_BULAN, PA_TAHUN);
                    Muktamad2(SMK8, SMK8D, Kod, gajiUpah, tggkk, potongan2);

                    //start 22-7-2018 - yg asal
                    //model.HR_TARIKH_FINALISED_HR = DateTime.Now;
                    //HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(model.HR_NO_PEKERJA);
                    //if (sAnsuran.ElementAt(0) != null && Kod == "00030")
                    //{
                    //    foreach (HR_MAKLUMAT_KEWANGAN8 ansuran in sAnsuran.ElementAt(0))
                    //    {
                    //        List<PA_PELARASAN> pelarasan2 = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == tMula && s.PA_BULAN == PA_BULAN && s.PA_TAHUN == PA_TAHUN && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                    //        spg.PA_PELARASAN.RemoveRange(pelarasan2);
                    //        spg.SaveChanges();
                    //        var no = 0;
                    //        foreach (HR_POTONGAN potongan in sPotongan)
                    //        {
                    //            if (potongan.HR_NILAI == null)
                    //            {
                    //                potongan.HR_NILAI = 0;
                    //            }

                    //            potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));

                    //            if (no == 0)
                    //            {
                    //                //potongan.HR_NILAI = -potongan.HR_NILAI;
                    //                if (potongan.HR_NILAI <= 0)
                    //                {
                    //                    //potongan
                    //                    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                    //                }
                    //                else
                    //                {
                    //                    //tunggakan
                    //                    potongan.HR_KOD_POTONGAN = tggkk.HR_KOD_UPAH;
                    //                }
                    //            }

                    //            string jenis = null;
                    //            string vot = null;
                    //            string singkatan = null;
                    //            string laporan = null;

                    //            HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                    //            if (salary != null)
                    //            {
                    //                jenis = "G";
                    //                laporan = "G";
                    //                singkatan = salary.HR_SINGKATAN;
                    //                vot = salary.HR_VOT_UPAH;
                    //            }
                    //            else
                    //            {
                    //                HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                    //                if (ellowance2 != null)
                    //                {
                    //                    jenis = "E";

                    //                    singkatan = ellowance2.HR_SINGKATAN;
                    //                    vot = ellowance2.HR_VOT_ELAUN;
                    //                }

                    //                HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                    //                if (deduction != null)
                    //                {
                    //                    jenis = "P";
                    //                    singkatan = deduction.HR_SINGKATAN;
                    //                    vot = deduction.HR_VOT_POTONGAN;
                    //                }

                    //                List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN || s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN).ToList();
                    //                if (ellowance.Count() > 0)
                    //                {
                    //                    laporan = "E";
                    //                }
                    //                else
                    //                {
                    //                    if (gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN)
                    //                    {
                    //                        laporan = "P";
                    //                    }
                    //                    else
                    //                    {
                    //                        laporan = "G";
                    //                    }
                    //                }

                    //            }

                    //            PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(model.HR_BULAN) && s.PA_TAHUN == Convert.ToInt16(model.HR_TAHUN) && s.PA_KOD_PELARASAN == potongan.HR_KOD_POTONGAN);
                    //            if (pelarasan == null)
                    //            {
                    //                pelarasan = new PA_PELARASAN();
                    //                pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                    //                pelarasan.PA_BULAN = Convert.ToInt32(model.HR_BULAN);
                    //                pelarasan.PA_TAHUN = Convert.ToInt16(model.HR_TAHUN);
                    //                pelarasan.PA_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                    //                pelarasan.PA_PERATUS = 0;
                    //                pelarasan.PA_NILAI = potongan.HR_NILAI;
                    //                pelarasan.PA_NILAI_MAKSIMUM = 0;
                    //                pelarasan.PA_NILAI_MINIMUM = 0;
                    //                pelarasan.PA_JENIS_PELARASAN = jenis;

                    //                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                    //                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                    //                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                    //                pelarasan.PA_PROSES_IND = "T";

                    //                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                    //                pelarasan.PA_SINGKATAN = singkatan;
                    //                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                    //                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                    //                pelarasan.PA_LAPORAN_IND = laporan;
                    //                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                    //                spg.PA_PELARASAN.Add(pelarasan);
                    //            }
                    //            else
                    //            {
                    //                pelarasan.PA_PERATUS = 0;
                    //                pelarasan.PA_NILAI = potongan.HR_NILAI;
                    //                pelarasan.PA_NILAI_MAKSIMUM = 0;
                    //                pelarasan.PA_NILAI_MINIMUM = 0;
                    //                pelarasan.PA_JENIS_PELARASAN = jenis;

                    //                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                    //                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                    //                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                    //                pelarasan.PA_PROSES_IND = "T";

                    //                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                    //                pelarasan.PA_SINGKATAN = singkatan;
                    //                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                    //                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                    //                pelarasan.PA_LAPORAN_IND = laporan;
                    //                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                    //                spg.Entry(pelarasan).State = EntityState.Modified;
                    //            }

                    //            spg.SaveChanges();
                    //            no++;

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (Kod == "00026")
                    //    {
                    //        pekerjaan.HR_GAJI_IND = "Y";
                    //        db.Entry(pekerjaan).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "00025")
                    //    {
                    //        pekerjaan.HR_GAJI_IND = "N";
                    //        db.Entry(pekerjaan).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "TP")
                    //    {
                    //        pekerjaan.HR_GAJI_IND = "T";
                    //        db.Entry(pekerjaan).State = EntityState.Modified;

                    //        HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.Find(model.HR_NO_PEKERJA);
                    //        peribadi2.HR_AKTIF_IND = "T";
                    //        db.Entry(peribadi2).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "00022")
                    //    {
                    //        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "Y";
                    //        db.Entry(pekerjaan).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "00037")
                    //    {
                    //        pekerjaan.HR_TANGGUH_GERAKGAJI_IND = "T";
                    //        db.Entry(pekerjaan).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "00036")
                    //    {
                    //        pekerjaan.HR_GRED = Convert.ToString(cariGred(null, modelDetail.HR_GRED).ORDINAL);
                    //        pekerjaan.HR_MATRIKS_GAJI = modelDetail.HR_MATRIKS_GAJI;
                    //        pekerjaan.HR_GAJI_POKOK = modelDetail.HR_GAJI_BARU;
                    //        pekerjaan.HR_KOD_GAJI = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_KOD_GAJI;
                    //        pekerjaan.HR_SISTEM = cariMatriks(modelDetail.HR_GRED, pekerjaan.HR_MATRIKS_GAJI, pekerjaan.HR_GAJI_POKOK).HR_SISTEM_SARAAN;
                    //        db.Entry(pekerjaan).State = EntityState.Modified;
                    //    }

                    //    if (Kod == "CUTI")
                    //    {
                    //        if (model.HR_KOD_PERUBAHAN == "00018")
                    //        {
                    //            pekerjaan.HR_GAJI_IND = "T";
                    //            db.Entry(pekerjaan).State = EntityState.Modified;
                    //        }

                    //    }

                    //    if (Kod == "00024" || Kod == "00039")
                    //    {
                    //        model.HR_NP_FINALISED_PA = model.HR_NP_FINALISED_HR;
                    //        model.HR_TARIKH_FINALISED_PA = DateTime.Now;
                    //        model.HR_FINALISED_IND_PA = "Y";
                    //    }

                    //    if (Kod == "00036" || Kod == "00031" || Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017") || Kod == "00015" || Kod == "00024" || Kod == "00039")
                    //    {
                    //        List<PA_PELARASAN> pelarasan2 = spg.PA_PELARASAN.Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_TARIKH_MULA == tMula && s.PA_BULAN == PA_BULAN && s.PA_TAHUN == PA_TAHUN && s.HR_KEW8_ID == model.HR_KEW8_ID).ToList();
                    //        spg.PA_PELARASAN.RemoveRange(pelarasan2);
                    //        spg.SaveChanges();
                    //        var no = 0;
                    //        foreach (HR_POTONGAN potongan in sPotongan)
                    //        {
                    //            if (potongan.HR_NILAI == null)
                    //            {
                    //                potongan.HR_NILAI = 0;
                    //            }

                    //            if (Kod == "00031")
                    //            {
                    //                potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                    //                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                    //            }
                    //            else if (Kod == "00039" || Kod == "00024")
                    //            {
                    //                if (Kod == "00024")
                    //                {
                    //                    HR_ELAUN cariElaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == modelDetail.HR_KOD_PELARASAN);
                    //                    if (cariElaun == null)
                    //                    {
                    //                        cariElaun = new HR_ELAUN();
                    //                    }
                    //                    if (model.HR_KEW8_IND == "E")
                    //                    {

                    //                        if (cariElaun.HR_KOD_TUNGGAKAN != null)
                    //                        {
                    //                            potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_TUNGGAKAN;
                    //                        }
                    //                        else
                    //                        {
                    //                            potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        if (cariElaun.HR_KOD_POTONGAN != null)
                    //                        {
                    //                            potongan.HR_KOD_POTONGAN = cariElaun.HR_KOD_POTONGAN;
                    //                        }
                    //                        else
                    //                        {
                    //                            potongan.HR_KOD_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    HR_POTONGAN cariPotongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == modelDetail.HR_KOD_PELARASAN);
                    //                    if (cariPotongan == null)
                    //                    {
                    //                        cariPotongan = new HR_POTONGAN();
                    //                    }
                    //                    potongan.HR_KOD_POTONGAN = cariPotongan.HR_KOD_POTONGAN;
                    //                }


                    //                potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                    //            }
                    //            else
                    //            {
                    //                potongan.HR_KOD_POTONGAN = potongan.HR_KOD_POTONGAN;
                    //                if (Kod == "00036")
                    //                {
                    //                    potongan.HR_NILAI = Convert.ToDecimal(potongan.HR_NILAI);
                    //                }
                    //                else
                    //                {
                    //                    potongan.HR_NILAI = -Math.Abs(Convert.ToDecimal(potongan.HR_NILAI));
                    //                }
                    //            }

                    //            //if (no == 0 && Kod != "00031")
                    //            //{
                    //            //    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                    //            //}

                    //            if (no == 0 && (Kod != "00031" && Kod != "00039" && Kod != "00024"))
                    //            {
                    //                //potongan.HR_NILAI = -potongan.HR_NILAI;
                    //                if (potongan.HR_NILAI <= 0)
                    //                {
                    //                    //potongan
                    //                    potongan.HR_KOD_POTONGAN = potongan2.HR_KOD_POTONGAN;
                    //                }
                    //                else
                    //                {
                    //                    //tunggakan
                    //                    potongan.HR_KOD_POTONGAN = tggkk.HR_KOD_UPAH;
                    //                }
                    //            }

                    //            //HR_POTONGAN pot = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                    //            //if (pot == null)
                    //            //{
                    //            //    pot = new HR_POTONGAN();
                    //            //}

                    //            //HR_GAJI_UPAHAN gUpah = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                    //            //if(gUpah == null)
                    //            //{
                    //            //    gUpah = new HR_GAJI_UPAHAN();
                    //            //}



                    //            string jenis = null;
                    //            string vot = null;
                    //            string singkatan = null;
                    //            string laporan = null;
                    //            //HR_MAKLUMAT_ELAUN_POTONGAN potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E" && db.HR_ELAUN.Where(e => e.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN && e.HR_KOD_ELAUN == s.HR_KOD_ELAUN_POTONGAN).Count() > 0);
                    //            //if (potonganInd == null)
                    //            //{
                    //            //    potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                    //            //    potonganInd = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G" && db.HR_GAJI_UPAHAN.Where(e => e.HR_KOD_UPAH == s.HR_KOD_ELAUN_POTONGAN && e.HR_VOT_UPAH == pot.HR_VOT_POTONGAN).Count() > 0);
                    //            //    if (potonganInd == null)
                    //            //    {
                    //            //        potonganInd = new HR_MAKLUMAT_ELAUN_POTONGAN();
                    //            //    }
                    //            //}

                    //            HR_GAJI_UPAHAN salary = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == potongan.HR_KOD_POTONGAN);
                    //            if (salary != null)
                    //            {
                    //                jenis = "G";
                    //                laporan = "G";
                    //                singkatan = salary.HR_SINGKATAN;
                    //                vot = salary.HR_VOT_UPAH;
                    //            }
                    //            else
                    //            {
                    //                HR_ELAUN ellowance2 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN);
                    //                if (ellowance2 != null)
                    //                {
                    //                    jenis = "E";

                    //                    singkatan = ellowance2.HR_SINGKATAN;
                    //                    vot = ellowance2.HR_VOT_ELAUN;
                    //                }

                    //                HR_POTONGAN deduction = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);
                    //                if (deduction != null)
                    //                {
                    //                    jenis = "P";
                    //                    singkatan = deduction.HR_SINGKATAN;
                    //                    vot = deduction.HR_VOT_POTONGAN;
                    //                }

                    //                List<HR_ELAUN> ellowance = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == potongan.HR_KOD_POTONGAN || s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN).ToList();
                    //                if (ellowance.Count() > 0)
                    //                {
                    //                    laporan = "E";
                    //                }
                    //                else
                    //                {
                    //                    if (gajiUpah.HR_VOT_UPAH != deduction.HR_VOT_POTONGAN || Kod == "00036")
                    //                    {
                    //                        laporan = "P";
                    //                    }
                    //                    else
                    //                    {
                    //                        laporan = "G";
                    //                    }
                    //                }

                    //            }


                    //            //if (Kod == "00031")
                    //            //{
                    //            //    jenis = "E";

                    //            //    singkatan = ellowance.HR_SINGKATAN;
                    //            //}
                    //            //else
                    //            //{  
                    //            //    pelarasan.PA_JENIS_PELARASAN = "P";
                    //            //}

                    //            if (Kod == "00039" || Kod == "00024")
                    //            {
                    //                HR_MAKLUMAT_ELAUN_POTONGAN ElaunPotongan4 = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == modelDetail.HR_KOD_PELARASAN);

                    //                var aktifind = "T";
                    //                if (model.HR_KEW8_IND == "E" && model.HR_TARIKH_MULA <= DateTime.Now && model.HR_TARIKH_AKHIR >= DateTime.Now)
                    //                {
                    //                    aktifind = "Y";
                    //                }

                    //                if (ElaunPotongan4 == null)
                    //                {
                    //                    ElaunPotongan4 = new HR_MAKLUMAT_ELAUN_POTONGAN();
                    //                    ElaunPotongan4.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                    //                    ElaunPotongan4.HR_KOD_ELAUN_POTONGAN = modelDetail.HR_KOD_PELARASAN;
                    //                    ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                    //                    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = jenis;
                    //                    ElaunPotongan4.HR_MOD_BAYARAN = "1";
                    //                    ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                    //                    ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                    //                    ElaunPotongan4.HR_AKTIF_IND = aktifind;
                    //                    ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                    //                    ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                    //                    ElaunPotongan4.HR_UBAH_IND = "B";
                    //                    ElaunPotongan4.HR_NP_KEYIN = peribadi.HR_NO_PEKERJA;
                    //                    ElaunPotongan4.HR_NP_UBAH = peribadi.HR_NO_PEKERJA;
                    //                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(ElaunPotongan4);
                    //                }
                    //                else
                    //                {

                    //                    ElaunPotongan4.HR_JUMLAH = modelDetail.HR_JUMLAH_PERUBAHAN_ELAUN;
                    //                    ElaunPotongan4.HR_ELAUN_POTONGAN_IND = jenis;
                    //                    ElaunPotongan4.HR_MOD_BAYARAN = "1";
                    //                    ElaunPotongan4.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
                    //                    ElaunPotongan4.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
                    //                    ElaunPotongan4.HR_AKTIF_IND = aktifind;
                    //                    ElaunPotongan4.HR_TARIKH_KEYIN = DateTime.Now;
                    //                    ElaunPotongan4.HR_TARIKH_UBAH = DateTime.Now;
                    //                    ElaunPotongan4.HR_UBAH_IND = "K";
                    //                    ElaunPotongan4.HR_NP_UBAH = peribadi.HR_NO_PEKERJA;
                    //                    db.Entry(ElaunPotongan4).State = EntityState.Modified;
                    //                }
                    //            }

                    //            PA_PELARASAN pelarasan = spg.PA_PELARASAN.AsEnumerable().SingleOrDefault(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && s.PA_BULAN == Convert.ToInt32(model.HR_BULAN) && s.PA_TAHUN == Convert.ToInt16(model.HR_TAHUN) && s.PA_KOD_PELARASAN == potongan.HR_KOD_POTONGAN);
                    //            if (pelarasan == null)
                    //            {
                    //                pelarasan = new PA_PELARASAN();
                    //                pelarasan.PA_NO_PEKERJA = model.HR_NO_PEKERJA;
                    //                pelarasan.PA_BULAN = Convert.ToInt32(model.HR_BULAN);
                    //                pelarasan.PA_TAHUN = Convert.ToInt16(model.HR_TAHUN);
                    //                pelarasan.PA_KOD_PELARASAN = potongan.HR_KOD_POTONGAN;
                    //                pelarasan.PA_PERATUS = 0;
                    //                pelarasan.PA_NILAI = potongan.HR_NILAI;
                    //                pelarasan.PA_NILAI_MAKSIMUM = 0;
                    //                pelarasan.PA_NILAI_MINIMUM = 0;
                    //                pelarasan.PA_JENIS_PELARASAN = jenis;

                    //                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                    //                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                    //                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                    //                pelarasan.PA_PROSES_IND = "T";

                    //                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                    //                pelarasan.PA_SINGKATAN = singkatan;
                    //                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                    //                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                    //                pelarasan.PA_LAPORAN_IND = laporan;
                    //                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                    //                spg.PA_PELARASAN.Add(pelarasan);
                    //            }
                    //            else
                    //            {
                    //                pelarasan.PA_PERATUS = 0;
                    //                pelarasan.PA_NILAI = potongan.HR_NILAI;
                    //                pelarasan.PA_NILAI_MAKSIMUM = 0;
                    //                pelarasan.PA_NILAI_MINIMUM = 0;
                    //                pelarasan.PA_JENIS_PELARASAN = jenis;

                    //                DateTime tKeyIn = Convert.ToDateTime(model.HR_TARIKH_KEYIN);

                    //                pelarasan.PA_TARIKH_MULA = new DateTime(tKeyIn.Year, tKeyIn.Month, 1);
                    //                pelarasan.PA_TARIKH_AKHIR = new DateTime(tKeyIn.Year, tKeyIn.Month + 1, 1).AddDays(-1);

                    //                pelarasan.PA_PROSES_IND = "T";

                    //                pelarasan.PA_VOT_PELARASAN = "11-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN + "-" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_UNIT + "-" + vot;
                    //                pelarasan.PA_SINGKATAN = singkatan;
                    //                pelarasan.PA_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
                    //                pelarasan.PA_TARIKH_PROSES = DateTime.Now;

                    //                pelarasan.PA_LAPORAN_IND = laporan;
                    //                pelarasan.HR_KEW8_ID = modelDetail.HR_KEW8_ID;
                    //                spg.Entry(pelarasan).State = EntityState.Modified;
                    //            }

                    //            spg.SaveChanges();
                    //            no++;

                    //        }

                    //    }
                    //}
                    //end 22-7-2018
                }

                var redirect = RedirectLink(Kod);

                var txtMss = "Data berjaya dikemaskini";
                if (model.HR_FINALISED_IND_HR == "Y")
                {
                    txtMss += " dan dimuktamadkan";
                }

                return Json(new { error = false, msg = txtMss, location = "../Kewangan8/" + redirect + "?key=1&value=" + model.HR_NO_PEKERJA }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.Kod = Kod;
            ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                foreach (var item in elaunPotongan)
                {
                    // && item.HR_TARIKH_AKHIR >= DateTime.Now
                    if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                        if (elaun4.HR_PERATUS_IND == "Y")
                        {
                            item.HR_JUMLAH = model.HR_GAJI_LAMA * (item.HR_JUMLAH / 100);
                        }
                        elaun3.Add(elaun4);

                    }
                    if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                    {
                        HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                        potongan3.Add(potongan4);

                    }
                }

            }

            if (Kod == "00031")
            {
                ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            }

            if (Kod == "00039")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList(), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
                }

            }

            if (Kod == "00024")
            {
                if (model.HR_KEW8_IND == "E")
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }
                else
                {
                    ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList(), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
                }

            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if(namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            decimal? gaji = 0;
            if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK != null)
            {
                gaji = pekerja.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            ViewBag.gaji = gaji;

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if (transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            return Json(new { error = false, msg = "Data tidak berjaya dikemaskini" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PadamKew8(int? id, string HR_NO_PEKERJA, string HR_KOD_PERUBAHAN, string HR_TARIKH_MULA, string Kod)
        {
            ViewBag.Kod = Kod;
            var date = Convert.ToDateTime(HR_TARIKH_MULA);
            if (id == null || HR_NO_PEKERJA == null || HR_KOD_PERUBAHAN == null || HR_TARIKH_MULA == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_KEWANGAN8 model = db.HR_MAKLUMAT_KEWANGAN8.SingleOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            List<HR_MAKLUMAT_KEWANGAN8> tarikhMulaAkhir = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).OrderBy(s => s.HR_TARIKH_MULA).ToList();
            if (model == null)
            {
                return HttpNotFound();
            }

            if (Kod == "CUTI" && model.HR_KOD_PERUBAHAN == "00017")
            {
                model.HR_TARIKH_MULA = tarikhMulaAkhir.FirstOrDefault().HR_TARIKH_MULA;
                model.HR_TARIKH_AKHIR = tarikhMulaAkhir.LastOrDefault().HR_TARIKH_AKHIR;
            }

            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
            if (Detail2 == null)
            {
                Detail2 = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                Detail2.HR_JUMLAH_PERUBAHAN_ELAUN = 0;
                Detail2.HR_JUMLAH_PERUBAHAN = 0;
            }
            int? gredDetail = Convert.ToInt32(Detail2.HR_GRED);

            ViewBag.HR_JUMLAH_PERUBAHAN_ELAUN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN_ELAUN));
            ViewBag.HR_JUMLAH_PERUBAHAN = Math.Abs(Convert.ToDecimal(Detail2.HR_JUMLAH_PERUBAHAN));
            ViewBag.HR_JENIS_PERGERAKAN = Detail2.HR_JENIS_PERGERAKAN;

            HR_MATRIKS_GAJI matriksDetail2 = cariMatriks(cariGred(gredDetail, null).SHORT_DESCRIPTION, Detail2.HR_MATRIKS_GAJI, Detail2.HR_GAJI_BARU);

            ViewBag.HR_GRED = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_BARU = matriksDetail2.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_BARU = matriksDetail2.HR_GAJI_MAX;
            ViewBag.HR_GAJI_BARU = Detail2.HR_GAJI_BARU;
            ViewBag.HR_MATRIKS_GAJI = Detail2.HR_MATRIKS_GAJI;

            if (Kod == "TMK" || Kod == "00032" || Kod == "00004")
            {
                HR_MAKLUMAT_PERIBADI mPeribadi4 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == Detail2.HR_NO_PEKERJA_PT);
                if (mPeribadi4 == null)
                {
                    mPeribadi4 = new HR_MAKLUMAT_PERIBADI();
                }

                ViewBag.HR_GAJI_MIN_BARU = Detail2.HR_GAJI_BARU;
                ViewBag.HR_NAMA_PEKERJA_PT = mPeribadi4.HR_NAMA_PEKERJA;
                ViewBag.HR_NO_PEKERJA_PT = Detail2.HR_NO_PEKERJA_PT;
                ViewBag.HR_PGT_BARU = matriksDetail2.HR_RM_KENAIKAN;
                ViewBag.HR_JAWATAN_BARU = Detail2.HR_JAWATAN_BARU;
                ViewBag.HR_KOD_PELARASAN = Detail2.HR_KOD_PELARASAN;
            }

            ViewBag.HR_PENERANGAN = "";
            var kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == model.HR_KOD_PERUBAHAN);
            if (kewangan8 != null)
            {
                ViewBag.HR_PENERANGAN = kewangan8.HR_PENERANGAN;
            }

            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
            int? gredPekerjaan = Convert.ToInt32(model.HR_GRED_LAMA);
            HR_MATRIKS_GAJI matriksPekerjaan = cariMatriks(cariGred(gredPekerjaan, null).SHORT_DESCRIPTION, model.HR_MATRIKS_GAJI_LAMA, model.HR_GAJI_LAMA);

            //var kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);
            ViewBag.HR_KOD_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_GRED_LAMA = cariGred(gredPekerjaan, null).SHORT_DESCRIPTION;
            ViewBag.HR_GAJI_MIN_LAMA = matriksPekerjaan.HR_GAJI_MIN;
            ViewBag.HR_GAJI_MAX_LAMA = matriksPekerjaan.HR_GAJI_MAX;
            ViewBag.HR_GAJI_LAMA = model.HR_GAJI_LAMA;
            ViewBag.HR_SISTEM = mPekerjaan.HR_SISTEM;
            ViewBag.HR_PGT_LAMA = matriksPekerjaan.HR_RM_KENAIKAN;
            ViewBag.kodGaji = matriksPekerjaan.HR_KOD_GAJI;

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == model.HR_KOD_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;

            //int? kodGred = Convert.ToInt32(mPekerjaan.HR_GRED);

            //ViewBag.gred = cariGred(kodGred, null).SHORT_DESCRIPTION;
            //if(cariGred(gredDetail, null).SHORT_DESCRIPTION != null)
            //{
            //    ViewBag.gred = cariGred(gredDetail, null).SHORT_DESCRIPTION;
            //}

            //ViewBag.kodGaji = mPekerjaan.HR_KOD_GAJI;
            //ViewBag.gaji = mPekerjaan.HR_GAJI_POKOK;

            HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == model.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
            if (gajiUpah == null)
            {
                gajiUpah = new HR_GAJI_UPAHAN();
            }

            HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
            if (potongan2 == null)
            {
                potongan2 = new HR_POTONGAN();
            }
            ViewBag.kodPGaji = potongan2.HR_KOD_POTONGAN;

            var jawatan_ind = "";

            if (mPekerjaan.HR_KAKITANGAN_IND == "Y")
            {
                jawatan_ind = "K" + mPekerjaan.HR_TARAF_JAWATAN;
            }
            else if (mPekerjaan.HR_KAKITANGAN_IND == "T")
            {
                jawatan_ind = "P" + mPekerjaan.HR_TARAF_JAWATAN;
            }

            HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
            if (gajiUpah == null)
            {
                tggkk = new HR_GAJI_UPAHAN();
            }
            ViewBag.kodTGaji = tggkk.HR_KOD_UPAH;

            List<HR_MAKLUMAT_KEWANGAN8> listAnsuran = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).ToList();
            var bilAnsuran = listAnsuran.Count();
            if (listAnsuran.Count() <= 1)
            {
                bilAnsuran = 1;
            }

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            ViewBag.nilaiPGaji = 0;
            ViewBag.nilaiPotongan = 0;
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA).ToList();
            if (elaunPotongan.Count() > 0)
            {
                decimal? jumElaun = 0;
                decimal? jumAwam = 0;
                foreach (var item in elaunPotongan)
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0004" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (elaun != null)
                    {
                        jumElaun = item.HR_JUMLAH;
                    }
                    HR_ELAUN awam = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == "K0003" && s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if (awam != null)
                    {
                        jumAwam = item.HR_JUMLAH;
                    }

                    if (Kod == "00030" || Kod == "00026" || Kod == "TP" || (Kod == "CUTI") || Kod == "00015" || Kod == "LNTKN")
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            if (elaun4 != null)
                            {
                                HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_TUNGGAKAN);
                                }
                                else
                                {
                                    Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == elaun4.HR_KOD_POTONGAN);
                                }

                                if (Detail == null)
                                {
                                    Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                                }
                                if (Detail.HR_JUMLAH_PERUBAHAN == null)
                                {
                                    Detail.HR_JUMLAH_PERUBAHAN = 0;
                                }
                                if (elaun4.HR_PERATUS_IND == "Y")
                                {
                                    item.HR_JUMLAH = model.HR_GAJI_LAMA * (item.HR_JUMLAH / 100);
                                }
                                elaun4.HR_NILAI = item.HR_JUMLAH;
                                elaun3.Add(elaun4);


                                HR_POTONGAN potongan4 = new HR_POTONGAN();
                                if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                                {
                                    potongan4.HR_KOD_POTONGAN = elaun4.HR_KOD_TUNGGAKAN;
                                    potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                    var peneranganElaun = "";
                                    HR_ELAUN elaunTunggakan = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == elaun4.HR_KOD_TUNGGAKAN);
                                    if (elaunTunggakan != null)
                                    {
                                        peneranganElaun = elaunTunggakan.HR_PENERANGAN_ELAUN;
                                    }
                                    potongan4.HR_PENERANGAN_POTONGAN = peneranganElaun;
                                }
                                else
                                {
                                    //potongan4.HR_KOD_POTONGAN = Detail.HR_KOD_PELARASAN;
                                    HR_POTONGAN potongan6 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_KOD_POTONGAN == Detail.HR_KOD_PELARASAN);
                                    if (potongan6 == null)
                                    {
                                        potongan6 = new HR_POTONGAN();
                                    }
                                    potongan4 = potongan6;
                                }
                                potongan4.HR_NILAI = Detail.HR_JUMLAH_PERUBAHAN;
                                ViewBag.nilaiPotongan += Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                                potongan3.Add(potongan4);
                            }
                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "G" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_MAKLUMAT_KEWANGAN8_DETAIL Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            if ((Kod == "00026" && model.HR_KEW8_IND == "T") || (Kod == "LNTKN" && model.HR_LANTIKAN_IND == "T"))
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == tggkk.HR_KOD_UPAH);
                            }
                            else
                            {
                                Detail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date && s.HR_KOD_PELARASAN == potongan2.HR_KOD_POTONGAN);
                            }

                            if (Detail == null)
                            {
                                Detail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                            }
                            if (Detail.HR_JUMLAH_PERUBAHAN == null)
                            {
                                Detail.HR_JUMLAH_PERUBAHAN = 0;
                            }
                            ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(Detail.HR_JUMLAH_PERUBAHAN)) * bilAnsuran;
                        }
                    }
                    else
                    {
                        // && item.HR_TARIKH_AKHIR >= DateTime.Now
                        if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                            elaun4.HR_NILAI = item.HR_JUMLAH;
                            elaun3.Add(elaun4);

                        }
                        if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                        {
                            HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                            potongan3.Add(potongan4);

                        }
                    }
                }
                ViewBag.nilaiPotongan += ViewBag.nilaiPGaji;
                if (Kod == "00030" && model.HR_KEW8_IND == "A")
                {
                    ViewBag.nilaiPGaji = model.HR_BIL;
                    //ViewBag.nilaiPotongan = model.HR_BIL;
                }

                ViewBag.elaun3 = elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList();
                ViewBag.potongan3 = potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList();
                ViewBag.itp = jumElaun;
                ViewBag.awam = jumAwam;
            }

            HR_MAKLUMAT_ELAUN_POTONGAN kodG = elaunPotongan.FirstOrDefault(s => s.HR_ELAUN_POTONGAN_IND == "G");
            if (kodG == null)
            {
                kodG = new HR_MAKLUMAT_ELAUN_POTONGAN();
            }
            ViewBag.kodG = kodG.HR_KOD_ELAUN_POTONGAN;

            //if (HR_KOD_PERUBAHAN == "00030")
            //{
            //    List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date).ToList<HR_MAKLUMAT_KEWANGAN8_DETAIL>();
            //    if (modelDetail.Count() > 0)
            //    {
            //        foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL item in modelDetail)
            //        {
            //            if(item.HR_JUMLAH_PERUBAHAN == null)
            //            {
            //                item.HR_JUMLAH_PERUBAHAN = 0;
            //            }
            //            if(item.HR_KOD_PELARASAN != potongan2.HR_KOD_POTONGAN)
            //            {
            //                HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                elaun3.Add(elaun4);

            //                HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_PELARASAN);
            //                potongan4.HR_NILAI = item.HR_JUMLAH_PERUBAHAN;
            //                potongan3.Add(potongan4);
            //            }
            //            else
            //            {
            //                ViewBag.nilaiPGaji = Math.Abs(Convert.ToDecimal(item.HR_JUMLAH_PERUBAHAN));
            //            }
            //        }
            //        ViewBag.elaun3 = elaun3;
            //        ViewBag.potongan3 = potongan3;

            //    }
            //}

            if (Kod == "00031" || Kod == "00039" || Kod == "00024")
            {
                HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == date);
                if (modelDetail == null)
                {
                    modelDetail = new HR_MAKLUMAT_KEWANGAN8_DETAIL();
                }

                ViewBag.kodPelarasan = modelDetail.HR_KOD_PELARASAN;

                if (Kod == "00031")
                {
                    ViewBag.HR_JUMLAH_PERUBAHAN = modelDetail.HR_JUMLAH_PERUBAHAN;
                    ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.Where(s => s.HR_KOD_KATEGORI == "K0015").OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                }

                if (Kod == "00039")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList(), "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN", modelDetail.HR_KOD_PELARASAN);
                    }
                }

                if (Kod == "00024")
                {
                    if (model.HR_KEW8_IND == "E")
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                    else
                    {
                        ViewBag.HR_KOD_PELARASAN = new SelectList(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList(), "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN", modelDetail.HR_KOD_PELARASAN);
                    }
                }
            }

            if (Kod == "TP")
            {
                ViewBag.HR_PENERANGAN = "TAMAT PERKHIDMATAN";
            }

            if (Kod == "00026")
            {
                ViewBag.HR_TARIKH_TAMAT = null;
                if (mPekerjaan.HR_TARAF_JAWATAN == "T")
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT);
                    }

                }
                else
                {
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_TARIKH_TAMAT = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_TAMAT_KONTRAK);
                    }

                }
            }

            List<SelectListItem> pengesahan = new List<SelectListItem>();
            pengesahan.Add(new SelectListItem { Value = "T", Text = "Tidak Aktif" });
            pengesahan.Add(new SelectListItem { Value = "P", Text = "Proses" });
            pengesahan.Add(new SelectListItem { Value = "Y", Text = "Muktamad" });
            ViewBag.pengesahan = pengesahan;

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ToList();
            ViewBag.sPegawai = mPeribadi;
            HR_MAKLUMAT_PERIBADI namaPegawai = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == model.HR_NP_FINALISED_HR);
            if (namaPegawai == null)
            {
                namaPegawai = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.HR_NAMA_PEGAWAI = namaPegawai.HR_NAMA_PEKERJA;

            HR_MAKLUMAT_PERIBADI pengesahan2 = mPeribadi.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y");
            if (pengesahan2 == null)
            {
                pengesahan2 = new HR_MAKLUMAT_PERIBADI();
            }
            ViewBag.pengesahan2 = pengesahan2.HR_NO_PEKERJA;

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;

            ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");

            if (Kod == "kew8")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00002" || s.HR_KOD_KEW8 == "00003" || s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00005" || s.HR_KOD_KEW8 == "00006" || s.HR_KOD_KEW8 == "00007" || s.HR_KOD_KEW8 == "00008" || s.HR_KOD_KEW8 == "00009" || s.HR_KOD_KEW8 == "00010" || s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018" || s.HR_KOD_KEW8 == "00023" || s.HR_KOD_KEW8 == "00027" || s.HR_KOD_KEW8 == "00028" || s.HR_KOD_KEW8 == "00039" || s.HR_KOD_KEW8 == "00040" || s.HR_KOD_KEW8 == "00042" || s.HR_KOD_KEW8 == "00044" || s.HR_KOD_KEW8 == "00045").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TP")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00011" || s.HR_KOD_KEW8 == "00014" || s.HR_KOD_KEW8 == "00016" || s.HR_KOD_KEW8 == "00020" || s.HR_KOD_KEW8 == "00035" || s.HR_KOD_KEW8 == "00044").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "CUTI")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00017" || s.HR_KOD_KEW8 == "00018").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "LNTKN")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00013" || s.HR_KOD_KEW8 == "00015" || s.HR_KOD_KEW8 == "00027").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }
            if (Kod == "TMK")
            {
                ViewBag.HR_KOD_PERUBAHAN = new SelectList(db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 == "00004" || s.HR_KOD_KEW8 == "00032").OrderBy(s => s.HR_PENERANGAN), "HR_KOD_KEW8", "HR_PENERANGAN");
            }

            ViewBag.SENARAI_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            //decimal? gaji = 0;
            //if (mPekerjaan.HR_GAJI_POKOK != null)
            //{
            //    gaji = mPekerjaan.HR_GAJI_POKOK;
            //}
            //ViewBag.HR_GAJI_LAMA = gaji;
            //if(Detail2.HR_GAJI_BARU != null)
            //{
            //    ViewBag.gaji = Detail2.HR_GAJI_BARU;
            //}
            List<GE_PARAMTABLE> gredList2 = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            List<GE_PARAMTABLE> gredList = new List<GE_PARAMTABLE>();
            foreach (GE_PARAMTABLE sGred in gredList2)
            {
                HR_JADUAL_GAJI jGaji = db.HR_JADUAL_GAJI.AsEnumerable().FirstOrDefault(s => s.HR_AKTIF_IND == "Y" && s.HR_GRED_GAJI.Trim() == sGred.SHORT_DESCRIPTION.Trim() || s.HR_GRED_GAJI == Detail2.HR_GRED);
                if (jGaji != null)
                {
                    gredList.Add(sGred);
                }

            }
            ViewBag.gredList = gredList.OrderBy(s => s.SHORT_DESCRIPTION).ToList();

            DateTime tarikhBulanLepas = DateTime.Now.AddMonths(-1);
            var tarikhBulanLepas2 = "01/" + tarikhBulanLepas.Month + "/" + tarikhBulanLepas.Year;
            DateTime tarikhBulanLepas3 = Convert.ToDateTime(tarikhBulanLepas2);

            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.AsEnumerable().Where(s => s.PA_NO_PEKERJA == model.HR_NO_PEKERJA && Convert.ToDateTime("01/" + s.PA_BULAN_GAJI + "/" + s.PA_TAHUN_GAJI) <= tarikhBulanLepas3).OrderByDescending(s => s.PA_TAHUN_GAJI).ThenByDescending(s => s.PA_BULAN_GAJI).FirstOrDefault();
            if (transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                transaksi.PA_GAJI_BERSIH = 0;
            }

            ViewBag.GajiBersih = transaksi.PA_GAJI_BERSIH;

            HR_PERATUS_KWSP PeratusKWSP = db.HR_PERATUS_KWSP.FirstOrDefault();
            if (PeratusKWSP == null)
            {
                PeratusKWSP = new HR_PERATUS_KWSP();
            }
            ViewBag.HR_KOD_PERATUS = PeratusKWSP.HR_KOD_PERATUS;
            ViewBag.HR_NILAI_PERATUS = PeratusKWSP.HR_NILAI_PERATUS;

            return PartialView("_PadamKew8", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamKew8(HR_MAKLUMAT_KEWANGAN8 model, HR_MAKLUMAT_KEWANGAN8_DETAIL modelDetail, string Kod, HR_MAKLUMAT_KEWANGAN8[][] sAnsuran)
        {
            var redirect = RedirectLink(Kod);

            List<HR_MAKLUMAT_KEWANGAN8> padamModel = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_ANSURAN_ID == model.HR_ANSURAN_ID && s.HR_NO_PEKERJA == model.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == model.HR_KOD_PERUBAHAN).ToList();
            List<HR_MAKLUMAT_KEWANGAN8_DETAIL> padamDetail = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

            foreach (HR_MAKLUMAT_KEWANGAN8 padam in padamModel)
            {
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> modelDetail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_KEW8_ID == padam.HR_KEW8_ID && s.HR_NO_PEKERJA == padam.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == padam.HR_KOD_PERUBAHAN).ToList();
                padamDetail.AddRange(modelDetail2);
            }

            if (model == null)
            {
                return HttpNotFound();
            }

            if(ModelState.IsValid)
            {
                db.HR_MAKLUMAT_KEWANGAN8.RemoveRange(padamModel);
                db.HR_MAKLUMAT_KEWANGAN8_DETAIL.RemoveRange(padamDetail);
                db.SaveChanges();

                return Json(new { error = false, msg = "Data berjaya dipadam", location = "../Kewangan8/" + redirect + "?key=1&value=" + model.HR_NO_PEKERJA }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, msg = "Data tidak berjaya dipadam" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GanjaranKontrak(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00031");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TahanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();  
            mPeribadi = CariPekerja(key, value, null, "00025");
            
            ViewBag.key = key;
            ViewBag.value = value;
            
            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult PotonganGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00030");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult BayarGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00026");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TamatPerkhidmatan(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "TP");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TanggungMemangkuKerja(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "TMK");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TanggungKerja(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00032");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult MemangkuKerja(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00004");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult TangguhPergerakanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00022");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }
        public ActionResult SambungPergerakanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00037");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult PindaanGaji(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00036");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SambungKontrak(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00015");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SemuaJenisElaun(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00024");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult Perlantikan(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "LNTKN");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult Cuti(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "CUTI");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public ActionResult SemuaJenisPotongan(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value, null, "00039");

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public JsonResult JumlahPerubahan(string HR_NO_PEKERJA, int? HR_JUMLAH_BULAN, decimal? HR_NILAI_EPF, decimal? HR_NILAI_PERATUS)
        {
            double? item = 0.00;
            var mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);
            if(mPekerjaan != null)
            {
                var HR_GAJI_POKOK = Convert.ToDouble(mPekerjaan.HR_GAJI_POKOK);
                var EPF = Convert.ToDouble(HR_NILAI_EPF);
                var PERATUS = Convert.ToDouble(HR_NILAI_PERATUS / 100);
                item = ((HR_GAJI_POKOK * PERATUS) * HR_JUMLAH_BULAN) - EPF;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LaporanKewangan8(int? bulan, int? tahun, string status, string kod)
        {
            //var model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(DateTime.Now.Month, DateTime.Now.Year);
            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == null)
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == DateTime.Now.Month && s.HR_TAHUN == DateTime.Now.Year).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }

            if(kod != null)
            {
                model = model.Where(s => s.HR_KOD_PERUBAHAN == kod).ToList();
            }

            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.kew8 = db.HR_KEWANGAN8.ToList();

            ViewBag.bulan = bulan;
            if (bulan == null)
            {
                ViewBag.bulan = DateTime.Now.Month;
            }

            ViewBag.year = tahun;
            if (tahun == null)
            {
                ViewBag.year = DateTime.Now.Year;
            }

            ViewBag.status = status;
            if (status == null)
            {
                ViewBag.status = "S";
            }

            List<SelectListItem> Bulan = new List<SelectListItem>();
            Bulan.Add(new SelectListItem { Text = "Januari", Value = "1" });
            Bulan.Add(new SelectListItem { Text = "Febuari", Value = "2" });
            Bulan.Add(new SelectListItem { Text = "Mac", Value = "3" });
            Bulan.Add(new SelectListItem { Text = "April", Value = "4" });
            Bulan.Add(new SelectListItem { Text = "Mei", Value = "5" });
            Bulan.Add(new SelectListItem { Text = "Jun", Value = "6" });
            Bulan.Add(new SelectListItem { Text = "Julai", Value = "7" });
            Bulan.Add(new SelectListItem { Text = "Ogos", Value = "8" });
            Bulan.Add(new SelectListItem { Text = "September", Value = "9" });
            Bulan.Add(new SelectListItem { Text = "Oktober", Value = "10" });
            Bulan.Add(new SelectListItem { Text = "November", Value = "11" });
            Bulan.Add(new SelectListItem { Text = "Disember", Value = "12" });
            ViewBag.month = Bulan;
            return View(model);
        }

        public ActionResult LaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {
            return LaporanKewangan8(bulan, tahun, status, null);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LaporanKewangan8Bulanan(List<HR_MAKLUMAT_KEWANGAN8> model, int? bulan, int? tahun, string status, HR_MAKLUMAT_KEWANGAN8[][] sAnsuran, string jenis)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.Where(s => s.HR_NO_KPBARU == User.Identity.Name && s.HR_AKTIF_IND == "Y").FirstOrDefault();

            foreach (HR_MAKLUMAT_KEWANGAN8 item in model)
            {
                List<HR_MAKLUMAT_KEWANGAN8> SMK8 = new List<HR_MAKLUMAT_KEWANGAN8>();
                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> SMK8D = new List<HR_MAKLUMAT_KEWANGAN8_DETAIL>();

                HR_MAKLUMAT_PERIBADI pekerja = mPeribadi.SingleOrDefault(s => s.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA == item.HR_NO_PEKERJA);
                

                HR_GAJI_UPAHAN gajiUpah = db.HR_GAJI_UPAHAN.FirstOrDefault(s => db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(g => g.HR_KOD_ELAUN_POTONGAN == s.HR_KOD_UPAH && g.HR_NO_PEKERJA == item.HR_NO_PEKERJA && g.HR_ELAUN_POTONGAN_IND == "G").Count() > 0);
                if (gajiUpah == null)
                {
                    gajiUpah = new HR_GAJI_UPAHAN();
                }

                var jawatan_ind = "";
                if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    jawatan_ind = "K" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }
                else if (pekerja.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    jawatan_ind = "P" + pekerja.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                }

                HR_GAJI_UPAHAN tggkk = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_JAWATAN_IND == jawatan_ind && s.HR_SINGKATAN == "TGGAJ");
                if (tggkk == null)
                {
                    tggkk = new HR_GAJI_UPAHAN();
                }

                HR_POTONGAN potongan2 = db.HR_POTONGAN.FirstOrDefault(s => s.HR_SINGKATAN == "PGAJI" && s.HR_VOT_POTONGAN == gajiUpah.HR_VOT_UPAH);
                if (potongan2 == null)
                {
                    potongan2 = new HR_POTONGAN();
                }

                
                HR_MAKLUMAT_KEWANGAN8 kew8 = new HR_MAKLUMAT_KEWANGAN8();
                if (status == "S")
                {
                    kew8 = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item.HR_KEW8_ID);
                }
                else
                {
                    kew8 = db.HR_MAKLUMAT_KEWANGAN8.FirstOrDefault(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item.HR_KEW8_ID);
                }
                SMK8.Add(kew8);

                List<HR_MAKLUMAT_KEWANGAN8_DETAIL> kew8Detail2 = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_TARIKH_MULA == item.HR_TARIKH_MULA && s.HR_KOD_PERUBAHAN == item.HR_KOD_PERUBAHAN && s.HR_KEW8_ID == item.HR_KEW8_ID).ToList();
                SMK8D.AddRange(kew8Detail2);
                HR_MAKLUMAT_KEWANGAN8_DETAIL kew8Detail = kew8Detail2.FirstOrDefault();

                var kod = item.HR_KOD_PERUBAHAN;
                if ((item.HR_KOD_PERUBAHAN == "00001" || item.HR_KOD_PERUBAHAN == "00002" || item.HR_KOD_PERUBAHAN == "00003" || item.HR_KOD_PERUBAHAN == "00004" || item.HR_KOD_PERUBAHAN == "00005" || item.HR_KOD_PERUBAHAN == "00006" || item.HR_KOD_PERUBAHAN == "00007" || item.HR_KOD_PERUBAHAN == "00008" || item.HR_KOD_PERUBAHAN == "00009" || item.HR_KOD_PERUBAHAN == "00010" || item.HR_KOD_PERUBAHAN == "00013" || item.HR_KOD_PERUBAHAN == "00015" || item.HR_KOD_PERUBAHAN == "00017" || item.HR_KOD_PERUBAHAN == "00018" || item.HR_KOD_PERUBAHAN == "00023" || item.HR_KOD_PERUBAHAN == "00027" || item.HR_KOD_PERUBAHAN == "00028" || item.HR_KOD_PERUBAHAN == "00039" || item.HR_KOD_PERUBAHAN == "00040" || item.HR_KOD_PERUBAHAN == "00042" || item.HR_KOD_PERUBAHAN == "00044" || item.HR_KOD_PERUBAHAN == "00045") && kew8Detail2.Count() <= 0)
                {
                    kod = "kew8";
                }

                if (item.HR_KOD_PERUBAHAN == "00011" || item.HR_KOD_PERUBAHAN == "000014" || item.HR_KOD_PERUBAHAN == "000016" || item.HR_KOD_PERUBAHAN == "000020" || item.HR_KOD_PERUBAHAN == "000035" || item.HR_KOD_PERUBAHAN == "000044")
                {
                    kod = "TP";
                }

                if (item.HR_KOD_PERUBAHAN == "00017" || item.HR_KOD_PERUBAHAN == "00018")
                {
                    kod = "CUTI";
                }

                if (item.HR_KOD_PERUBAHAN == "00013" || item.HR_KOD_PERUBAHAN == "00015" || item.HR_KOD_PERUBAHAN == "00027")
                {
                    kod = "LNTKN";
                }

                //if (item.HR_KOD_PERUBAHAN == "00004" || item.HR_KOD_PERUBAHAN == "00032")
                //{
                //    kod = "TMK";
                //}

                List<HR_POTONGAN> potongan = new List<HR_POTONGAN>();

                foreach(HR_MAKLUMAT_KEWANGAN8_DETAIL detail in kew8Detail2)
                {
                    if(detail.HR_KOD_PELARASAN != null && (detail.HR_JUMLAH_PERUBAHAN != null ))
                    {
                        HR_POTONGAN p = new HR_POTONGAN();
                        p.HR_KOD_POTONGAN = detail.HR_KOD_PELARASAN;
                        p.HR_NILAI = detail.HR_JUMLAH_PERUBAHAN;
                        potongan.Add(p);
                    }
                }

                if (kew8.HR_FINALISED_IND_HR != "Y" && jenis == "Muktamad")
                {
                    kew8.HR_UBAH_IND = "0";
                    kew8.HR_FINALISED_IND_HR = "Y";
                    kew8.HR_NP_FINALISED_HR = peribadi.HR_NO_PEKERJA;

                    db.Entry(kew8).State = EntityState.Modified;
                    db.SaveChanges();

                    HR_MAKLUMAT_KEWANGAN8[][] ansuran = null;
                    if (kod == "00030")
                    {
                        ansuran = sAnsuran;
                    }
                    
                    //if(kod == "00001")
                    //{
                    //    Muktamad(kew8, kew8Detail, null, kod, potongan, gajiUpah, potongan2, tggkk, pekerja, null, item.HR_TARIKH_MULA, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                        
                    //}
                    //else
                    //{
                    //    Muktamad(kew8, kew8Detail, ansuran, kod, potongan, gajiUpah, potongan2, tggkk, pekerja, peribadi, item.HR_TARIKH_MULA, kew8.HR_BULAN, Convert.ToInt16(kew8.HR_TAHUN));
                    //}
                    Muktamad2(SMK8, SMK8D, kod, gajiUpah, tggkk, potongan2);
                }

                if(jenis == "Padam")
                {
                    kew8.HR_NP_UBAH_HR = peribadi.HR_NO_PEKERJA;
                    kew8.HR_TARIKH_UBAH_HR = DateTime.Now;
                    PadamMuktamad(kew8, kew8Detail2, kod);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
            //return LaporanKewangan8(bulan, tahun, status, null);
        }

        public ActionResult LaporanTanggungKerja(int? bulan, int? tahun, string status)
        {
            return LaporanKewangan8(bulan, tahun, status, "00032");
        }

        [HttpPost]
        public FileStreamResult PDFLaporanKewangan8(int? bulan, int? tahun, string status, string kod)
        {
            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            if (kod != null)
            {
                model = model.Where(s => s.HR_KOD_PERUBAHAN == kod).ToList();
            }

            List<HR_KEWANGAN8> sKewangan8 = new List<HR_KEWANGAN8>();

            foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == pekerja.HR_KOD_PERUBAHAN);
                if(kewangan8 == null)
                {
                    kewangan8 = new HR_KEWANGAN8();
                }
                sKewangan8.Add(kewangan8);
            }

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();

            var html = "<html><head>";
            html += "<title>Senarai Pergerakan Gaji</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
            int newNo2 = 0;
            foreach (HR_KEWANGAN8 kew in sKewangan8.GroupBy(s => s.HR_KOD_KEW8).Select(s => s.FirstOrDefault()))
            {
                string breakPage2 = "";
                if (newNo2 != 0)
                {
                    breakPage2 = "page-break-before:always";
                }
                newNo2++;
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='"+ breakPage2 + "'>";
                html += "<tr>";
                html += "<td valign='top' height='40' style='font-size: 80%;'>";
                html += "<strong>KEWANGAN 8 :  "+kew.HR_KOD_KEW8+ "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + kew.HR_PENERANGAN+"</strong>";
                html += "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td style='font-size: 80%' align='center'>";
                
                var no = 0;
                var newNo = 0;
                foreach (var l in model.Join(db.HR_MAKLUMAT_PERIBADI, HR_MAKLUMAT_KEWANGAN8 => HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA, HR_MAKLUMAT_PERIBADI => HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA, (HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI) => new { HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI }).OrderBy(s => s.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA))
                {
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    string HR_TARIKH_MULA = "TIADA";
                    string HR_TARIKH_AKHIR = "TIADA";
                    string HR_AKTIF_IND = "TIADA";

                    HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G");
                    if(elaunPotongan != null)
                    {
                        HR_TARIKH_MULA = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_MULA);
                        HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_AKHIR);
                        HR_AKTIF_IND = elaunPotongan.HR_AKTIF_IND;
                    }

                    if (l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN == kew.HR_KOD_KEW8)
                    {
                        string breakPage = "";
                        if (newNo >= 2)
                        {
                            newNo = 0;
                            document.NewPage();
                            breakPage = "page-break-before:always";
                        }
                        newNo++;
                        GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        html += "<table width='100%' cellpadding='5' cellspacing='0' style='"+ breakPage + "'>";
                        ++no;
                        html += "<tr>";
                        html += "<td colspan='5' style='font-size: 80%'>" + kew.HR_KOD_KEW8 + "&nbsp;&nbsp;&nbsp;&nbsp;" + kew.HR_PENERANGAN + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='35%' valign='top' rowspan='2' style='font-size: 80%'><strong>" + peribadi.HR_NAMA_PEKERJA + "</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>MATRIKS GAJI</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI + "</td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>TARIKH MULA</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + HR_TARIKH_MULA + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='12%' style='font-size: 80%'><strong>GAJI POKOK</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>RM " + string.Format("{0:#,0.00}",peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) + "</td>";
                        html += "<td width='12%' style='font-size: 80%'><strong>TARIKH TAMAT</strong></td>";
                        html += "<td width='12%' style='font-size: 80%'>" + HR_TARIKH_AKHIR + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td width='35%' valign='top' style='font-size: 80%'><strong>NO PEKERJA : </strong>&nbsp;&nbsp;&nbsp;" + peribadi.HR_NO_PEKERJA + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><strong>STATUS GAJI<br/>(HR)</strong></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><strong>STATUS GEPC</strong></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + HR_AKTIF_IND + "</td>";
                        html += "</tr>";

                        //html += "<tr>";
                        //html += "<td align='center' style='font-size: 80%'>" + no + "</td>";
                        //html += "<td style='font-size: 80%'>" + l.HR_NO_PEKERJA + "</td>";
                        //html += "<td style='font-size: 80%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                        //html += "<td align='center' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_TARIKH_KEYIN) + "</td>";
                        //html += "<td style='font-size: 80%'>" + jabatan.GE_KETERANGAN_JABATAN + "</td>";
                        //html += "<td style='font-size: 80%'>" + jawatan.HR_NAMA_JAWATAN + "</td>";
                        //html += "</tr>";
                        html += "</table>";

                        html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                        html += "<tr>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>TARIKH MULA</u></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>TARIKH AKHIR</u></td>";
                        html += "<td width='35%' style='font-size: 80%'><u>BUTIR PERUBAHAN</u></td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'><u>FINALISED</u></td>";
                        html += "</tr>";
                        html += "<tr>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA) + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR) + "</td>";
                        html += "<td width='35%' style='font-size: 80%'>" + l.HR_MAKLUMAT_KEWANGAN8.HR_BUTIR_PERUBAHAN + "</td>";
                        html += "<td width='12%' valign='top' style='font-size: 80%'>" + l.HR_MAKLUMAT_KEWANGAN8.HR_FINALISED_IND_HR + "</td>";
                        html += "</tr>";
                        html += "</table>";

                        html += "<table width='100%' cellpadding='5' cellspacing='0'>";
                        html += "<tr>";
                        html += "<td width='5%' style='font-size: 80%'><u>KOD</u></td>";
                        html += "<td width='35%' style='font-size: 80%'><u>KETERANGAN</u></td>";
                        html += "<td align='right' width='10%' style='font-size: 80%'><u>JUMLAH</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>GAJI BARU</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>STATUS GEPC</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>TARIKH MULA</u></td>";
                        html += "<td align='center' width='10%' style='font-size: 80%'><u>TARIKH AKHIR</u></td>";
                        html += "</tr>";
                        

                        List<HR_MAKLUMAT_KEWANGAN8_DETAIL> sDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA).ToList();
                        if (sDetail.Count > 0)
                        {
                            foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in sDetail)
                            {
                                string HR_KETERANGAN = null;
                                HR_GAJI_UPAHAN gaji = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN);
                                if(gaji == null)
                                {
                                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                                    if(elaun == null)
                                    {
                                        HR_POTONGAN potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == detail.HR_KOD_PELARASAN);
                                        if(potongan == null)
                                        {
                                            HR_CARUMAN caruman = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == detail.HR_KOD_PELARASAN);
                                            if(caruman != null)
                                            {
                                                HR_KETERANGAN = caruman.HR_PENERANGAN_CARUMAN;
                                            }
                                        }
                                        else
                                        {
                                            HR_KETERANGAN = potongan.HR_PENERANGAN_POTONGAN;
                                        }
                                    }
                                    else
                                    {
                                        HR_KETERANGAN = elaun.HR_PENERANGAN_ELAUN;
                                    }
                                }
                                else
                                {
                                    HR_KETERANGAN = gaji.HR_PENERANGAN_UPAH;
                                }
                                

                                html += "<tr>";
                                html += "<td width='6%' style='font-size: 80%'>" + detail.HR_KOD_PELARASAN + "</td>";
                                html += "<td width='35%' style='font-size: 80%'>" + HR_KETERANGAN + "</td>";
                                html += "<td align='right' width='10%' style='font-size: 80%'>" + string.Format("{0:#,0.00}", detail.HR_JUMLAH_PERUBAHAN) + "</td>";
                                html += "<td align='right' width='10%' style='font-size: 80%'>" + string.Format("{0:#,0.00}", detail.HR_GAJI_BARU) + "</td>";
                                html += "<td width='10%' style='font-size: 80%'></td>";
                                html += "<td width='10%' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA) + "</td>";
                                html += "<td width='10%' style='font-size: 80%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR) + "</td>";
                                html += "</tr>";
                            }
                        }
                        html += "</table>";
                        html += "<br/>";
                        html += "<br/>";
                        html += "<hr/>";
                    }
                }
                html += "</td>";
                html += "</tr>";
                html += "</table>";
                html += "<br/>";
            }

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            var input = new MemoryStream(bytes);
            
            var xmlWorker = XMLWorkerHelper.GetInstance();
            //string imagepath = Server.MapPath("~/Content/img/logo-o.png");

            var associativeArray = new Dictionary<int?, string>() { { 1, "Januari" }, { 2, "Febuari" }, { 3, "Mac" }, { 4, "Appril" }, { 5, "Mei" }, { 6, "Jun" }, { 7, "Julai" }, { 8, "Ogos" }, { 9, "september" }, { 10, "Oktober" }, { 11, "November" }, { 12, "Disember" } };
            var Bulan = "";
            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    Bulan = m.Value;
                }

            }

            //iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
            iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Paragraph tarikhCetak = new iTextSharp.text.Paragraph("Tarikh Cetak:   " + string.Format("{0:dd/MM/yyyy}", DateTime.Now), contentFont);
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI KEWANGAN 8 BULAN "+ Bulan.ToUpper() +" TAHUN "+ tahun);
            //iTextSharp.text.Paragraph paragraph2 = new iTextSharp.text.Paragraph("Bulan                       " + Bulan, contentFont);
            //iTextSharp.text.Paragraph paragraph3 = new iTextSharp.text.Paragraph("Tahun                       " + tahun, contentFont);
            paragraph.Alignment = Element.ALIGN_CENTER;
            //pic.ScaleToFit(100f, 80f);
            //pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
            //pic.IndentationRight = 30f;
            //pic.SpacingBefore = 9f;
            //paragraph.SpacingBefore = 10f;
            //paragraph2.SpacingBefore = 10f;
            //pic.BorderWidthTop = 36f;
            //paragraph2.SetLeading(20f, 0);
            //document.Add(pic);
            tarikhCetak.IndentationRight = 100f;
            tarikhCetak.Alignment = Element.ALIGN_RIGHT;
            document.Add(tarikhCetak);
            document.Add(paragraph);
            //document.Add(paragraph2);
            //document.Add(paragraph3);
            document.Add(new iTextSharp.text.Paragraph("\n"));
            //document.Add(new iTextSharp.text.Paragraph("\n"));

            //PdfPTable table = new PdfPTable(3);
            //PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns"));
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            //table.AddCell(cell);
            //table.AddCell("Col 1 Row 1");
            //table.AddCell("Col 2 Row 1");
            //table.AddCell("Col 3 Row 1");
            //table.AddCell("Col 1 Row 2");
            //table.AddCell("Col 2 Row 2");
            //table.AddCell("Col 3 Row 2");
            //document.Add(table);

            xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

            iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Paragraph paragraph4 = new iTextSharp.text.Paragraph("Copyright © " + DateTime.Now.Year + " Sistem Bandaraya Petaling Jaya. All Rights Reserved\nUser Id: " + User.Identity.Name.ToLower() + " - Tarikh print: " + DateTime.Now.ToString("dd-MM-yyyy"), contentFont2);
            document.Add(paragraph4);

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
            
        }

        [HttpPost]
        public FileResult EXCLaporanKewangan8(int? bulan, int? tahun, string status, string kod)
        {
            List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();
            if (status == "S")
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            else
            {
                model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun && s.HR_FINALISED_IND_HR == status).OrderBy(s => s.HR_KOD_PERUBAHAN).ThenByDescending(s => s.HR_TARIKH_MULA).ToList<HR_MAKLUMAT_KEWANGAN8>();
            }
            if (kod != null)
            {
                model = model.Where(s => s.HR_KOD_PERUBAHAN == kod).ToList();
            }
            List<HR_KEWANGAN8> sKewangan8 = new List<HR_KEWANGAN8>();

            foreach (HR_MAKLUMAT_KEWANGAN8 pekerja in model)
            {
                HR_KEWANGAN8 kewangan8 = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == pekerja.HR_KOD_PERUBAHAN);
                if (kewangan8 == null)
                {
                    kewangan8 = new HR_KEWANGAN8();
                }
                sKewangan8.Add(kewangan8);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (HR_KEWANGAN8 kew in sKewangan8.GroupBy(s => s.HR_KOD_KEW8).Select(s => s.FirstOrDefault()))
                {
                    var cellleft = 1;
                    var cellright = 3;
                    var ws = wb.Worksheets.Add("KEWANGAN8 " + kew.HR_KOD_KEW8);

                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.AdjustTo(80);
                    ws.PageSetup.PaperSize = XLPaperSize.A3Paper;
                    ws.PageSetup.VerticalDpi = 400;
                    ws.PageSetup.HorizontalDpi = 400;

                    var imagePath = Server.MapPath("~/Content/img/logo-mbpj.gif");
                    var image = ws.AddPicture(imagePath)
                        .MoveTo(ws.Cell("B" + cellleft).Address)
                        .Scale(0.1); // optional: resize picture
                    ws.Cell("A" + cellleft).Value = "MAJLIS BANDARAYA PETALING JAYA\nSENARAI KEWANGAN 8\nMBPJ";
                    ws.Cell("A" + cellleft).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Cell("A" + cellleft).Style.Font.FontSize = 12;
                    ws.Cell("A" + cellleft).Style.Font.Bold = true;
                    ws.Range("A" + cellleft + ":I" + cellright).Merge();

                    cellleft += 5;
                    cellright += 3;

                    ws.Cell("A" + cellleft).Value = "KEWANGAN 8 :   " + kew.HR_KOD_KEW8 + "      " + kew.HR_PENERANGAN;
                    ws.Cell("A" + cellleft).Style.Font.Bold = true;
                    ws.Range("A" + cellleft + ":I" + cellright).Merge();


                    int tblNo = 0;
                    foreach (var l in model.Join(db.HR_MAKLUMAT_PERIBADI, HR_MAKLUMAT_KEWANGAN8 => HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA, HR_MAKLUMAT_PERIBADI => HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA, (HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI) => new { HR_MAKLUMAT_KEWANGAN8, HR_MAKLUMAT_PERIBADI }).OrderBy(s => s.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA))
                    {
                        HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA);
                        if (peribadi == null)
                        {
                            peribadi = new HR_MAKLUMAT_PERIBADI();
                        }

                        string HR_TARIKH_MULA = "TIADA";
                        string HR_TARIKH_AKHIR = "TIADA";
                        string HR_AKTIF_IND = "TIADA";

                        HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G");
                        if (elaunPotongan != null)
                        {
                            HR_TARIKH_MULA = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_MULA);
                            HR_TARIKH_AKHIR = string.Format("{0:dd/MM/yyyy}", elaunPotongan.HR_TARIKH_AKHIR);
                            HR_AKTIF_IND = elaunPotongan.HR_AKTIF_IND;
                        }

                        if (l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN == kew.HR_KOD_KEW8)
                        {
                            GE_JABATAN jabatan = mc.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                            if (jabatan == null)
                            {
                                jabatan = new GE_JABATAN();
                            }
                            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                            if (jawatan == null)
                            {
                                jawatan = new HR_JAWATAN();
                            }

                            ws.Column(1).Width = 10;
                            ws.Column(2).Width = 5;
                            ws.Column(3).Width = 15;
                            ws.Column(4).Width = 60;
                            ws.Column(5).Width = 15;
                            ws.Column(6).Width = 15;
                            ws.Column(7).Width = 15;
                            ws.Column(8).Width = 15;
                            ws.Column(9).Width = 15;

                            cellleft += 2;
                            cellright += 2;

                            ws.Cell("A" + cellleft).Value = kew.HR_KOD_KEW8 + "  " + kew.HR_PENERANGAN;
                            ws.Cell("A" + cellleft).Style.Font.Bold = true;
                            ws.Range("A" + cellleft + ":I" + cellright).Merge();

                            cellleft += 1;
                            cellright += 2;


                            ws.Cell("A" + cellleft).Value = peribadi.HR_NAMA_PEKERJA;
                            ws.Cell("A" + cellleft).Style.Font.Bold = true;
                            ws.Range("A" + cellleft + ":D" + cellright).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                            ws.Cell("E" + cellleft).Value = "MATRIKS GAJI";
                            ws.Cell("E" + cellleft).Style.Font.Bold = true;

                            ws.Cell("F" + cellleft).Value = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;

                            ws.Cell("G" + cellleft).Value = "TARIKH MULA";
                            ws.Cell("G" + cellleft).Style.Font.Bold = true;

                            ws.Cell("H" + cellleft).Value = HR_TARIKH_MULA;
                            ws.Cell("H" + cellleft).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            cellleft += 1;
                            //cellright += 1;


                            ws.Cell("E" + cellleft).Value = "GAJI POKOK";
                            ws.Cell("E" + cellleft).Style.Font.Bold = true;

                            ws.Cell("F" + cellleft).Value = string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);

                            ws.Cell("G" + cellleft).Value = "TARIKH TAMAT";
                            ws.Cell("G" + cellleft).Style.Font.Bold = true;

                            ws.Cell("H" + cellleft).Value = HR_TARIKH_AKHIR;
                            ws.Cell("H" + cellleft).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            cellleft += 1;
                            cellright += 1;

                            ws.Cell("A" + cellleft).Value = "NO PEKERJA :    " + peribadi.HR_NO_PEKERJA;
                            ws.Cell("A" + cellleft).Style.Font.Bold = true;
                            ws.Range("A" + cellleft + ":D" + cellright).Merge();

                            ws.Cell("E" + cellleft).Value = "STATUS GAJI\n(HR)";
                            ws.Cell("E" + cellleft).Style.Font.Bold = true;

                            ws.Cell("F" + cellleft).Value = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND;

                            ws.Cell("G" + cellleft).Value = "STATUS GEPC";
                            ws.Cell("G" + cellleft).Style.Font.Bold = true;

                            ws.Cell("H" + cellleft).Value = HR_AKTIF_IND;

                            cellleft += 2;
                            cellright += 2;

                            ws.Cell("A" + cellleft).Value = "TARIKH MULA";
                            ws.Cell("A" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;
                            ws.Range("A" + cellleft + ":B" + cellright).Merge();

                            ws.Cell("C" + cellleft).Value = "TARIKH AKHIR";
                            ws.Cell("C" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                            ws.Cell("D" + cellleft).Value = "BUTIR PERUBAHAN";
                            ws.Cell("D" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                            ws.Cell("E" + cellleft).Value = "MUKTAMAD";
                            ws.Cell("E" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                            cellleft += 1;
                            cellright += 1;

                            ws.Cell("A" + cellleft).Value = string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA);
                            ws.Range("A" + cellleft + ":B" + cellright).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                            ws.Cell("C" + cellleft).Value = string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR);
                            ws.Cell("C" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                            ws.Cell("D" + cellleft).Value = l.HR_MAKLUMAT_KEWANGAN8.HR_BUTIR_PERUBAHAN;
                            ws.Cell("D" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                            ws.Cell("E" + cellleft).Value = l.HR_MAKLUMAT_KEWANGAN8.HR_FINALISED_IND_HR;
                            ws.Cell("E" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                            //ws.Column(1).Width = 15;
                            //ws.Column(2).Width = 15;
                            //ws.Column(3).Width = 60;
                            //ws.Column(4).Width = 15;

                            //DataTable dt = new DataTable("A"+tblNo.ToString());

                            //dt.Columns.Add("TARIKH MULA");
                            //dt.Columns.Add("TARIKH AKHIR");
                            //dt.Columns.Add("BUTIR PERUBAHAN");
                            //dt.Columns.Add("MUKTAMAD");

                            //dt.Rows.Add(string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA), string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR), l.HR_MAKLUMAT_KEWANGAN8.HR_BUTIR_PERUBAHAN, l.HR_MAKLUMAT_KEWANGAN8.HR_FINALISED_IND_HR);

                            //cellleft += 2;
                            //cellright += 2;
                            //ws.Cell(cellleft, 1).InsertTable(dt.AsEnumerable());

                            cellleft += 2;
                            cellright += 2;

                            List<HR_MAKLUMAT_KEWANGAN8_DETAIL> sDetail = db.HR_MAKLUMAT_KEWANGAN8_DETAIL.Where(s => s.HR_NO_PEKERJA == l.HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == l.HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN && s.HR_TARIKH_MULA == l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA).ToList();
                            if (sDetail.Count > 0)
                            {
                                //ws.Column(1).Width = 10;
                                //ws.Column(2).Width = 60;
                                //ws.Column(3).Width = 15;
                                //ws.Column(4).Width = 15;
                                //ws.Column(5).Width = 15;
                                //ws.Column(6).Width = 15;
                                //ws.Column(7).Width = 15;

                                //DataTable dt2 = new DataTable("B" + tblNo.ToString());
                                //dt2.Columns.Add("KOD");
                                //dt2.Columns.Add("KETERANGAN");
                                //dt2.Columns.Add("JUMLAH");
                                //dt2.Columns.Add("GAJI BARU");
                                //dt2.Columns.Add("STATUS GEPC");
                                //dt2.Columns.Add("TARIKH MULA");
                                //dt2.Columns.Add("TARIKH AKHIR");

                                ws.Cell("A" + cellleft).Value = "KOD";
                                ws.Cell("A" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                ws.Cell("B" + cellleft).Value = "KETERANGAN";
                                ws.Cell("B" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;
                                ws.Range("B" + cellleft + ":D" + cellright).Merge();


                                ws.Cell("E" + cellleft).Value = "JUMLAH";
                                ws.Cell("E" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                ws.Cell("F" + cellleft).Value = "GAJI BARU";
                                ws.Cell("F" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                ws.Cell("G" + cellleft).Value = "STATUS GEPC";
                                ws.Cell("G" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                ws.Cell("H" + cellleft).Value = "TARIKH MULA";
                                ws.Cell("H" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                ws.Cell("I" + cellleft).Value = "TARIKH AKHIR";
                                ws.Cell("I" + cellleft).Style.Font.Underline = XLFontUnderlineValues.Single;

                                cellleft += 1;
                                cellright += 1;

                                //int cellleft2 = 0;
                                //int cellright2 = 0;
                                foreach (HR_MAKLUMAT_KEWANGAN8_DETAIL detail in sDetail)
                                {
                                    string HR_KETERANGAN = null;
                                    HR_GAJI_UPAHAN gaji = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == detail.HR_KOD_PELARASAN);
                                    if (gaji == null)
                                    {
                                        HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == detail.HR_KOD_PELARASAN);
                                        if (elaun == null)
                                        {
                                            HR_POTONGAN potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == detail.HR_KOD_PELARASAN);
                                            if (potongan == null)
                                            {
                                                HR_CARUMAN caruman = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == detail.HR_KOD_PELARASAN);
                                                if (caruman != null)
                                                {
                                                    HR_KETERANGAN = caruman.HR_PENERANGAN_CARUMAN;
                                                }
                                            }
                                            else
                                            {
                                                HR_KETERANGAN = potongan.HR_PENERANGAN_POTONGAN;
                                            }
                                        }
                                        else
                                        {
                                            HR_KETERANGAN = elaun.HR_PENERANGAN_ELAUN;
                                        }
                                    }
                                    else
                                    {
                                        HR_KETERANGAN = gaji.HR_PENERANGAN_UPAH;
                                    }
                                    //dt2.Rows.Add(detail.HR_KOD_PELARASAN, HR_KETERANGAN, string.Format("{0:#,0.00}", detail.HR_JUMLAH_PERUBAHAN), null, null, null, null);

                                    ws.Cell("A" + cellleft).Value = detail.HR_KOD_PELARASAN;

                                    ws.Cell("B" + cellleft).Value = HR_KETERANGAN;
                                    ws.Range("B" + cellleft + ":D" + cellright).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    ws.Cell("E" + cellleft).Value = string.Format("{0:#,0.00}", detail.HR_JUMLAH_PERUBAHAN);
                                    ws.Cell("E" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    ws.Cell("F" + cellleft).Value = string.Format("{0:#,0.00}", detail.HR_GAJI_BARU);
                                    ws.Cell("F" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    ws.Cell("G" + cellleft).Value = null;
                                    ws.Cell("G" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    ws.Cell("H" + cellleft).Value = string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA);
                                    ws.Cell("H" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    ws.Cell("I" + cellleft).Value = string.Format("{0:dd/MM/yyyy}", l.HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR);
                                    ws.Cell("I" + cellleft).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                                    cellleft++;
                                    cellright++;
                                }
                                cellleft += 2;
                                cellright += 2;
                                //ws.Cell(cellleft, 1).InsertTable(dt2.AsEnumerable());

                                //cellleft += cellleft2;
                                //cellright += cellright2;
                            }

                            cellleft += 2;
                            cellright += 2;
                        }
                        tblNo++;
                    }
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Laporan_Kewangan8.xlsx");
                }
            }
        }

        [HttpPost]
        public FileStreamResult PDFLaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {
            return PDFLaporanKewangan8(bulan, tahun, status, null);
        }

        [HttpPost]
        public FileResult EXCLaporanKewangan8Bulanan(int? bulan, int? tahun, string status)
        {
            return EXCLaporanKewangan8(bulan, tahun, status, null);
        }

        [HttpPost]
        public FileStreamResult PDFLaporanTanggungKerja(int? bulan, int? tahun, string status)
        {
            return PDFLaporanKewangan8(bulan, tahun, status, "00032");
        }

        [HttpPost]
        public FileResult EXCLaporanTanggungKerja(int? bulan, int? tahun, string status)
        {
            return EXCLaporanKewangan8(bulan, tahun, status, "00032");
        }

        public ActionResult GajiMin(string HR_GRED)
        {
            List<HR_JADUAL_GAJI> item = new List<HR_JADUAL_GAJI>();
            List<HR_JADUAL_GAJI> jadualGaji = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_GAJI_MIN).ToList();
            foreach(HR_JADUAL_GAJI item2 in jadualGaji)
            {
                List<HR_MATRIKS_GAJI> matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GAJI_MIN == item2.HR_GAJI_MIN).ToList();
                if (matriks.Count() > 0)
                {
                    item.Add(item2);
                }
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiMin2(string HR_GRED, decimal? HR_GAJI_MIN)
        {
            HR_JADUAL_GAJI item = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN == HR_GAJI_MIN).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_GAJI_MIN).FirstOrDefault();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiMax(string HR_GRED, decimal? HR_GAJI_MIN)
        {
            List<HR_MATRIKS_GAJI> item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN >= HR_GAJI_MIN).OrderByDescending(s => s.HR_GAJI_MAX).GroupBy(s => s.HR_GAJI_MAX).Select(s => s.FirstOrDefault()).ToList();
            if(item.Count() < 0)
            {
                item = new List<HR_MATRIKS_GAJI>();
                HR_MATRIKS_GAJI item2 = new HR_MATRIKS_GAJI();
                item2.HR_GAJI_MIN = 0;
                item2.HR_GAJI_MAX = 0;
                item2.HR_GAJI_POKOK = 0;
                item2.HR_RM_KENAIKAN = 0;
                item2.HR_PERINGKAT = 0;
                item2.HR_TAHAP = 0;

                item.Add(item2);

            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiPokok(string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX)
        {
            List<HR_MATRIKS_GAJI> item = new List<HR_MATRIKS_GAJI>();
            if(HR_GAJI_MIN != null && HR_GAJI_MAX != null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK >= HR_GAJI_MIN && s.HR_GAJI_POKOK <= HR_GAJI_MAX)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else if(HR_GAJI_MIN != null && HR_GAJI_MAX == null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK >= HR_GAJI_MIN)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else if (HR_GAJI_MIN == null && HR_GAJI_MAX != null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK <= HR_GAJI_MAX)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            return Json(item.OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).GroupBy(s => new { s.HR_GAJI_POKOK }).Select(s => s.FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GajiPokok2(string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX)
        {
            HR_MATRIKS_GAJI item = new HR_MATRIKS_GAJI();
            if (HR_GAJI_MIN != null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN == HR_GAJI_MIN).OrderBy(s => s.HR_TAHAP).FirstOrDefault();
            }
            else
            {
                item.HR_GAJI_MIN = 0;
                item.HR_GAJI_MAX = 0;
                item.HR_GAJI_POKOK = 0;
                item.HR_RM_KENAIKAN = 0;
                item.HR_PERINGKAT = 0;
                item.HR_TAHAP = 0;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KodMatriks(string HR_GRED, decimal? HR_GAJI_POKOK)
        {
            HR_MATRIKS_GAJI item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_POKOK == HR_GAJI_POKOK).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).FirstOrDefault();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PerubahanGaji(string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX, decimal? HR_GAJI_POKOK)
        //{
        //    HR_MATRIKS_GAJI item = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN == HR_GAJI_MIN && s.HR_GAJI_MAX == HR_GAJI_MAX && s.HR_GAJI_POKOK == HR_GAJI_POKOK).FirstOrDefault();
        //    if(item == null)
        //    {
        //        item = new HR_MATRIKS_GAJI();
        //    }
        //    return Json(item, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult JenisPergerakan(string HR_NO_PEKERJA, string HR_JENIS_PERGERAKAN)
        {
            PergerakanGajiModels item = new PergerakanGajiModels();
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_AKTIF_IND == "Y").SingleOrDefault();
            if(peribadi != null)
            {
                int? gred = null;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE sGred = mc.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                if (sGred == null)
                {
                    sGred = new GE_PARAMTABLE();
                }
                int? peringkat = null;
                decimal? tahap = null;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI != null)
                {
                    peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Trim();
                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(0, 1) == "P")
                    {
                        peringkat = Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(1, 1));
                    }
                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(2, 1) == "T" && peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.ToCharArray().Count() > 3)
                    {
                        tahap = Convert.ToDecimal(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI.Substring(3));
                    }
                }
                
                string pkt = "P" + peringkat;
                decimal? kenaikan = 0;
                decimal? gajiPokokBaru = 0;
                decimal? gajiPokokBaru2 = 0;
                decimal? gaji_maxsimum = 0;
                //kenaikan
                decimal? tunggakan = 0;
                decimal? jumPerubahan = 0;
                HR_JADUAL_GAJI jadualGaji = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == pkt);
                if (jadualGaji != null)
                {
                    kenaikan = jadualGaji.HR_RM_KENAIKAN;
                    gaji_maxsimum = jadualGaji.HR_GAJI_MAX;
                }

                gajiPokokBaru = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + kenaikan;

                if (gajiPokokBaru > gaji_maxsimum)
                {
                    gajiPokokBaru2 = gaji_maxsimum;
                }
                else
                {
                    gajiPokokBaru2 = gajiPokokBaru;
                }
                HR_MATRIKS_GAJI matriks = new HR_MATRIKS_GAJI();
                if (HR_JENIS_PERGERAKAN == "D")
                {
                    jumPerubahan = (DateTime.Now.Month - Convert.ToDateTime(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month) * kenaikan;
                    matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_GAJI_POKOK == gajiPokokBaru2).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                }
                else
                {
                    matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_GAJI_POKOK == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK).OrderByDescending(s => s.HR_TAHAP).FirstOrDefault();
                }

                if(matriks == null)
                {
                    matriks = new HR_MATRIKS_GAJI();
                    matriks.HR_GAJI_MIN = 0;
                    matriks.HR_GAJI_MAX = 0;
                    matriks.HR_GAJI_POKOK = 0;
                    matriks.HR_RM_KENAIKAN = 0;
                }

                if (HR_JENIS_PERGERAKAN != "D")
                {
                    jumPerubahan = matriks.HR_GAJI_POKOK - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                }

                tunggakan = matriks.HR_GAJI_POKOK - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;

                if (matriks != null)
                {
                    item.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                    item.HR_GRED = sGred.SHORT_DESCRIPTION;
                    item.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                    item.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    item.HR_GAJI_BARU = matriks.HR_GAJI_POKOK;
                    item.HR_MATRIKS_GAJI = "P" + matriks.HR_PERINGKAT + "T" + Convert.ToDouble(matriks.HR_TAHAP);
                    item.HR_JUMLAH_PERUBAHAN = jumPerubahan;
                    item.HR_KENAIKAN = tunggakan;
                }

                
            }
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PerubahanGaji(string HR_NO_PEKERJA, decimal? HR_GAJI_BARU)
        {
            decimal? item = 0;
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            if (peribadi != null)
            {
                item = HR_GAJI_BARU - peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobList()
        {
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_KPBARU == User.Identity.Name);
            List<HR_KEWANGAN8> ListQ8 = db.HR_KEWANGAN8.ToList();
            List<PergerakanGajiModels> item = new List<PergerakanGajiModels>();
            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = new List<HR_MAKLUMAT_KEWANGAN8>();
            foreach(HR_KEWANGAN8 Q8 in ListQ8)
            {
                
                if (Q8.HR_KOD_KEW8 == "00001")
                {
                    kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => (s.HR_FINALISED_IND_HR == "T" || s.HR_FINALISED_IND_HR == "P") && s.HR_NP_FINALISED_HR == peribadi.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_KEW8).GroupBy(s => s.HR_TARIKH_MULA).Select(s => s.FirstOrDefault()).ToList();
                }
                else
                {
                    kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => (s.HR_FINALISED_IND_HR == "T" || s.HR_FINALISED_IND_HR == "P") && s.HR_NP_FINALISED_HR == peribadi.HR_NO_PEKERJA && s.HR_KOD_PERUBAHAN == Q8.HR_KOD_KEW8).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();
                }
                
                string key = null;
                string value = null;
                int? bulan = null;
                foreach (var kew8 in kewangan8)
                {
                    HR_MAKLUMAT_PERIBADI peribadi2 = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA);
                    PergerakanGajiModels data = new PergerakanGajiModels();
                    data.HR_TARIKH_MULA = kew8.HR_TARIKH_MULA;
                    data.HR_NO_PEKERJA = kew8.HR_NO_PEKERJA;
                    if(Q8.HR_KOD_KEW8 == "00001")
                    {
                        data.COUNTLIST = kewangan8.Where(s => s.HR_TARIKH_MULA == kew8.HR_TARIKH_MULA).Count();
                    }
                    else
                    {
                        data.COUNTLIST = kewangan8.Where(s => s.HR_NO_PEKERJA == kew8.HR_NO_PEKERJA).Count();
                    }


                    key = "4";
                    bulan = Convert.ToDateTime(data.HR_TARIKH_MULA).Month;
                    if (data.COUNTLIST == 1)
                    {
                        key = "1";
                        value = data.HR_NO_PEKERJA;
                    }

                    if (Q8.HR_KOD_KEW8 == "00001")
                    {
                        data.HR_BUTIR_PERUBAHAN = "Seramai <strong>" + data.COUNTLIST + "orang </strong> yang belum muktamatkan pergerakan gaji!. <a href='../Kewangan8/PengesahanPergerakanGaji?key=" + key + "&value=" + value + "&bulan=" + bulan + "' class='display-normal'>Klik sini</a> untuk hantar pengesahan anda<br>" +
                                                                        "<span class='pull-right font-xs text-muted'>" +
                                                                            "<i>" + string.Format("{0:dd/MM/yyyy}", data.HR_TARIKH_MULA) + "</i>" +
                                                                        "</span>";
                    }
                    else
                    {
                        var link = Q8.HR_PENERANGAN.Replace(" ", "").ToLower();
                        data.HR_BUTIR_PERUBAHAN = "<strong>" + peribadi2.HR_NAMA_PEKERJA + "</strong> belum muktamatkan "+Q8.HR_PENERANGAN.ToLower()+"!. <a href='../Kewangan8/" + link + "?key=" + key + "&value=" + value + "' class='display-normal'>Klik sini</a> untuk hantar pengesahan anda<br>" +
                                                                        "<span class='pull-right font-xs text-muted'>" +
                                                                            "<i>" + string.Format("{0:dd/MM/yyyy}", data.HR_TARIKH_MULA) + "</i>" +
                                                                        "</span>";
                    }
                        
                    item.Add(data);
                }
            }
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JumlahPelarasan(string HR_NO_PEKERJA, string JENIS, string KOD)
        {
            db.Configuration.ProxyCreationEnabled = false;

            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);
            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
                peribadi.HR_MAKLUMAT_PEKERJAAN = new HR_MAKLUMAT_PEKERJAAN();
            }

            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach (HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotongan)
            {
                if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                {
                    HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    if(elaun4.HR_PERATUS_IND == "Y")
                    {
                        item.HR_JUMLAH = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK * (item.HR_JUMLAH / 100);
                    }
                    elaun4.HR_NILAI = item.HR_JUMLAH;
                    elaun3.Add(elaun4);
                }
            }

            if(JENIS == "T")
            {
                return Json(elaun3.Join(db.HR_ELAUN, ELAUN => ELAUN.HR_KOD_TUNGGAKAN, TUNGGAKAN => TUNGGAKAN.HR_KOD_ELAUN, (ELAUN, TUNGGAKAN) => new { ELAUN, KOD_PELARASAN = TUNGGAKAN.HR_KOD_ELAUN, PENERANGAN = TUNGGAKAN.HR_PENERANGAN_ELAUN }).OrderBy(s => s.ELAUN.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(elaun3.Join(db.HR_POTONGAN, ELAUN => ELAUN.HR_KOD_POTONGAN, POTONGAN => POTONGAN.HR_KOD_POTONGAN, (ELAUN, POTONGAN) => new { ELAUN, KOD_PELARASAN = POTONGAN.HR_KOD_POTONGAN, PENERANGAN = POTONGAN.HR_PENERANGAN_POTONGAN }).OrderBy(s => s.ELAUN.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
            } 
        }

        public ActionResult KodPelarasan(string HR_NO_PEKERJA, string Kod, string Value)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<HR_ELAUN> elaun3 = new List<HR_ELAUN>();
            List<HR_POTONGAN> potongan3 = new List<HR_POTONGAN>();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach (HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotongan)
            {
                if (item.HR_ELAUN_POTONGAN_IND == "E" && item.HR_AKTIF_IND == "Y")
                {
                    HR_ELAUN elaun4 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                    //elaun4.HR_NILAI = item.HR_JUMLAH;
                    elaun3.Add(elaun4);
                }
                if (item.HR_ELAUN_POTONGAN_IND == "P" && item.HR_AKTIF_IND == "Y")
                {
                    HR_POTONGAN potongan4 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                    potongan3.Add(potongan4);
                }
            }
            if(Kod == "00039")
            {
                if(Value == "P")
                {
                    return Json(potongan3.OrderBy(s => s.HR_PENERANGAN_POTONGAN), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (Value == "P")
                {
                    return Json(elaun3.OrderBy(s => s.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN), JsonRequestBehavior.AllowGet);
                }
            }
            
        }
        public ActionResult CariJumlahPotonganElaun(string HR_NO_PEKERJA, string Kod, string KodElaun)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);
            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
                peribadi.HR_MAKLUMAT_PEKERJAAN = new HR_MAKLUMAT_PEKERJAAN();
            }
            HR_MAKLUMAT_ELAUN_POTONGAN item = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == KodElaun && s.HR_AKTIF_IND == "Y");
            if(item == null)
            {
                item = new HR_MAKLUMAT_ELAUN_POTONGAN();
                
                if (Kod == "00024")
                {
                    HR_ELAUN elaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == KodElaun);
                    if (elaun == null)
                    {
                        elaun = new HR_ELAUN();
                    }
                    if(elaun.HR_PERATUS_IND == "Y")
                    {
                        elaun.HR_NILAI = peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK * (elaun.HR_NILAI / 100);
                    }
                    item.HR_JUMLAH = elaun.HR_NILAI;
                    item.HR_KOD_ELAUN_POTONGAN = elaun.HR_KOD_ELAUN;
                    item.HR_TARIKH_MULA = DateTime.Now;
                }
                if (Kod == "00039")
                {
                    HR_POTONGAN potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == KodElaun);
                    if (potongan == null)
                    {
                        potongan = new HR_POTONGAN();
                    }
                    item.HR_JUMLAH = potongan.HR_NILAI;
                    item.HR_KOD_ELAUN_POTONGAN = potongan.HR_KOD_POTONGAN;
                    item.HR_TARIKH_MULA = DateTime.Now;
                }
                
                
                if(item.HR_JUMLAH == null)
                {
                    item.HR_JUMLAH = 0;
                }
            }
            else
            {
                item.HR_MAKLUMAT_PEKERJAAN = null;
                item.HR_KOD_ELAUN_POTONGAN = null;
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConvertTarikhMula(string tarikh)
        {
            var date = Convert.ToDateTime(tarikh);
            var date2 = string.Format("{0:MM/dd/yyyy}", date);
            return Json(date2, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariPegawai(string HR_NP_FINALISED_HR)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PERIBADI pegawai = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == HR_NP_FINALISED_HR || s.HR_NAMA_PEKERJA == HR_NP_FINALISED_HR);
            if (pegawai == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CariPegawai2(string HR_NP_FINALISED_HR)
        {
            db.Configuration.ProxyCreationEnabled = false;
            mc.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PERIBADI pegawai = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).OrderBy(s => s.HR_NAMA_PEKERJA).ThenByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault(s => s.HR_NO_PEKERJA == HR_NP_FINALISED_HR || s.HR_NAMA_PEKERJA == HR_NP_FINALISED_HR);
            if (pegawai == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int? gred = null;
                if(pegawai.HR_MAKLUMAT_PEKERJAAN.HR_GRED != null)
                {
                    gred = Convert.ToInt32(pegawai.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                }
                GE_PARAMTABLE gred2 = cariGred(gred, null);
                HR_MATRIKS_GAJI matriks = cariMatriks(gred2.SHORT_DESCRIPTION, pegawai.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI, pegawai.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK);

                return Json(new { HR_NO_PEKERJA = pegawai.HR_NO_PEKERJA, HR_NAMA_PEKERJA = pegawai.HR_NAMA_PEKERJA, HR_JAWATAN = pegawai.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN, HR_GRED = gred2.SHORT_DESCRIPTION, HR_GAJI_POKOK = pegawai.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK, HR_MATRIKS_GAJI = pegawai.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI, HR_GAJI_MIN = matriks.HR_GAJI_MIN, HR_GAJI_MAX = matriks.HR_GAJI_MAX, HR_RM_KENAIKAN = matriks.HR_RM_KENAIKAN}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CariGred2(string HR_GRED)
        {
            db.Configuration.ProxyCreationEnabled = false;
            GE_PARAMTABLE gred = mc.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == HR_GRED);
            if (gred == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AjaxSenaraiGred()
        {
            mc.Configuration.ProxyCreationEnabled = false;
            List<GE_PARAMTABLE> gred = mc.GE_PARAMTABLE.Where(s => s.GROUPID == 109).OrderBy(s => s.SHORT_DESCRIPTION).ToList();
            if (gred.Count() <= 0)
            {
                gred = new List<GE_PARAMTABLE>();
            }

            return Json(gred, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariGajiBulanan(string HR_NO_PEKERJA, int? BULAN)
        {
            PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.FirstOrDefault(s => s.PA_NO_PEKERJA == HR_NO_PEKERJA && s.PA_TAHUN_GAJI == DateTime.Now.Year && s.PA_BULAN_GAJI == BULAN);
            if(transaksi == null)
            {
                transaksi = new PA_TRANSAKSI_GAJI();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.FirstOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);
                if(pekerjaan == null)
                {
                    pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }
                transaksi.PA_GAJI_POKOK = pekerjaan.HR_GAJI_POKOK;
            }
            return Json(transaksi.PA_GAJI_POKOK, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariGajiBaru(string HR_GRED, decimal? HR_GAJI_BARU)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HR_MATRIKS_GAJI gaji = db.HR_MATRIKS_GAJI.FirstOrDefault(s => s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_POKOK == HR_GAJI_BARU);
            if (gaji == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DataAnsuran(string HR_NO_PEKERJA, int? HR_ANSURAN_ID, string HR_KOD_PERUBAHAN)
        {
            var kew8 = from HR_MAKLUMAT_KEWANGAN8 in db.HR_MAKLUMAT_KEWANGAN8
                       join HR_MAKLUMAT_KEWANGAN8_DETAIL in db.HR_MAKLUMAT_KEWANGAN8_DETAIL
                       on new { HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA, HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN, HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA, HR_MAKLUMAT_KEWANGAN8.HR_KEW8_ID } equals new { HR_MAKLUMAT_KEWANGAN8_DETAIL.HR_NO_PEKERJA, HR_MAKLUMAT_KEWANGAN8_DETAIL.HR_KOD_PERUBAHAN, HR_MAKLUMAT_KEWANGAN8_DETAIL.HR_TARIKH_MULA, HR_MAKLUMAT_KEWANGAN8_DETAIL.HR_KEW8_ID }
                       where HR_MAKLUMAT_KEWANGAN8.HR_NO_PEKERJA == HR_NO_PEKERJA && HR_MAKLUMAT_KEWANGAN8.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN && HR_MAKLUMAT_KEWANGAN8.HR_ANSURAN_ID == HR_ANSURAN_ID
                       orderby HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA
                       select new { HR_TARIKH_MULA_ANSURAN = HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_MULA, HR_TARIKH_AKHIR_ANSURAN = HR_MAKLUMAT_KEWANGAN8.HR_TARIKH_AKHIR };

            List<HR_MAKLUMAT_KEWANGAN8> kewangan8 = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_ANSURAN_ID == HR_ANSURAN_ID && s.HR_KOD_PERUBAHAN == HR_KOD_PERUBAHAN).ToList();
            if (kewangan8.Count() > 1)
            {
                return Json(kew8.GroupBy(s => s.HR_TARIKH_MULA_ANSURAN).Select(s => s.FirstOrDefault()).OrderBy(s => s.HR_TARIKH_MULA_ANSURAN), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            
        }

        public JsonResult CariPergerakanGaji(int? id, string kod, string tarikh)
        {
            var date = Convert.ToDateTime(tarikh);
            db.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_KEWANGAN8 kew8 = db.HR_MAKLUMAT_KEWANGAN8.OrderByDescending(s => s.HR_KEW8_ID).FirstOrDefault(s => s.HR_KEW8_ID == id && s.HR_KOD_PERUBAHAN == kod && s.HR_TARIKH_MULA == date);
            if (kew8 == null)
            {
                kew8 = new HR_MAKLUMAT_KEWANGAN8();
            }

            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == kew8.HR_NP_FINALISED_HR);
            if(peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }

            PergerakanGajiModels item = new PergerakanGajiModels();
            item.HR_NO_SURAT_KEBENARAN = kew8.HR_NO_SURAT_KEBENARAN;
            item.HR_BUTIR_PERUBAHAN = kew8.HR_BUTIR_PERUBAHAN;
            item.HR_NP_FINALISED_HR = kew8.HR_NP_FINALISED_HR;
            item.HR_NAMA_PEGAWAI = peribadi.HR_NAMA_PEKERJA;
            item.HR_FINALISED_IND_HR = kew8.HR_FINALISED_IND_HR;

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HasilKiraan2(string startDate, string endDate, string kod, string noPekerja)
        {
            List<ParamTableBulanGajiModels> dataList = new List<ParamTableBulanGajiModels>();

            if (startDate == "")
            {
                startDate = null;
            }
            if (endDate == "")
            {
                endDate = null;
            }

            if (startDate != null && endDate != null)
            {
                DateTime crrnDate = Convert.ToDateTime(startDate);
                DateTime crrnDate2 = Convert.ToDateTime(startDate);
                DateTime endDate2 = Convert.ToDateTime(endDate);

                if (crrnDate <= endDate2)
                {
                    while (crrnDate.Month <= endDate2.Month)
                    {
                        ParamTableBulanGajiModels data = new ParamTableBulanGajiModels();
                        PA_TRANSAKSI_GAJI transaksi = spg.PA_TRANSAKSI_GAJI.FirstOrDefault(s => s.PA_NO_PEKERJA == noPekerja && s.PA_TAHUN_GAJI == crrnDate.Year && s.PA_BULAN_GAJI == crrnDate.Month);
                        if (transaksi == null)
                        {
                            transaksi = new PA_TRANSAKSI_GAJI();
                            HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.FirstOrDefault(s => s.HR_NO_PEKERJA == noPekerja);
                            if (pekerjaan == null)
                            {
                                pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                            }
                            transaksi.PA_GAJI_POKOK = pekerjaan.HR_GAJI_POKOK;
                        }

                        var endOfDayInMonth = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1).AddDays(-1);
                        var lastDayOfMonth = endOfDayInMonth.Day;

                        if (crrnDate.Month == endDate2.Month)
                        {
                            endOfDayInMonth = endDate2;
                        }

                        var totalDay = (crrnDate - endOfDayInMonth).TotalDays;

                        data.Gaji = transaksi.PA_GAJI_POKOK;
                        data.Bulan = crrnDate.Month;
                        data.Hari = Math.Abs(Convert.ToInt32(totalDay));
                        dataList.Add(data);

                        if (crrnDate.Month >= endDate2.Month && crrnDate.Year >= endDate2.Year)
                        {
                            break;
                        }
                        crrnDate = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1);
                    }
                }
                
            }
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KiraPelarasan(string startDate, string endDate, string keyInDate, decimal? gaji, string kod, string jenis)
        {
            if(startDate == "")
            {
                startDate = null;
            }
            if (endDate == "")
            {
                endDate = null;
            }
            decimal jumPelarasan = 0;
            if (startDate != null && endDate != null)
            {
                DateTime crrnDate = Convert.ToDateTime(startDate);
                DateTime crrnDate2 = Convert.ToDateTime(startDate);
                DateTime crrnDate3 = new DateTime(crrnDate2.Year, crrnDate2.Month, 1);
                DateTime endDate2 = Convert.ToDateTime(endDate);
                DateTime endDate3 = new DateTime(endDate2.Year, endDate2.Month, 1);
                DateTime keyInDate2 = Convert.ToDateTime(keyInDate);
                DateTime keyInDate3 = new DateTime(keyInDate2.Year, keyInDate2.Month, 1);
                decimal gajiPokok = Convert.ToDecimal(gaji);
                if (kod != "00017")
                {
                    //potongan
                    if (crrnDate > keyInDate2)
                    {

                        while (crrnDate3 >= keyInDate3)
                        {
                            var endOfDayInMonth = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1).AddDays(-1);
                            var lastDayOfMonth = endOfDayInMonth.Day;

                            if (crrnDate.Month == keyInDate2.Month)
                            {
                                endOfDayInMonth = crrnDate2;
                            }

                            var totalDay = (keyInDate2 - endOfDayInMonth).TotalDays;

                            //if (kod == "TMK" || kod == "00032" || kod == "00004")
                            //{
                            //    totalDay -= 1;
                            //}

                            jumPelarasan += Math.Abs((gajiPokok * Convert.ToDecimal(totalDay) / lastDayOfMonth));

                            //if (crrnDate.Month > endDate2.Month && crrnDate.Year >= endDate2.Year)
                            //{
                            //    break;
                            //}
                            crrnDate = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(-1);
                            crrnDate3 = crrnDate;
                        }
                    }
                    else if (crrnDate < keyInDate2)
                    {
                        while (crrnDate3 < keyInDate3)
                        {
                            var endOfDayInMonth = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1).AddDays(-1);
                            var lastDayOfMonth = endOfDayInMonth.Day;

                            if (crrnDate.Month == endDate2.Month)
                            {
                                endOfDayInMonth = endDate2;
                            }

                            var totalDay = (crrnDate - endOfDayInMonth).TotalDays;

                            if (kod == "TMK" || kod == "00032" || kod == "00004" || kod == "LNTKN" || kod == "00036")
                            {
                                totalDay -= 1;
                            }

                            jumPelarasan += Math.Abs((gajiPokok * Convert.ToDecimal(totalDay) / lastDayOfMonth));

                            if (crrnDate3 >= endDate3)
                            {
                                break;
                            }
                            crrnDate = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1);
                            crrnDate3 = crrnDate;
                        }
                    }
                }
                else
                {
                    if (crrnDate < endDate2)
                    {
                        while (crrnDate3 <= endDate3)
                        {
                            var endOfDayInMonth = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1).AddDays(-1);
                            var lastDayOfMonth = endOfDayInMonth.Day; //30,31

                            if (crrnDate.Month == endDate2.Month)
                            {
                                endOfDayInMonth = endDate2;
                            }

                            var totalDay = (crrnDate - endOfDayInMonth).TotalDays;

                            totalDay -= 1;

                            jumPelarasan += Math.Abs((gajiPokok * Convert.ToDecimal(totalDay) / lastDayOfMonth));

                            if (crrnDate3>= endDate3)
                            {
                                break;
                            }
                            crrnDate = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1);
                            crrnDate3 = crrnDate;
                        }
                    }
                }
            }
            return Json(jumPelarasan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BulanCutiSeparuhGaji(string startDate, string endDate)
        {
            DateTime crrnDate = Convert.ToDateTime(startDate);
            DateTime endDate2 = Convert.ToDateTime(endDate);

            List<HR_ANSURAN> senaraiBulan = new List<HR_ANSURAN>();

            while (crrnDate.Month <= endDate2.Month)
            {
                var endOfDayInMonth = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1).AddDays(-1);
                var lastDayOfMonth = endOfDayInMonth.Day; //30,31

                if (crrnDate.Month == endDate2.Month)
                {
                    endOfDayInMonth = endDate2;
                }

                HR_ANSURAN bulan = new HR_ANSURAN();
                bulan.HR_TARIKH_MULA_ANSURAN = crrnDate;
                bulan.HR_TARIKH_AKHIR_ANSURAN = endOfDayInMonth;

                senaraiBulan.Add(bulan);

                if (crrnDate.Month >= endDate2.Month && crrnDate.Year >= endDate2.Year)
                {
                    break;
                }
                crrnDate = new DateTime(crrnDate.Year, crrnDate.Month, 1).AddMonths(1);
            }
            return Json(senaraiBulan, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DataKewangan8(string value, string kod)
        //{
        //    List<HR_MAKLUMAT_KEWANGAN8> model = new List<HR_MAKLUMAT_KEWANGAN8>();

        //    List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
        //    HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == value);

        //    if (peribadi == null)
        //    {
        //        peribadi = new HR_MAKLUMAT_PERIBADI();
        //    }

        //    if (kod == "kew8")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00002" || s.HR_KOD_PERUBAHAN == "00003" || s.HR_KOD_PERUBAHAN == "00004" || s.HR_KOD_PERUBAHAN == "00005" || s.HR_KOD_PERUBAHAN == "00006" || s.HR_KOD_PERUBAHAN == "00007" || s.HR_KOD_PERUBAHAN == "00008" || s.HR_KOD_PERUBAHAN == "00009" || s.HR_KOD_PERUBAHAN == "00010" || s.HR_KOD_PERUBAHAN == "00013" || s.HR_KOD_PERUBAHAN == "00015" || s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018" || s.HR_KOD_PERUBAHAN == "00023" || s.HR_KOD_PERUBAHAN == "00027" || s.HR_KOD_PERUBAHAN == "00028" || s.HR_KOD_PERUBAHAN == "00039" || s.HR_KOD_PERUBAHAN == "00040" || s.HR_KOD_PERUBAHAN == "00042" || s.HR_KOD_PERUBAHAN == "00044" || s.HR_KOD_PERUBAHAN == "00045")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else if (kod == "TP")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00011" || s.HR_KOD_PERUBAHAN == "00014" || s.HR_KOD_PERUBAHAN == "00016" || s.HR_KOD_PERUBAHAN == "00020" || s.HR_KOD_PERUBAHAN == "00035" || s.HR_KOD_PERUBAHAN == "00044")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else if (kod == "CUTI")
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && (s.HR_KOD_PERUBAHAN == "00017" || s.HR_KOD_PERUBAHAN == "00018")).ToList<HR_MAKLUMAT_KEWANGAN8>();
        //    }
        //    else
        //    {
        //        model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_NO_PEKERJA == value && s.HR_KOD_PERUBAHAN == kod).ToList();
        //    }
        //    return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        //}

        //auto post
        [HttpPost]
        public ActionResult AutoAktif(string HR_NO_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunPotonganList = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).ToList();
            foreach(HR_MAKLUMAT_ELAUN_POTONGAN item in elaunPotonganList)
            {
                HR_MAKLUMAT_ELAUN_POTONGAN elaunPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                if (elaunPotongan != null && (item.HR_TARIKH_MULA != null || item.HR_TARIKH_AKHIR != null))
                {
                    if (item.HR_AUTO_IND == "Y")
                    {
                        if (item.HR_TARIKH_MULA <= DateTime.Now && item.HR_TARIKH_AKHIR >= DateTime.Now)
                        {
                            elaunPotongan.HR_AKTIF_IND = "Y";
                        }
                        else
                        {
                            elaunPotongan.HR_AKTIF_IND = "T";
                        }
                        db.Entry(elaunPotongan).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                
            }
            return null;
        }

        public List<HR_MAKLUMAT_KEWANGAN8> BilPekerja2(string key, string value, int? bulan, string tarafpekerja)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = CariPekerja(key, value, bulan, "00001");
            sPeribadi = sPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja).ToList();
            List<HR_MAKLUMAT_KEWANGAN8> Kew8 = db.HR_MAKLUMAT_KEWANGAN8.AsEnumerable().Where(s => s.HR_KOD_PERUBAHAN == "00001" && Convert.ToDateTime(s.HR_TARIKH_MULA).Month == bulan && s.HR_TAHUN == DateTime.Now.Year && sPeribadi.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == tarafpekerja).Count() > 0).OrderByDescending(s => s.HR_KEW8_ID).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();

            var T = 0; var K = 0; var M = 0;

            foreach (HR_MAKLUMAT_PERIBADI peribadi in sPeribadi.ToList())
            {
                HR_MAKLUMAT_KEWANGAN8 Q8 = Kew8.FirstOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA);
                if (Q8 != null)
                {
                    sPeribadi.Remove(peribadi);
                }
            }

            T = sPeribadi.Count();
            K = Kew8.AsEnumerable().Where(s => s.HR_FINALISED_IND_HR != "Y" && s.HR_UBAH_IND == "1").Count();
            M = Kew8.AsEnumerable().Where(s => s.HR_FINALISED_IND_HR == "Y" && s.HR_UBAH_IND == "0").Count();

            List<HR_MAKLUMAT_KEWANGAN8> bil = new List<HR_MAKLUMAT_KEWANGAN8>();
            HR_MAKLUMAT_KEWANGAN8 dataT = new HR_MAKLUMAT_KEWANGAN8();
            dataT.HR_KEW8_IND = "T";
            dataT.HR_BUTIR_PERUBAHAN = "Tambah";
            dataT.HR_BIL = T;
            bil.Add(dataT);

            HR_MAKLUMAT_KEWANGAN8 dataK = new HR_MAKLUMAT_KEWANGAN8();
            dataK.HR_KEW8_IND = "K";
            dataK.HR_BUTIR_PERUBAHAN = "Kemaskini";
            dataK.HR_BIL = K;
            bil.Add(dataK);

            HR_MAKLUMAT_KEWANGAN8 dataM = new HR_MAKLUMAT_KEWANGAN8();
            dataM.HR_KEW8_IND = "M";
            dataM.HR_BUTIR_PERUBAHAN = "Muktamad";
            dataM.HR_BIL = M;
            bil.Add(dataM);

            return bil;
        }

        public JsonResult BilPekerja(string key, string value, int? bulan, string tarafpekerja)
        {
            List<HR_MAKLUMAT_KEWANGAN8> bil = BilPekerja2(key, value, bulan, tarafpekerja);
            return Json(bil, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
                mc.Dispose();
                spg.Dispose();
            }
            base.Dispose(disposing);
        }
        public enum ManageMessageId
        {
            Error,
            Success
        }
    }
}
