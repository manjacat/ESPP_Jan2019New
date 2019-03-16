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
            if(string.IsNullOrEmpty(noPekerja))
            {
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SuratPengesahanHospital()
        {
            return CreateReport();
        }

        [NonAction]
        private ActionResult CreateReport()
        {
            string root = Server.MapPath("~");
            string rootURL = Request.Url.Host;

            string htmlFileLocation = root + @"Reports\SuratPengesahanHospital.html";
            //string imageRootURL = rootURL + "/Images/";
            string pdfString = System.IO.File.ReadAllText(htmlFileLocation);

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
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
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