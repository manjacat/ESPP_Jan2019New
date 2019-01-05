using eSPP.Models;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.App_Helpers.ExcelHelper
{
    public class BonusSambilanReport : ExcelHelper
    {

        public static string GetBulanData(BonusSambilanDetailModel data, int currentMonth)
        {
            string retString = string.Empty;
            switch (currentMonth)
            {
                case (1):
                    retString = data.Jan.ToString();
                    break;
                case (2):
                    retString = data.Feb.ToString();
                    break;
                case (3):
                    retString = data.Mac.ToString();
                    break;
                case (4):
                    retString = data.April.ToString();
                    break;
                case (5):
                    retString = data.Mei.ToString();
                    break;
                case (6):
                    retString = data.Jun.ToString();
                    break;
                case (7):
                    retString = data.Julai.ToString();
                    break;
                case (8):
                    retString = data.Ogos.ToString();
                    break;
                case (9):
                    retString = data.September.ToString();
                    break;
                case (10):
                    retString = data.Oktober.ToString();
                    break;
                case (11):
                    retString = data.November.ToString();
                    break;
                case (12):
                    retString = data.Disember.ToString();
                    break;
                default:
                    retString = string.Empty;
                    break;
            }
            return retString;
        }

        private static int SetBonusSambilanTitle(IWorkbook workbook, ISheet sheet1,
            List<BonusSambilanDetailModel> printData, int startrow)
        {

            ICellStyle alignCenter = AlignCenter(workbook);
            int columnStart = 0;
            IRow nRow = sheet1.CreateRow(startrow);
            ICell nCell = nRow.CreateCell(columnStart);
            nCell.CellStyle = alignCenter;

            int year = printData.Select(x => x.TahunBonus).FirstOrDefault();
            int monthStart = printData.Select(x => x.MinBulan).FirstOrDefault();
            int monthEnd = printData.Select(x => x.MaxBulan).FirstOrDefault();
            int monthDibayar = printData.Select(x => x.BulanBonus).FirstOrDefault();
            int monthDiff = monthEnd - monthStart;

            string title = string.Format("BONUS DIBAYAR PADA {0} {1}",
                GetBulanLongString(monthDibayar).ToUpper(), year);
            nCell.SetCellValue(title);

            //merge all column in title
            int totalColumn = 10 + monthDiff;
            var cra0 = new CellRangeAddress(startrow, startrow, 0, totalColumn);
            sheet1.AddMergedRegion(cra0);

            return startrow + 2;
        }

        private static int SetBonusSambilanHeader(IWorkbook workbook, ISheet sheet1, 
            List<BonusSambilanDetailModel> printData, int startrow)
        {
            ICellStyle alignCenter = BorderAlignCenterColoured(workbook);
            int columnStart = 0;
            IRow nRow = sheet1.CreateRow(startrow);

            int monthStart = printData.Select(x => x.MinBulan).FirstOrDefault();
            int monthEnd = printData.Select(x => x.MaxBulan).FirstOrDefault();
            int monthDiff = monthEnd - monthStart + 1;

            for (int col = columnStart; col <= columnStart + 5; col++)
            {
                ICell nCell = nRow.CreateCell(col);
                nCell.CellStyle = alignCenter;
                if (col == columnStart)
                {
                    nCell.SetCellValue("NAMA");
                }
                else if (col == columnStart + 1)
                {
                    nCell.SetCellValue("NO PEKERJA");
                }
                else if (col == columnStart + 2)
                {
                    nCell.SetCellValue("NO KAD PENGENALAN");
                }
                else if (col == columnStart + 3)
                {
                    nCell.SetCellValue("NO AKAUN BANK");
                }
                else if (col == columnStart + 4)
                {
                    nCell.SetCellValue("NO KWSP");
                }
                else if (col == columnStart + 5)
                {
                    nCell.SetCellValue("TARIKH LANTIKAN");
                }
                sheet1.AutoSizeColumn(col);
            }
            int currentMonth = monthStart;
            for(int jcol = columnStart + 6; jcol <= columnStart + 6 + monthDiff; jcol++)
            {
                ICell nCell = nRow.CreateCell(jcol);
                nCell.CellStyle = alignCenter;
                string bulanString = GetBulanString(currentMonth);
                nCell.SetCellValue(bulanString);
                currentMonth++;
                sheet1.AutoSizeColumn(jcol);
            }
            int colStart_monthDiff = columnStart + 6 + monthDiff;
            for (int kcol = colStart_monthDiff; kcol <= colStart_monthDiff + 4; kcol++)
            {
                ICell nCell = nRow.CreateCell(kcol);
                nCell.CellStyle = alignCenter;
                if (kcol == colStart_monthDiff)
                {
                    nCell.SetCellValue("JUMLAH");
                }
                else if (kcol == colStart_monthDiff + 1)
                {
                    nCell.SetCellValue("PURATA");
                }
                else if (kcol == colStart_monthDiff + 2)
                {
                    nCell.SetCellValue("BONUS LAYAK");
                }
                else if (kcol == colStart_monthDiff + 3)
                {
                    nCell.SetCellValue("BONUS DITERIMA");
                }
                else if (kcol == colStart_monthDiff + 4)
                {
                    nCell.SetCellValue("CATATAN");
                }               
                sheet1.AutoSizeColumn(kcol);
            }
            return startrow + 1;
        }

        private static int SetBonusSambilanBody(IWorkbook workbook, ISheet sheet1,
            List<BonusSambilanDetailModel> printData, int startrow)
        {
            int monthStart = printData.Select(x => x.MinBulan).FirstOrDefault();
            int monthEnd = printData.Select(x => x.MaxBulan).FirstOrDefault();
            int monthDiff = monthEnd - monthStart + 1;
            
            foreach (BonusSambilanDetailModel data in printData)
            {
                BindData(workbook, sheet1, data, startrow, monthStart, monthDiff);
                startrow++;
            }
            return startrow;
        }

        private static void BindData(IWorkbook workbook, ISheet sheet1,
            BonusSambilanDetailModel data, int startrow,
            int monthStart, int monthDiff)
        {
            int columnStart = 0;
            ICellStyle alignCenter = BorderAlignCenter(workbook);
            IRow nRow = sheet1.CreateRow(startrow);

            for (int col = columnStart; col <= columnStart + 5; col++)
            {
                ICell nCell = nRow.CreateCell(col);
                nCell.CellStyle = alignCenter;
                if (col == columnStart)
                {
                    nCell.SetCellValue(data.Nama);
                }
                else if (col == columnStart + 1)
                {
                    nCell.SetCellValue(data.NoPekerja);
                }
                else if (col == columnStart + 2)
                {
                    nCell.SetCellValue(data.NoKadPengenalan);
                }
                else if (col == columnStart + 3)
                {
                    nCell.SetCellValue(data.NoAkaunBank);
                }
                else if (col == columnStart + 4)
                {
                    nCell.SetCellValue(data.NoKWSP);
                }
                else if (col == columnStart + 5)
                {
                    nCell.SetCellValue(data.TarikhLantikanString);
                }
                sheet1.AutoSizeColumn(col);
            }
            int currentMonth = monthStart;
            for (int jcol = columnStart + 6; jcol <= columnStart + 6 + monthDiff; jcol++)
            {
                ICell nCell = nRow.CreateCell(jcol);
                nCell.CellStyle = alignCenter;
                string bulanString = GetBulanData(data, currentMonth);
                nCell.SetCellValue(bulanString);
                currentMonth++;
                sheet1.AutoSizeColumn(jcol);
            }
            int colStart_monthDiff = columnStart + 6 + monthDiff;
            for (int kcol = colStart_monthDiff; kcol <= colStart_monthDiff + 4; kcol++)
            {
                ICell nCell = nRow.CreateCell(kcol);
                nCell.CellStyle = alignCenter;
                if (kcol == colStart_monthDiff)
                {
                    nCell.SetCellValue(data.JumlahGaji.ToString());
                }
                else if (kcol == colStart_monthDiff + 1)
                {
                    nCell.SetCellValue(data.GajiPurata.ToString());
                }
                else if (kcol == colStart_monthDiff + 2)
                {
                    try
                    {
                        if (data.BonusLayak != null)
                        {
                            //always show 2 dec places
                            nCell.SetCellValue(data.BonusLayak.Value.ToString("0.00"));
                        }
                    }
                    catch
                    {
                        nCell.SetCellValue("0.00");
                    }
                }
                else if (kcol == colStart_monthDiff + 3)
                {
                    try
                    {
                        if(data.BonusDiterima != null)
                        {
                            //always show 2 dec places
                            nCell.SetCellValue(data.BonusDiterima.Value.ToString("0.00"));
                        }
                    }
                    catch
                    {
                        nCell.SetCellValue("0.00");
                    }
                }
                else if (kcol == colStart_monthDiff + 4)
                {
                    nCell.SetCellValue(data.Catatan);
                }
                sheet1.AutoSizeColumn(kcol);
            }
        }


        public static IWorkbook GetReport(int bulan, int tahun)
        {
            IWorkbook workbook = new XSSFWorkbook();

            List<BonusSambilanDetailModel> printData
                   = BonusSambilanDetailModel.GetBonusSambilanDetailData(bulan, tahun);

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            int currentRow;
            currentRow = SetBonusSambilanTitle(workbook, sheet1, printData, 0);
            currentRow = SetBonusSambilanHeader(workbook, sheet1, printData, currentRow);
            currentRow = SetBonusSambilanBody(workbook, sheet1, printData, currentRow);
            return workbook;
        }
    }
}