using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Data.Entity;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.tool.xml;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Core.Objects;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Office.Interop.Word;
using ClosedXML.Excel;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using eSPP.App_Helpers.ExcelHelper;

namespace eSPP.Controllers
{
    public class KakitanganSambilanController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filtercontext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var GE_PEKERJA = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN)
              .Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y"
              && s.HR_AKTIF_IND == ("Y")
              && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("N") ||
              s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("A"))).ToList();
            var PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.TUNGGAKAN = db.HR_MAKLUMAT_ELAUN_POTONGAN.ToList();
            ViewBag.ELAUN = db.HR_ELAUN.ToList();
            ViewBag.POTONGAN = db.HR_POTONGAN.ToList();
            ViewBag.CARUMAN = db.HR_CARUMAN.ToList();
            ViewBag.GAJI = db.HR_GAJI_UPAHAN.ToList();
            ViewBag.PEKERJA = PEKERJA;
            ViewBag.HR_PEKERJA = GE_PEKERJA; //ini dropdown
        }

        public enum ManageMessageId
        {
            Tambah,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ResetPassword,
            Kemaskini,
            KemaskiniTunggakan,
            BayarTunggakan,
            TambahBonus,
            Error,
            Muktamad
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sejarah(ManageMessageId? message,
            string HR_PEKERJA, string tahunbekerja, string bulanbekerja,
            string tahundibayar, string bulandibayar)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.Tambah ? "Permohonan Telah Berjaya Disimpan."
              : message == ManageMessageId.Muktamad ? "Permohonan Anda Telah Muktamad."
              : message == ManageMessageId.Kemaskini ? "Permohonan Telah Berjaya Dikemaskini."
              : message == ManageMessageId.BayarTunggakan ? "Permohonan Telah Berjaya Dikemaskini."
              : "";

            PageSejarahModel page = new PageSejarahModel();
            if (!string.IsNullOrEmpty(HR_PEKERJA))
            {
                //if got No pekerja
                page = new PageSejarahModel(HR_PEKERJA, tahunbekerja, bulanbekerja,
                          tahundibayar, bulandibayar);
                page.jumlahhari = page.GetJumlahHari();
                page.jumlahot = page.GetJumlahOT();
            }
            else
            {
                if (!string.IsNullOrEmpty(bulanbekerja) && !string.IsNullOrEmpty(bulandibayar)
                    && !string.IsNullOrEmpty(tahunbekerja) && !string.IsNullOrEmpty(tahundibayar))
                {
                    try
                    {
                        page.bulanbekerja = Convert.ToInt32(bulanbekerja);
                        page.bulandibayar = Convert.ToInt32(bulandibayar);
                        page.tahunbekerja = Convert.ToInt32(tahunbekerja);
                        page.tahundibayar = Convert.ToInt32(tahundibayar);
                    }
                    catch
                    {

                    }
                }
            }

            ViewBag.bulandibayar = GetBulanDropdown(page.bulandibayar);
            ViewBag.tunggakanbulandibayar = ViewBag.bulandibayar;
            ViewBag.bulanbekerja = GetBulanDropdown(page.bulanbekerja);
            ViewBag.tunggakanbulanbekerja = GetBulanDropdown(page.tunggakanbulanbekerja);
            return View(page);
        }

        [NonAction]
        private List<SelectListItem> GetBulanDropdown(int bulanSelected)
        {
            List<SelectListItem> ddlbulan = new List<SelectListItem>
            {
                new SelectListItem { Text = "Januari", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };

            List<SelectListItem> outputDDL = new List<SelectListItem>();
            foreach (SelectListItem bulan in ddlbulan)
            {
                SelectListItem m = new SelectListItem
                {
                    Text = bulan.Text,
                    Value = bulan.Value
                };
                if (m.Value == bulanSelected.ToString())
                {
                    m.Selected = true;
                }
                outputDDL.Add(m);
            }
            return outputDDL;
        }


        [HttpPost]
        public ActionResult Sejarah(PageSejarahModel page, string Command)
        {
            var user = User.Identity.GetUserId();
            PageSejarahModel.Insert(page, user, Command);
            string info = string.Empty;
            if (Command == "Hantar")
            {
                info = ManageMessageId.Tambah.ToString();
                PageSejarahModel newPage = new PageSejarahModel();
                //open same page, buat empty form
                return RedirectToAction("Sejarah",
                new
                {
                    message = info,
                    page.tahunbekerja,
                    page.tahundibayar,
                    page.bulandibayar,
                    page.bulanbekerja
                });
            }
            else if (Command == "Kemaskini")
            {
                info = ManageMessageId.Kemaskini.ToString();
                //show same page
                return RedirectToAction("Sejarah",
                   new
                   {
                       message = info,
                       page.HR_PEKERJA,
                       page.tahunbekerja,
                       page.tahundibayar,
                       page.bulandibayar,
                       page.bulanbekerja
                   });
            }
            else //Muktamad
            {
                info = ManageMessageId.Muktamad.ToString();
                //redirect to menu
                return RedirectToAction("ListTransaksiSambilan", "PengurusanKakitanganSambilan",
                new
                {
                    message = info,
                    month1 = page.bulanbekerja,
                    year1 = page.tahunbekerja,
                    year2 = page.tahundibayar,
                    month2 = page.bulandibayar
                });
            }           
        }

        public ActionResult BonusSambilanDetail(string month, string year, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.Tambah ? "Data Telah Berjaya Disimpan."
              : message == ManageMessageId.Muktamad ? "Data Anda Telah Muktamad."
              : message == ManageMessageId.Kemaskini ? "Data Telah Berjaya Dikemaskini."
              : message == ManageMessageId.Error ? "Gagal Kemaskini/Tambah Data."
              : "";

            ViewBag.MinBulan = 0;
            ViewBag.MaxBulan = 0;
            ViewBag.MaxTahun = 0;
            ViewBag.BulanBonus = 0;
            List<BonusSambilanDetailModel> list = new List<BonusSambilanDetailModel>();
            try
            {
                int monthInt = Convert.ToInt32(month);
                int yearInt = Convert.ToInt32(year);
                list = BonusSambilanDetailModel.GetBonusSambilanDetailData(monthInt, yearInt);
                bool isMuktamad = list.Select(m => m.IsMuktamad).FirstOrDefault();
                if (isMuktamad)
                {
                    ViewBag.IsMuktamad = "true";
                }
                else
                {
                    ViewBag.IsMuktamad = string.Empty;
                }
                ViewBag.MinBulan = list.Select(x => x.MinBulan).Min();
                ViewBag.MaxBulan = monthInt;
                ViewBag.MaxTahun = yearInt;
                ViewBag.BulanBonus = monthInt;
            }
            catch
            {

            }

            return View(list);
        }

        [HttpPost]
        public ActionResult UpdateBonus(string bonusDiterima, string bulanBonus,
            string tahunBonus, string noPekerja, string isTemp = "")
        {
            ManageMessageId outputMsg;
            try
            {
                int month = Convert.ToInt32(bulanBonus);
                int year = Convert.ToInt32(tahunBonus);
                decimal bonusDiterima_dec = Convert.ToDecimal(bonusDiterima);
                HR_BONUS_SAMBILAN_DETAIL.UpdateBonusDiterima(month, year, noPekerja, bonusDiterima_dec);
                outputMsg = ManageMessageId.Kemaskini;
            }
            catch
            {
                outputMsg = ManageMessageId.Error;
            }

            if (!string.IsNullOrEmpty(isTemp))
            {
                return RedirectToAction("TambahBonus",
                    new
                    {
                        month = bulanBonus,
                        year = tahunBonus,
                        message = outputMsg,
                        isTemp = "yes"
                    });
            }
            else
            {
                return RedirectToAction("BonusSambilanDetail", new { month = bulanBonus, year = tahunBonus, message = outputMsg });
            }
        }

        [HttpPost]
        public ActionResult UpdateCatatan(string catatan, string bulanBonus,
            string tahunBonus, string noPekerja, string isTemp = "")
        {
            ManageMessageId outputMsg;
            try
            {
                int month = Convert.ToInt32(bulanBonus);
                int year = Convert.ToInt32(tahunBonus);
                HR_BONUS_SAMBILAN_DETAIL.UpdateCatatan(month, year, noPekerja, catatan);
                outputMsg = ManageMessageId.Kemaskini;
            }
            catch
            {
                outputMsg = ManageMessageId.Error;
            }

            if (!string.IsNullOrEmpty(isTemp))
            {
                return RedirectToAction("TambahBonus",
                    new
                    {
                        month = bulanBonus,
                        year = tahunBonus,
                        message = outputMsg,
                        isTemp = "yes"
                    });
            }
            else
            {
                return RedirectToAction("BonusSambilanDetail", new { month = bulanBonus, year = tahunBonus, message = outputMsg });
            }
        }

        public ActionResult UpdateMuktamad(string bulanBonus, string tahunBonus, string noPekerja = null)
        {
            ManageMessageId outputMsg;
            try
            {
                int month = Convert.ToInt32(bulanBonus);
                int year = Convert.ToInt32(tahunBonus);
                HR_BONUS_SAMBILAN_DETAIL.UpdateMuktamad(month, year, noPekerja);
                outputMsg = ManageMessageId.Muktamad;
            }
            catch
            {
                outputMsg = ManageMessageId.Error;
            }

            return RedirectToAction("BonusSambilanDetail", new { month = bulanBonus, year = tahunBonus, message = outputMsg });
        }

        public ActionResult TambahBonus(string tahunDibayar, string month = "1", string year = "0",
            string isTemp = "")
        {
            List<BonusSambilanDetailModel> bonus = new List<BonusSambilanDetailModel>();
            if (isTemp != "yes")
            {
                return View(bonus);
            }
            else
            {
                int monthInt = Convert.ToInt32(month);
                int yearInt = Convert.ToInt32(year);
                bonus = BonusSambilanDetailModel.GetBonusSambilanDetailData(monthInt, yearInt);
                int startMonth = bonus.Select(x => x.MinBulan).FirstOrDefault();
                int endMonth = bonus.Select(x => x.MaxBulan).FirstOrDefault();
                ViewBag.MinBulan = startMonth;
                ViewBag.MaxBulan = endMonth;
                ViewBag.BulanBonus = month;
                ViewBag.MaxTahun = yearInt;
                return View(bonus);
            }
        }

        [HttpPost]
        public ActionResult TambahBonus(string bulanBekerja, string bulanDibayar,
            string tahunDibayar, string bulanBekerjaHingga, string Command)
        {
            if (Command == "Tambah")
            {
                //TODO add to HR_BONUS_SAMBILAN_DETAIL
                ManageMessageId outputMsg = ManageMessageId.Tambah;
                HR_BONUS_SAMBILAN_DETAIL.UpdateTambahBonus();
                return RedirectToAction("UrusBonus", "PengurusanKakitanganSambilan",
                    new { month = bulanDibayar, tahunbekerja = tahunDibayar, message = outputMsg });
            }
            else if (Command == "Batal")
            {
                HR_BONUS_SAMBILAN_DETAIL.DeleteTambahBonus();
                return RedirectToAction("TambahBonus",
                    new { month = bulanDibayar, year = tahunDibayar });
            }
            else //preview ("Pratonton")
            {
                List<BonusSambilanDetailModel> bonus = new List<BonusSambilanDetailModel>();
                ViewBag.MinBulan = 0;
                ViewBag.MaxBulan = 0;
                ViewBag.MaxTahun = 0;
                ViewBag.BulanBonus = 0;
                try
                {
                    int startMonth = Convert.ToInt32(bulanBekerja);
                    int endMonth = Convert.ToInt32(bulanBekerjaHingga);
                    int month = Convert.ToInt32(bulanDibayar);
                    int year = Convert.ToInt32(tahunDibayar);
                    ViewBag.MinBulan = startMonth;
                    ViewBag.MaxBulan = endMonth;
                    ViewBag.BulanBonus = month;
                    ViewBag.MaxTahun = year;
                    bonus = BonusSambilanDetailModel.GetDetailsFromTransaksi(startMonth, month, year, endMonth);
                    if (bonus.Count() > 0)
                    {
                        HR_BONUS_SAMBILAN_DETAIL.InsertTambahBonus(bonus); //add to database
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                return View(bonus);
            }
        }

        public ActionResult PrintBonus(string bulanBonus, string tahunBonus)
        {
            try
            {
                int month = Convert.ToInt32(bulanBonus);
                int year = Convert.ToInt32(tahunBonus);
                IWorkbook workbook = BonusSambilanReport.GetReport(month, year);
                // code to create workbook 
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string saveAsFileName = string.Format("BonusSambilanReport-{0:d}.xlsx", DateTime.Now).Replace("/", "-");
                    byte[] bytes = exportData.ToArray();
                    return File(bytes, "application/vnd.ms-excel", saveAsFileName);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return RedirectToAction("UrusBonus", "PengurusanKakitanganSambilan", new { tahunbekerja  = tahunBonus});
            }

        }

        [HttpPost]
        public ActionResult TambahGandaan
            (string gandaan, string bulanBonus,
            string tahunBonus, string isTemp = "")
        {
            ManageMessageId outputMsg;
            try
            {
                int month = Convert.ToInt32(bulanBonus);
                int year = Convert.ToInt32(tahunBonus);
                decimal multiply = Convert.ToDecimal(gandaan);
                HR_BONUS_SAMBILAN_DETAIL.UpdateBonusDiterima(month, year, multiply);
                outputMsg = ManageMessageId.Kemaskini;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                outputMsg = ManageMessageId.Error;
            }

            if (!string.IsNullOrEmpty(isTemp))
            {
                return RedirectToAction("TambahBonus",
                    new
                    {
                        month = bulanBonus,
                        year = tahunBonus,
                        message = outputMsg,
                        isTemp = "yes"
                    });
            }
            else
            {
                return RedirectToAction("BonusSambilanDetail", new { month = bulanBonus, year = tahunBonus, message = outputMsg });
            }
        }
    }
}