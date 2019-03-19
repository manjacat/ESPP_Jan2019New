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

namespace eSPP.Controllers
{
    public class LaporanController : Controller
    {
        // GET: Laporan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string jenis, string value)
        {
            if (!string.IsNullOrEmpty(jenis))
            {
                List<CariPekerjaModel> searchList = CariPekerjaModel.GetPekerja(jenis, value);
                return View(searchList);
            }
            else
            {
                return View(new List<CariPekerjaModel>());
            }
        }


        public ActionResult SuratPengesahanHospital(string noPekerja)
        {
            SuratPengesahanHospitalModel model = new SuratPengesahanHospitalModel();
            if (!string.IsNullOrEmpty(noPekerja))
            {
                model = SuratPengesahanHospitalModel.GetByNoPekerja(noPekerja);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SuratPengesahanHospital(SuratPengesahanHospitalModel model)
        {
            if (model != null)
            {
                return CreateReport(model);
            }
            else return new EmptyResult();
        }

        [NonAction]
        private ActionResult CreateReport(SuratPengesahanHospitalModel model)
        {
            string pdfString = GetSuratPengesahanHospitalString(model);
            string exportData = string.Format(pdfString);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);

            var document = new Document(PageSize.A4, 30, 30, 28, 28);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                iTextSharp.text.Font contentFont = 
                    iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        private string GetSuratPengesahanHospitalString(SuratPengesahanHospitalModel model)
        {
            string root = Server.MapPath("~");
            string rootURL = Request.Url.Host;

            string htmlFileLocation = root + @"Reports\SuratPengesahanHospital.html";
            //string imageRootURL = rootURL + "/Images/";
            string output = System.IO.File.ReadAllText(htmlFileLocation);

            if (model != null)
            {
                output = output.Replace("@TARIKH", model.TarikhString);
                output = output.Replace("@NAMAPEKERJA", model.NamaPekerja);
                output = output.Replace("@NOKPBARU", model.NoKPBaru);
                output = output.Replace("@NOPEKERJA", model.NoPekerja);
                output = output.Replace("@NAMAHOSPITAL", model.HospitalName);
                output = output.Replace("@JAWATAN", model.Jawatan);
                output = output.Replace("@GRED", model.GredGaji);
                output = output.Replace("@GAJIBULANAN", model.GajiBulanan.ToString("#,##0.00"));

                string baris1 = string.Empty;
                string baris2 = string.Empty;
                int counter = 1;
                foreach (MaklumatTanggunganModel s in model.MaklumatTanggungan)
                {

                    baris1 += string.Format("<tr><td width='50px'>&nbsp;</td>"
                        + "<td><span style='font-size:12px;'>Nama: </span><span style='font-size:12px; text-decoration: underline'>{0}</span></td>"
                        + "<td><span style='font-size:12px;'>No KP: </span><span style='font-size:12px; text-decoration: underline'>{1}</span></td>"
                        + "</tr>", s.Nama, s.NoKP);

                    baris2 += string.Format("<span style='font-size:12px;'>{0}</span>"
                        + "<span style='font-size:12px;font-weight:bold'>{1}</span><br/>",
                        counter, s.Nama);
                    counter++;
                }
                output = output.Replace("@TANGGUNGAN1", baris1);
                output = output.Replace("@TANGGUNGAN2", baris2);
            }
            return output;
        }

        public ActionResult TawaranLantikan()
        {
            return View();
        }
        public ActionResult PengesahanJawatan()
        {
            return View();
        }
        public ActionResult LaporanTuntutanPerjalanan()
        {
            return View();
        }
        public ActionResult PenyedianLaporan()
        {
            return View();
        }
        public ActionResult LaporanAhliMajlis()
        {
            return View();
        }
    }
}