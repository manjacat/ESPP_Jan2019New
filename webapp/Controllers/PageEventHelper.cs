using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Controllers
{
    public class PageEventHelper : PdfPageEventHelper
    {
        iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);

        public int? bulan { get; set; }
        public int? tahun { get; set; }
        public string tarafpekerja { get; set; }
        public string jenis { get; set; }
        // This is the contentbyte object of the writer  
        PdfContentByte cb;

        // we will put the final number of pages in a template  
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer  
        BaseFont bf = null;

        // This keeps track of the creation time  
        DateTime PrintTime = DateTime.Now;

        #region Fields  
        private string _header;
        #endregion

        #region Properties  
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            if (jenis != "MK")
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

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/img/logo-mbpj.gif"));
                pic.ScaleAbsolute(100f, 40f);
                //iTextSharp.text.Paragraph tajuk = new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA\nLAPORAN BAYARAN PERGERAKAN GAJI UNTUK BULAN "+ Bulan.ToUpper() + " "+ tahun +" BAGI KAKITANGAN\nMBPJ", contentFont);
                iTextSharp.text.Paragraph tajuk = new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA\nSENARAI PERGERAKAN GAJI UNTUK BULAN " + Bulan.ToUpper() + " BAGI " + kakitangan.ToUpper() + "\nMBPJ", contentFont);
                float[] columnWidths2 = { 2f, 5f };
                PdfPTable pdfTab2 = new PdfPTable(columnWidths2);

                PdfPCell pdfC1 = new PdfPCell(new iTextSharp.text.Paragraph("TARIKH", contentFont2));
                PdfPCell pdfC2 = new PdfPCell(new iTextSharp.text.Paragraph("  :  " + PrintTime.ToShortDateString(), contentFont2));

                PdfPCell pdfC3 = new PdfPCell(new iTextSharp.text.Paragraph("MASA", contentFont2));
                PdfPCell pdfC4 = new PdfPCell(new iTextSharp.text.Paragraph("  :  " + string.Format("{0:HH:mm:ss}", DateTime.Now), contentFont2));

                PdfPCell pdfC5 = new PdfPCell(new iTextSharp.text.Paragraph("MUKA", contentFont2));
                PdfPCell pdfC6 = new PdfPCell(new iTextSharp.text.Paragraph("  :  " + writer.PageNumber, contentFont2));


                pdfC1.Border = 0;
                pdfC2.Border = 0;
                pdfC3.Border = 0;
                pdfC4.Border = 0;
                pdfC5.Border = 0;
                pdfC6.Border = 0;


                pdfTab2.AddCell(pdfC1);
                pdfTab2.AddCell(pdfC2);
                pdfTab2.AddCell(pdfC3);
                pdfTab2.AddCell(pdfC4);
                pdfTab2.AddCell(pdfC5);
                pdfTab2.AddCell(pdfC6);

                pdfTab2.WidthPercentage = 30;

                float[] columnWidths = { 1f, 9f, 2f };

                //Create PdfTable object  
                PdfPTable pdfTab = new PdfPTable(columnWidths);
                //We will have to create separate cells to include image logo and 2 separate strings  
                //Row 1  
                PdfPCell pdfCell1 = new PdfPCell(pic);
                PdfPCell pdfCell2 = new PdfPCell(tajuk);
                PdfPCell pdfCell3 = new PdfPCell(pdfTab2);

                //set the alignment of all three cells and set border to 0  
                pdfCell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;

                pdfCell1.VerticalAlignment = Element.ALIGN_TOP;
                pdfCell2.VerticalAlignment = Element.ALIGN_TOP;
                pdfCell3.VerticalAlignment = Element.ALIGN_TOP;


                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;

                //add all three cells into PdfTable  
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);

                //pdfTab.TotalWidth = document.PageSize.Width - 80f;
                //pdfTab.WidthPercentage = 70;

                pdfTab.TotalWidth = 800f;
                pdfTab.LockedWidth = true;

                //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;      

                //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable  
                //first param is start row. -1 indicates there is no end row and all the rows to be included to write  
                //Third and fourth param is x and y position to start writing  
                pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                //set pdfContent value  

                //Move the pointer and draw line to separate header section from rest of page  
                //cb.MoveTo(40, document.PageSize.Height - 100);
                //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
                //cb.Stroke();

                //Move the pointer and draw line to separate footer section from rest of page  
                cb.MoveTo(40, document.PageSize.GetBottom(40));
                cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(40));
                cb.Stroke();
            }
            else
            {
                Rectangle pageSize = document.PageSize;
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                "MUKA SURAT : " + writer.PageNumber,
                pageSize.GetLeft(50),
                pageSize.GetBottom(30), 0);
                cb.EndText();
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();
        }
    }
}