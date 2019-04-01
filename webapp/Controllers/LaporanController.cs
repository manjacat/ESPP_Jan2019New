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
        public ActionResult SuratPengesahanHospital(SuratPengesahanHospitalModel model, string Command)
        {
            if (model != null)
            {
                if(Command == "printDoc")
                {
                    return CreateDocxReport(model);
                }
                return CreatePdfReport(model);
            }
            else return new EmptyResult();
        }

        [NonAction]
        private ActionResult CreatePdfReport(SuratPengesahanHospitalModel model)
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

                    baris2 += string.Format("<span style='font-size:12px;'>&nbsp;</span>"
                        + "<span style='font-size:12px;font-weight:bold;margin-left:10px;'>{0}</span><br/>",
                        s.Nama);
                    counter++;
                }
                output = output.Replace("@TANGGUNGAN1", baris1);
                output = output.Replace("@TANGGUNGAN2", baris2);
            }
            return output;
        }

        public ActionResult CreateDocxReport(SuratPengesahanHospitalModel model)
        {

            string path_file = Server.MapPath("~/Content/template/");
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            object missing = System.Reflection.Missing.Value;
            string[] filePaths = Directory.GetFiles(path_file + "SuratPergerakanGaji\\");
            foreach (string filePath in filePaths)
                System.IO.File.Delete(filePath);

            Microsoft.Office.Interop.Word.Document Doc = WordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            object start = 0; object end = 0;
            Microsoft.Office.Interop.Word.Range rng = Doc.Range(ref start, ref missing);

            string tempFile = "SuratPengesahanHospital\\SuratPengesahanHospital(" + model.NoPekerja + ").docx";

            string LT1, LT2, LT3, LT4, LT5, LT6, LT7;
            string T1, T2, T3, T4, T5, T6, T7;

            LT1 = model.MaklumatTanggungan.Count > 0 ? string.Format("Nama: {0}.     No KP: {1}", 
                model.MaklumatTanggungan[0].Nama, model.MaklumatTanggungan[0].NoKP) : string.Empty;
            LT2 = model.MaklumatTanggungan.Count > 1 ? string.Format("Nama: {0}.     No KP: {1}", 
                model.MaklumatTanggungan[1].Nama, model.MaklumatTanggungan[1].NoKP) : string.Empty;
            LT3 = model.MaklumatTanggungan.Count > 2 ? string.Format("Nama: {0}.     No KP: {1}", 
                model.MaklumatTanggungan[2].Nama, model.MaklumatTanggungan[2].NoKP) : string.Empty;
            LT4 = model.MaklumatTanggungan.Count > 3 ? string.Format("Nama: {0}.     No KP: {1}"
                , model.MaklumatTanggungan[3].Nama, model.MaklumatTanggungan[3].NoKP) : string.Empty;
            LT5 = model.MaklumatTanggungan.Count > 4 ? string.Format("Nama: {0}.     No KP: {1}"
                , model.MaklumatTanggungan[4].Nama, model.MaklumatTanggungan[4].NoKP) : string.Empty;
            LT6 = model.MaklumatTanggungan.Count > 5 ? string.Format("Nama: {0}.     No KP: {1}", 
                model.MaklumatTanggungan[5].Nama, model.MaklumatTanggungan[5].NoKP) : string.Empty;
            LT7 = model.MaklumatTanggungan.Count > 6 ? string.Format("Nama: {0}.     No KP: {1}", 
                model.MaklumatTanggungan[6].Nama, model.MaklumatTanggungan[6].NoKP) : string.Empty;

            T1 = model.MaklumatTanggungan.Count > 0 ? model.MaklumatTanggungan[0].Nama : string.Empty;
            T2 = model.MaklumatTanggungan.Count > 1 ? model.MaklumatTanggungan[1].Nama : string.Empty;
            T3 = model.MaklumatTanggungan.Count > 2 ? model.MaklumatTanggungan[2].Nama : string.Empty;
            T4 = model.MaklumatTanggungan.Count > 3 ? model.MaklumatTanggungan[3].Nama : string.Empty;
            T5 = model.MaklumatTanggungan.Count > 4 ? model.MaklumatTanggungan[4].Nama : string.Empty;
            T6 = model.MaklumatTanggungan.Count > 5 ? model.MaklumatTanggungan[5].Nama : string.Empty;
            T7 = model.MaklumatTanggungan.Count > 6 ? model.MaklumatTanggungan[6].Nama : string.Empty;

            var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
                templateEngine.Process(
                    source: path_file + "SuratPengesahanHospital.docx",
                    destination: path_file + tempFile,
                    data: new
                    {
                        tarikh = model.TarikhString,
                        namahospital = model.HospitalName,
                        namapegawai = model.NamaPekerja,
                        nokpbaru = model.NoKPBaru,
                        jawatan = model.Jawatan,
                        gred = model.GredGaji,
                        gajibulanan = model.GajiBulanan.ToString("#,##0.00"),
                        listtanggungan1 = LT1,
                        listtanggungan2 = LT2,
                        listtanggungan3 = LT3,
                        listtanggungan4 = LT4,
                        listtanggungan5 = LT5,
                        listtanggungan6 = LT6,
                        listtanggungan7 = LT7,
                        tanggungan1 = T1,
                        tanggungan2 = T2,
                        tanggungan3 = T3,
                        tanggungan4 = T4,
                        tanggungan5 = T5,
                        tanggungan6 = T6,
                        tanggungan7 = T7
                    });



                rng.InsertFile(path_file + tempFile, 
                    ref missing, ref missing, ref missing, ref missing);

                // now make start to point to the end of the content of the first document
                start = WordApp.ActiveDocument.Content.End - 1;
                rng = Doc.Range(ref start, ref missing);

            //foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in Doc.Paragraphs)
            //{
            //    paragraph.Range.Font.Name = "Times New Roman";
            //    paragraph.Range.Font.Size = 11;
            //    paragraph.Format.SpaceAfter = 0.1f;
            //}

            Doc.SaveAs2(path_file + tempFile);
            Doc.Application.Quit();
            try
            {
                WordApp.Quit();
            }
            catch
            {

            }

            //System.IO.FileInfo file = new System.IO.FileInfo(path_file + tempFile);

            //Response.Clear();
            //Response.AddHeader("content-length", file.Length.ToString());
            //Response.AddHeader("content-disposition", "attachment; filename = SuratPengesahanHospital.docx");
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            //Response.TransmitFile(path_file + "SuratPengesahanHospital.docx");
            //Response.Flush();
            //Response.Close();

            //return View();
            string fullFilePath = path_file + tempFile;
            return File(fullFilePath, 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

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