using eSPP.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.App_Helpers.ExcelHelper
{
    public class BorangAReport : ExcelHelper
    {
        private static int SetDataRow(IWorkbook workbook, ISheet sheet1, 
            int startrow, ReportBorangAModel reportData)
        {
            ICellStyle alignCenter = AlignLeft(workbook);
            ICellStyle alignRight = AlignRight(workbook);
            int columnStart = 0;
            foreach(PekerjaReportModel x in reportData.PekerjaSambilan)
            {
                IRow nRow = sheet1.CreateRow(startrow);
                for (int col = columnStart; col <= columnStart + 4; col++)
                {
                    ICell nCell = nRow.CreateCell(col);
                    nCell.CellStyle = alignCenter;
                    if (col == columnStart)
                    {
                        nCell.SetCellValue(x.Bil);
                    }
                    else if (col == columnStart + 1)
                    {
                        nCell.SetCellValue(x.NoKadPengenalan);
                    }
                    else if (col == columnStart + 2)
                    {
                        nCell.SetCellValue(x.NoKesSosial);
                    }
                    else if (col == columnStart + 3)
                    {
                        nCell.SetCellValue(x.NamaPekerja);
                    }
                    else if (col == columnStart + 4)
                    {
                        nCell.CellStyle = alignRight;
                        nCell.SetCellValue(String.Format("RM{0:0.00}", x.CarumanRM));
                    }
                    sheet1.AutoSizeColumn(col);
                }
                startrow++;
            }
            return startrow;            
        }

        public static IWorkbook GetReport(int bulanProses, int tahunProses, 
            string jenisReport)
        {
            IWorkbook workbook = new XSSFWorkbook();

            ReportBorangAModel report = ReportBorangAModel.GetReport(bulanProses, tahunProses, jenisReport);

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            int currentRow;
            currentRow = SetBorangAHeader(workbook, sheet1, bulanProses, tahunProses);
            currentRow = SetBayaran(workbook, sheet1, currentRow + 1);
            currentRow = SetMajikan(workbook, sheet1, currentRow + 2);
            currentRow = SetTableHeader(workbook, sheet1, currentRow + 2);
            currentRow = SetDataRow(workbook, sheet1, currentRow + 1, report);
            
            return workbook;
        }
    }
}