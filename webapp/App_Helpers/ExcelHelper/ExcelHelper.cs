using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace eSPP.App_Helpers.ExcelHelper
{
    public enum Extension
    {
        xlsx = 0,
        xls = 1
    }

    public abstract class ExcelHelper
    {
        #region Styles        
        protected static ICellStyle AlignCenter(IWorkbook workbook)
        {
            ICellStyle center = workbook.CreateCellStyle();
            center.Alignment = HorizontalAlignment.Center;
            return center;
        }

        protected static ICellStyle BorderAlignCenter(IWorkbook workbook)
        {
            ICellStyle center = workbook.CreateCellStyle();
            center.BorderTop = BorderStyle.Medium;
            center.BorderLeft = BorderStyle.Medium;
            center.BorderRight = BorderStyle.Medium;
            center.BorderBottom = BorderStyle.Medium;
            center.Alignment = HorizontalAlignment.Center;
            return center;
        }

        protected static ICellStyle BorderAlignCenterColoured(IWorkbook workbook)
        {
            ICellStyle center = workbook.CreateCellStyle();
            center.BorderTop = BorderStyle.Medium;
            center.BorderLeft = BorderStyle.Medium;
            center.BorderRight = BorderStyle.Medium;
            center.BorderBottom = BorderStyle.Medium;
            center.FillBackgroundColor = IndexedColors.Grey25Percent.Index;
            //center.FillPattern = FillPattern.SolidForeground;
            center.Alignment = HorizontalAlignment.Center;
            return center;
        }

        protected static ICellStyle AlignLeft(IWorkbook workbook)
        {
            ICellStyle left = workbook.CreateCellStyle();
            left.Alignment = HorizontalAlignment.Left;
            return left;
        }

        protected static ICellStyle AlignRight(IWorkbook workbook)
        {
            ICellStyle right = workbook.CreateCellStyle();
            right.Alignment = HorizontalAlignment.Left;
            return right;
        }

        public static string GetBulanString(int bulan)
        {
            //var associativeArray = new Dictionary<int?, string>() { { 1, "JANUARI" }, { 2, "FEBRUARI" }, { 3, "MAC" }, { 4, "APRIL" }, { 5, "MEI" }, { 6, "JUN" }, { 7, "JULAI" }, { 8, "OGOS" }, { 9, "SEPTEMBER" }, { 10, "OKTOBER" }, { 11, "NOVEMBER" }, { 12, "DISEMBER" } };
            var associativeArray =
                new Dictionary<int?, string>() {
                    { 1, "JAN" },
                    { 2, "FEB" },
                    { 3, "MAC" },
                    { 4, "APR" },
                    { 5, "MEI" },
                    { 6, "JUN" },
                    { 7, "JUL" },
                    { 8, "OGOS" },
                    { 9, "SEPT" },
                    { 10, "OKT" },
                    { 11, "NOV" },
                    { 12, "DIS" } };
            string bulanString = "";
            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    return m.Value;
                }
            }
            return bulanString;
        }

        public static string GetBulanLongString(int bulan)
        {
            var associativeArray = new Dictionary<int?, string>() { { 1, "JANUARI" }, { 2, "FEBRUARI" }, { 3, "MAC" }, { 4, "APRIL" }, { 5, "MEI" }, { 6, "JUN" }, { 7, "JULAI" }, { 8, "OGOS" }, { 9, "SEPTEMBER" }, { 10, "OKTOBER" }, { 11, "NOVEMBER" }, { 12, "DISEMBER" } };            
            string bulanString = "";
            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    return m.Value;
                }
            }
            return bulanString;
        }

        //row 0-4
        protected static int SetBorangAHeader(IWorkbook workbook, ISheet sheet1, int bulan, int tahun)
        {
            var associativeArray = new Dictionary<int?, string>() { { 1, "JANUARI" }, { 2, "FEBRUARI" }, { 3, "MAC" }, { 4, "APRIL" }, { 5, "MEI" }, { 6, "JUN" }, { 7, "JULAI" }, { 8, "OGOS" }, { 9, "SEPTEMBER" }, { 10, "OKTOBER" }, { 11, "NOVEMBER" }, { 12, "DISEMBER" } };
            string bulanString = "";
            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    bulanString = m.Value;
                }
            }

            string bulanTahunString = string.Format("{0} {1}", bulanString, tahun);
            DateTime tarikhAkhir = new DateTime(tahun, bulan, 1);
            tarikhAkhir = tarikhAkhir.AddMonths(1);
            tarikhAkhir = tarikhAkhir.AddDays(-1);

            ICellStyle alignCenter = AlignCenter(workbook);
            int colWidth = 6;

            for (int row = 0; row <= 4; row++)
            {
                IRow nRow = sheet1.CreateRow(row);
                ICell nCell = nRow.CreateCell(0);
                nCell.CellStyle = alignCenter;
                switch (row)
                {
                    case (0):
                        nCell.SetCellValue("PERTUBUHAN KESELEMATAN SOSIAL");
                        ICell cellBorang = nRow.CreateCell(colWidth);
                        cellBorang.SetCellValue("BORANG");
                        break;
                    case (1):
                        nCell.SetCellValue("JADUAL CARUMAN BULANAN");
                        ICell cell8a = nRow.CreateCell(colWidth);
                        cell8a.SetCellValue("8A");
                        break;
                    case (2):
                        nCell.SetCellValue("UNTUK CARUMAN BULAN " + bulanTahunString);
                        break;
                    case (3):
                        nCell.SetCellValue("JUMLAH CARUMAN UNTUK BULAN DI ATAS HENDAKLAH DIBAYAR");
                        break;
                    case (4):
                        nCell.SetCellValue("TIDAK LEWAT DARIPADA " + string.Format("{0:dd/MM/yyyy}",tarikhAkhir));
                        ICell cellLembaran = nRow.CreateCell(colWidth);
                        cellLembaran.SetCellValue("LEMBARAN: 1");
                        break;
                    default:
                        break;
                }
                //merge all column            
                var cra0 = new CellRangeAddress(row, row, 0, colWidth - 1);
                sheet1.AddMergedRegion(cra0);
            }
            return 4;
        }

        protected static int SetBayaran(IWorkbook workbook, ISheet sheet1, int startRow)
        {
            int columnStart = 2;
            for(int row = startRow; row <= startRow + 1; row++)
            {
                IRow nRow = sheet1.CreateRow(row);
                ICell nCell = nRow.CreateCell(columnStart);
                ICell nCell3 = nRow.CreateCell(columnStart + 1);

                if(row == startRow)
                {
                    nCell.SetCellValue("[  ] BAYARAN TUNAI");
                    nCell3.SetCellValue("AMAUN");
                }
                else
                {
                    nCell.SetCellValue("[  ] BAYARAN CEK");
                    nCell3.SetCellValue("NO. CEK ............................. R");
                }
            }
            return startRow + 2;
        }

        protected static int SetMajikan(IWorkbook workbook, ISheet sheet1, int startrow)
        {
            int columnStart = 2;
            for (int row = startrow ; row <= startrow + 6; row++)
            {
                IRow nRow = sheet1.CreateRow(row);
                ICell nCell = nRow.CreateCell(columnStart);
                ICell nCell3 = nRow.CreateCell(columnStart + 1);
                if (row == startrow)
                {
                    nCell.SetCellValue("NO. KOD");
                    nCell3.SetCellValue("B3200000538V");
                }
                else if(row == startrow + 1)
                {
                    nCell.SetCellValue("MAJIKAN");
                }
                else if(row == startrow + 3)
                {
                    nCell.SetCellValue("NAMA DAN");
                    nCell3.SetCellValue("DATUK BANDAR");
                }
                else if (row == startrow + 4)
                {
                    nCell.SetCellValue("ALAMAT MAJIKAN");
                    nCell3.SetCellValue("MAJLIS BANDARAYA PETALING JAYA");
                }
                else if (row == startrow + 5)
                {
                    nCell3.SetCellValue("JALAN YONG SHOOK LIN");
                }
                else if (row == startrow + 6)
                {
                    nCell3.SetCellValue("46675 PETALING JAYA");
                }
            }
            return startrow + 6;
        }

        protected static int SetTableHeader(IWorkbook workbook, ISheet sheet1, int startrow)
        {
            ICellStyle alignCenter = AlignCenter(workbook);
            int columnStart = 0;
            IRow nRow = sheet1.CreateRow(startrow);
            for (int col = columnStart; col <= columnStart + 4; col++)
            {
                ICell nCell = nRow.CreateCell(col);
                nCell.CellStyle = alignCenter;
                if(col == columnStart)
                {
                    nCell.SetCellValue("BIL");
                }
                else if(col == columnStart + 1)
                {
                    nCell.SetCellValue("NO KAD");
                }
                else if (col == columnStart + 2)
                {
                    nCell.SetCellValue("NO PEND");
                }
                else if (col == columnStart + 3)
                {
                    nCell.SetCellValue("NAMA PEKERJA (MENGIKUT KAD PENGENALAN)");
                }
                else if (col == columnStart + 4)
                {
                    nCell.SetCellValue("CARUMAN (4)");
                }
                sheet1.AutoSizeColumn(col);
            }
            IRow nRow2 = sheet1.CreateRow(startrow + 1);
            for (int col = columnStart; col <= columnStart + 4; col++)
            {
                ICell nCell = nRow2.CreateCell(col);
                nCell.CellStyle = alignCenter;
                if (col == columnStart)
                {

                }
                else if (col == columnStart + 1)
                {
                    nCell.SetCellValue("PENGENALAN (1)");
                }
                else if (col == columnStart + 2)
                {
                    nCell.SetCellValue("KES SOSIAL (2)");
                }
                else if (col == columnStart + 3)
                {
                    nCell.SetCellValue("(3)");
                }
                else if (col == columnStart + 4)
                {
                    nCell.SetCellValue("RM SEN");
                }
                sheet1.AutoSizeColumn(col);
            }
            return startrow + 1;
        }

        #endregion

        #region Export Excel
        public static IWorkbook ExportExcel(Extension extension)
        {
            //testing
            IWorkbook workbook = new XSSFWorkbook();
            if (extension == Extension.xls)
            {
                workbook = new HSSFWorkbook();
            }

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            int currentRow;
            currentRow = SetBorangAHeader(workbook, sheet1, 3, 2017);
            currentRow = SetBayaran(workbook, sheet1, currentRow + 1);
            currentRow = SetMajikan(workbook, sheet1, currentRow + 2);
            currentRow = SetTableHeader(workbook, sheet1, currentRow + 2);

            return workbook;
        }
        #endregion

    }
}