using eSPP.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace eSPP.Controllers
{
    public class LuarNegaraPengesahanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        // GET: LuarNegara


        public ActionResult SenaraiLuarNegara()
        {
            List<HR_SEMINAR_LUAR> Msem = new List<HR_SEMINAR_LUAR>();
            List<HR_SEMINAR_LUAR_DETAIL> Mdata = new List<HR_SEMINAR_LUAR_DETAIL>();

            List<HR_SEMINAR_LUAR> seminaran = db.HR_SEMINAR_LUAR.ToList();
            List<HR_SEMINAR_LUAR_DETAIL> seminarandetails = db.HR_SEMINAR_LUAR_DETAIL.ToList();

            LuarNegaraModels LuarNegara = new LuarNegaraModels();

            LuarNegara.HR_SEMINAR_LUAR = seminaran;
            LuarNegara.HR_SEMINAR_LUAR_DETAIL = seminarandetails;

            return View(LuarNegara);
        }


        public ActionResult InfoPengesahanNegara(string id)
        {
            if (id == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);


            if (seminar == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");
            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");
            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();
            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;
            return PartialView("InfoPengesahanNegara", seminar);
        }


        public ActionResult CariBulan()
        {
         
            List<HR_SEMINAR_LUAR_DETAIL> model = db.HR_SEMINAR_LUAR_DETAIL.AsEnumerable().Where(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == DateTime.Now.Month && Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Year == DateTime.Now.Year).ToList<HR_SEMINAR_LUAR_DETAIL>();

            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.ToList();
            ViewBag.jabatan = db2.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.bulan = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
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

        [HttpPost]
        public ActionResult CariBulan(int? bulan, int? tahun)
        {
            
            List<HR_SEMINAR_LUAR_DETAIL> model = db.HR_SEMINAR_LUAR_DETAIL.AsEnumerable().Where(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == DateTime.Now.Month && Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Year == DateTime.Now.Year).ToList<HR_SEMINAR_LUAR_DETAIL>();
            if (bulan != null && tahun != null)
            {
                //model2 = db2.ZATUL_MUKTAMAT_PERGERAKAN_GAJI(bulan, tahun);
                model = db.HR_SEMINAR_LUAR_DETAIL.AsEnumerable().Where(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == bulan && Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Year == tahun && s.HR_SEMINAR_LUAR.HR_LULUS_MENTERI_IND == "Y").ToList<HR_SEMINAR_LUAR_DETAIL>();
            }
            ViewBag.peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.ToList();
            ViewBag.jabatan = db2.GE_JABATAN.ToList();
            ViewBag.jawatan = db.HR_JAWATAN.ToList();
            ViewBag.bulan = bulan;
            ViewBag.year = tahun;
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

        public FileStreamResult PDFSenarai(int? bulan, int? tahun)
        {

            List<HR_SEMINAR_LUAR_DETAIL> model = db.HR_SEMINAR_LUAR_DETAIL.AsEnumerable().Where(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == bulan && Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Year == tahun && s.HR_SEMINAR_LUAR.HR_LULUS_MENTERI_IND == "Y").ToList<HR_SEMINAR_LUAR_DETAIL>();
            List<GE_JABATAN> sJabatan = new List<GE_JABATAN>();

            foreach (HR_SEMINAR_LUAR_DETAIL pekerja in model)
            {
                
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pekerja.HR_NO_PEKERJA);
                GE_JABATAN jabatan2 = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                sJabatan.Add(jabatan2);
            }

            var html = "<html><head>";
            html += "<title>Senarai Ke Luar Negara</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
           

                foreach (HR_SEMINAR_LUAR_DETAIL deta in model.GroupBy(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == bulan && Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Year == tahun && s.HR_SEMINAR_LUAR.HR_LULUS_MENTERI_IND == "Y").Select(s => s.FirstOrDefault()))
            {
                //html += "<p>" + deta.Where(s => Convert.ToDateTime(s.HR_SEMINAR_LUAR.HR_TARIKH_MULA).Month == bulan  "</p>";
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 1px solid black;'>";

                //html += "<thead>";
                html += "<tr>";
               
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>BIL</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>NAMA</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>JAWATAN</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>JABATAN</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>NEGARA YANG DILAWATI</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>TARIKH PERGI</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>TARIKH BALIK</strong></td>";
                html += "<td style='border: 1px solid black; font-size: 60%'><strong>TUJUAN</strong></td>";
                html += "</tr>";
                //html += "</thead>";
                //html += "<tbody>";
                var no = 0;
                foreach (var l in model)
                {
                    
                    HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == l.HR_NO_PEKERJA);
                    GE_JABATAN jab = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                    if (peribadi == null)
                    {
                        peribadi = new HR_MAKLUMAT_PERIBADI();
                    }

                    if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN == jab.GE_KOD_JABATAN)
                    {
                        GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                        if (jabatan == null)
                        {
                            jabatan = new GE_JABATAN();
                        }
                        HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                        if (jawatan == null)
                        {
                            jawatan = new HR_JAWATAN();
                        }
                        ++no;
                        html += "<tr>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + no + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + jawatan.HR_NAMA_JAWATAN + "</td>";
                        html += "<td style='border: 1px solid black; font-size: 60%'>" + jabatan.GE_KETERANGAN_JABATAN + "</td>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + l.HR_SEMINAR_LUAR.HR_TEMPAT + "</td>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_SEMINAR_LUAR.HR_TARIKH_MULA) + "</td>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + string.Format("{0:dd/MM/yyyy}", l.HR_SEMINAR_LUAR.HR_TARIKH_TAMAT) + "</td>";
                        html += "<td align='center' style='border: 1px solid black; font-size: 60%'>" + l.HR_SEMINAR_LUAR.HR_NAMA_SEMINAR + "</td>";
                        html += "</tr>";
                    }
                }

                html += "</table>";
            }

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

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

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI NAMA PEGAWAI DAN KAKITANGAN MBPJ KE LUAR NEGARA BAGI SUKU TAHUN KETIGA");
                iTextSharp.text.Paragraph paragraph2 = new iTextSharp.text.Paragraph("Bulan                       " + Bulan, contentFont);
                iTextSharp.text.Paragraph paragraph3 = new iTextSharp.text.Paragraph("Tahun                       " + tahun, contentFont);
                paragraph.Alignment = Element.ALIGN_JUSTIFIED;
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                //pic.SpacingBefore = 9f;
                paragraph.SpacingBefore = 10f;
                paragraph2.SpacingBefore = 10f;
                //pic.BorderWidthTop = 36f;
                //paragraph2.SetLeading(20f, 0);
                document.Add(pic);
                document.Add(paragraph);
                document.Add(paragraph2);
                document.Add(paragraph3);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

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
        }

        public ActionResult EditPengesahanNegara(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);

            if (seminar == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");

            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");

            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;

            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();

            return PartialView("_EditPengesahanNegara", seminar);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPengesahanNegara([Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND,HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI")] HR_SEMINAR_LUAR seminar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seminar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiLuarNegara");
            }
            return View(seminar);
        }

        //public ActionResult  EditLulusPengesahanNegara(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    }
        //    HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);

        //    if (seminar == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    List<SelectListItem> bulan = new List<SelectListItem>
        //    {
        //        //new SelectListItem { Text = "JANUARI", Value = "1" },
        //        //new SelectListItem { Text = "FEBRUARI", Value = "2" },
        //        //new SelectListItem { Text = "MAC", Value = "3" },
        //        //new SelectListItem { Text = "APRIL", Value = "4" },
        //        //new SelectListItem { Text = "MAY", Value = "5" },
        //        //new SelectListItem { Text = "JUN", Value = "6" },
        //        //new SelectListItem { Text = "JULAI", Value = "7" },
        //        //new SelectListItem { Text = "OGOS", Value = "8" },
        //        //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
        //        //new SelectListItem { Text = "OKTOBER", Value = "10" },
        //        //new SelectListItem { Text = "NOVEMBER", Value = "11" },
        //        //new SelectListItem { Text = "DISEMBER", Value = "12" }
        //    };
        //    ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

        //    List<SelectListItem> belanja = new List<SelectListItem>
        //    {
        //        new SelectListItem { Text = "MBPJ", Value = "M" },
        //        new SelectListItem { Text = "SENDIRI", Value = "S" },
        //        new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
        //    };
        //    ViewBag.belanja = new SelectList(belanja, "Value", "Text");

        //    List<SelectListItem> luluskementerian = new List<SelectListItem>
        //    {
        //        new SelectListItem { Text = "Ya", Value = "Y" },
        //        new SelectListItem { Text = "Tidak", Value = "T" },
        //    };
        //    ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");

        //    var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
        //    ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;

        //    ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();

        //    return PartialView("_EditLulusPengesahanNegara", seminar);
        //}

        public ActionResult EditLulusPengesahanNegara(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR sahseminar = new HR_SEMINAR_LUAR();
            

            sahseminar.HR_TARIKH_SOKONG = DateTime.Now;
            var tarikhsokong = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_SOKONG);
            ViewBag.HR_TARIKH_SOKONG = tarikhsokong;

            sahseminar.HR_TARIKH_LULUS_MENTERI = DateTime.Now;
            var tarikhlulus = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_LULUS_MENTERI);
            ViewBag.HR_TARIKH_LULUS_MENTERI = tarikhlulus;

            List<HR_SEMINAR_LUAR_DETAIL> details = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_KOD_LAWATAN == id).ToList();
            
            if (details == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.id = id;
            
            ViewBag.HR_NAMA_PEKERJA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            return PartialView("_EditLulusPengesahanNegara", details);
        }

        public ActionResult LulusPengesahanNegara(string id, string no)
        {
            if (id == null && no == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            List<HR_SEMINAR_LUAR_DETAIL> details = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_KOD_LAWATAN == id && s.HR_NO_PEKERJA == no).ToList();
            
            if (details == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.id = id;
            return PartialView("_LulusPengesahanNegara", details);
        }

        public JsonResult CariNama(string HR_NO_PEKERJA)
        {
            MaklumatKakitanganModels model = new MaklumatKakitanganModels();
            // Test test = new Test();
            HR_MAKLUMAT_PERIBADI item = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();

            if (item == null)
            {
                item = new HR_MAKLUMAT_PERIBADI();
            }
            HR_MAKLUMAT_PEKERJAAN item1 = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            if (item1 == null)
            {
                item1 = new HR_MAKLUMAT_PEKERJAAN();
            }
            model.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi(); //newobject
            model.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
            
            model.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
   
            //model.GE_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            // model.GE_BAHAGIAN = bahagian.GE_KETERANGAN;
            // test.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
            // test.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TambahLulusPengesahanNegara(string id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_SEMINAR_LUAR sahseminar = new HR_SEMINAR_LUAR();
            sahseminar.HR_TARIKH_SOKONG = DateTime.Now;
            var tarikhsokong = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_SOKONG);
            ViewBag.HR_TARIKH_SOKONG = tarikhsokong;
            sahseminar.HR_TARIKH_LULUS_MENTERI = DateTime.Now;
            var tarikhlulus = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_LULUS_MENTERI);
            ViewBag.HR_TARIKH_LULUS_MENTERI = tarikhlulus;
            List<HR_SEMINAR_LUAR_DETAIL> details = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_KOD_LAWATAN == id).ToList();
            
            if (details == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.id = id;
            return PartialView("_TambahLulusPengesahanNegara", details);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahLulusPengesahanNegara([Bind(Include = "HR_KOD_LAWATAN,HR_NO_PEKERJA, HR_KERAP_IND, HR_LAPORAN_IND, HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI,HR_TARIKHMULA_MANGKU,HR_TARIKHAKHIR_MANGKU,HR_TIKET_KAPAL,HR_PENGINAPAN,HR_LAIN,HR_JUMLAH_BELANJA,HR_NAMA_PEGAWAI,HR_HUBUNGAN,HR_ALAMAT_PEGAWAI,HR_NOTEL_PEGAWAI,HR_EMAIL_PEGAWAI,HR_ALASAN")]HR_SEMINAR_LUAR_DETAIL luardetail, [Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND")] HR_SEMINAR_LUAR luar)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);
            
            if (ModelState.IsValid)
            {
                db.HR_SEMINAR_LUAR_DETAIL.Add(luardetail);
                db.SaveChanges();
                return RedirectToAction("SenaraiLuarNegara");
            }
        
            return PartialView( "_TambahLulusPengesahanNegara",luardetail);
        }

        public ActionResult PadamLulusPengesahanNegara(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_SEMINAR_LUAR sahseminar = new HR_SEMINAR_LUAR();
            sahseminar.HR_TARIKH_SOKONG = DateTime.Now;
            var tarikhsokong = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_SOKONG);
            ViewBag.HR_TARIKH_SOKONG = tarikhsokong;
            sahseminar.HR_TARIKH_LULUS_MENTERI = DateTime.Now;
            var tarikhlulus = string.Format("{0:dd/MM/yyyy}", sahseminar.HR_TARIKH_LULUS_MENTERI);
            ViewBag.HR_TARIKH_LULUS_MENTERI = tarikhlulus;
            
            List<HR_SEMINAR_LUAR> seminar = db.HR_SEMINAR_LUAR.Where(s => s.HR_KOD_LAWATAN == id).ToList();

            if (seminar == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.id = id;
            return PartialView("_PadamLulusPengesahanNegara", seminar);
        }

        
        [HttpPost, ActionName("PadamLulusPengesahanNegara")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SEMINAR_LUAR seminar, string id, string key, string value)
        {
            seminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN);
            db.HR_SEMINAR_LUAR.Remove(seminar);
            db.SaveChanges();
            {
                return RedirectToAction("SenaraiPemohon", new { key = key, value = value });
            }
        }


    }
} 